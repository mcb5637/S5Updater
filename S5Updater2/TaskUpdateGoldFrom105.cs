using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S5Updater2
{
    internal class TaskUpdateGoldFrom105 : IUpdaterTask
    {
        internal required MainWindow MM;
        internal Status Status;

        public async Task Work(ProgressDialog.ReportProgressDel r)
        {
            Status = Status.Ok;
            try
            {
                if (MM.Reg.GoldPath == null)
                    throw new NullReferenceException();
                //new Process()
                //{
                //    StartInfo = new ProcessStartInfo()
                //    {
                //        FileName = "https://www.siedler-maps.de/downloads/siedler_dedk/settlers_5_v1.06.exe",
                //    }
                //}.Start();
                //MessageBox.Show(Resources.TaskUpdate105_MsgPatch);
                //bool s = !MM.Valid.IsGold105(MM.Reg.GoldPath);
                //r(100, Resources.TaskUpdate105_Successful + s.ToString());
                string patchfile = Path.Combine(MM.Reg.GoldPath, "Tmp_patch.exe");
                r(0, 100, Res.Prog_U105_DownloadPatch, Res.Prog_U105_DownloadPatch);
                await MainUpdater.DownloadFile("https://www.siedler-maps.de/downloads/siedler_dedk/settlers_5_v1.06.exe", patchfile, r);
                r(0, 100, Res.Prog_U105_WaitForExec, Res.Prog_U105_WaitForExec);
                //MessageBox.Show(Resources.TaskUpdate105_MsgPatch);
                Process p = Process.Start(patchfile);
                p.WaitForExit();
                bool s = !MM.Valid.IsGold105(MM.Reg.GoldPath);
                r(100, 100, Res.Prog_U105_Successful + s.ToString(), Res.Prog_U105_Successful + s.ToString());
                if (!s)
                    Status = Status.Error;
                File.Delete(patchfile);
            }
            catch (Exception e)
            {
                r(0, 100, e.ToString(), e.ToString());
                Status = Status.Error;
            }
        }
    }
}
