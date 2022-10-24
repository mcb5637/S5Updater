using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S5Updater
{
    internal class RegistryHandler
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
        internal static readonly string GoldVideo = "PlayIntroVideos";
        internal static readonly string HEKey = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Ubisoft\\Launcher\\Installs\\11786";
        internal static readonly string HEInstallLoc = "InstallDir";

        internal static readonly string HEDefaultSteamInstall = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\The Settlers - Heritage of Kings - History Edition";

        internal static readonly string S5UpdaterKey = "HKEY_LOCAL_MACHINE\\SOFTWARE\\S5Updater";
        internal static readonly string MappackEMSKey = "EMS";
        internal static readonly string MappackSpeedwarKey = "Speedwar";
        internal static readonly string MappackBSKey = "BS";

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
            GoldHasReg = true;
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
                if (r == null || !(r is int))
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

        private bool RegistryReadBool(string keyname, string valuename, bool defaultret)
        {
            object o = Registry.GetValue(keyname, valuename, defaultret ? 1 : 0);
            if (o == null)
                return defaultret;
            return (int)o > 0;
        }
        private void RegistryWriteBool(string keyname, string valuename, bool wr)
        {
            Registry.SetValue(keyname, valuename, wr ? 1 : 0, RegistryValueKind.DWord);
        }

        internal bool ShowIntroVideo
        {
            get => RegistryReadBool(GoldDevKey, GoldVideo, true);
            set => RegistryWriteBool(GoldDevKey, GoldVideo, value);
        }

        internal bool DownloadMappackEMS
        {
            get => RegistryReadBool(S5UpdaterKey, MappackEMSKey, true);
            set => RegistryWriteBool(S5UpdaterKey, MappackEMSKey, value);
        }

        internal bool DownloadMappackSpeedwar
        {
            get => RegistryReadBool(S5UpdaterKey, MappackSpeedwarKey, true);
            set => RegistryWriteBool(S5UpdaterKey, MappackSpeedwarKey, value);
        }

        internal bool DownloadMappackBS
        {
            get => RegistryReadBool(S5UpdaterKey, MappackBSKey, true);
            set => RegistryWriteBool(S5UpdaterKey, MappackBSKey, value);
        }
    }
}
