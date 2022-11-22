using Microsoft.SqlServer.Server;
using S5Updater.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace S5Updater
{
    internal class TaskManageDebuggerDlls : IUpdaterTask
    {
        internal MainMenu MM;
        internal int Status;
        private static readonly string[] Extras = new string[] { "bin", "extra1\\bin", "extra2\\bin" };

        public void Work(ProgressDialog.ReportProgressDel r)
        {
            Status = MainMenu.Status_OK;
            try
            {
                r(0, null);
                if (MM.HookEnabled && TaskUpdateHook.IsInstalled(MM.Reg.GoldPath))
                {
                    foreach (string e in Extras)
                    {
                        string debug = Path.Combine(MM.Reg.GoldPath, e, "LuaDebugger.dll");
                        File.Copy(Path.Combine(MM.Reg.GoldPath, e, "S5CppLogic.dll"), debug, true);

                        string sett = Path.Combine(MM.Reg.GoldPath, e, "CppLogicOptions.txt");
                        if (File.Exists(sett))
                        {
                            string txt = File.ReadAllText(sett).Trim();
                            if (txt.Equals("DoNotLoad=1") || txt.Equals("DoNotLoad=0")
                                || MessageBox.Show(string.Format(Resources.TaskManageDlls_QstRemoveSettings, sett), "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                File.Delete(sett);
                            }
                        }
                    }
                    if (MM.DebuggerEnabled && TaskUpdateDebugger.IsInstalled(MM.Reg.GoldPath))
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
                else if (MM.DebuggerEnabled && TaskUpdateDebugger.IsInstalled(MM.Reg.GoldPath))
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
                r(100, null);
            }
            catch (Exception e)
            {
                r(0, e.ToString());
                Status = MainMenu.Status_Error;
            }
        }
    }
}
