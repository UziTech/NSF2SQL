namespace NSF2SQL
{
    partial class ExportTarget
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
            this.ServerRadioButton = new System.Windows.Forms.RadioButton();
            this.FileRadioButton = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.ServerTypeComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ServerRadioButton
            // 
            this.ServerRadioButton.AutoSize = true;
            this.ServerRadioButton.Checked = true;
            this.ServerRadioButton.Location = new System.Drawing.Point(195, 89);
            this.ServerRadioButton.Name = "ServerRadioButton";
            this.ServerRadioButton.Size = new System.Drawing.Size(135, 36);
            this.ServerRadioButton.TabIndex = 1;
            this.ServerRadioButton.TabStop = true;
            this.ServerRadioButton.Text = "Server";
            this.ServerRadioButton.UseVisualStyleBackColor = true;
            // 
            // FileRadioButton
            // 
            this.FileRadioButton.AutoSize = true;
            this.FileRadioButton.Location = new System.Drawing.Point(195, 138);
            this.FileRadioButton.Name = "FileRadioButton";
            this.FileRadioButton.Size = new System.Drawing.Size(99, 36);
            this.FileRadioButton.TabIndex = 2;
            this.FileRadioButton.Text = "File";
            this.FileRadioButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(176, 32);
            this.label1.TabIndex = 3;
            this.label1.Text = "Server Type:";
            // 
            // ServerTypeComboBox
            // 
            this.ServerTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ServerTypeComboBox.FormattingEnabled = true;
            this.ServerTypeComboBox.Items.AddRange(new object[] {
            "MySQL",
            "SQL Server (T-SQL)"});
            this.ServerTypeComboBox.Location = new System.Drawing.Point(195, 10);
            this.ServerTypeComboBox.Name = "ServerTypeComboBox";
            this.ServerTypeComboBox.Size = new System.Drawing.Size(493, 39);
            this.ServerTypeComboBox.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(84, 89);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 32);
            this.label2.TabIndex = 5;
            this.label2.Text = "Target:";
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(206, 207);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(116, 46);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(394, 207);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(116, 46);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // ExportTarget
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(717, 274);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ServerTypeComboBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.FileRadioButton);
            this.Controls.Add(this.ServerRadioButton);
            this.Name = "ExportTarget";
            this.Text = "Export SQL";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton ServerRadioButton;
        private System.Windows.Forms.RadioButton FileRadioButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox ServerTypeComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}