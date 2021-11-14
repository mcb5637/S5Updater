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
    class TaskUpdateHook : IUpdaterTask
    {
        internal MainMenu MM;
        internal int Status;

        private static readonly string HookRepo = "https://github.com/mcb5637/S5BinkHook/releases/latest/download/";
        private static readonly string[] Extras = new string[] { "bin", "extra1\\bin", "extra2\\bin" };

        public void Work(ProgressDialog.ReportProgressDel r)
        {
            Status = MainMenu.Status_OK;
            List<FileWithHash> hashes = LoadHashes(r);
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
                r(0, Resources.TaskHook_Patching);
                string patchfile = Path.Combine(MM.Reg.GoldPath, "Tmp_hook.zip");
                MainUpdater.DownlaodFile(HookRepo + "bin.zip", patchfile, r);
                foreach (string e in Extras)
                {
                    if (File.Exists(Path.Combine(MM.Reg.GoldPath, e, "settlershok.exe")))
                    {
                        string bw = Path.Combine(MM.Reg.GoldPath, e, "binkw32.dll");
                        string bwor = Path.Combine(MM.Reg.GoldPath, e, "binkw32_orig.dll");
                        if (!File.Exists(bwor))
                            File.Copy(bw, bwor);
                        if (File.Exists(bw))
                            File.SetAttributes(bw, FileAttributes.Normal);
                        using (ZipArchive a = ZipFile.OpenRead(patchfile))
                        {
                            foreach (ZipArchiveEntry en in a.Entries)
                            {
                                if (en.IsFolder())
                                {
                                    continue;
                                }
                                string destinationFileName = Path.Combine(MM.Reg.GoldPath, e, en.FullName);
                                Directory.CreateDirectory(Path.GetDirectoryName(destinationFileName));
                                en.ExtractToFile(destinationFileName, true);
                            }
                        }
                    }
                }
                File.Delete(patchfile);
            }
            catch (Exception e)
            {
                r(0, e.ToString());
                Status = MainMenu.Status_Error;
            }
        }

        private bool CheckHashes(List<FileWithHash> hashes, ProgressDialog.ReportProgressDel r)
        {
            try
            {
                if (!File.Exists(Path.Combine(MM.Reg.GoldPath, "bin\\binkw32_orig.dll")))
                    return true;
                foreach (FileWithHash f in hashes)
                {
                    string path = Path.Combine(MM.Reg.GoldPath, "bin", f.File);
                    if (!File.Exists(path))
                        return true;
                    if (!MM.Valid.GetFileHash(path).Equals(f.Hash))
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
                r(0, Resources.TaskHook_Validating);
                byte[] d = MainUpdater.DownlaodFileBytes(HookRepo + "hashes.txt", r);
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

        static public bool IsInstalled(string path)
        {
            return File.Exists(Path.Combine(path, "bin\\S5CppLogic.dll"));
        }
        static public bool IsEnabled(string path)
        {
            if (!File.Exists(Path.Combine(path, "bin\\S5CppLogic.dll")))
                return false;
            string opt = Path.Combine(path, "bin\\CppLogicOptions.txt");
            if (!File.Exists(opt))
                return true;
            using (StreamReader s = new StreamReader(opt))
            {
                for (string line = s.ReadLine(); line != null; line = s.ReadLine())
                {
                    if (line.StartsWith("DoNotLoad"))
                    {
                        return line.EndsWith("0");
                    }
                }
            }
            return true;
        }
        static public void SetEnabled(string path, bool enabled)
        {
            foreach (string e in Extras)
            {
                using (StreamWriter w = new StreamWriter(Path.Combine(path, e, "CppLogicOptions.txt"), false))
                {
                    w.Write("DoNotLoad=");
                    w.WriteLine(enabled ? "0" : "1");
                }
            }
        }

        private class FileWithHash
        {
            internal string Hash, File;
        }
    }
}
