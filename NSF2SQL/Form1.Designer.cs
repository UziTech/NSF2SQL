namespace NSF2SQL
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.bGetDatabases = new System.Windows.Forms.Button();
            this.bExportDocuments = new System.Windows.Forms.Button();
            this.bBrowse = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.btnBrowseAttachmentsFolder = new System.Windows.Forms.Button();
            this.gpbAttachments = new System.Windows.Forms.GroupBox();
            this.txbAttachmentsFolder = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.gpbAttachments.SuspendLayout();
            this.SuspendLayout();
            // 
            // bGetDatabases
            // 
            this.bGetDatabases.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bGetDatabases.Location = new System.Drawing.Point(11, 278);
            this.bGetDatabases.Name = "bGetDatabases";
            this.bGetDatabases.Size = new System.Drawing.Size(123, 23);
            this.bGetDatabases.TabIndex = 2;
            this.bGetDatabases.Text = "Search Server";
            this.bGetDatabases.UseVisualStyleBackColor = true;
            this.bGetDatabases.Click += new System.EventHandler(this.bSearchServer_Click);
            // 
            // bExportDocuments
            // 
            this.bExportDocuments.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bExportDocuments.Location = new System.Drawing.Point(269, 278);
            this.bExportDocuments.Name = "bExportDocuments";
            this.bExportDocuments.Size = new System.Drawing.Size(123, 23);
            this.bExportDocuments.TabIndex = 4;
            this.bExportDocuments.Text = "Export Documents";
            this.bExportDocuments.UseVisualStyleBackColor = true;
            this.bExportDocuments.Click += new System.EventHandler(this.bExportDocuments_Click);
            // 
            // bBrowse
            // 
            this.bBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bBrowse.Location = new System.Drawing.Point(140, 278);
            this.bBrowse.Name = "bBrowse";
            this.bBrowse.Size = new System.Drawing.Size(123, 23);
            this.bBrowse.TabIndex = 3;
            this.bBrowse.Text = "Search Computer";
            this.bBrowse.UseVisualStyleBackColor = true;
            this.bBrowse.Click += new System.EventHandler(this.bSearchComputer_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "nsf";
            this.openFileDialog1.Filter = "Lotus Notes Database|*.nsf";
            this.openFileDialog1.Multiselect = true;
            this.openFileDialog1.Title = "Open Lotus Notes Databases (.nsf)";
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.HideSelection = false;
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.Location = new System.Drawing.Point(12, 25);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.ShowNodeToolTips = true;
            this.treeView1.ShowPlusMinus = false;
            this.treeView1.ShowRootLines = false;
            this.treeView1.Size = new System.Drawing.Size(380, 188);
            this.treeView1.TabIndex = 1;
            this.treeView1.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseDoubleClick);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "database");
            this.imageList1.Images.SetKeyName(1, "folder");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "NSF Databases";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "sql";
            this.saveFileDialog1.FileName = "export.sql";
            this.saveFileDialog1.Filter = "SQL File|*.sql";
            // 
            // btnBrowseAttachmentsFolder
            // 
            this.btnBrowseAttachmentsFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnBrowseAttachmentsFolder.Location = new System.Drawing.Point(6, 24);
            this.btnBrowseAttachmentsFolder.Name = "btnBrowseAttachmentsFolder";
            this.btnBrowseAttachmentsFolder.Size = new System.Drawing.Size(40, 23);
            this.btnBrowseAttachmentsFolder.TabIndex = 7;
            this.btnBrowseAttachmentsFolder.Text = "...";
            this.btnBrowseAttachmentsFolder.UseVisualStyleBackColor = true;
            this.btnBrowseAttachmentsFolder.Click += new System.EventHandler(this.btnBrowseAttachmentsFolder_Click);
            // 
            // gpbAttachments
            // 
            this.gpbAttachments.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gpbAttachments.Controls.Add(this.txbAttachmentsFolder);
            this.gpbAttachments.Controls.Add(this.btnBrowseAttachmentsFolder);
            this.gpbAttachments.Location = new System.Drawing.Point(15, 219);
            this.gpbAttachments.Name = "gpbAttachments";
            this.gpbAttachments.Size = new System.Drawing.Size(377, 53);
            this.gpbAttachments.TabIndex = 8;
            this.gpbAttachments.TabStop = false;
            this.gpbAttachments.Text = "Select directory for attachments";
            // 
            // txbAttachmentsFolder
            // 
            this.txbAttachmentsFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txbAttachmentsFolder.Location = new System.Drawing.Point(53, 26);
            this.txbAttachmentsFolder.Name = "txbAttachmentsFolder";
            this.txbAttachmentsFolder.ReadOnly = true;
            this.txbAttachmentsFolder.Size = new System.Drawing.Size(318, 20);
            this.txbAttachmentsFolder.TabIndex = 8;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(404, 310);
            this.Controls.Add(this.gpbAttachments);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.bBrowse);
            this.Controls.Add(this.bExportDocuments);
            this.Controls.Add(this.bGetDatabases);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(420, 167);
            this.Name = "Form1";
            this.Text = "NSF 2 SQL";
            this.gpbAttachments.ResumeLayout(false);
            this.gpbAttachments.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bGetDatabases;
        private System.Windows.Forms.Button bExportDocuments;
        private System.Windows.Forms.Button bBrowse;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button btnBrowseAttachmentsFolder;
        private System.Windows.Forms.GroupBox gpbAttachments;
        private System.Windows.Forms.TextBox txbAttachmentsFolder;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    }
}

