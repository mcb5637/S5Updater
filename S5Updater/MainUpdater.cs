using bbaToolS5;
using S5Updater.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

[assembly:AssemblyVersion("0.1.9.11")]

namespace S5Updater
{
    static class MainUpdater
    {
        [STAThread]
        public static void Main()
        {
            if (File.Exists(".\\AutoDownloader.exe.bak"))
                File.Delete(".\\AutoDownloader.exe.bak");
#if !DEBUG
            if (CheckUpdate())
                return;
#endif

            //CultureInfo i = new CultureInfo("en-us");
            //CultureInfo.DefaultThreadCurrentCulture = i;
            //CultureInfo.DefaultThreadCurrentUICulture = i;
            Application.EnableVisualStyles();
            Application.Run(new MainMenu());
        }

        private static bool CheckUpdate()
        {
            try
            {
                byte[] d = DownloadFileBytes("https://github.com/mcb5637/S5Updater/releases/latest/download/versionguid.txt", null);
                if (Encoding.ASCII.GetString(d) != Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId.ToString())
                {
                    if (MessageBox.Show(Resources.Qst_UpdateUpdater, Resources.TitleMainMenu, MessageBoxButtons.YesNo) != DialogResult.Yes)
                        return false;
                    Process.Start(".\\AutoDownloader.exe");
                    return true;
                }
            }
            catch (Exception)
            {
            }
            return false;
        }


        internal static void Copy(string sourceDirectory, string targetDirectory, IEnumerable<string> exclude)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget, exclude);
        }

        private static void CopyAll(DirectoryInfo source, DirectoryInfo target, IEnumerable<string> exclude)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                if (exclude.Contains(fi.Name))
                    continue;
                //Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                if (exclude.Contains(diSourceSubDir.Name))
                    continue;
                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir, exclude);
            }
        }

        internal static void PackDirToBba(string sourcedir, string targetfile)
        {
            BbaArchive a = new BbaArchive();
            a.ReadFromFolder(sourcedir);
            Directory.CreateDirectory(Path.GetDirectoryName(targetfile));
            a.WriteToBba(targetfile);
            a.Clear();
        }

        internal static int IndexOfArrayElement<T, E>(this T[] array, E o, Func<T, E> f) where E: class
        {
            for (int i = 0; i < array.Length; i++)
                if (f(array[i]).Equals(o))
                    return i;
            return -1;
        }
        internal static int IndexOfArrayElement<T>(this T[] array, int o, Func<T, int> f)
        {
            for (int i = 0; i < array.Length; i++)
                if (f(array[i]) == o)
                    return i;
            return -1;
        }

        internal static bool IsDirNotExistingOrEmpty(string dir)
        {
            return !Directory.Exists(dir) || !Directory.EnumerateFileSystemEntries(dir).Any();
        }

        internal static void DownloadFile(string uri, string file, ProgressDialog.ReportProgressDel r)
        {
            if (File.Exists(file))
                File.Delete(file);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            using (WebClient cl = new WebClient())
            {
                cl.DownloadProgressChanged += (_, e) =>
                {
                    r(e.ProgressPercentage, null);
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

        internal static byte[] DownloadFileBytes(string uri, ProgressDialog.ReportProgressDel r)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            using (WebClient cl = new WebClient())
            {
                cl.DownloadProgressChanged += (_, e) =>
                {
                    r?.Invoke(e.ProgressPercentage, null);
                };
                Exception err = null;
                byte[] ret = null;
                cl.DownloadDataCompleted += (_, e) =>
                {
                    err = e.Error;
                    if (err == null)
                        ret = e.Result;
                    lock (cl)
                    {
                        Monitor.Pulse(cl);
                    }
                };
                lock (cl)
                {
                    cl.DownloadDataAsync(new Uri(uri));
                    Monitor.Wait(cl);
                }
                if (err != null)
                    throw new IOException("failed to download file", err);
                return ret;
            }
        }

        internal static bool IsFolder(this ZipArchiveEntry entry)
        {
            return entry.FullName.EndsWith("/");
        }

        internal static string GetParentDir(string path)
        {
            return Path.GetFullPath(Path.Combine(path, ".."));
        }

        public static bool IsSubDirectoryOf(string candidate, string other)
        {
            var isChild = false;
            try
            {
                var candidateInfo = new DirectoryInfo(candidate);
                var otherInfo = new DirectoryInfo(other);

                while (candidateInfo.Parent != null)
                {
                    if (candidateInfo.Parent.FullName == otherInfo.FullName)
                    {
                        isChild = true;
                        break;
                    }
                    else candidateInfo = candidateInfo.Parent;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return isChild;
        }

        public static void CreateLinkPS(string lnk, string to, string para = "", Action<string> log = null)
        {
            ProcessStartInfo i = new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = $"$WshShell = New-Object -comObject WScript.Shell\n$Shortcut = $WshShell.CreateShortcut(\\\"{lnk}\\\")\n$Shortcut.TargetPath = \\\"{to}\\\"\n$shortcut.Arguments = \\\"{para}\\\"\n$Shortcut.Save()",
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };
            Process pr = new Process
            {
                StartInfo = i
            };
            pr.Start();
            pr.WaitForExit();
            if (log != null)
            {
                log("out: " + pr.StandardOutput.ReadToEnd());
                log("err: " + pr.StandardError.ReadToEnd());
            }
        }
    }
}
