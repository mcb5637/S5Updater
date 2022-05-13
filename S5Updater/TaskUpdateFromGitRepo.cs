using S5Updater.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S5Updater
{
    abstract class TaskUpdateFromGitRepo : IUpdaterTask
    {
        internal MainMenu MM;
        internal int Status;

        protected static readonly string[] Extras = new string[] { "bin", "extra1\\bin", "extra2\\bin" };
        protected abstract string HashesAdress { get; }
        protected abstract string ZipAdress { get; }
        protected abstract string ValidatingLog { get; }

        protected virtual bool ForceUpdate()
        {
            return false;
        }
        protected virtual void PreUpdate()
        {

        }
        protected virtual void PostUpdate(ProgressDialog.ReportProgressDel r)
        {

        }
        protected virtual string FileNameRedirect(string name)
        {
            return name;
        }

        public void Work(ProgressDialog.ReportProgressDel r)
        {
            Status = MainMenu.Status_OK;
            IEnumerable<FileWithHash> hashes = LoadHashes(r);
            if (Status != MainMenu.Status_OK)
                return;
            bool upNeeded = CheckHashes(hashes, r);
            if (Status != MainMenu.Status_OK)
                return;
            if (!upNeeded)
            {
                r(0, Resources.TaskHook_Ok);
                return;
            }
            Update(r);
        }

        private void Update(ProgressDialog.ReportProgressDel r)
        {
            try
            {
                PreUpdate();
                r(0, Resources.TaskHook_Patching);
                string patchfile = Path.Combine(MM.Reg.GoldPath, "Tmp_gitrepo.zip");
                MainUpdater.DownlaodFile(ZipAdress, patchfile, r);
                foreach (string e in Extras)
                {
                    if (File.Exists(Path.Combine(MM.Reg.GoldPath, e, "settlershok.exe")))
                    {
                        using (ZipArchive a = ZipFile.OpenRead(patchfile))
                        {
                            foreach (ZipArchiveEntry en in a.Entries)
                            {
                                if (en.IsFolder())
                                {
                                    continue;
                                }
                                string destinationFileName = Path.Combine(MM.Reg.GoldPath, e, FileNameRedirect(en.FullName));
                                Directory.CreateDirectory(Path.GetDirectoryName(destinationFileName));
                                en.ExtractToFile(destinationFileName, true);
                            }
                        }
                    }
                }
                File.Delete(patchfile);
                PostUpdate(r);
            }
            catch (Exception e)
            {
                r(0, e.ToString());
                Status = MainMenu.Status_Error;
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
                    string path = Path.Combine(MM.Reg.GoldPath, "bin", f.File);
                    if (!File.Exists(path))
                        return true;
                    if (!InstallValidator.GetFileHash(path).Equals(f.Hash))
                    {
                        r(0, f.File);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                r(0, e.ToString());
                Status = MainMenu.Status_Error;
                return true;
            }
        }

        private List<FileWithHash> LoadHashes(ProgressDialog.ReportProgressDel r)
        {
            try
            {
                r(0, ValidatingLog);
                byte[] d = MainUpdater.DownlaodFileBytes(HashesAdress, r);
                List<FileWithHash> files = new List<FileWithHash>();
                using (StreamReader re = new StreamReader(new MemoryStream(d)))
                {
                    string s = re.ReadLine();
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
                r(0, e.ToString());
                Status = MainMenu.Status_Error;
                return null;
            }
        }

        private class FileWithHash
        {
            internal string Hash, File;
        }
    }
}
