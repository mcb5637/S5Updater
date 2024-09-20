using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S5Updater2
{
    internal class TaskManageDebuggerDlls : IUpdaterTask
    {
        internal required MainWindow MM;
        internal Status Status;
        private static readonly string[] Extras = ["bin", "extra1\\bin", "extra2\\bin"];
        internal required bool CppLogic, Debugger;

        public Task Work(ProgressDialog.ReportProgressDel r)
        {
            Status = Status.Ok;
            try
            {
                if (MM.Reg.GoldPath == null)
                    throw new NullReferenceException();
                r(0, 100, null, null);
                bool cppl = CppLogic && TaskUpdateHook.IsInstalled(MM.Reg.GoldPath);
                bool deb = Debugger && TaskUpdateDebugger.IsInstalled(MM.Reg.GoldPath);
                if (cppl)
                {
                    foreach (string e in Extras)
                    {
                        string debug = Path.Combine(MM.Reg.GoldPath, e, "LuaDebugger.dll");
                        File.Copy(Path.Combine(MM.Reg.GoldPath, e, "S5CppLogic.dll"), debug, true);

                        //string sett = Path.Combine(MM.Reg.GoldPath, e, "CppLogicOptions.txt");
                        //if (File.Exists(sett))
                        //{
                        //    string txt = File.ReadAllText(sett).Trim();
                        //    if (txt.Equals("DoNotLoad=1") || txt.Equals("DoNotLoad=0")
                        //        || MessageBox.Show(string.Format(Resources.TaskManageDlls_QstRemoveSettings, sett), "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        //    {
                        //        File.Delete(sett);
                        //    }
                        //}
                    }
                    if (deb)
                    {
                        foreach (string e in Extras)
                        {
                            string debugo = Path.Combine(MM.Reg.GoldPath, e, "LuaDebuggerOrig.dll");
                            File.Copy(Path.Combine(MM.Reg.GoldPath, e, "LuaDebuggerFile.dll"), debugo, true);
                        }
                    }
                    else
                    {
                        foreach (string e in Extras)
                        {
                            string debugo = Path.Combine(MM.Reg.GoldPath, e, "LuaDebuggerOrig.dll");
                            if (File.Exists(debugo))
                                File.Delete(debugo);
                        }
                    }
                }
                else if (deb)
                {
                    foreach (string e in Extras)
                    {
                        string debug = Path.Combine(MM.Reg.GoldPath, e, "LuaDebugger.dll");
                        File.Copy(Path.Combine(MM.Reg.GoldPath, e, "LuaDebuggerFile.dll"), debug, true);
                    }
                    foreach (string e in Extras)
                    {
                        string debugo = Path.Combine(MM.Reg.GoldPath, e, "LuaDebuggerOrig.dll");
                        if (File.Exists(debugo))
                            File.Delete(debugo);
                    }
                }
                else
                {
                    foreach (string e in Extras)
                    {
                        string debug = Path.Combine(MM.Reg.GoldPath, e, "LuaDebugger.dll");
                        if (File.Exists(debug))
                            File.Delete(debug);
                    }
                    foreach (string e in Extras)
                    {
                        string debugo = Path.Combine(MM.Reg.GoldPath, e, "LuaDebuggerOrig.dll");
                        if (File.Exists(debugo))
                            File.Delete(debugo);
                    }
                }
                r(100, 100, null, null);
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
