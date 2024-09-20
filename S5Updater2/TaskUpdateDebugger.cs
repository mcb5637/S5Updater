using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S5Updater2
{
    class TaskUpdateDebugger : TaskUpdateFromGitRelease
    {
        protected override string HashesAdress => "https://github.com/mcb5637/SettlersLuaDebugger/releases/latest/download/debuggerhashes.txt";
        protected override string ZipAdress => "https://github.com/mcb5637/SettlersLuaDebugger/releases/latest/download/DebugS5.zip";
        protected override string ValidatingLog => Res.Prog_Debugger_Updating;
        private bool Debugger = false, CppLogic = false;

        protected override string? ZipPathToExtractPath(string e, string name)
        {
            if ("LuaDebugger.dll".Equals(name))
                name = "LuaDebuggerFile.dll";
            return base.ZipPathToExtractPath(e, name);
        }

        protected override Task PreUpdate()
        {
            Debugger = MM.DebuggerEnabled;
            CppLogic = MM.CppLogicEnabled;
            return Task.CompletedTask;
        }

        protected override async Task PostUpdate(ProgressDialog.ReportProgressDel r)
        {
            if (Status != Status.Ok)
                return;
            TaskManageDebuggerDlls t = new()
            {
                MM = MM,
                Debugger = Debugger,
                CppLogic = CppLogic,
            };
            await t.Work(r);
            Status = t.Status;
        }

        static public bool IsInstalled(string? path)
        {
            if (path == null)
                return false;
            return File.Exists(Path.Combine(path, "bin\\LuaDebuggerFile.dll"));
        }

        static public bool IsEnabled(string? path, InstallValidator v)
        {
            if (path == null)
                return false;
            if (!IsInstalled(path))
                return false;
            string debfil = Path.Combine(path, "bin\\LuaDebuggerFile.dll");
            string deb = Path.Combine(path, "bin\\LuaDebugger.dll");
            if (TaskUpdateHook.IsEnabled(path, v))
            {
                deb = Path.Combine(path, "bin\\LuaDebuggerOrig.dll");
            }
            return InstallValidator.GetFileHash(debfil) == InstallValidator.GetFileHash(deb);
        }
    }
}
