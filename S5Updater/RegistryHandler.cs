using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S5Updater
{
    class RegistryHandler
    {
        internal string GoldPath;
        internal bool GoldHasReg;
        internal static readonly string GoldKey = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Blue Byte\\The Settlers - Heritage of Kings";
        internal static readonly string GoldValue = "InstallPath";

        internal string LoadGoldPathFromRegistry()
        {
            string r = Registry.GetValue(GoldKey, GoldValue, null) as string;
            if (r != null)
            {
                GoldPath = r;
                if (IsGoldValid())
                {
                    GoldHasReg = true;
                }
            }

            return r;
        }

        internal bool IsGoldValid()
        {
            return GoldPath != null && File.Exists(Path.Combine(GoldPath, "bin\\settlershok.exe"));
        }

        internal void SetGoldReg()
        {
            if (GoldHasReg)
                throw new ArgumentException();
            Registry.SetValue(GoldKey, GoldValue, GoldPath, RegistryValueKind.String);
        }
    }
}
