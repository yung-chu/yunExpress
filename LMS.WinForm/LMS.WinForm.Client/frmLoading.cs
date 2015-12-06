using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace LMS.WinForm.Client
{
    public partial class FrmLoading : Form
    {
        public string Message { set { label1.Text = value; } }

        public FrmLoading(BackgroundWorker worker)
        {
            InitializeComponent();
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Dispose();
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        }

    }

    public class BackgroundLoading
    {
        private readonly BackgroundWorker _worker=new BackgroundWorker();
        private readonly Action _work;
        private readonly string _title = "处理中...";
        private readonly string _message = "处理中...";
        private readonly Form _form;

        public BackgroundLoading(Form form, Action work)
        {
            _form = form;
            _work = work;
        }

        public BackgroundLoading(Form form, Action work, string title)
            : this(form, work)
        {
            _title = title;
        }

        public BackgroundLoading(Form form, Action work, string title, string message)
            : this(form, work, title)
        {
            _message = message;
        }

        void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            _work();
        }

        public void Show()
        {
            _worker.DoWork += _worker_DoWork;
            _worker.RunWorkerAsync();

            FrmLoading frm = new FrmLoading(_worker)
                {
                    StartPosition = FormStartPosition.CenterScreen,
                    Text = _title,
                    Message = _message
                };
            frm.ShowDialog(_form);
        }
    }
}
