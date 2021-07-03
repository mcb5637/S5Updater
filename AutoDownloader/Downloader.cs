using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoDownloader
{
    public partial class Downloader : Form
    {
        public Downloader()
        {
            InitializeComponent();
        }

        private void Downloader_Load(object sender, EventArgs e)
        {
            Worker.RunWorkerAsync();
        }


        internal void DownlaodFile(string uri, string file)
        {
            if (File.Exists(file))
                File.Delete(file);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            using (WebClient cl = new WebClient())
            {
                cl.DownloadProgressChanged += (_, e) =>
                {
                    Worker.ReportProgress(e.ProgressPercentage);
                };
                Exception err = null;
                cl.DownloadFileCompleted += (_, e) =>
                {
                    err = e.Error;
                    lock (cl)
                    {
                        Monitor.Pulse(cl);
                    }
                };
                lock (cl)
                {
                    cl.DownloadFileAsync(new Uri(uri), file);
                    Monitor.Wait(cl);
                }
                if (err != null)
                    throw new IOException("failed to download file", err);
            }
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                DownlaodFile("https://github.com/mcb5637/S5Updater/releases/latest/download/Release.zip", ".\\update.zip");
                if (File.Exists(".\\AutoDownloader.exe.bak"))
                    File.Delete(".\\AutoDownloader.exe.bak");
                File.Move(".\\AutoDownloader.exe", ".\\AutoDownloader.exe.bak");
                using (ZipArchive a = ZipFile.OpenRead(".\\update.zip"))
                {
                    foreach (ZipArchiveEntry en in a.Entries)
                    {
                        if (en.FullName.EndsWith("/"))
                        {
                            continue;
                        }
                        string destinationFileName = Path.Combine(".\\", en.FullName);
                        Directory.CreateDirectory(Path.GetDirectoryName(destinationFileName));
                        en.ExtractToFile(destinationFileName, true);
                    }
                }
                File.Delete(".\\update.zip");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            PB_Bar.Value = e.ProgressPercentage;
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Close();
            Process.Start(".\\S5Updater.exe");
        }
    }
}
