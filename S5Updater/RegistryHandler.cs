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
        internal static readonly string GoldReso = "DefaultResolution";
        internal static readonly string GoldDevMode = "DevelopmentMachine";
        internal static readonly string GoldDevKey = GoldKey + "\\Development";

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
    }
}
