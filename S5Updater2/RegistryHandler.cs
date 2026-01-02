using bbaLib;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using static S5Updater2.MainUpdater;

namespace S5Updater2
{
    internal class RegistryHandler
    {
        internal string? GoldPath;
        internal bool GoldHasReg;
        internal string? HEPath;
        internal string? WinePrefix;
        private const string GoldKey32 = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Blue Byte\\The Settlers - Heritage of Kings";
        private const string GoldKey64 = "HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\Blue Byte\\The Settlers - Heritage of Kings";
        private const string GoldDevKeyA = "\\Development";
        private const string GoldInstallLoc = "InstallPath";
        private const string GoldReso = "DefaultResolution";
        private const string GoldDevMode = "DevelopmentMachine";
        private const string GoldLanguage = "Language";
        private const string GoldVideo = "PlayIntroVideos";
        private const string HEKey = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Ubisoft\\Launcher\\Installs\\11786";
        private const string HEInstallLoc = "InstallDir";

        internal const string HEDefaultSteamInstall = "C:/Program Files (x86)/Steam/steamapps/common/The Settlers - Heritage of Kings - History Edition";

        private static string GoldKey => Environment.Is64BitProcess ? GoldKey64 : GoldKey32;
        private static string GoldDevKey => GoldKey + GoldDevKeyA;

        private object? GetReg(string keyName, string? valName, object? defaultVal)
        {
            if (OperatingSystem.IsWindows())
                return Registry.GetValue(keyName, valName, defaultVal);
            {
                ProcessStartInfo i = new()
                {
                    FileName = "wine",
                    Arguments = $"regedit /E \"./data.reg\" \"{keyName}\"",
                    Environment =
                    {
                        ["WINEPREFIX"] = WinePrefix
                    }
                };
                Process pr = new()
                {
                    StartInfo = i
                };
                pr.Start();
                pr.WaitForExit();
                if (pr.ExitCode != 0)
                    return AWExitCode.Invalid;
                bool subobj = false;
                string searchkey = $"[{keyName}]";
                string searchval = $"\"{valName}\"=";
                foreach (var l in File.ReadLines("./data.reg"))
                {
                    if (l.StartsWith('['))
                    {
                        subobj = l == searchkey;
                        continue;
                    }

                    if (subobj && l.StartsWith(searchval))
                    {
                        var data = l[searchval.Length..];
                        if (data.StartsWith('"'))
                        {
                            data = data[1..^1];
                            return data;
                        }
                        if (data.StartsWith("dword:"))
                        {
                            data = data[6..];
                            if (uint.TryParse(data, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out var r))
                            {
                                return unchecked((int)r);
                            }
                        }
                    }
                }
            }
            return defaultVal;
        }
        private AWExitCode SetReg(string keyName, string valName, object? val)
        {
            if (!OperatingSystem.IsWindows())
            {
                string data;
                if (val == null)
                {
                    data = "-";
                }
                else if (val is string s)
                {
                    data = $"\"{s}\"";
                }
                else
                {
                    int v = (int)val;
                    data = $"dword:{unchecked((uint)v):X8}";
                }

                File.WriteAllText("./data.reg", $"Windows Registry Editor Version 5.00\n\n[{keyName}]\n\"{valName}\"={data}", Encoding.Unicode);
                ProcessStartInfo i = new()
                {
                    FileName = "wine",
                    Arguments = "regedit /C \"./data.reg\"",
                    Environment =
                    {
                        ["WINEPREFIX"] = WinePrefix
                    }
                };
                Process pr = new()
                {
                    StartInfo = i
                };
                pr.Start();
                pr.WaitForExit();
                return pr.ExitCode == 0 ? AWExitCode.Success : AWExitCode.Invalid;
            }
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

        internal string? Resolution => GetReg(GoldDevKey, GoldReso, null) as string;

        internal AWExitCode SetResolution(string value)
        {
            return SetReg(GoldDevKey, GoldReso, value);
        }

        internal uint? DevMode
        {
            get
            {
                object? r = GetReg(GoldDevKey, GoldDevMode, null);
                if (r is not int i)
                    return null;
                return unchecked((uint)i);
            }
        }
        internal AWExitCode SetDevMode(uint? value)
        {
            return SetReg(GoldDevKey, GoldDevMode, value == null ? null : unchecked((int)(uint)value));
        }

        internal string? Language => GetReg(GoldDevKey, GoldLanguage, null) as string;

        internal AWExitCode SetLanguage(string value)
        {
            return SetReg(GoldDevKey, GoldLanguage, value);
        }

        private bool RegistryReadBool(string keyname, string valuename, bool defaultret)
        {
            object? o = GetReg(keyname, valuename, defaultret ? 1 : 0);
            if (o == null)
                return defaultret;
            return (int)o > 0;
        }
        private AWExitCode RegistryWriteBool(string keyname, string valuename, bool wr)
        {
            return SetReg(keyname, valuename, wr ? 1 : 0);
        }

        internal bool ShowIntroVideo => RegistryReadBool(GoldDevKey, GoldVideo, true);

        internal AWExitCode SetShowIntroVideo(bool value)
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
        private static readonly string[] VSCGuids = ["EA457B21-F73E-494C-ACAB-524FDE069978", "F8A2A208-72B3-4D61-95FC-8A65D340689B", "1287CAD5-7C8D-410D-88B9-0D1EE4A83FF2", "C26E74D1-022E-4238-8B9D-1E7564A36CC9"];
        internal IEnumerable<string> VSCCmdPaths
        {
            get
            {
                if (!OperatingSystem.IsWindows())
                    return ["code"];
                return VSCGuids.Select(x => $"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\{{{x}}}_is1")
                    .Select(x => GetReg(x, "DisplayIcon", null)).OfType<string>().Select(Path.GetDirectoryName).NotNull()
                    .Select(x => Path.Combine(x, "bin/code.cmd")).Where(File.Exists);
            }
        }
    }
}
