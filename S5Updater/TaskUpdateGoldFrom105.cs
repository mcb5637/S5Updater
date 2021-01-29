using S5Updater.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace S5Updater
{
    class TaskUpdateGoldFrom105 : IUpdaterTask
    {
        internal MainMenu MM;
        internal int Status;

        public void Work(ProgressDialog.ReportProgressDel r)
        {
            Status = MainMenu.Status_OK;
            try
            {
                string patchfile = Path.Combine(MM.Reg.GoldPath, "Tmp_patch.exe");
                r(0, Resources.TaskUpdate105_DownloadPatch);
                MainUpdater.DownlaodFile("https://www.siedler-maps.de/downloads/siedler_dedk/settlers_5_v1.06.exe", patchfile, r);
                r(0, Resources.TaskUpdate105_WaitForExec);
                MessageBox.Show(Resources.TaskUpdate105_MsgPatch);
                Process p = Process.Start(patchfile);
                p.WaitForExit();
                bool s = !MM.Valid.IsGold105(MM.Reg.GoldPath);
                r(100, Resources.TaskUpdate105_Successful + s.ToString());
                if (!s)
                    Status = MainMenu.Status_Error;
                File.Delete(patchfile);
            }
            catch (Exception e)
            {
                r(0, e.ToString());
                Status = MainMenu.Status_Error;
            }
        }
    }
}
