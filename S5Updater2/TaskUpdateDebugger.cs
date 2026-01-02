using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace S5Updater2
{
    class TaskUpdateDebugger : TaskUpdateFromGitRelease
    {
        protected override string HashesAddress =>
            VSCAdaptor ? "https://github.com/mcb5637/S5DebugAdaptor/releases/latest/download/debuggerhashes.txt" : "https://github.com/mcb5637/SettlersLuaDebugger/releases/latest/download/debuggerhashes.txt";
        protected override string ZipAddress =>
            VSCAdaptor ? "https://github.com/mcb5637/S5DebugAdaptor/releases/latest/download/DebugS5.zip" : "https://github.com/mcb5637/SettlersLuaDebugger/releases/latest/download/DebugS5.zip";
        protected override string ValidatingLog => Res.Prog_Debugger_Updating;
        private bool Debugger = false, CppLogic = false;
        private bool VSCAdaptor => MM.DebuggerVSCAdaptor;

        private const string ExtensionAddress = "https://github.com/mcb5637/S5DebugAdaptor/releases/latest/download/debuggerlatestextension.txt";

        internal required RegistryHandler Reg;

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
            if (Status != Status.Ok)
                return;
            if (VSCAdaptor)
            {
                if (MM.Reg.GoldPath == null)
                    throw new NullReferenceException();
                r(0, 100, Res.Prog_Debugger_Extension, Res.Prog_Debugger_Extension);
                string v = await MainUpdater.DownloadFileString(ExtensionAddress, r);
                string patchfile = Path.Combine(MM.Reg.GoldPath, $"s5luadebug-{v}.vsix");
                await MainUpdater.DownloadFile($"https://github.com/mcb5637/S5DebugAdaptor/releases/latest/download/s5luadebug-{v}.vsix", patchfile, r);
                foreach (string p in Reg.VSCCmdPaths)
                {
                    string log = Res.Prog_Debugger_Extension_Into + p;
                    r(0, 100, log, log);
                    ProcessStartInfo i = new()
                    {
                        FileName = p,
                        Arguments = $"--install-extension \"{patchfile}\"",
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true,
                        UseShellExecute = false
                    };
                    Process pr = new()
                    {
                        StartInfo = i
                    };
                    pr.Start();
                    string s = await pr.StandardOutput.ReadToEndAsync();
                    r(0, 100, s, s);
                    await pr.WaitForExitAsync();
                }
            }
        }

        public static bool IsInstalled(string? path)
        {
            if (path == null)
                return false;
            return File.Exists(Path.Combine(path, "bin/LuaDebuggerFile.dll"));
        }

        public static bool IsEnabled(string? path, InstallValidator v)
        {
            if (path == null)
                return false;
            if (!IsInstalled(path))
                return false;
            string debfil = Path.Combine(path, "bin/LuaDebuggerFile.dll");
            string deb = Path.Combine(path, "bin/LuaDebugger.dll");
            if (TaskUpdateHook.IsEnabled(path, v))
            {
                deb = Path.Combine(path, "bin/LuaDebuggerOrig.dll");
            }
            return InstallValidator.GetFileHash(debfil) == InstallValidator.GetFileHash(deb);
        }
    }
}
