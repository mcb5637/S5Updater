using Avalonia.Animation;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static S5Updater2.MainUpdater;

namespace S5Updater2
{
    internal class RegistryHandler
    {
        internal string? GoldPath;
        internal bool GoldHasReg;
        internal string? HEPath;
        internal const string GoldKey32 = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Blue Byte\\The Settlers - Heritage of Kings";
        internal const string GoldKey64 = "HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\Blue Byte\\The Settlers - Heritage of Kings";
        internal const string GoldDevKeyA = "\\Development";
        internal const string GoldInstallLoc = "InstallPath";
        internal const string GoldReso = "DefaultResolution";
        internal const string GoldDevMode = "DevelopmentMachine";
        internal const string GoldLanguage = "Language";
        internal const string GoldVideo = "PlayIntroVideos";
        internal const string HEKey = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Ubisoft\\Launcher\\Installs\\11786";
        internal const string HEInstallLoc = "InstallDir";

        internal const string HEDefaultSteamInstall = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\The Settlers - Heritage of Kings - History Edition";

        internal static string GoldKey => Environment.Is64BitProcess ? GoldKey64 : GoldKey32;
        internal static string GoldDevKey => GoldKey + GoldDevKeyA;

        internal static object? GetReg(string keyname, string? valName, object? defaultval)
        {
            if (OperatingSystem.IsWindows())
                return Registry.GetValue(keyname, valName, defaultval);
            return null;
        }
        internal static AWExitCode SetReg(string keyName, string valName, object? val)
        {
            if (!OperatingSystem.IsWindows())
                return AWExitCode.InvalidOS;
            try
            {
                if (val == null)
                {
                    if (valName == null)
                        return AWExitCode.Invalid;
                    using RegistryKey? key = Registry.CurrentUser.OpenSubKey(keyName, true);
                    key?.DeleteValue(valName);
                    return AWExitCode.Success;
                }
                else
                {
                    RegistryValueKind k = RegistryValueKind.DWord;
                    if (val is string)
                        k = RegistryValueKind.String;
                    Registry.SetValue(keyName, valName, val, k);
                    return AWExitCode.Success;
                }
            }
            catch (UnauthorizedAccessException)
            {
                return RunAdminReg(keyName, valName, val);
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

        internal AWExitCode SetGoldReg()
        {
            GoldHasReg = true;
            return SetReg(GoldKey, GoldInstallLoc, GoldPath);
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
        }
        internal static AWExitCode SetResolution(string value)
        {
            return SetReg(GoldDevKey, GoldReso, value);
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
        }
        internal static AWExitCode SetDevMode(uint? value)
        {
            return SetReg(GoldDevKey, GoldDevMode, value == null ? null : unchecked((int)(uint)value));
        }

        internal static string? Language
        {
            get => GetReg(GoldDevKey, GoldLanguage, null) as string;
        }
        internal static AWExitCode SetLanguage(string value)
        {
            return SetReg(GoldDevKey, GoldLanguage, value);
        }

        private static bool RegistryReadBool(string keyname, string valuename, bool defaultret)
        {
            object? o = GetReg(keyname, valuename, defaultret ? 1 : 0);
            if (o == null)
                return defaultret;
            return (int)o > 0;
        }
        private static AWExitCode RegistryWriteBool(string keyname, string valuename, bool wr)
        {
            return SetReg(keyname, valuename, wr ? 1 : 0);
        }

        internal static bool ShowIntroVideo
        {
            get => RegistryReadBool(GoldDevKey, GoldVideo, true);
        }
        internal static AWExitCode SetShowIntroVideo(bool value)
        {
            return RegistryWriteBool(GoldDevKey, GoldVideo, value);
        }
    }
}
