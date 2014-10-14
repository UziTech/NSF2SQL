/*
 * Author: Tony Brix, http://tonybrix.info
 * License: MIT
 */ 
using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Taskbar;

public class ProgressDialog
{
    private BackgroundWorker worker = new BackgroundWorker();

    public event CancelEventHandler Canceled;
    public event RunWorkerCompletedEventHandler Completed;
    public event ProgressChangedEventHandler ProgressChanged;
    public event DoWorkEventHandler DoWork;

    private dialogForm dialog = new dialogForm();

    public ProgressDialog()
    {
        worker = new BackgroundWorker();
        worker.ProgressChanged += Worker_ProgressChanged;
        worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        worker.DoWork += worker_DoWork;
        worker.WorkerSupportsCancellation = true;
        worker.WorkerReportsProgress = true;
        dialog.Canceled += dialog_Canceled;
    }

    void dialog_Canceled(object sender, CancelEventArgs e)
    {
        worker.CancelAsync();
        if (Canceled != null)
        {
            Canceled(this, e);
        }
    }

    void worker_DoWork(object sender, DoWorkEventArgs e)
    {
        if (DoWork != null)
        {
            DoWork(this, e);
            e.Cancel = Cancelled;
        }
        else
        {
            MessageBox.Show("No work to do!", "No Work", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public void Run()
    {
        if (TaskbarManager.IsPlatformSupported)
        {
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);
        }
        worker.RunWorkerAsync();
        dialog.ShowDialog();
    }

    public void Run(object argument)
    {
        if (TaskbarManager.IsPlatformSupported)
        {
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);
        }
        worker.RunWorkerAsync(argument);
        dialog.ShowDialog();
    }

    private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        if (Completed != null)
        {
            Completed(this, e);
        }
        if (TaskbarManager.IsPlatformSupported)
        {
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
        }
        dialog.Close();
    }

    private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
        if (ProgressChanged != null)
        {
            ProgressChanged(this, e);
        }
    }

    public void ReportProgress(int percentProgress, object userState)
    {
        worker.ReportProgress(percentProgress, userState);
    }

    public void ReportProgress(int percentProgress)
    {
        worker.ReportProgress(percentProgress);
    }

    public string Title
    {
        get { return dialog.Text; }
        set { dialog.Text = value; }
    }

    public string Message
    {
        get { return dialog.message.Text; }
        set { dialog.message.Text = value; }
    }

    public int Progress
    {
        get { return dialog.progressBar.Value; }
        set {
            if (TaskbarManager.IsPlatformSupported)
            {
                TaskbarManager.Instance.SetProgressValue(value, 100);
            }
            dialog.progressBar.Value = value;
        }
    }

    public bool Cancelled
    {
        get { return worker.CancellationPending; }
    }

    public ProgressBarStyle Style
    {
        get { return dialog.progressBar.Style; }
        set { dialog.progressBar.Style = value; }
    }

    private class dialogForm : Form
    {
        public Label message;
        public ProgressBar progressBar;
        private Button bCancel;
        public event CancelEventHandler Canceled;

        public dialogForm()
        {
            this.message = new Label();
            this.progressBar = new ProgressBar();
            this.bCancel = new Button();
            this.SuspendLayout();
            // 
            // message
            // 
            this.message.AutoSize = true;
            this.message.Location = new Point(12, 9);
            this.message.Name = "message";
            this.message.Size = new Size(54, 13);
            this.message.TabIndex = 0;
            this.message.Text = "Loading...";
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right)));
            this.progressBar.Location = new Point(12, 25);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new Size(421, 23);
            this.progressBar.TabIndex = 1;
            // 
            // bCancel
            // 
            this.bCancel.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Right)));
            this.bCancel.Location = new Point(358, 54);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new Size(75, 23);
            this.bCancel.TabIndex = 2;
            this.bCancel.Text = "Cancel";
            this.bCancel.UseVisualStyleBackColor = true;
            this.bCancel.Click += new EventHandler(this.bCancel_Click);
            // 
            // loadingDialog
            // 
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(445, 89);
            this.ControlBox = false;
            this.Controls.Add(this.bCancel);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.message);
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.Name = "loadingDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Loading";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            if (TaskbarManager.IsPlatformSupported)
            {
                TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Error);
            }
            if (MessageBox.Show("Are you sure you want to cancel?", "Cancel", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                bCancel.Enabled = false;
                this.Text = "Canceling...";
                if (Canceled != null)
                {
                    Canceled(this, new CancelEventArgs(true));
                }
            }
        }
    }
}
