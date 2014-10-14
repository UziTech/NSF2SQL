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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.bGetDatabases = new System.Windows.Forms.Button();
            this.bExportDocuments = new System.Windows.Forms.Button();
            this.bBrowse = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // bGetDatabases
            // 
            this.bGetDatabases.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bGetDatabases.Location = new System.Drawing.Point(11, 257);
            this.bGetDatabases.Name = "bGetDatabases";
            this.bGetDatabases.Size = new System.Drawing.Size(123, 23);
            this.bGetDatabases.TabIndex = 2;
            this.bGetDatabases.Text = "Search Server";
            this.bGetDatabases.UseVisualStyleBackColor = true;
            this.bGetDatabases.Click += new System.EventHandler(this.bGetDatabases_Click);
            // 
            // bExportDocuments
            // 
            this.bExportDocuments.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bExportDocuments.Location = new System.Drawing.Point(269, 257);
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
            this.bBrowse.Location = new System.Drawing.Point(140, 257);
            this.bBrowse.Name = "bBrowse";
            this.bBrowse.Size = new System.Drawing.Size(123, 23);
            this.bBrowse.TabIndex = 3;
            this.bBrowse.Text = "Search Computer";
            this.bBrowse.UseVisualStyleBackColor = true;
            this.bBrowse.Click += new System.EventHandler(this.bBrowse_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "nsf";
            this.openFileDialog1.Filter = "Lotus Notes Database|*.nsf";
            this.openFileDialog1.Multiselect = true;
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.HideSelection = false;
            this.treeView1.Location = new System.Drawing.Point(12, 25);
            this.treeView1.Name = "treeView1";
            this.treeView1.ShowNodeToolTips = true;
            this.treeView1.Size = new System.Drawing.Size(380, 226);
            this.treeView1.TabIndex = 1;
            this.treeView1.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseDoubleClick);
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
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(404, 289);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.bBrowse);
            this.Controls.Add(this.bExportDocuments);
            this.Controls.Add(this.bGetDatabases);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(420, 167);
            this.Name = "Form1";
            this.Text = "NSF 2 SQL";
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
    }
}

