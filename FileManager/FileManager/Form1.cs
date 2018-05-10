using System;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using PacketLibrary;

namespace FileManager
{
    public partial class FMServer : Form
    {
        private NetworkStream stream = null;
        private TcpListener listener = null;

        private byte[] sendBuf = new byte[PUtility.BUF_LEN];
        private byte[] recvBuf = new byte[PUtility.BUF_LEN];

        private Thread thread;

        public FMServer()
        {
            InitializeComponent();
        }

        private void Send()
        {
            this.stream.Write(this.sendBuf, 0, this.sendBuf.Length);
            this.stream.Flush();
            Array.Clear(this.sendBuf, 0, this.sendBuf.Length);
        }

        public void Run()
        {
            // Connect
            try
            {
                this.listener = new TcpListener(Int32.Parse(txtPort.Text));
                this.listener.Start();
                
                this.Invoke(new MethodInvoker(delegate ()
                {
                    txtLog.AppendText("waiting for client access...\r\n");
                }));

                TcpClient client = this.listener.AcceptTcpClient();
                this.stream = client.GetStream();
                this.Invoke(new MethodInvoker(delegate ()
                {
                    txtLog.AppendText("Client Access!!\r\n");
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            // IO
            int nRead = 0;
            while (true)
            {
                try
                {
                    nRead = 0;
                    Array.Clear(this.recvBuf, 0, this.recvBuf.Length);
                    nRead = this.stream.Read(recvBuf, 0, PUtility.BUF_LEN);
                }
                catch
                {
                    MessageBox.Show("IO Error");
                    return;
                }

                if (nRead == 0)
                {
                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        txtLog.AppendText("Client Disconnect...\r\n");
                        this.stream.Close();
                        this.listener.Stop();
                        this.thread.Abort();
                    }));
                    return;
                }

                Packet packet = (Packet)Packet.Deserialize(this.recvBuf);

                switch ((int)packet.Type)
                {
                    case (int)PacketType.FileMeta:
                        HandleFileMeta();
                        break;
                }
            }
        }

        private void HandleFileMeta()
        {
            FileMeta fileMeta = (FileMeta)Packet.Deserialize(this.recvBuf);
            this.Invoke(new MethodInvoker(delegate ()
            {
                txtLog.AppendText("File Name : " + fileMeta.fileName + "\r\n");
                txtLog.AppendText("File Hash : " + fileMeta.md5Hash + "\r\n");
            }));
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            this.thread = new Thread(new ThreadStart(Run));
            this.thread.Start();
        }
    }
}
