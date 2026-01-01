using Avalonia;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Net.Http;
using bbaLib;
using System.Threading.Tasks;
using System.Text;
using System.ComponentModel;

namespace S5Updater2
{
    internal static class MainUpdater
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();


        internal static void Copy(string sourceDirectory, string targetDirectory, IEnumerable<string> exclude, ProgressDialog.ReportProgressDel r)
        {
            DirectoryInfo diSource = new(sourceDirectory);
            DirectoryInfo diTarget = new(targetDirectory);

            CopyAll(diSource, diTarget, exclude, r);
        }

        private static void CopyAll(DirectoryInfo source, DirectoryInfo target, IEnumerable<string> exclude, ProgressDialog.ReportProgressDel r)
        {
            Directory.CreateDirectory(target.FullName);

            foreach (FileInfo fi in source.GetFiles())
            {
                // ReSharper disable once PossibleMultipleEnumeration
                if (exclude.Contains(fi.Name))
                    continue;
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
                r(0, 100, fi.Name, null);
            }

            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                // ReSharper disable once PossibleMultipleEnumeration
                if (exclude.Contains(diSourceSubDir.Name))
                    continue;
                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                // ReSharper disable once PossibleMultipleEnumeration
                CopyAll(diSourceSubDir, nextTargetSubDir, exclude, r);
            }
        }

        internal static void PackDirToBba(string sourcedir, string targetfile, IEnumerable<string> ignore, ProgressDialog.ReportProgressDel r)
        {
            using BbaArchive a = new();
            a.ReadFromFolder(sourcedir, (s) =>
            {
                r(s.Progress, 100, s.AdditionalString, null);
            }, false, "", (p) => !ignore.Any(p.StartsWith));
            a.SearchAndLinkDuplicates();
            string? path = Path.GetDirectoryName(targetfile);
            if (path != null)
                Directory.CreateDirectory(path);
            a.WriteToBba(targetfile, (s) =>
            {
                r(s.Progress, 100, s.AdditionalString, null);
            }, true, _ => true);
        }

        internal static bool IsDirNotExistingOrEmpty(string dir)
        {
            return !Directory.Exists(dir) || !Directory.EnumerateFileSystemEntries(dir).Any();
        }

        internal static async Task DownloadFile(string uri, string file, ProgressDialog.ReportProgressDel r)
        {
            if (File.Exists(file))
                File.Delete(file);
            await using FileStream s = new(file, FileMode.Create, FileAccess.Write);
            await DownloadAsync(uri, r, s);
        }

        internal static async Task DownloadAsync(string uri, ProgressDialog.ReportProgressDel r, Stream target)
        {
            using HttpClient cl = new();
            using HttpResponseMessage res = await cl.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
            long len = res.Content.Headers.ContentLength ?? -1;
            await using Stream source = await res.Content.ReadAsStreamAsync();
            int buffsize = 1024;
            byte[] buffer = new byte[buffsize];
            long totalBytesRead = 0;
            int bytesRead;
            while ((bytesRead = await source.ReadAsync(buffer).ConfigureAwait(false)) != 0)
            {
                await target.WriteAsync(buffer.AsMemory(0, bytesRead)).ConfigureAwait(false);
                totalBytesRead += bytesRead;
                r((int)totalBytesRead, (int)len, null, null);
            }
        }

        internal static async Task<byte[]> DownloadFileBytes(string uri, ProgressDialog.ReportProgressDel r)
        {
            using MemoryStream s = new();
            await DownloadAsync(uri, r, s);
            return s.ToArray();
        }

        internal static async Task<string> DownloadFileString(string uri, ProgressDialog.ReportProgressDel r)
        {
            using MemoryStream s = new();
            await DownloadAsync(uri, r, s);
            s.Position = 0;
            using StreamReader sr = new(s, Encoding.UTF8);
            return await sr.ReadToEndAsync();
        }

        internal static bool IsFolder(this ZipArchiveEntry entry)
        {
            return entry.FullName.EndsWith('/');
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

        public static void CreateLinkPowershell(string lnk, string to, string para = "", Action<string>? log = null)
        {
            ProcessStartInfo i = new()
            {
                FileName = "powershell",
                Arguments = $"$WshShell = New-Object -comObject WScript.Shell\n$Shortcut = $WshShell.CreateShortcut(\\\"{lnk}\\\")\n$Shortcut.TargetPath = \\\"{to}\\\"\n$shortcut.Arguments = \\\"{para}\\\"\n$Shortcut.Save()",
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };
            Process pr = new()
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

        public enum AWExitCode
        {
            Success = 0,
            Invalid = 1,
            Unknown,
            InvalidOS,
            AccessDenied,
        }

        public static AWExitCode RunAdminReg(string k, string k2, object? v)
        {
            string vs;
            string vt;
            if (v is string s)
            {
                vs = s;
                vt = "string";
            }
            else if (v is int d)
            {
                vs = d.ToString();
                vt = "dword";
            }
            else
            {
                return AWExitCode.Invalid;
            }
            ProcessStartInfo i = new()
            {
                FileName = ".\\S5UpdaterAdminWorker.exe",
                CreateNoWindow = true,
                UseShellExecute = true,
                Verb = "runas",
            };
            i.ArgumentList.Add("SetReg");
            i.ArgumentList.Add(k);
            i.ArgumentList.Add(k2);
            i.ArgumentList.Add(vs);
            i.ArgumentList.Add(vt);
            Process pr = new()
            {
                StartInfo = i
            };
            try
            {
                pr.Start();
                pr.WaitForExit();
                return (AWExitCode)pr.ExitCode;
            }
            catch (Win32Exception)
            {
                return AWExitCode.AccessDenied;
            }
        }
        public static AWExitCode RunFullAccess(string d)
        {
            ProcessStartInfo i = new()
            {
                FileName = ".\\S5UpdaterAdminWorker.exe",
                CreateNoWindow = true,
                UseShellExecute = true,
                Verb = "runas",
            };
            i.ArgumentList.Add("FullAccess");
            i.ArgumentList.Add(d);
            Process pr = new()
            {
                StartInfo = i
            };
            try
            {
                pr.Start();
                pr.WaitForExit();
                return (AWExitCode)pr.ExitCode;
            }
            catch (Win32Exception)
            {
                return AWExitCode.AccessDenied;
            }
        }
        public static bool HasWriteAccess(string dir)
        {
            try
            {
                using FileStream fs = File.Create(Path.Combine(dir, "updatertestfile"), 1, FileOptions.DeleteOnClose);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsControlledFolderAccessForbidden()
        {
            if (!OperatingSystem.IsWindows())
                return false;
            ProcessStartInfo i = new()
            {
                FileName = "powershell",
                Arguments = $"$p = Get-MpPreference\n$p.EnableControlledFolderAccess\n",
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };
            Process pr = new()
            {
                StartInfo = i
            };
            pr.Start();
            string s = pr.StandardOutput.ReadToEnd();
            pr.WaitForExit();
            if (s == "")
                return false;
            if (s.Trim() == "0")
                return false;
            i = new()
            {
                FileName = "powershell",
                Arguments = $"$p = Get-MpPreference\n$p.ControlledFolderAccessAllowedApplications\n",
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };
            pr = new()
            {
                StartInfo = i
            };
            pr.Start();
            string? loc = Environment.ProcessPath;
            if (loc != null)
                return false;
            for (string? l = pr.StandardOutput.ReadLine(); l != null; l = pr.StandardOutput.ReadLine())
            {
                if (loc == l.Trim())
                    return true;
            }
            pr.WaitForExit();
            return true;
        }

        public static AWExitCode SetControlledFolderAccessException(string? path = null)
        {
            path ??= Environment.ProcessPath;
            if (path == null)
                return AWExitCode.Success;
            ProcessStartInfo i = new()
            {
                FileName = "powershell",
                Arguments = $"Add-MpPreference -ControlledFolderAccessAllowedApplications \"{path}\"\n",
                CreateNoWindow = true,
                UseShellExecute = true,
                Verb = "runas",
            };
            Process pr = new()
            {
                StartInfo = i
            };
            try
            {
                pr.Start();
                pr.WaitForExit();
                return (AWExitCode)pr.ExitCode;
            }
            catch (Win32Exception)
            {
                return AWExitCode.AccessDenied;
            }
        }

        public static IEnumerable<T> NotNull<T>(this IEnumerable<T?> l)
        {
            foreach (var v in l)
                if (v is not null)
                    yield return v;
        }
    }
}
