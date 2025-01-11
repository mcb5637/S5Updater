using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S5Updater2
{
    internal class TaskUpdate106 : IUpdaterTask
    {
        internal required MainWindow MM;
        internal Status Status;

        public async Task Work(ProgressDialog.ReportProgressDel r)
        {
            Status = Status.Ok;
            await PatchExe(r);
        }

        private async Task PatchExe(ProgressDialog.ReportProgressDel r)
        {
            try
            {
                if (MM.Reg.GoldPath == null)
                    throw new NullReferenceException();
                await MM.EnsureWriteAccess(MM.Reg.GoldPath);
                string patchfile = Path.Combine(MM.Reg.GoldPath, "Tmp_w10cu.zip");
                r(0, 100, Res.Prog_U106_Download, Res.Prog_U106_Download);
                await MainUpdater.DownloadFile("https://github.com/mcb5637/s5winfix/archive/refs/heads/master.zip", patchfile, r);
                r(100, 100, Res.Done, Res.Done);
                r(0, 100, Res.Prog_U106_Copy, Res.Prog_U106_Copy);
                string settlershok = Path.Combine(MM.Reg.GoldPath, "bin\\settlershok.exe");
                string mapeditor = Path.Combine(MM.Reg.GoldPath, "bin\\shokmapeditor.exe");
                using (ZipArchive a = ZipFile.OpenRead(patchfile))
                {
                    CheckReadOnly(settlershok);
                    CheckReadOnly(mapeditor);
                    a.GetEntry("s5winfix-master/settlershok_w10cu.exe")?.ExtractToFile(settlershok, true);
                    a.GetEntry("s5winfix-master/shokmapeditor_w10cu.exe")?.ExtractToFile(mapeditor, true);
                }
                CopyExtra(settlershok, mapeditor, "extra1\\bin");
                CopyExtra(settlershok, mapeditor, "extra2\\bin");
                File.Delete(patchfile);
                r(100, 100, Res.Done, Res.Done);
            }
            catch (Exception e)
            {
                r(0, 100, null, e.ToString());
                Status = Status.Error;
            }
        }

        private void CopyExtra(string settlershok, string mapeditor, string extra)
        {
            if (MM.Reg.GoldPath == null)
                throw new NullReferenceException();
            string extrabin = Path.Combine(MM.Reg.GoldPath, extra);
            if (Directory.Exists(extrabin))
            {
                string settlershokex = Path.Combine(extrabin, "settlershok.exe");
                string mapeditorex = Path.Combine(extrabin, "shokmapeditor.exe");
                CheckReadOnly(settlershokex);
                CheckReadOnly(mapeditorex);
                File.Copy(settlershok, settlershokex, true);
                File.Copy(mapeditor, mapeditorex, true);
            }
        }

        private static void CheckReadOnly(string f)
        {
            if (File.Exists(f))
                File.SetAttributes(f, FileAttributes.Normal);
        }
    }
}
