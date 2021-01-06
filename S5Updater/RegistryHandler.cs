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
        internal string HEPath;
        internal static readonly string GoldKey = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Blue Byte\\The Settlers - Heritage of Kings";
        internal static readonly string GoldDevKey = GoldKey + "\\Development";
        internal static readonly string GoldInstallLoc = "InstallPath";
        internal static readonly string GoldReso = "DefaultResolution";
        internal static readonly string GoldDevMode = "DevelopmentMachine";
        internal static readonly string GoldLanguage = "Language";
        internal static readonly string HEKey = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Ubisoft\\Launcher\\Installs\\11786";
        internal static readonly string HEInstallLoc = "InstallDir";

        internal string LoadGoldPathFromRegistry(InstallValidator vali)
        {
            string r = Registry.GetValue(GoldKey, GoldInstallLoc, null) as string;
            if (r != null)
            {
                GoldPath = r;
                if (vali.IsValidGold(r))
                {
                    GoldHasReg = true;
                }
            }

            return r;
        }

        internal void SetGoldReg()
        {
            if (GoldHasReg)
                throw new ArgumentException();
            Registry.SetValue(GoldKey, GoldInstallLoc, GoldPath, RegistryValueKind.String);
        }

        internal string LoadHEPathFromRegistry()
        {
            string r = Registry.GetValue(HEKey, HEInstallLoc, null) as string;
            if (r != null)
            {
                HEPath = r;
            }
            return r;
        }

        internal string GetPCName()
        {
            return Environment.MachineName;
        }

        internal string Resolution
        {
            get => Registry.GetValue(GoldDevKey, GoldReso, null) as string;
            set => Registry.SetValue(GoldDevKey, GoldReso, value, RegistryValueKind.String);
        }

        internal uint? DevMode
        {
            get {
                object r = Registry.GetValue(GoldDevKey, GoldDevMode, null);
                if (r == null)
                    return null;
                int i = (int)r;
                return unchecked((uint)i);
            }
            set => Registry.SetValue(GoldDevKey, GoldDevMode, unchecked((int)(uint)value), RegistryValueKind.DWord);
        }

        internal string Language
        {
            get => Registry.GetValue(GoldDevKey, GoldLanguage, null) as string;
            set => Registry.SetValue(GoldDevKey, GoldLanguage, value, RegistryValueKind.String);
        }
    }
}
