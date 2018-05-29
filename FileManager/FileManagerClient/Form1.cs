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
                //root.ImageIndex = 2;

                if (viewClient.SelectedNode == null)
                    viewClient.SelectedNode = root;
                //root.SelectedImageIndex = root.ImageIndex;
                root.Nodes.Add("");
            }
        }

        private void PopulateServerTreeView()
        {
            MessageBox.Show(caseName);
            TreeNode root = new TreeNode(caseName);
            root.Tag = "C";

            Recv();
            DirList dirList = (DirList)Packet.Deserialize(this.recvBuf);
            
            foreach (string dir in dirList.dirList)
            {
                TreeNode subNode = new TreeNode(dir);
                subNode.Tag = "D";
                root.Nodes.Add(subNode);
            }
            viewServer.Nodes.Add(root);
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
                Process.Start(dirPath + "\\" + item.Text);
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
                    //item.ImageIndex = 0;
                    item.Tag = "D";
                }

                fiArr = di.GetFiles();
                foreach (FileInfo fis in fiArr)
                {
                    item = listClient.Items.Add(fis.Name);
                    item.SubItems.Add(fis.Length.ToString());
                    item.SubItems.Add(fis.LastWriteTime.ToString());
                    //item.ImageIndex = 1;
                    item.Tag = "F";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
        
        private void btnUpload_Click(object sender, EventArgs e)
        {
            FileInfo f = new FileInfo(filePath);
            string md5 = PUtility.CalculateMD5(filePath);
            FileMeta fileMeta = new FileMeta(
                f.Length,
                filePath,
                md5);
            fileMeta.Type = (int)PacketType.FileMeta;
            Packet.Serialize(fileMeta).CopyTo(this.sendBuf, 0);
            this.Send();

            byte[] file = File.ReadAllBytes(filePath);
            
            this.stream.Write(file, 0, file.Length);
            this.stream.Flush();
            
            Recv();

            ACK ack = (ACK)Packet.Deserialize(this.recvBuf);
            if (ack.isOK)
            {
                MessageBox.Show("성공적으로 업로드하였습니다!");
            }
            else
            {
                MessageBox.Show("파일 업로드 실패");
            }
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
    }
}
