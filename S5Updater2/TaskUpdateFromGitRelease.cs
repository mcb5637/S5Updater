using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S5Updater2
{
    internal abstract class TaskUpdateFromGitRelease : IUpdaterTask
    {
        internal required MainWindow MM;
        internal Status Status;

        protected static readonly string[] Extras = ["bin", "extra1\\bin", "extra2\\bin"];
        protected abstract string HashesAdress { get; }
        protected abstract string ZipAdress { get; }
        protected abstract string ValidatingLog { get; }

        protected virtual bool ForceUpdate()
        {
            return false;
        }
        protected virtual Task PreUpdate()
        {
            return Task.CompletedTask;
        }
        protected virtual Task PostUpdate(ProgressDialog.ReportProgressDel r)
        {
            return Task.CompletedTask;
        }
        protected virtual string? ZipPathToExtractPath(string e, string name)
        {
            if (MM.Reg.GoldPath == null)
                throw new NullReferenceException();
            return Path.Combine(MM.Reg.GoldPath, e, name);
        }

        public async Task Work(ProgressDialog.ReportProgressDel r)
        {
            Status = Status.Ok;
            IEnumerable<FileWithHash>? hashes = await LoadHashes(r);
            if (Status != Status.Ok)
                return;
            if (hashes == null)
            {
                r(100, 100, "empty hashes", "empty hashes");
                return;
            }
            bool upNeeded = CheckHashes(hashes, r);
            if (Status != Status.Ok)
                return;
            if (!upNeeded)
            {
                r(100, 100, Res.Prog_GRel_NoChange, Res.Prog_GRel_NoChange);
                return;
            }
            await Update(r);
        }

        private async Task Update(ProgressDialog.ReportProgressDel r)
        {
            try
            {
                if (MM.Reg.GoldPath == null)
                    throw new NullReferenceException();
                await MM.EnsureWriteAccess(MM.Reg.GoldPath);
                await PreUpdate();
                r(0, 100, Res.Prog_GRel_Patching, Res.Prog_GRel_Patching);
                string patchfile = Path.Combine(MM.Reg.GoldPath, "Tmp_gitrepo.zip");
                await MainUpdater.DownloadFile(ZipAdress, patchfile, r);
                foreach (string e in Extras)
                {
                    if (File.Exists(Path.Combine(MM.Reg.GoldPath, e, "settlershok.exe")))
                    {
                        using ZipArchive a = ZipFile.OpenRead(patchfile);
                        foreach (ZipArchiveEntry en in a.Entries)
                        {
                            if (en.IsFolder())
                                continue;
                            string? destinationFileName = ZipPathToExtractPath(e, en.FullName);
                            if (destinationFileName == null)
                                continue;
                            string? dirname = Path.GetDirectoryName(destinationFileName);
                            if (dirname != null)
                                Directory.CreateDirectory(dirname);
                            en.ExtractToFile(destinationFileName, true);
                        }
                    }
                }
                File.Delete(patchfile);
                await PostUpdate(r);
            }
            catch (Exception e)
            {
                r(0, 100, e.ToString(), e.ToString());
                Status = Status.Error;
            }
        }
        private bool CheckHashes(IEnumerable<FileWithHash> hashes, ProgressDialog.ReportProgressDel r)
        {
            try
            {
                if (ForceUpdate())
                    return true;
                foreach (FileWithHash f in hashes)
                {
                    string? path = ZipPathToExtractPath("bin", f.File);
                    if (path == null)
                        continue;
                    if (!File.Exists(path))
                        return true;
                    string? hash = InstallValidator.GetFileHash(path);
                    if (hash == null || !hash.Equals(f.Hash))
                    {
                        r(0, 100, f.File, f.File);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                r(0, 100, e.ToString(), e.ToString());
                Status = Status.Error;
                return true;
            }
        }

        private async Task<List<FileWithHash>?> LoadHashes(ProgressDialog.ReportProgressDel r)
        {
            try
            {
                r(0, 100, ValidatingLog, ValidatingLog);
                byte[] d = await MainUpdater.DownloadFileBytes(HashesAdress, r);
                List<FileWithHash> files = [];
                using (StreamReader re = new(new MemoryStream(d)))
                {
                    string? s = re.ReadLine();
                    while (s != null)
                    {
                        string[] spl = s.Split(' ');
                        files.Add(new FileWithHash()
                        {
                            File = spl[0],
                            Hash = spl[1]
                        });
                        s = re.ReadLine();
                    }
                }
                return files;
            }
            catch (Exception e)
            {
                r(0, 100, e.ToString(), e.ToString());
                Status = Status.Error;
                return null;
            }
        }

        private class FileWithHash
        {
            internal required string Hash, File;
        }
    }
}
