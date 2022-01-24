using S5Updater.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S5Updater
{
    class TaskUpdateDebugger : TaskUpdateFromGitRepo
    {
        protected override string HashesAdress => "https://github.com/mcb5637/SettlersLuaDebugger/releases/latest/download/debuggerhashes.txt";
        protected override string ZipAdress => "https://github.com/mcb5637/SettlersLuaDebugger/releases/latest/download/DebugS5.zip";
        protected override string ValidatingLog => Resources.TaskDebugger_Updating;

        protected override string FileNameRedirect(string name)
        {
            if ("LuaDebugger.dll".Equals(name))
                return "LuaDebuggerFile.dll";
            return base.FileNameRedirect(name);
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
            return File.Exists(Path.Combine(path, "bin\\LuaDebuggerFile.dll"));
        }

        static public bool IsEnabled(string path, InstallValidator v)
        {
            if (!IsInstalled(path))
                return false;
            string debfil = Path.Combine(path, "bin\\LuaDebuggerFile.dll");
            string deb = Path.Combine(path, "bin\\LuaDebugger.dll");
            if (TaskUpdateHook.IsEnabled(path, v))
            {
                deb = Path.Combine(path, "bin\\LuaDebuggerOrig.dll");
            }
            return v.GetFileHash(debfil).Equals(v.GetFileHash(deb));
        }
    }
}
