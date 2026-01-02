using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using MsBox.Avalonia;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static S5Updater2.MainUpdater;

namespace S5Updater2
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        internal readonly RegistryHandler Reg = new();
        private ProgressDialog Prog => new() { MM = this };
        internal readonly InstallValidator Valid = new();
        private readonly UserScriptManager UserScript = new();
        private readonly Settings Set;

        private static readonly Resolution[] Resolutions = [ new("default", "0", false),
            new("select", "select", false), new("1920x1080", "1920 x 1080 x 32", true), new("2500x1400", "2500 x 1400 x 32", true),
            new("2560x1440", "2560 x 1440 x 32", true), new("3440x1440", "3440 x 1440 x 32", true), new("1680x1050", "1680 x 1050 x 32", true)
        ]; // todo maybe add current screen resolution?
        private static readonly Language[] Languages = [new("Deutsch", "de"), new("English", "en"), new("US-English", "us"),
            new("French", "fr"), new("Polish", "pl"), new("Chinese", "zh"), new("Czech", "cs"), new("Dutch", "nl"),
            new("Hungarian", "hu"), new("Italian", "it"), new("Russian", "ru"), new("Slovakian", "sk"),
            new("Spanish", "es")
        ];

        private bool ListenToEvents = false;

        public MainWindow()
        {
            if (Design.IsDesignMode)
            {
                InitializeComponent();
                Set = new();
                return;
            }

            Set = Settings.Load();
            if (OperatingSystem.IsWindows())
            {
                Reg.LoadGoldPathFromRegistry(Valid);
                Reg.LoadHEPathFromRegistry();
            }
            else
            {
                Reg.WinePrefix = Set.WinePath;
                Reg.GoldPath = Set.GoldPath;
                Reg.HEPath = Set.HEPath;
            }

            UserScript.Read(Reg);

            DataContext = this;
            InitializeComponent();
            AddHandler(DragDrop.DropEvent, DropFile);

            UpdateEverything();
            ListenToEvents = true;
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
            try
            {
                if (Clipboard == null)
                    return;
                await Clipboard.SetTextAsync(OutputLog.Text);
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }
        private void UpdateEverything()
        {
            OnPropertyChanged(null);
        }
        private async Task CheckStatus(Status status)
        {
            if (status != Status.Ok)
            {
                var box = MessageBoxManager.GetMessageBoxStandard("", Res.ErrorDetected);
                ShowLog.IsChecked = true;
                await box.ShowAsync();
            }
        }
        private async Task CheckExitCode(AWExitCode status, string log, string suc)
        {
            if (status != AWExitCode.Success)
            {
                Log(log + status);
                var box = MessageBoxManager.GetMessageBoxStandard("", Res.ErrorDetected);
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
            try
            {
                if (e.Data.Contains(DataFormats.Files))
                {
                    string? gold = Reg.GoldPath;
                    string? he = Reg.HEPath;
                    bool gv = Valid.IsValidGold(gold);
                    bool hv = Valid.IsValidHENotConverted(he);
                    string[]? files = e.Data.GetFiles()?.Select((x) => x.Path.LocalPath).ToArray();
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
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }

        internal string GoldPath => Res.GoldPath + (Reg.GoldPath ?? "-");
        internal string HEPath => Res.HEPath + (Reg.HEPath ?? "-");
        internal bool GoldPathValid => Valid.IsValidGold(Reg.GoldPath);
        internal bool HEPathValid => Valid.IsValidHENotConverted(Reg.HEPath);
        private async void SelectGoldVersionAsync(object sender, RoutedEventArgs e)
        {
            try
            {
                IReadOnlyList<IStorageFolder> f = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
                {
                    Title = Res.SelectGold,
                });
                if (f.Count > 0)
                {
                    Reg.GoldPath = f[0].Path.LocalPath;
                    if (!OperatingSystem.IsWindows())
                        Set.GoldPath = f[0].Path.LocalPath;
                    Log(Res.Log_SetGoldPath + Reg.GoldPath);
                    Maps = [];
                    ModPacks = [];
                    UpdateEverything();
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }
        private async void SelectHEVersionAsync(object sender, RoutedEventArgs e)
        {
            try
            {
                IReadOnlyList<IStorageFolder> f = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
                {
                    Title = Res.SelectHE,
                });
                if (f.Count > 0)
                {
                    Reg.HEPath = f[0].Path.LocalPath;
                    if (!OperatingSystem.IsWindows())
                        Set.HEPath = f[0].Path.LocalPath;
                    Log(Res.Log_SetHEPath + Reg.HEPath);
                    UpdateEverything();
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }
        private async void SetGoldRegistry(object s, RoutedEventArgs e)
        {
            try
            {
                var box = MessageBoxManager.GetMessageBoxStandard("", Res.ReallyOverrideRegistry, MsBox.Avalonia.Enums.ButtonEnum.YesNo);
                if (await box.ShowAsync() == MsBox.Avalonia.Enums.ButtonResult.Yes)
                {
                    await CheckExitCode(Reg.SetGoldReg(), Res.Log_SetGoldReg, Res.Log_SetGoldReg + Reg.GoldPath);
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
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
            try
            {
                TaskUpdateGoldFrom105 t = new()
                {
                    MM = this
                };
                await Prog.ShowProgressDialog(t);
                UpdateEverything();
                await CheckStatus(t.Status);
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
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
            try
            {
                TaskUpdate106 t = new()
                {
                    MM = this
                };
                await Prog.ShowProgressDialog(t);
                UpdateEverything();
                await CheckStatus(t.Status);
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
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
            try
            {
                TaskUpdateHook t = new()
                {
                    MM = this
                };
                await Prog.ShowProgressDialog(t);
                UpdateEverything();
                await CheckStatus(t.Status);
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }
        public bool CppLogicEnabled
        {
            get => TaskUpdateHook.IsEnabled(Reg.GoldPath, Valid);
            set
            {
                if (!ListenToEvents)
                    return;
                DebuggerDllChanged(null, value);
            }
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
            try
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
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }

        internal static IList<Resolution> ResolutionData => Resolutions;
        internal Resolution SelectedResolution
        {
            get
            {
                var res = Reg.Resolution;
                return Resolutions.FirstOrDefault((r) => r.RegValue == res, Resolutions[0]);
            }
        }
        internal async void SelectedResolution_Changed(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (!ListenToEvents)
                    return;
                if ((sender as ComboBox)?.SelectedItem is Resolution value)
                {
                    if (Reg.Resolution == value.RegValue)
                        return;
                    await CheckExitCode(Reg.SetResolution(value.RegValue), Res.Log_SetReso, Res.Log_SetReso + value.Show);
                    if (value.NeedsDev)
                        await SetDevMode(true);
                    UpdateEverything();
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }
        internal static IList<Language> LanguageData => Languages;
        internal Language SelectedLanguage
        {
            get
            {
                var res = Reg.Language;
                return Languages.FirstOrDefault((r) => r.RegValue == res, Languages[0]);
            }
        }
        internal async void SelectedLanguage_Changed(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (!ListenToEvents)
                    return;
                if ((sender as ComboBox)?.SelectedItem is Language value)
                {
                    if (Reg.Language == value.RegValue)
                        return;
                    await CheckExitCode(Reg.SetLanguage(value.RegValue), Res.Log_SetLang, Res.Log_SetLang + value.Show);
                    UpdateEverything();
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }
        internal bool DevMode => DevHashCalc.CalcHash(RegistryHandler.GetPCName()) == Reg.DevMode;

        internal async void DevMode_Changed(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ListenToEvents)
                    return;
                if (sender is CheckBox cb)
                {
                    if (cb.IsChecked == null)
                        return;
                    await SetDevMode((bool)cb.IsChecked);
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }
        private async Task SetDevMode(bool val)
        {
            if (DevMode == val)
                return;
            uint v = val ? DevHashCalc.CalcHash(RegistryHandler.GetPCName()) : 0;
            await CheckExitCode(Reg.SetDevMode(v), Res.Log_SetDev, Res.Log_SetDev + v);
            UpdateEverything();
        }

        internal bool ShowIntroVideo => Reg.ShowIntroVideo;

        internal async void ShowIntroVideo_Changed(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ListenToEvents)
                    return;
                if (sender is CheckBox cb)
                {
                    if (cb.IsChecked == null)
                        return;
                    bool val = (bool)cb.IsChecked;
                    if (Reg.ShowIntroVideo == val)
                        return;
                    await CheckExitCode(Reg.SetShowIntroVideo(val), Res.Log_SetIntroVideo, Res.Log_SetIntroVideo + val);
                    UpdateEverything();
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }


        private async void ConvertHE(object sender, RoutedEventArgs e)
        {
            try
            {
                TaskConvertHE t = new()
                {
                    MM = this
                };
                await Prog.ShowProgressDialog(t);
                UpdateEverything();
                await CheckStatus(t.Status);
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
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
            try
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
                    MainUpdater.CreateLinkPowershell(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Settlers HoK Editor Extra1.lnk"),
                        Path.Combine(p, "bin/shokmapeditor.exe"), "-extra1", Log);
                    MainUpdater.CreateLinkPowershell(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Settlers HoK Editor Extra2.lnk"),
                        Path.Combine(p, "bin/shokmapeditor.exe"), "-extra2", Log);
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }
        private async void InstallMapHE(object s, RoutedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }

        public bool DebuggerVSCAdaptor
        {
            get => Set.DebuggerVSCAdaptor;
            set
            {
                if (!ListenToEvents)
                    return;
                Set.DebuggerVSCAdaptor = value;
            }
        }

        private async void UpdateDebugger(object sender, RoutedEventArgs e)
        {
            try
            {
                TaskUpdateDebugger t = new()
                {
                    MM = this,
                    Reg = Reg,
                };
                await Prog.ShowProgressDialog(t);
                UpdateEverything();
                await CheckStatus(t.Status);
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }
        public bool DebuggerEnabled
        {
            get => TaskUpdateDebugger.IsEnabled(Reg.GoldPath, Valid);
            set { 
                if (!ListenToEvents)
                    return;
                DebuggerDllChanged(value, null);
            }
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
            try
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
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }

        private void ShowLogChanged(object sender, RoutedEventArgs e)
        {
            if (!ListenToEvents)
                return;
            bool show = ShowLog.IsChecked == true;
            LogSplit.DisplayMode = show ? SplitViewDisplayMode.Inline : SplitViewDisplayMode.Overlay;
            LogSplit.IsPaneOpen = show;
        }

        internal bool EasyMode
        {
            get;
            set
            {
                if (!ListenToEvents)
                    return;
                field = value;
                OnPropertyChanged(null);
            }
        } = !Design.IsDesignMode;

        internal static IList<MapPack> MapPacksData => TaskUpdateMPMaps.Packs;
        internal IList<MapPack> MapPacksSelected
        {
            get => MapPacksData.Where(GetMapPackUpdateFromSettings).ToList();
            set
            {
                if (!ListenToEvents)
                    return;
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
            if (!ListenToEvents)
                return;
            if ((sender as ListBox)?.SelectedItems is IList<MapPack> i)
                MapPacksSelected = i;
        }
        private async void UpdateMappacks(object sender, RoutedEventArgs e)
        {
            try
            {
                TaskUpdateMPMaps t = new()
                {
                    MM = this
                };
                await Prog.ShowProgressDialog(t);
                UpdateEverything();
                await CheckStatus(t.Status);
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }

        internal IList<MapUpdate> ModPacks { get; private set; } = [];
        internal IList<MapUpdate> ModPacksSelected
        {
            get => ModPacks.Where(GetModPacksUpdateFromSettings).ToList();
            set
            {
                if (!ListenToEvents)
                    return;
                foreach (MapUpdate p in ModPacks)
                {
                    bool s = value.Any((x) => x.File == p.File);
                    Set.SelectedModPacks[p.File] = s;
                }
            }
        }
        private bool GetModPacksUpdateFromSettings(MapUpdate p)
        {
            return !Set.SelectedModPacks.TryGetValue(p.File, out bool r) || r;
        }
        // binding ModPacksSelected only reads...
        internal void ModPacksSelected_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (!ListenToEvents)
                return;
            if ((sender as ListBox)?.SelectedItems is IList<MapUpdate> i)
                ModPacksSelected = i;
        }
        private async void ScanModPacks(object sender, RoutedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }
        private async void UpdateModPacks(object sender, RoutedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }

        internal IList<MapUpdate> Maps { get; private set; } = [];
        internal IList<MapUpdate> MapsSelected
        {
            get => Maps.Where(GetMapsUpdateFromSettings).ToList();
            set
            {
                if (!ListenToEvents)
                    return;
                foreach (MapUpdate p in Maps)
                {
                    bool s = value.Any((x) => x.File == p.File);
                    Set.SelectedMaps[p.File] = s;
                }
            }
        }
        private bool GetMapsUpdateFromSettings(MapUpdate p)
        {
            return !Set.SelectedMaps.TryGetValue(p.File, out bool r) || r;
        }
        // binding ModPacksSelected only reads...
        internal void MapsSelected_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (!ListenToEvents)
                return;
            if ((sender as ListBox)?.SelectedItems is IList<MapUpdate> i)
                MapsSelected = i;
        }
        private async void ScanMaps(object sender, RoutedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }
        private async void UpdateMaps(object sender, RoutedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }

        internal bool UserScriptZoom
        {
            get => UserScript.Zoom;
            set
            {
                if (!ListenToEvents)
                    return;
                UserScript.Zoom = value;
                UserScript.Update(Reg, Log);
            }
        }
        internal static IList<PlayerColor> ColorData => UserScriptManager.PlayerColors;
        internal PlayerColor ColorSelected
        {
            get => ColorData.FirstOrDefault((c) => c.Value == UserScript.PlayerColor, ColorData[0]);
            set
            {
                if (!ListenToEvents)
                    return;
                UserScript.PlayerColor = value.Value;
                UserScript.Update(Reg, Log);
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

        internal bool WineEnabled => !OperatingSystem.IsWindows();

        internal string WinePath
        {
            get => Set.WinePath;
            set => Set.WinePath = value;
        }

        internal async void SelectWinePath(object sender, RoutedEventArgs e)
        {
            try
            {
                IReadOnlyList<IStorageFolder> f = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
                {
                    Title = Res.SelectWine,
                });
                if (f.Count > 0)
                {
                    Set.WinePath = f[0].Path.LocalPath;
                    Reg.WinePrefix = Set.WinePath;
                    Log(Res.Log_SetWine + Set.WinePath);
                    UpdateEverything();
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }
    }
}