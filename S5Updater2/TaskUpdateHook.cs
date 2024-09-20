using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S5Updater2
{
    class TaskUpdateHook : TaskUpdateFromGitRelease
    {
        protected override string HashesAdress => "https://github.com/mcb5637/S5BinkHook/releases/latest/download/hashes.txt";
        protected override string ZipAdress => "https://github.com/mcb5637/S5BinkHook/releases/latest/download/S5CppLogic.zip";
        protected override string ValidatingLog => Res.Prog_CppL_Validating;
        private bool Debugger = false, CppLogic = false;

        protected override bool ForceUpdate()
        {
            if (MM.Reg.GoldPath == null)
                throw new NullReferenceException();
            return File.Exists(Path.Combine(MM.Reg.GoldPath, "bin\\binkw32_orig.dll"));
        }

        protected override Task PreUpdate()
        {
            if (MM.Reg.GoldPath == null)
                throw new NullReferenceException();
            Debugger = MM.DebuggerEnabled;
            CppLogic = MM.CppLogicEnabled;
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
            return Task.CompletedTask;
        }

        protected override async Task PostUpdate(ProgressDialog.ReportProgressDel r)
        {
            if (Status != Status.Error)
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
            return File.Exists(Path.Combine(path, "bin\\S5CppLogic.dll")) && !File.Exists(Path.Combine(path, "bin\\binkw32_orig.dll"));
        }

        static public bool IsEnabled(string? path, InstallValidator v)
        {
            if (path == null)
                return false;
            if (!IsInstalled(path))
                return false;
            string cppl = Path.Combine(path, "bin\\S5CppLogic.dll");
            string deb = Path.Combine(path, "bin\\LuaDebugger.dll");
            string? cpplhash = InstallValidator.GetFileHash(cppl);
            string? debhash = InstallValidator.GetFileHash(deb);
            return cpplhash == debhash;
        }

        protected override string? ZipPathToExtractPath(string e, string name)
        {
            if (MM.Reg.GoldPath == null)
                throw new NullReferenceException();
            if (name == "CppLogic.bba")
            {
                if (e == "bin")
                    return Path.Combine(MM.Reg.GoldPath, "ModPacks", name);
                else
                    return null;
            }
            return base.ZipPathToExtractPath(e, name);
        }
    }
}
