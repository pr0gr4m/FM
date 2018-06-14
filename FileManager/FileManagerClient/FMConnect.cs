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
    public partial class FMConnect : Form
    {
        private NetworkStream stream = null;
        private TcpClient client = null;

        private byte[] sendBuf = new byte[PUtility.BUF_LEN];
        private byte[] recvBuf = new byte[PUtility.BUF_LEN];

        bool isConnect = false;

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

        public FMConnect()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            this.client = new TcpClient();
            try
            {
                this.client.Connect(txtIP.Text, PUtility.PORT_NUM);
            }
            catch
            {
                MessageBox.Show("접속 에러");
                return;
            }
            this.stream = this.client.GetStream();
            this.btnConnect.Enabled = false;
            this.btnConnect.Text = "Connected";
            InitCase();
        }

        private void InitCase()
        {
            while (true)
            {
                Recv();
                Packet packet = (Packet)Packet.Deserialize(this.recvBuf);
                if ((int)packet.Type == (int)PacketType.EOP)
                    break;

                Case c = (Case)Packet.Deserialize(this.recvBuf);
                listCase.Items.Add(c.caseName);
            }
            this.txtCase.Enabled = true;
            this.btnNew.Enabled = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (stream != null)
                stream.Close();
            if (client != null)
                client.Close();
            this.Close();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (String.Compare(txtCase.Text, "") == 0)
            {
                MessageBox.Show("Enter Case Name");
                return;
            }
            Case c = new Case(txtCase.Text);
            c.Type = (int)PacketType.Case;
            Packet.Serialize(c).CopyTo(this.sendBuf, 0);
            this.Send();

            Recv();
            ACK ack = (ACK)Packet.Deserialize(this.recvBuf);
            if (ack.isOK)
            {   // Success
                listCase.Items.Add(txtCase.Text);
            }
            else
            {
                MessageBox.Show("The case already exists.");
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (listCase.SelectedItem == null)
            {
                MessageBox.Show("Select a case");
                return;
            }

            Case c = new Case(listCase.GetItemText(listCase.SelectedItem));
            c.Type = (int)PacketType.CaseSelected;
            Packet.Serialize(c).CopyTo(this.sendBuf, 0);
            this.Send();

            FMClient fmclient = new FMClient(client, stream, CloseConnect,
                c.caseName);
            fmclient.Show();
            this.Hide();
        }

        private void CloseConnect()
        {
            this.Close();
        }
    }
}
