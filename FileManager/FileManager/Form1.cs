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
        private string pathDir = @"C:\FMServer\";
        private string pathCur;

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

        private int Recv()
        {
            int nRead = 0;
            try
            {
                Array.Clear(this.recvBuf, 0, this.recvBuf.Length);
                nRead = this.stream.Read(recvBuf, 0, PUtility.BUF_LEN);
            }
            catch
            {
                MessageBox.Show("IO Error");
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
            }

            return nRead;
        }

        public void Run()
        {
            // Connect
            try
            {
                this.listener = new TcpListener(PUtility.PORT_NUM);
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

            InitCase();
            SetCase();

            // IO
            while (true)
            {
                if (Recv() == 0)
                    return;

                Packet packet = (Packet)Packet.Deserialize(this.recvBuf);

                switch ((int)packet.Type)
                {
                    case (int)PacketType.FileMeta:
                        HandleFileMeta();
                        break;

                    case (int)PacketType.ReqDirList:
                        HandleReqDirList();
                        break;

                    case (int)PacketType.ReqFileList:
                        HandleReqFileList();
                        break;
                }
            }
        }

        private void InitCase()
        {
            string[] dirs = Directory.GetDirectories(pathDir);
            foreach (string dir in dirs)
            {
                Case c = new Case(Path.GetFileName(dir));
                c.Type = (int)PacketType.Case;
                Packet.Serialize(c).CopyTo(this.sendBuf, 0);
                this.Send();
            }
            Packet packet = new Packet();
            packet.Type = (int)PacketType.EOP;
            Packet.Serialize(packet).CopyTo(this.sendBuf, 0);
            this.Send();
        }

        private void SetCase()
        {
            while (true)
            {
                if (Recv() == 0)
                    this.Close();

                Packet packet = (Packet)Packet.Deserialize(this.recvBuf);

                switch ((int)packet.Type)
                {
                    case (int)PacketType.Case:
                        CreateNewCase();
                        break;

                    case (int)PacketType.CaseSelected:
                        SelectCase();
                        SendSubDirList(pathDir);
                        return;
                }
            }
        }

        private void CreateNewCase()
        {
            Case c = (Case)Packet.Deserialize(this.recvBuf);
            ACK ack = new ACK(false);
            ack.Type = (int)PacketType.ACK;
            if (!Directory.Exists(c.caseName))
            {
                ack.isOK = true;
                Directory.CreateDirectory(c.caseName);
            }
            Packet.Serialize(ack).CopyTo(this.sendBuf, 0);
            this.Send();
        }

        private void SelectCase()
        {
            Case c = (Case)Packet.Deserialize(this.recvBuf);
            Directory.SetCurrentDirectory(c.caseName);
            pathCur = pathDir + c.caseName + "\\";
            this.Invoke(new MethodInvoker(delegate ()
            {
                txtLog.AppendText("Case Name : " + c.caseName + "\r\n");
            }));
        }

        private void SendSubDirList(string dir)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            DirectoryInfo[] diArr = dirInfo.GetDirectories();
            string[] list = new string[diArr.Length];
            int i = 0;
            foreach (DirectoryInfo di in diArr)
            {
                list[i++] = di.Name;
            }
            DirList dirList = new DirList(list);
            Packet.Serialize(dirList).CopyTo(this.sendBuf, 0);
            this.Send();
        }

        private void SendSubFileList(string dir)
        {
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(dir);
                FileInfo[] fiArr = dirInfo.GetFiles();
                FileMeta[] metas = new FileMeta[fiArr.Length];
                int i = 0;
                foreach (FileInfo fi in fiArr)
                {
                    FileMeta meta = new FileMeta(fi.Length, fi.Name,
                        "", fi.LastWriteTime.ToString());
                    metas[i++] = meta;
                }
                FileList fileList = new FileList(metas);
                Packet.Serialize(fileList).CopyTo(this.sendBuf, 0);
                this.Send();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void HandleReqFileList()
        {
            DirInfo dirInfo = (DirInfo)Packet.Deserialize(this.recvBuf);
            SendSubFileList(pathDir + dirInfo.dirName);
            pathCur = pathDir + dirInfo.dirName + "\\";
            Directory.SetCurrentDirectory(pathCur);
        }

        private void HandleReqDirList()
        {
            DirInfo dirInfo = (DirInfo)Packet.Deserialize(this.recvBuf);
            SendSubDirList(pathDir + dirInfo.dirName);
        }

        private void HandleFileMeta()
        {
            FileMeta fileMeta = (FileMeta)Packet.Deserialize(this.recvBuf);
            this.Invoke(new MethodInvoker(delegate ()
            {
                txtLog.AppendText("File Name : " + fileMeta.fileName + "\r\n");
                txtLog.AppendText("File Hash : " + fileMeta.md5Hash + "\r\n");
            }));

            ACK ack = new ACK();
            ack.Type = (int)PacketType.ACK;
            if (File.Exists(pathCur + Path.GetFileName(fileMeta.fileName)))
            {
                ack.isOK = false;
                Packet.Serialize(ack).CopyTo(this.sendBuf, 0);
                this.Send();

                Recv();
                ack = (ACK)Packet.Deserialize(this.recvBuf);

                if (ack.isOK == true)
                {
                    // version
                    try
                    {
                        PUtility.CompressWithVersion(pathCur + Path.GetFileName(fileMeta.fileName));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }
            else
            {
                Packet.Serialize(ack).CopyTo(this.sendBuf, 0);
                this.Send();
            }

            FileStream fs = File.Open(pathCur + 
                Path.GetFileName(fileMeta.fileName), FileMode.Create);
            BinaryWriter writer = new BinaryWriter(fs);

            int nRecv = 0;
            long nRemain = fileMeta.fileLength;
            byte[] buff = new byte[PUtility.BUF_LEN];
            try
            {
                while ((nRecv = stream.Read(buff, 0, buff.Length)) > 0)
                {
                    writer.Write(buff, 0, nRecv);
                    writer.Flush();
                    nRemain -= nRecv;
                    if (nRemain <= 0)
                        break;
                }
            }
            catch
            {
                ACK ackFail = new ACK(false);
                ackFail.Type = (int)PacketType.ACK;

                Packet.Serialize(ackFail).CopyTo(this.sendBuf, 0);
                this.Send();
            }
            finally
            {
                writer.Close();
                fs.Close();
            }

            string md5 = PUtility.CalculateMD5(pathCur +
                Path.GetFileName(fileMeta.fileName));
            if (String.Compare(md5, fileMeta.md5Hash) == 0)
            {
                ACK ackSuc = new ACK();
                ackSuc.Type = (int)PacketType.ACK;

                Packet.Serialize(ackSuc).CopyTo(this.sendBuf, 0);
                this.Send();

                File.WriteAllText(pathCur + 
                    Path.GetFileNameWithoutExtension(fileMeta.fileName) +
                    ".gpg", md5);
            }
            else
            {
                ACK ackFail = new ACK(false);
                ackFail.Type = (int)PacketType.ACK;

                Packet.Serialize(ackFail).CopyTo(this.sendBuf, 0);
                this.Send();
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            this.thread = new Thread(new ThreadStart(Run));
            this.thread.Start();
        }

        private void SetDirectory()
        {
            if (!Directory.Exists(pathDir))
                Directory.CreateDirectory(pathDir);
            Directory.SetCurrentDirectory(pathDir);

            this.Invoke(new MethodInvoker(delegate ()
            {
                txtLog.AppendText("Storage Path : " + pathDir + "\r\n");
            }));
        }

        private void FMServer_Load(object sender, EventArgs e)
        {
            txtIP.Text = PUtility.GetLocalIP();
            SetDirectory();
        }
    }
}
