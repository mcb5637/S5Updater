using bbaLib;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
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

        internal const string HEDefaultSteamInstall = "C:/Program Files (x86)/Steam/steamapps/common/The Settlers - Heritage of Kings - History Edition";

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

        // ReSharper disable once InconsistentNaming
        internal static string GetPCName()
        {
            return Environment.MachineName;
        }

        internal static string? Resolution => GetReg(GoldDevKey, GoldReso, null) as string;

        internal static AWExitCode SetResolution(string value)
        {
            return SetReg(GoldDevKey, GoldReso, value);
        }

        internal static uint? DevMode
        {
            get
            {
                object? r = GetReg(GoldDevKey, GoldDevMode, null);
                if (r is not int i)
                    return null;
                return unchecked((uint)i);
            }
        }
        internal static AWExitCode SetDevMode(uint? value)
        {
            return SetReg(GoldDevKey, GoldDevMode, value == null ? null : unchecked((int)(uint)value));
        }

        internal static string? Language => GetReg(GoldDevKey, GoldLanguage, null) as string;

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

        internal static bool ShowIntroVideo => RegistryReadBool(GoldDevKey, GoldVideo, true);

        internal static AWExitCode SetShowIntroVideo(bool value)
        {
            return RegistryWriteBool(GoldDevKey, GoldVideo, value);
        }

        internal string? GoldDocuments
        {
            get
            {
                string? g = GoldPath;
                if (g == null)
                    return null;
                string? f = "THE SETTLERS - Heritage of Kings";
                string arch = Path.Combine(g, "base/lang.bba");
                if (File.Exists(arch))
                {
                    BbaArchive a = new();
                    a.ReadBba(arch);
                    var file = a.GetFileByName("config\\language.xml");
                    if (file != null)
                    {
                        f = XDocument.Load(file.GetStream()).Element("root")?.Element("MyDocumentsSubfolder")?.Value;
                    }
                }
                if (f == null)
                    return null;
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), f);
            }
        }

        // see https://github.com/Microsoft/vscode/issues/37807
        internal static readonly string[] VSCGuids = ["EA457B21-F73E-494C-ACAB-524FDE069978", "F8A2A208-72B3-4D61-95FC-8A65D340689B", "1287CAD5-7C8D-410D-88B9-0D1EE4A83FF2", "C26E74D1-022E-4238-8B9D-1E7564A36CC9"];
        internal static IEnumerable<string> VSCCmdPaths
        {
            get
            {
                // TODO linux ?
                return VSCGuids.Select(x => $"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\{{{x}}}_is1")
                    .Select(x => GetReg(x, "DisplayIcon", null)).OfType<string>().Select(Path.GetDirectoryName).NotNull()
                    .Select(x => Path.Combine(x, "bin/code.cmd")).Where(File.Exists);
            }
        }
    }
}
