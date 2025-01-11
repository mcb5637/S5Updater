using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using MsBox.Avalonia;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static S5Updater2.MainUpdater;

namespace S5Updater2
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        internal RegistryHandler Reg = new();
        internal ProgressDialog Prog => new() { MM = this };
        internal InstallValidator Valid = new();
        internal UserScriptManager USM = new();
        internal Settings Set;

        private static readonly Resolution[] Resolutions = [ new Resolution("default", "0", false),
            new Resolution("select", "select", false), new Resolution("1920x1080", "1920 x 1080 x 32", true), new Resolution("2500x1400", "2500 x 1400 x 32", true),
            new Resolution("2560x1440", "2560 x 1440 x 32", true), new Resolution("3440x1440", "3440 x 1440 x 32", true)
        ]; // todo maybe add current screnn resolution?
        private static readonly Language[] Languages = [new Language("Deutsch", "de"), new Language("English", "en"), new Language("US-English", "us"),
            new Language("French", "fr"), new Language("Polish", "pl"), new Language("Chinese", "zh"), new Language("Czech", "cs"), new Language("Dutch", "nl"),
            new Language("Hungarian", "hu"), new Language("Italian", "it"), new Language("Russian", "ru"), new Language("Slovakian", "sk"),
            new Language("Spanish", "sp")
        ];

        public MainWindow()
        {
            if (Design.IsDesignMode)
            {
                InitializeComponent();
                Set = new();
                return;
            }

            Reg.LoadGoldPathFromRegistry(Valid);
            Reg.LoadHEPathFromRegistry();
            USM.Read();
            Set = Settings.Load();

            DataContext = this;
            InitializeComponent();
            AddHandler(DragDrop.DropEvent, DropFile);

            UpdateEverything();
        }

        private void OnClosing(object s, WindowClosingEventArgs a)
        {
            Prog.Close();
            Set.Save();
        }

        public new event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string? propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        internal void Log(string msg)
        {
            OutputLog.Text += msg + Environment.NewLine;
        }
        internal async void CopyLog(object sender, RoutedEventArgs e)
        {
            if (Clipboard == null)
                return;
            await Clipboard.SetTextAsync(OutputLog.Text);
        }
        internal void UpdateEverything()
        {
            OnPropertyChanged(null);
        }
        private async Task CheckStatus(Status status)
        {
            if (status != Status.Ok)
            {
                var box = MessageBoxManager.GetMessageBoxStandard("", Res.ErrorDetected, MsBox.Avalonia.Enums.ButtonEnum.Ok);
                ShowLog.IsChecked = true;
                await box.ShowAsync();
            }
        }
        private async Task CheckExitCode(AWExitCode status, string log, string suc)
        {
            if (status != AWExitCode.Success)
            {
                Log(log + status);
                var box = MessageBoxManager.GetMessageBoxStandard("", Res.ErrorDetected, MsBox.Avalonia.Enums.ButtonEnum.Ok);
                ShowLog.IsChecked = true;
                await box.ShowAsync();
            }
            else
            {
                Log(suc);
            }
        }
        private async Task InstallMap(string path, bool allowModPacks, params string[] files)
        {
            TaskInstallMap t = new()
            {
                MM = this,
                TargetPath = path,
                Files = files,
                AllowModPacks = allowModPacks,
            };
            await Prog.ShowProgressDialog(t);
            await CheckStatus(t.Status);
        }
        private async void DropFile(object? sender, DragEventArgs e)
        {
            if (e.Data.Contains(DataFormats.Files))
            {
                string? gold = Reg.GoldPath;
                string? he = Reg.HEPath;
                bool gv = Valid.IsValidGold(gold);
                bool hv = Valid.IsValidHENotConverted(he);
                string[]? files = e.Data.GetFiles()?.Select((x) => x.Path.LocalPath)?.ToArray();
                if (files == null)
                    return;
                if (gv)
                {
                    if (gold == null)
                        throw new NullReferenceException();
                    await InstallMap(gold, true, files);
                }
                if (hv)
                {
                    if (he == null)
                        throw new NullReferenceException();
                    await InstallMap(he, false, files);
                }
            }
        }

        internal string GoldPath => Res.GoldPath + (Reg.GoldPath ?? "-");
        internal string HEPath => Res.HEPath + (Reg.HEPath ?? "-");
        internal bool GoldPathValid => Valid.IsValidGold(Reg.GoldPath);
        internal bool HEPathValid => Valid.IsValidHENotConverted(Reg.HEPath);
        private async void SelectGoldVersionAsync(object sender, RoutedEventArgs e)
        {
            IReadOnlyList<IStorageFolder> f = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
            {
                Title = Res.SelectGold,
            });
            if (f.Count > 0)
            {
                Reg.GoldPath = f[0].Path.LocalPath;
                Log(Res.Log_SetGoldPath + Reg.GoldPath);
                Maps = [];
                ModPacks = [];
                UpdateEverything();
            }
        }
        private async void SelectHEVersionAsync(object sender, RoutedEventArgs e)
        {
            IReadOnlyList<IStorageFolder> f = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
            {
                Title = Res.SelectHE,
            });
            if (f.Count > 0)
            {
                Reg.HEPath = f[0].Path.LocalPath;
                Log(Res.Log_SetHEPath + Reg.HEPath);
                UpdateEverything();
            }
        }
        private async void SetGoldRegistry(object s, RoutedEventArgs e)
        {
            var box = MessageBoxManager.GetMessageBoxStandard("", Res.ReallyOverrideRegistry, MsBox.Avalonia.Enums.ButtonEnum.YesNo);
            if (await box.ShowAsync() == MsBox.Avalonia.Enums.ButtonResult.Yes)
            {
                await CheckExitCode(Reg.SetGoldReg(), Res.Log_SetGoldReg, Res.Log_SetGoldReg + Reg.GoldPath);
            }
        }

        internal bool Patch105Enabled
        {
            get
            {
                string? gp = Reg.GoldPath;
                if (gp == null)
                    return false;
                if (!EasyMode)
                    return true;
                return Valid.IsValidGold(gp) && Valid.IsGold105(gp);
            }
        }
        private async void Patch105(object s, RoutedEventArgs e)
        {
            TaskUpdateGoldFrom105 t = new()
            {
                MM = this
            };
            await Prog.ShowProgressDialog(t);
            UpdateEverything();
            await CheckStatus(t.Status);
        }
        internal bool Patch106Enabled
        {
            get
            {
                string? gp = Reg.GoldPath;
                if (gp == null)
                    return false;
                if (!EasyMode)
                    return true;
                return Valid.IsValidGold(gp) && !Valid.IsGold105(gp) && !Valid.IsExeGold(gp);
            }
        }
        private async void Patch106(object s, RoutedEventArgs e)
        {
            TaskUpdate106 t = new()
            {
                MM = this
            };
            await Prog.ShowProgressDialog(t);
            UpdateEverything();
            await CheckStatus(t.Status);
        }
        internal bool GoldAllPatched
        {
            get
            {
                string? gp = Reg.GoldPath;
                if (gp == null)
                    return false;
                if (!EasyMode)
                    return true;
                return Valid.IsValidGold(gp) && Valid.IsExeGold(gp);
            }
        }
        internal bool GoldAllPatchedNoOverride
        {
            get
            {
                string? gp = Reg.GoldPath;
                if (gp == null)
                    return false;
                return Valid.IsValidGold(gp) && Valid.IsExeGold(gp);
            }
        }
        private async void UpdateCppLogic(object s, RoutedEventArgs e)
        {
            TaskUpdateHook t = new()
            {
                MM = this
            };
            await Prog.ShowProgressDialog(t);
            UpdateEverything();
            await CheckStatus(t.Status);
        }
        public bool CppLogicEnabled
        {
            get => TaskUpdateHook.IsEnabled(Reg.GoldPath, Valid);
            set => DebuggerDllChanged(null, value);
        }
        public bool CppLogicInstalled
        {
            get
            {
                string? gp = Reg.GoldPath;
                if (gp == null)
                    return false;
                return Valid.IsValidGold(gp) && TaskUpdateHook.IsInstalled(gp);
            }
        }
        private async void InstallMapGold(object s, RoutedEventArgs e)
        {
            IReadOnlyList<IStorageFile> r = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
            {
                AllowMultiple = true,
                FileTypeFilter = [new("Maps") { Patterns = ["*.s5x", "*.zip", "*.bba"] }],
            });
            if (r.Count > 0)
            {
                string p = Reg.GoldPath ?? throw new NullReferenceException();
                await InstallMap(p, true, r.Select((x) => x.Path.LocalPath).ToArray());
            }
        }

        internal static IList<Resolution> ResolutionData => Resolutions;
        internal static Resolution SelectedResolution
        {
            get
            {
                var res = RegistryHandler.Resolution;
                return Resolutions.FirstOrDefault((r) => r.RegValue == res, Resolutions[0]);
            }
        }
        internal async void SelectedResolution_Changed(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox)?.SelectedItem is Resolution value)
            {
                if (RegistryHandler.Resolution == value.RegValue)
                    return;
                await CheckExitCode(RegistryHandler.SetResolution(value.RegValue), Res.Log_SetReso, Res.Log_SetReso + value.Show);
                if (value.NeedsDev)
                    await SetDevMode(true);
                UpdateEverything();
            }
        }
        internal static IList<Language> LanguageData => Languages;
        internal static Language SelectedLanguage
        {
            get
            {
                var res = RegistryHandler.Language;
                return Languages.FirstOrDefault((r) => r.RegValue == res, Languages[0]);
            }
        }
        internal async void SelectedLanguage_Changed(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox)?.SelectedItem is Language value)
            {
                if (RegistryHandler.Language == value.RegValue)
                    return;
                await CheckExitCode(RegistryHandler.SetLanguage(value.RegValue), Res.Log_SetLang, Res.Log_SetLang + value.Show);
                UpdateEverything();
            }
        }
        internal static bool DevMode
        {
            get => DevHashCalc.CalcHash(RegistryHandler.GetPCName()) == RegistryHandler.DevMode;
        }
        internal async void DevMode_Changed(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox cb)
            {
                if (cb.IsChecked == null)
                    return;
                await SetDevMode((bool)cb.IsChecked);
            }
        }
        private async Task SetDevMode(bool val)
        {
            if (DevMode == val)
                return;
            uint v = val ? DevHashCalc.CalcHash(RegistryHandler.GetPCName()) : 0;
            await CheckExitCode(RegistryHandler.SetDevMode(v), Res.Log_SetDev, Res.Log_SetDev + v);
            UpdateEverything();
        }

        internal static bool ShowIntroVideo
        {
            get => RegistryHandler.ShowIntroVideo;
        }
        internal async void ShowIntroVideo_Changed(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox cb)
            {
                if (cb.IsChecked == null)
                    return;
                bool val = (bool)cb.IsChecked;
                if (RegistryHandler.ShowIntroVideo == val)
                    return;
                await CheckExitCode(RegistryHandler.SetShowIntroVideo(val), Res.Log_SetIntroVideo, Res.Log_SetIntroVideo + val);
                UpdateEverything();
            }
        }


        private async void ConvertHE(object sender, RoutedEventArgs e)
        {
            TaskConvertHE t = new()
            {
                MM = this
            };
            await Prog.ShowProgressDialog(t);
            UpdateEverything();
            await CheckStatus(t.Status);
        }
        internal bool ConvertHEEnabled
        {
            get
            {
                string? gp = Reg.GoldPath;
                string? hp = Reg.HEPath;
                if (Valid.IsValidGold(gp))
                    return false;
                if (hp == null)
                    return false;
                if (!Valid.IsValidHENotConverted(hp))
                    return false;
                if (!EasyMode)
                    return true;
                if (string.IsNullOrEmpty(gp))
                    return true;
                return MainUpdater.IsDirNotExistingOrEmpty(gp) && !MainUpdater.IsSubDirectoryOf(gp, hp);
            }
        }
        internal bool HEValid
        {
            get
            {
                string? hp = Reg.HEPath;
                if (hp == null)
                    return false;
                if (!EasyMode)
                    return true;
                return Valid.IsValidHENotConverted(hp);
            }
        }
        private async void HEFixEditor(object sender, RoutedEventArgs e)
        {
            string? p = Reg.HEPath;
            if (p == null)
                return;
            if (!Valid.IsValidHE(p))
                return;
            try
            {
                File.Delete(Path.Combine(p, "extra2\\shr\\MapEditor\\LandscapeSets\\customset.xml"));
            }
            catch (IOException ex)
            {
                Log(ex.ToString());
            }
            try
            {
                File.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Documents\\THE SETTLERS 5 - History Edition\\MapEditor\\LandscapeSets\\customset.xml"));
            }
            catch (IOException ex)
            {
                Log(ex.ToString());
            }
            var box = MessageBoxManager.GetMessageBoxStandard("", Res.HEEditor_CreateLinks, MsBox.Avalonia.Enums.ButtonEnum.YesNo);
            if (await box.ShowAsync() == MsBox.Avalonia.Enums.ButtonResult.Yes)
            {
                MainUpdater.CreateLinkPS(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Settlers HoK Editor Extra1.lnk"),
                    Path.Combine(p, "bin/shokmapeditor.exe"), "-extra1", Log);
                MainUpdater.CreateLinkPS(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Settlers HoK Editor Extra2.lnk"),
                    Path.Combine(p, "bin/shokmapeditor.exe"), "-extra2", Log);
            }
        }
        private async void InstallMapHE(object s, RoutedEventArgs e)
        {
            IReadOnlyList<IStorageFile> r = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
            {
                AllowMultiple = true,
                FileTypeFilter = [new("Maps") { Patterns = ["*.s5x", "*.zip"] }],
            });
            if (r.Count > 0)
            {
                string p = Reg.HEPath ?? throw new NullReferenceException();
                await InstallMap(p, false, r.Select((x) => x.Path.LocalPath).ToArray());
            }
        }

        private async void UpdateDebugger(object sender, RoutedEventArgs e)
        {
            TaskUpdateDebugger t = new()
            {
                MM = this
            };
            await Prog.ShowProgressDialog(t);
            UpdateEverything();
            await CheckStatus(t.Status);
        }
        public bool DebuggerEnabled
        {
            get => TaskUpdateDebugger.IsEnabled(Reg.GoldPath, Valid);
            set => DebuggerDllChanged(value, null);
        }
        public bool DebuggerInstalled
        {
            get
            {
                string? gp = Reg.GoldPath;
                if (gp == null)
                    return false;
                return Valid.IsValidGold(gp) && TaskUpdateDebugger.IsInstalled(gp);
            }
        }
        private async void DebuggerDllChanged(bool? debugger, bool? cpplogic)
        {
            TaskManageDebuggerDlls t = new()
            {
                MM = this,
                Debugger = debugger ?? DebuggerEnabled,
                CppLogic = cpplogic ?? CppLogicEnabled,
            };
            await Prog.ShowProgressDialog(t);
            Log(Res.Log_SetDebuggerEnabled + t.Debugger);
            Log(Res.Log_SetHookEnabled + t.CppLogic);
            UpdateEverything();
            await CheckStatus(t.Status);
        }

        private void ShowLogChanged(object sender, RoutedEventArgs e)
        {
            bool show = ShowLog.IsChecked == true;
            LogSplit.DisplayMode = show ? SplitViewDisplayMode.Inline : SplitViewDisplayMode.Overlay;
            LogSplit.IsPaneOpen = show;
        }
        private bool _easyMode = !Design.IsDesignMode;
        internal bool EasyMode {
            get => _easyMode;
            set
            {
                _easyMode = value;
                OnPropertyChanged(null);
            }
        }

        internal static IList<MapPack> MapPacksData => TaskUpdateMPMaps.Packs;
        internal IList<MapPack> MapPacksSelected
        {
            get
            {
                return MapPacksData.Where(GetMapPackUpdateFromSettings).ToList();
            }
            set
            {
                foreach (MapPack p in MapPacksData)
                {
                    bool s = value.Contains(p);
                    Set.SelectedMapPacks[p.RegKey] = s;
                }
            }
        }
        internal bool GetMapPackUpdateFromSettings(MapPack p)
        {
            return !Set.SelectedMapPacks.TryGetValue(p.RegKey, out bool r) || r;
        }
        // binding MapPacksSelected only reads...
        internal void MapPacksSelected_Changed(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListBox)?.SelectedItems is IList<MapPack> i)
                MapPacksSelected = i;
        }
        private async void UpdateMappacks(object sender, RoutedEventArgs e)
        {
            TaskUpdateMPMaps t = new()
            {
                MM = this
            };
            await Prog.ShowProgressDialog(t);
            UpdateEverything();
            await CheckStatus(t.Status);
        }

        internal IList<MapUpdate> ModPacks { get; private set; } = [];
        internal IList<MapUpdate> ModPacksSelected
        {
            get
            {
                return ModPacks.Where(GetModPacksUpdateFromSettings).ToList();
            }
            set
            {
                foreach (MapUpdate p in ModPacks)
                {
                    bool s = value.Any((x) => x.File == p.File);
                    Set.SelectedModPacks[p.File] = s;
                }
            }
        }
        internal bool GetModPacksUpdateFromSettings(MapUpdate p)
        {
            return !Set.SelectedModPacks.TryGetValue(p.File, out bool r) || r;
        }
        // binding ModPacksSelected only reads...
        internal void ModPacksSelected_Changed(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListBox)?.SelectedItems is IList<MapUpdate> i)
                ModPacksSelected = i;
        }
        private async void ScanModPacks(object sender, RoutedEventArgs e)
        {
            TaskScanMaps t = new()
            {
                MM = this,
                Type = new TaskUpdateMapsTypeModPack(),
            };
            await Prog.ShowProgressDialog(t);
            ModPacks = t.Maps;
            UpdateEverything();
            await CheckStatus(t.Status);
        }
        private async void UpdateModPacks(object sender, RoutedEventArgs e)
        {
            TaskUpdateMaps t = new()
            {
                MM = this,
                Maps = ModPacksSelected,
                Type = new TaskUpdateMapsTypeModPack(),
            };
            await Prog.ShowProgressDialog(t);
            UpdateEverything();
            await CheckStatus(t.Status);
        }

        internal IList<MapUpdate> Maps { get; private set; } = [];
        internal IList<MapUpdate> MapsSelected
        {
            get
            {
                return Maps.Where(GetMapsUpdateFromSettings).ToList();
            }
            set
            {
                foreach (MapUpdate p in Maps)
                {
                    bool s = value.Any((x) => x.File == p.File);
                    Set.SelectedMaps[p.File] = s;
                }
            }
        }
        internal bool GetMapsUpdateFromSettings(MapUpdate p)
        {
            return !Set.SelectedMaps.TryGetValue(p.File, out bool r) || r;
        }
        // binding ModPacksSelected only reads...
        internal void MapsSelected_Changed(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListBox)?.SelectedItems is IList<MapUpdate> i)
                MapsSelected = i;
        }
        private async void ScanMaps(object sender, RoutedEventArgs e)
        {
            TaskScanMaps t = new()
            {
                MM = this,
                Type = new TaskUpdateMapsTypeMap(),
            };
            await Prog.ShowProgressDialog(t);
            Maps = t.Maps;
            UpdateEverything();
            await CheckStatus(t.Status);
        }
        private async void UpdateMaps(object sender, RoutedEventArgs e)
        {
            TaskUpdateMaps t = new()
            {
                MM = this,
                Maps = MapsSelected,
                Type = new TaskUpdateMapsTypeMap(),
            };
            await Prog.ShowProgressDialog(t);
            UpdateEverything();
            await CheckStatus(t.Status);
        }

        internal bool US_Zoom
        {
            get => USM.Zoom;
            set
            {
                USM.Zoom = value;
                USM.Update(Log);
            }
        }
        internal static IList<PlayerColor> ColorData => UserScriptManager.PlayerColors;
        internal PlayerColor ColorSelected
        {
            get => ColorData.FirstOrDefault((c) => c.Value == USM.PlayerColor, ColorData[0]);
            set
            {
                USM.PlayerColor = value.Value;
                USM.Update(Log);
            }
        }

        internal int MaxScrollerHeight => 500;

        // never call from main thread!!!
        public async Task EnsureWriteAccess(string dir)
        {
            if (!HasWriteAccess(dir))
            {
                var ec = RunFullAccess(dir);
                await Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    await CheckExitCode(ec, Res.Log_AllowAccess, Res.Log_AllowAccess + dir);
                });
            }
        }
    }
}