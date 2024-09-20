using Avalonia.Animation;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S5Updater2
{
    internal class RegistryHandler
    {
        internal string? GoldPath;
        internal bool GoldHasReg;
        internal string? HEPath;
        internal const string GoldKey = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Blue Byte\\The Settlers - Heritage of Kings";
        internal const string GoldDevKey = GoldKey + "\\Development";
        internal const string GoldInstallLoc = "InstallPath";
        internal const string GoldReso = "DefaultResolution";
        internal const string GoldDevMode = "DevelopmentMachine";
        internal const string GoldLanguage = "Language";
        internal const string GoldVideo = "PlayIntroVideos";
        internal const string HEKey = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Ubisoft\\Launcher\\Installs\\11786";
        internal const string HEInstallLoc = "InstallDir";

        internal const string HEDefaultSteamInstall = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\The Settlers - Heritage of Kings - History Edition";

        internal const string S5UpdaterKey = "HKEY_LOCAL_MACHINE\\SOFTWARE\\S5Updater";

        internal static object? GetReg(string keyname, string? valName, object? defaultval)
        {
            if (OperatingSystem.IsWindows())
                return Registry.GetValue(keyname, valName, defaultval);
            return null;
        }
        internal static void SetReg(string keyName, string? valName, object? val)
        {
            if (!OperatingSystem.IsWindows())
                return;
            if (val == null)
            {
                if (valName == null)
                    return;
                using RegistryKey? key = Registry.CurrentUser.OpenSubKey(keyName, true);
                key?.DeleteValue(valName);
            }
            else
            {
                RegistryValueKind k = RegistryValueKind.DWord;
                if (val is string)
                    k = RegistryValueKind.String;
                Registry.SetValue(keyName, valName, val, k);
            }
        }

        internal string? LoadGoldPathFromRegistry(InstallValidator vali)
        {
            string? r = GetReg(GoldKey, GoldInstallLoc, null) as string;
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
            GoldHasReg = true;
            SetReg(GoldKey, GoldInstallLoc, GoldPath);
        }

        internal string? LoadHEPathFromRegistry()
        {
            string? r = GetReg(HEKey, HEInstallLoc, null) as string;
            if (r != null)
            {
                HEPath = r;
            }
            return r;
        }

        internal static string GetPCName()
        {
            return Environment.MachineName;
        }

        internal static string? Resolution
        {
            get => GetReg(GoldDevKey, GoldReso, null) as string;
            set => SetReg(GoldDevKey, GoldReso, value);
        }

        internal static uint? DevMode
        {
            get
            {
                object? r = GetReg(GoldDevKey, GoldDevMode, null);
                if (r == null || r is not int)
                    return null;
                int i = (int)r;
                return unchecked((uint)i);
            }
            set => SetReg(GoldDevKey, GoldDevMode, value == null ? null : unchecked((int)(uint)value));
        }

        internal static string? Language
        {
            get => GetReg(GoldDevKey, GoldLanguage, null) as string;
            set => SetReg(GoldDevKey, GoldLanguage, value);
        }

        private static bool RegistryReadBool(string keyname, string valuename, bool defaultret)
        {
            object? o = GetReg(keyname, valuename, defaultret ? 1 : 0);
            if (o == null)
                return defaultret;
            return (int)o > 0;
        }
        private static void RegistryWriteBool(string keyname, string valuename, bool wr)
        {
            SetReg(keyname, valuename, wr ? 1 : 0);
        }

        internal static bool ShowIntroVideo
        {
            get => RegistryReadBool(GoldDevKey, GoldVideo, true);
            set => RegistryWriteBool(GoldDevKey, GoldVideo, value);
        }

        static internal bool GetUpdateMapPack(string key)
        {
            return RegistryReadBool(S5UpdaterKey, key, true);
        }
        static internal void SetUpdateMapPack(string key, bool v)
        {
            RegistryWriteBool(S5UpdaterKey, key, v);
        }
    }
}
