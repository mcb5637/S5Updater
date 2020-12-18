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
        internal static readonly string GoldKey = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Blue Byte\\The Settlers - Heritage of Kings";
        internal static readonly string GoldValue = "InstallPath";

        internal string LoadGoldPathFromRegistry()
        {
            string r = Registry.GetValue(GoldKey, GoldValue, null) as string;
            if (r != null)
                GoldPath = r;
            return r;
        }

        internal bool IsGoldValid()
        {
            return File.Exists(Path.Combine(GoldPath, "bin\\settlershok.exe"));
        }
    }
}
