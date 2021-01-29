using System;
using System.Collections.Generic;
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

[assembly:AssemblyVersion("0.1.0.0")]

namespace S5Updater
{
    static class MainUpdater
    {
        [STAThread]
        public static void Main()
        {
            //CultureInfo i = new CultureInfo("en-us");
            //CultureInfo.DefaultThreadCurrentCulture = i;
            //CultureInfo.DefaultThreadCurrentUICulture = i;
            Application.EnableVisualStyles();
            Application.Run(new MainMenu());
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

        internal static int IndexOfArrayElement<T, E>(this T[] array, E o, Func<T, E> f) where E: class
        {
            for (int i = 0; i < array.Length; i++)
                if (f(array[i]).Equals(o))
                    return i;
            return -1;
        }

        internal static bool IsDirNotExistingOrEmpty(string dir)
        {
            return !Directory.Exists(dir) || !Directory.EnumerateFileSystemEntries(dir).Any();
        }

        internal static void DownlaodFile(string uri, string file, ProgressDialog.ReportProgressDel r)
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
    }
}
