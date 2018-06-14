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
using System.Diagnostics;

namespace FileManagerClient
{
    public partial class FMClient : Form
    {
        private NetworkStream stream = null;
        private TcpClient client = null;

        private byte[] sendBuf = new byte[PUtility.BUF_LEN];
        private byte[] recvBuf = new byte[PUtility.BUF_LEN];

        private string dirPath = null;
        private string filePath = null;
        private string fdirPath = null;
        private string servFile = null;
        private Action closeConnect;
        private string caseName;
        
        private void Send()
        {
            this.stream.Write(this.sendBuf, 0, this.sendBuf.Length);
            this.stream.Flush();
            Array.Clear(this.sendBuf, 0, this.sendBuf.Length);
        }

        private void Recv()
        {
            try
            {
                Array.Clear(this.recvBuf, 0, this.recvBuf.Length);
                this.stream.Read(recvBuf, 0, PUtility.BUF_LEN);
            }
            catch
            {
                MessageBox.Show("Recv Error");
                return;
            }
        }

        public FMClient(TcpClient client, NetworkStream stream)
        {
            this.client = client;
            this.stream = stream;
            InitializeComponent();
        }

        public FMClient(TcpClient client, NetworkStream stream, 
            Action closeConnect) : this(client, stream)
        {
            this.closeConnect = closeConnect;
        }

        public FMClient(TcpClient client, NetworkStream stream, 
            Action closeConnect, string caseName) : this(client, stream, closeConnect)
        {
            this.caseName = caseName;
        }

        private void PopulateClientTreeView()
        {
            string[] drvList;
            TreeNode root;
            drvList = Environment.GetLogicalDrives();

            foreach (string drv in drvList)
            {
                root = viewClient.Nodes.Add(drv);
                root.ImageIndex = 1;

                if (viewClient.SelectedNode == null)
                    viewClient.SelectedNode = root;
                root.SelectedImageIndex = root.ImageIndex;
                root.Nodes.Add("");
            }
        }

        private void PopulateServerTreeView()
        {
            TreeNode root = new TreeNode(caseName);
            root.Tag = "C";
            root.ImageIndex = 0;

            Recv();
            DirList dirList = (DirList)Packet.Deserialize(this.recvBuf);
            
            foreach (string dir in dirList.dirList)
            {
                TreeNode subNode = new TreeNode(dir);
                subNode.Tag = "D";
                root.Nodes.Add(subNode);
            }
            viewServer.Nodes.Add(root);
            viewServer.SelectedNode = root;
        }

        private void SetPlus(TreeNode node)
        {
            string path;
            DirectoryInfo dir;
            DirectoryInfo[] di;

            try
            {
                path = node.FullPath;
                dir = new DirectoryInfo(path);
                di = dir.GetDirectories();
                if (di.Length > 0)
                    node.Nodes.Add("");
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.ToString());
            }
        }

        private void OpenFiles()
        {
            ListView.SelectedListViewItemCollection siList;
            siList = listClient.SelectedItems;

            foreach (ListViewItem item in siList)
                OpenItem(item);
        }

        private void SelectFiles()
        {
            ListView.SelectedListViewItemCollection siList;
            siList = listClient.SelectedItems;

            foreach (ListViewItem item in siList)
                SelectItem(item);
        }

        private void SelectItem(ListViewItem item)
        {
            if (item.Tag.ToString() == "F")
                filePath = dirPath + "\\" + item.Text;
            else if (item.Tag.ToString() == "D")
                fdirPath = dirPath + "\\" + item.Text;
        }

        private void SelectServerFiles()
        {
            servFile = listServer.FocusedItem.Text;
            /*
            ListView.SelectedListViewItemCollection siList;
            siList = listClient.SelectedItems;

            foreach (ListViewItem item in siList)
            {
                servFile = item.Text;
            }
            */
        }

        private void OpenItem(ListViewItem item)
        {
            TreeNode node, child;

            if (item.Tag.ToString() == "D")
            {
                node = viewClient.SelectedNode;
                node.Expand();

                child = node.FirstNode;

                while (child != null)
                {
                    if (child.Text == item.Text)
                    {
                        viewClient.SelectedNode = child;
                        viewClient.Focus();
                        break;
                    }
                    child = child.NextNode;
                }
            }
            else
            {
                switch (Path.GetExtension(item.Text))
                {
                    case ".txt":
                        TextViewer tv = new TextViewer();
                        tv.getTextBox().Text = File.ReadAllText(dirPath + "\\" + item.Text);
                        tv.Show();
                        break;

                    case ".jpg":
                    case ".png":
                        PictureViewer pv = new PictureViewer();
                        pv.getPictureBox().Image = Image.FromFile(@dirPath + "\\" + item.Text);
                        pv.Show();
                        break;

                    default:

                        Process.Start(dirPath + "\\" + item.Text);
                        break;
                }
            }
        }

        private void viewClient_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            string path;
            DirectoryInfo dir;
            DirectoryInfo[] di;
            TreeNode node;

            try
            {
                e.Node.Nodes.Clear();
                path = e.Node.FullPath;
                dir = new DirectoryInfo(path);
                di = dir.GetDirectories();

                foreach (DirectoryInfo dirs in di)
                {
                    node = e.Node.Nodes.Add(dirs.Name);
                    SetPlus(node);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void viewServer_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            string path;
            TreeNode node;

            try
            {
                e.Node.Nodes.Clear();
                path = e.Node.FullPath;
                DirInfo dirInfo = new DirInfo(path);
                dirInfo.Type = (int)PacketType.ReqDirList;
                Packet.Serialize(dirInfo).CopyTo(this.sendBuf, 0);
                this.Send();

                Recv();
                DirList dirList = (DirList)Packet.Deserialize(this.recvBuf);

                foreach (string dir in dirList.dirList)
                {
                    node = e.Node.Nodes.Add(dir);
                    node.Nodes.Add("");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void viewClient_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            DirectoryInfo di;
            DirectoryInfo[] diarr;
            ListViewItem item;
            FileInfo[] fiArr;

            try
            {
                di = new DirectoryInfo(e.Node.FullPath);
                dirPath = e.Node.FullPath.Substring(0, 2) + e.Node.FullPath.Substring(3);
                listClient.Items.Clear();
                
                diarr = di.GetDirectories();
                foreach (DirectoryInfo tdis in diarr)
                {
                    item = listClient.Items.Add(tdis.Name);
                    item.SubItems.Add("");
                    item.SubItems.Add(tdis.LastWriteTime.ToString());
                    item.ImageIndex = 1;
                    item.Tag = "D";
                }

                fiArr = di.GetFiles();
                foreach (FileInfo fis in fiArr)
                {
                    item = listClient.Items.Add(fis.Name);
                    item.SubItems.Add(fis.Length.ToString());
                    item.SubItems.Add(fis.LastWriteTime.ToString());
                    item.ImageIndex = 2;
                    item.Tag = "F";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void viewServer_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            string path = e.Node.FullPath;
            ListViewItem item;
            listServer.Items.Clear();

            try
            {
                DirInfo dirInfo = new DirInfo(path);
                dirInfo.Type = (int)PacketType.ReqDirList;
                Packet.Serialize(dirInfo).CopyTo(this.sendBuf, 0);
                this.Send();

                Recv();
                DirList dirList = (DirList)Packet.Deserialize(this.recvBuf);

                foreach (string dir in dirList.dirList)
                {
                    item = listServer.Items.Add(dir);
                    item.ImageIndex = 1;
                    item.Tag = "D";
                }

                dirInfo.Type = (int)PacketType.ReqFileList;
                Packet.Serialize(dirInfo).CopyTo(this.sendBuf, 0);
                this.Send();

                Recv();
                FileList fileList = (FileList)Packet.Deserialize(this.recvBuf);
                foreach (FileMeta meta in fileList.fileList)
                {
                    item = listServer.Items.Add(meta.fileName);
                    item.SubItems.Add(meta.fileLength.ToString());
                    item.SubItems.Add(meta.lastModified);
                    item.ImageIndex = 2;
                    item.Tag = "F";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void listClient_DoubleClick(object sender, EventArgs e)
        {
            OpenFiles();
        }


        private void listClient_Click(object sender, EventArgs e)
        {
            SelectFiles();
        }

        private void SendFile(string fileName)
        {
            FileInfo f = new FileInfo(fileName);
            string md5 = PUtility.CalculateMD5(fileName);
            FileMeta fileMeta = new FileMeta(
                f.Length,
                fileName,
                md5);
            fileMeta.Type = (int)PacketType.FileMeta;
            Packet.Serialize(fileMeta).CopyTo(this.sendBuf, 0);
            this.Send();

            Recv();
            ACK ack = (ACK)Packet.Deserialize(this.recvBuf);
            if (ack.isOK == false)
            {   // same name file
                ack = new ACK(false);
                if (MessageBox.Show("A same name already exists.\r\n\r\n" +
                    "Do you want version control?", "FM", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    ack.isOK = true;
                }
                Packet.Serialize(ack).CopyTo(this.sendBuf, 0);
                this.Send();
            }

            if (f.Length != 0)
            {
                byte[] file = File.ReadAllBytes(fileName);

                this.stream.Write(file, 0, file.Length);
                this.stream.Flush();
            }

            Recv();

            ack = (ACK)Packet.Deserialize(this.recvBuf);
            if (ack.isOK)
            {
                //MessageBox.Show("성공적으로 업로드하였습니다!");
            }
            else
            {
                MessageBox.Show("파일 업로드 실패");
            }
        }

        private void RecvFile(string fileName)
        {
            FileMeta reqFile = new FileMeta(
                0,
                fileName,
                "");
            reqFile.Type = (int)PacketType.ReqFile;
            Packet.Serialize(reqFile).CopyTo(this.sendBuf, 0);
            this.Send();

            Recv();

            FileMeta fileMeta = (FileMeta)Packet.Deserialize(this.recvBuf);

            FileStream fs = File.Open(dirPath + "\\" +
                Path.GetFileName(fileMeta.fileName), FileMode.Create);
            BinaryWriter writer = new BinaryWriter(fs);

            int nRecv = 0;
            long nRemain = fileMeta.fileLength;
            byte[] buff = new byte[PUtility.BUF_LEN];
            try
            {
                if (fileMeta.fileLength != 0)
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

            ACK ackSuc = new ACK();
            ackSuc.Type = (int)PacketType.ACK;
            //MessageBox.Show("Recv Success");

            Packet.Serialize(ackSuc).CopyTo(this.sendBuf, 0);
            this.Send();
        }
        
        private void btnUpload_Click(object sender, EventArgs e)
        {
            if (listClient.SelectedItems[0].Tag.ToString() == "F")
            {
                SendFile(filePath);
            }
            else if (listClient.SelectedItems[0].Tag.ToString() == "D")
            {
                FileMeta finfo = new FileMeta(0, fdirPath + ".tmp.zip", "");
                finfo.Type = (int)PacketType.SendDir;
                Packet.Serialize(finfo).CopyTo(this.sendBuf, 0);
                this.Send();
                PUtility.CompressDirectoryTmp(fdirPath);
                SendFile(fdirPath + ".tmp.zip");
                File.Delete(fdirPath + ".tmp.zip");
            }
            var item = viewServer.SelectedNode;
            viewServer.SelectedNode = null;
            viewServer.SelectedNode = item;
        }

        private void FMClient_FormClosed(object sender, FormClosedEventArgs e)
        {
            closeConnect();
        }

        private void FMClient_Load(object sender, EventArgs e)
        {
            PopulateClientTreeView();
            PopulateServerTreeView();
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            if (listServer.SelectedItems[0].Tag.ToString() == "F")
                RecvFile(servFile);
            else if (listServer.SelectedItems[0].Tag.ToString() == "D")
            {
                FileMeta finfo = new FileMeta(0, servFile, "");
                finfo.Type = (int)PacketType.ReqDir;
                Packet.Serialize(finfo).CopyTo(this.sendBuf, 0);
                this.Send();
                RecvFile(servFile + ".tmp.zip");
                PUtility.ExtractDirectoryTmp(dirPath + "\\" + Path.GetFileName(servFile + ".tmp.zip"));
                File.Delete(dirPath + "\\" + Path.GetFileName(servFile + ".tmp.zip"));
            }
            var item = viewClient.SelectedNode;
            viewClient.SelectedNode = null;
            viewClient.SelectedNode = item;
        }

        private void listServer_Click(object sender, EventArgs e)
        {
            SelectServerFiles();
        }
    }
}
