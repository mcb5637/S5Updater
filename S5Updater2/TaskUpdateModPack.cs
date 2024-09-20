using bbaLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S5Updater2
{
    internal class TaskUpdateModPack : IUpdaterTask
    {
        internal required MainWindow MM;
        internal Status Status;
        internal required IEnumerable<string> ModPacks;

        public async Task Work(ProgressDialog.ReportProgressDel r)
        {
            Status = Status.Ok;
            try
            {
                if (MM.Reg.GoldPath == null)
                    throw new NullReferenceException();
                string outPath = Path.Combine(MM.Reg.GoldPath, "ModPacks");
                using BbaArchive a = new();
                foreach (string n in ModPacks)
                {
                    r(0, 100, n, n);
                    a.Clear();
                    a.ReadBba(Path.Combine(outPath, n+".bba"));
                    S5ModPackInfo? i = a.GetModPackInfo(n);
                    if (i == null)
                        continue;
                    if (i.VersionURL == null || i.UpdateURL == null)
                        continue;
                    if (i.Version == await MainUpdater.DownloadFileString(i.UpdateURL, r))
                    {
                        r(0, 100, "not needed", "not needed");
                        continue;
                    }
                    string tmp = Path.Combine(MM.Reg.GoldPath, n+".bba");
                    await MainUpdater.DownloadFile(i.UpdateURL, tmp, r);
                    TaskInstallMap inst = new() {
                        MM = MM,
                        Files = [tmp],
                        TargetPath = MM.Reg.GoldPath,
                        AllowModPacks = true,
                    };
                    await inst.Work(r);
                    File.Delete(tmp);
                    if (inst.Status != Status.Ok)
                        Status = inst.Status;
                }
            }
            catch (Exception e)
            {
                r(0, 100, e.ToString(), e.ToString());
                Status = Status.Error;
            }
        }
    }
    internal class TaskScanModPack : IUpdaterTask
    {
        internal required MainWindow MM;
        internal Status Status;
        internal List<string> ModPacks = [];

        public Task Work(ProgressDialog.ReportProgressDel r)
        {
            Status = Status.Ok;
            try
            {
                if (MM.Reg.GoldPath == null)
                    throw new NullReferenceException();
                ModPacks.Clear();
                string outPath = Path.Combine(MM.Reg.GoldPath, "ModPacks");
                using BbaArchive a = new();
                foreach (FileInfo mp in new DirectoryInfo(outPath).EnumerateFiles())
                {
                    r(0, 100, mp.Name, null);
                    if (mp.Extension != ".bba")
                        continue;
                    string n = Path.GetFileNameWithoutExtension(mp.FullName);
                    a.Clear();
                    a.ReadBba(mp.FullName);
                    S5ModPackInfo? i = a.GetModPackInfo(n);
                    if (i == null)
                        continue;
                    if (i.VersionURL == null || i.UpdateURL == null)
                        continue;
                    ModPacks.Add(n);
                }
            }
            catch (Exception e)
            {
                r(0, 100, e.ToString(), e.ToString());
                Status = Status.Error;
            }
            return Task.CompletedTask;
        }
    }
}
