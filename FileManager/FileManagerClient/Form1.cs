using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.IO;
using PacketLibrary;

namespace FileManagerClient
{
    public partial class FMClient : Form
    {
        private NetworkStream stream = null;
        private TcpClient client = null;

        private byte[] sendBuf = new byte[PUtility.BUF_LEN];
        private byte[] recvBuf = new byte[PUtility.BUF_LEN];

        public FMClient()
        {
            InitializeComponent();
        }

        private void Send()
        {
            this.stream.Write(this.sendBuf, 0, this.sendBuf.Length);
            this.stream.Flush();
            Array.Clear(this.sendBuf, 0, this.sendBuf.Length);
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            this.client = new TcpClient();
            try
            {
                this.client.Connect(txtIP.Text, Int32.Parse(txtPort.Text));
            }
            catch
            {
                MessageBox.Show("접속 에러");
                return;
            }
            this.stream = this.client.GetStream();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (openFileDlg.ShowDialog() == DialogResult.OK)
            {
                FileInfo f = new FileInfo(openFileDlg.FileName);
                string md5 = PUtility.CalculateMD5(openFileDlg.FileName);
                FileMeta fileMeta = new FileMeta(
                    f.Length,
                    openFileDlg.FileName,
                    md5);
                labelHash.Text = md5;
                fileMeta.Type = (int)PacketType.FileMeta;
                Packet.Serialize(fileMeta).CopyTo(this.sendBuf, 0);
                this.Send();
            }
        }
    }
}
