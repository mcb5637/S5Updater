using S5Updater.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace S5Updater
{
    public partial class ProgressDialog : Form
    {
        internal delegate void ReportProgressDel(int p, string l);

        private IUpdaterTask Task;
        private MainMenu MM;

        public ProgressDialog()
        {
            InitializeComponent();
        }

        private void ProgressDialog_Load(object sender, EventArgs e)
        {
            Text = Resources.TitleMainMenu;
        }

        internal void ShowWorkDialog(IUpdaterTask task, MainMenu mm)
        {
            Task = task;
            MM = mm;
            Bar.Value = 0;
            ShowDialog();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Task.Work(ReportProgress);
        }

        private void ReportProgress(int p, string l)
        {
            Worker.ReportProgress(p, l);
        } 

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage > -1)
            {
                Bar.Value = e.ProgressPercentage;
                if (e.UserState != null)
                    MM.Log((string)e.UserState);
            }
            else
            {
                Txt_Log.Text = (string)e.UserState;
            }
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DialogResult = DialogResult.OK;
            Task = null;
            Close();
        }

        private void ProgressDialog_Shown(object sender, EventArgs e)
        {
            Worker.RunWorkerAsync();
        }

        private void ProgressDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Task != null)
                e.Cancel = true;
        }
    }
}
