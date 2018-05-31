namespace FileManagerClient
{
    partial class FMClient
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.openFileDlg = new System.Windows.Forms.OpenFileDialog();
            this.btnUpload = new System.Windows.Forms.Button();
            this.viewClient = new System.Windows.Forms.TreeView();
            this.listClient = new System.Windows.Forms.ListView();
            this.cliFileName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cliFileSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cliFileDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.viewServer = new System.Windows.Forms.TreeView();
            this.listServer = new System.Windows.Forms.ListView();
            this.servFileName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.servFileSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.servFileDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDlg
            // 
            this.openFileDlg.FileName = "openFileDialog1";
            // 
            // btnUpload
            // 
            this.btnUpload.Location = new System.Drawing.Point(419, 243);
            this.btnUpload.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(42, 41);
            this.btnUpload.TabIndex = 1;
            this.btnUpload.Text = "◀";
            this.btnUpload.UseVisualStyleBackColor = true;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // viewClient
            // 
            this.viewClient.Dock = System.Windows.Forms.DockStyle.Top;
            this.viewClient.Location = new System.Drawing.Point(3, 17);
            this.viewClient.Name = "viewClient";
            this.viewClient.Size = new System.Drawing.Size(394, 294);
            this.viewClient.TabIndex = 3;
            this.viewClient.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.viewClient_BeforeExpand);
            this.viewClient.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.viewClient_BeforeSelect);
            // 
            // listClient
            // 
            this.listClient.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.cliFileName,
            this.cliFileSize,
            this.cliFileDate});
            this.listClient.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.listClient.Location = new System.Drawing.Point(3, 317);
            this.listClient.Name = "listClient";
            this.listClient.Size = new System.Drawing.Size(394, 367);
            this.listClient.TabIndex = 4;
            this.listClient.UseCompatibleStateImageBehavior = false;
            this.listClient.View = System.Windows.Forms.View.Details;
            this.listClient.Click += new System.EventHandler(this.listClient_Click);
            this.listClient.DoubleClick += new System.EventHandler(this.listClient_DoubleClick);
            // 
            // cliFileName
            // 
            this.cliFileName.Text = "File Name";
            this.cliFileName.Width = 157;
            // 
            // cliFileSize
            // 
            this.cliFileSize.Text = "File Size";
            this.cliFileSize.Width = 93;
            // 
            // cliFileDate
            // 
            this.cliFileDate.Text = "Last Modified";
            this.cliFileDate.Width = 208;
            // 
            // viewServer
            // 
            this.viewServer.Dock = System.Windows.Forms.DockStyle.Top;
            this.viewServer.Location = new System.Drawing.Point(3, 17);
            this.viewServer.Name = "viewServer";
            this.viewServer.Size = new System.Drawing.Size(394, 294);
            this.viewServer.TabIndex = 5;
            this.viewServer.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.viewServer_BeforeExpand);
            this.viewServer.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.viewServer_BeforeSelect);
            // 
            // listServer
            // 
            this.listServer.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.servFileName,
            this.servFileSize,
            this.servFileDate});
            this.listServer.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.listServer.Location = new System.Drawing.Point(3, 317);
            this.listServer.Name = "listServer";
            this.listServer.Size = new System.Drawing.Size(394, 367);
            this.listServer.TabIndex = 6;
            this.listServer.UseCompatibleStateImageBehavior = false;
            this.listServer.View = System.Windows.Forms.View.Details;
            // 
            // servFileName
            // 
            this.servFileName.Text = "File Name";
            this.servFileName.Width = 147;
            // 
            // servFileSize
            // 
            this.servFileSize.Text = "File Size";
            this.servFileSize.Width = 81;
            // 
            // servFileDate
            // 
            this.servFileDate.Text = "Last Modified";
            this.servFileDate.Width = 200;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.viewClient);
            this.groupBox1.Controls.Add(this.listClient);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Right;
            this.groupBox1.Location = new System.Drawing.Point(480, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(400, 687);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Client";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.listServer);
            this.groupBox2.Controls.Add(this.viewServer);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(400, 687);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Server";
            // 
            // FMClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(880, 687);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnUpload);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "FMClient";
            this.Text = "FMClient";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FMClient_FormClosed);
            this.Load += new System.EventHandler(this.FMClient_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.OpenFileDialog openFileDlg;
        private System.Windows.Forms.Button btnUpload;
        private System.Windows.Forms.TreeView viewClient;
        private System.Windows.Forms.ListView listClient;
        private System.Windows.Forms.ColumnHeader cliFileName;
        private System.Windows.Forms.ColumnHeader cliFileSize;
        private System.Windows.Forms.ColumnHeader cliFileDate;
        private System.Windows.Forms.TreeView viewServer;
        private System.Windows.Forms.ListView listServer;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ColumnHeader servFileName;
        private System.Windows.Forms.ColumnHeader servFileSize;
        private System.Windows.Forms.ColumnHeader servFileDate;
    }
}

