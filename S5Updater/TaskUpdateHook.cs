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
    class TaskUpdateHook : TaskUpdateFromGitRepo
    {
        protected override string HashesAdress => "https://github.com/mcb5637/S5BinkHook/releases/latest/download/hashes.txt";
        protected override string ZipAdress => "https://github.com/mcb5637/S5BinkHook/releases/latest/download/S5CppLogic.zip";
        protected override string ValidatingLog => Resources.TaskHook_Validating;

        protected override bool ForceUpdate()
        {
            return File.Exists(Path.Combine(MM.Reg.GoldPath, "bin\\binkw32_orig.dll"));
        }

        protected override void PreUpdate()
        {
            foreach (string e in Extras)
            {
                string bw = Path.Combine(MM.Reg.GoldPath, e, "binkw32.dll");
                string bwor = Path.Combine(MM.Reg.GoldPath, e, "binkw32_orig.dll");
                if (File.Exists(bwor))
                {
                    File.Delete(bw);
                    File.Move(bwor, bw);
                }
            }
        }

        protected override void PostUpdate(ProgressDialog.ReportProgressDel r)
        {
            if (Status != MainMenu.Status_OK)
                return;
            TaskManageDebuggerDlls t = new TaskManageDebuggerDlls
            {
                MM = MM
            };
            t.Work(r);
            Status = t.Status;
        }

        static public bool IsInstalled(string path)
        {
            return File.Exists(Path.Combine(path, "bin\\S5CppLogic.dll")) && !File.Exists(Path.Combine(path, "bin\\binkw32_orig.dll"));
        }

        static public bool IsEnabled(string path, InstallValidator v)
        {
            string cppl = Path.Combine(path, "bin\\S5CppLogic.dll");
            string deb = Path.Combine(path, "bin\\LuaDebugger.dll");
            return IsInstalled(path) && InstallValidator.GetFileHash(cppl).Equals(InstallValidator.GetFileHash(deb));
        }
    }
}
