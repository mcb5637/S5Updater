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
    class TaskUpdate106 : IUpdaterTask
    {
        internal MainMenu MM;
        internal int Status;

        public void Work(ProgressDialog.ReportProgressDel r)
        {
            Status = MainMenu.Status_OK;
            PatchExe(r);
        }

        private void PatchExe(ProgressDialog.ReportProgressDel r)
        {
            try
            {
                string patchfile = Path.Combine(MM.Reg.GoldPath, "Tmp_w10cu.zip");
                r(0, Resources.TaskUpdate106_DownloadFileExe);
                MainUpdater.DownlaodFile("http://www.dedk.de/patch/w10cu.zip", patchfile, r);
                r(100, Resources.Done);
                r(0, Resources.TaskUpdate106_PatchExe);
                string settlershok = Path.Combine(MM.Reg.GoldPath, "bin\\settlershok.exe");
                string mapeditor = Path.Combine(MM.Reg.GoldPath, "bin\\shokmapeditor.exe");
                using (ZipArchive a = ZipFile.OpenRead(patchfile))
                {
                    a.GetEntry("settlershok_w10cu.exe").ExtractToFile(settlershok, true);
                    a.GetEntry("shokmapeditor_w10cu.exe").ExtractToFile(mapeditor, true);
                }
                CopyExtra(settlershok, mapeditor, "extra1\\bin");
                CopyExtra(settlershok, mapeditor, "extra2\\bin");
                File.Delete(patchfile);
                r(100, Resources.Done);
            }
            catch (Exception e)
            {
                r(0, e.ToString());
                Status = MainMenu.Status_Error;
            }
        }

        private void CopyExtra(string settlershok, string mapeditor, string extra)
        {
            string extrabin = Path.Combine(MM.Reg.GoldPath, extra);
            if (Directory.Exists(extrabin))
            {
                File.Copy(settlershok, Path.Combine(extrabin, "settlershok.exe"), true);
                File.Copy(mapeditor, Path.Combine(extrabin, "shokmapeditor.exe"), true);
            }
        }
    }
}
