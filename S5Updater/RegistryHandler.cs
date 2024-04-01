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
        internal const string MappackEMSKey = "EMS";
        internal const string MappackSpeedwarKey = "Speedwar";
        internal const string MappackBSKey = "BS";
        internal const string MappackStrongholdKey = "Stronghold";
        internal const string MappackRandomChaosKey = "RandomChaos";

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

        internal bool DownloadMappackStronghold
        {
            get => RegistryReadBool(S5UpdaterKey, MappackStrongholdKey, true);
            set => RegistryWriteBool(S5UpdaterKey, MappackStrongholdKey, value);
        }
        internal bool DownloadMappackRandomChaos
        {
            get => RegistryReadBool(S5UpdaterKey, MappackRandomChaosKey, true);
            set => RegistryWriteBool(S5UpdaterKey, MappackRandomChaosKey, value);
        }
    }
}
