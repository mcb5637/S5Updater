﻿using bbaToolS5;
using S5Updater.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace S5Updater
{
    public partial class MainMenu : Form
    {
        private const string InfoInternalPath = "maps\\externalmap\\info.xml";
        internal static readonly int Status_OK = 0;
        internal static readonly int Status_Error = 1;

        internal RegistryHandler Reg;
        internal ProgressDialog Prog;
        internal DevHashCalc HashCalc;
        internal InstallValidator Valid;

        private static readonly Resolution[] Resolutions = new Resolution[] { new Resolution("default", "0", false),
            new Resolution("select", "select", false), new Resolution("1920x1080", "1920 x 1080 x 32", true), new Resolution("2500x1400", "2500 x 1400 x 32", true),
            new Resolution("2560x1440", "2560 x 1440 x 32", true), new Resolution("3440x1440", "3440 x 1440 x 32", true)
        }; // todo maybe add current screnn resolution?
        private static readonly Language[] Languages = new Language[] {new Language("Deutsch", "de"), new Language("English", "en"), new Language("US-English", "us"),
            new Language("French", "fr"), new Language("Polish", "pl"), new Language("Chinese", "zh"), new Language("Czech", "cs"), new Language("Dutch", "nl"),
            new Language("Hungarian", "hu"), new Language("Italian", "it"), new Language("Russian", "ru"), new Language("Slovakian", "sk"),
            new Language("Spanish", "sp")
        };

        private bool Updating = false;

        public MainMenu()
        {
            InitializeComponent();
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {
            Reg = new RegistryHandler();
            Prog = new ProgressDialog();
            HashCalc = new DevHashCalc();
            Valid = new InstallValidator();

            Text = Resources.TitleMainMenu;
            GroupBox_Installation.Text = Resources.TitleInstallation;
            BTN_SetGold.Text = Resources.Txt_SetGold;
            BTN_SetHE.Text = Resources.Txt_SetGold;
            GroupBox_Updates.Text = Resources.TitleUpdate;
            Btn_UpdateMPMaps.Text = Resources.Txt_UpdateMP;
            CB_EasyMode.Text = Resources.Txt_EasyMode;
            CB_ShowLog.Text = Resources.Txt_ShowLog;
            Btn_GoldSave.Text = Resources.Txt_GoldSetReg;
            GroupBox_Settings.Text = Resources.TitleSettings;
            GroupBox_Registry.Text = Resources.TitleReg;
            LBL_Reso.Text = Resources.Lbl_Reso;
            LBL_Langua.Text = Resources.Txt_Langua;
            GroupBox_Convert.Text = Resources.TitleConvert;
            Btn_ConvertHE.Text = Resources.Txt_Convert;
            CB_DevMode.Text = Resources.Txt_DevMode;
            CB_ShowIntro.Text = Resources.Txt_ShowIntro;
            BTN_UpdateFrom105.Text = Resources.Txt_UpdateFrom105;
            BTN_Patch106.Text = Resources.Txt_Update106;
            Btn_MapInstallerGold.Text = Resources.Txt_InstallMap;
            Btn_MapInstallerHE.Text = Resources.Txt_InstallMap;
            Btn_UpdateHook.Text = Resources.Txt_UpdateHook;
            Cb_EnableHook.Text = Resources.Txt_EnableHook;
            tabPageGeneral.Text = Resources.Txt_TabPageGeneral;
            tabPageMaps.Text = Resources.Txt_TabPageMaps;
            BTN_UpdateMappacks.Text = Resources.Txt_Btn_UpdateMappacks;

            int idx = checkedListBox_Mappacks.FindString("EMS");
            checkedListBox_Mappacks.SetItemChecked(idx, Reg.DownloadMappackEMS);
            idx = checkedListBox_Mappacks.FindString("Speedwar");
            checkedListBox_Mappacks.SetItemChecked(idx, Reg.DownloadMappackSpeedwar);
            idx = checkedListBox_Mappacks.FindString("BS");
            checkedListBox_Mappacks.SetItemChecked(idx, Reg.DownloadMappackBS);

#if DEBUG
            BTN_DBG_HashFile.Visible = true;
#else
            BTN_DBG_HashFile.Visible = false;
#endif

            Updating = true;
            ComboBox_Reso.Items.AddRange(Resolutions);
            int i = Resolutions.IndexOfArrayElement(Reg.Resolution, (x) => x.RegValue);
            if (i == -1)
                i = 0;
            ComboBox_Reso.SelectedIndex = i;
            CB_DevMode.Checked = HashCalc.CalcHash(Reg.GetPCName()) == Reg.DevMode;
            ComboBox_Langua.Items.AddRange(Languages);
            i = Languages.IndexOfArrayElement(Reg.Language, (X) => X.RegValue);
            if (i == -1)
                i = 0;
            ComboBox_Langua.SelectedIndex = i;

            CB_ShowIntro.Checked = Reg.ShowIntroVideo;
            Updating = false;

            CB_ShowLog_CheckedChanged(null, null);
            CB_EasyMode_CheckedChanged(null, null);

            Reg.LoadGoldPathFromRegistry(Valid);
            Reg.LoadHEPathFromRegistry();
            if (!Valid.IsValidHENotConverted(Reg.HEPath) && Valid.IsValidHENotConverted(RegistryHandler.HEDefaultSteamInstall))
                Reg.HEPath = RegistryHandler.HEDefaultSteamInstall;
            Log(Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId.ToString());
            Log(Resources.Log_SetGold + (Reg.GoldPath ?? Resources._null));
            Log(Resources.Log_SetHE + (Reg.HEPath ?? Resources._null));
            UpdateInstallation();
        }

        private void UpdateInstallation()
        {
            LBL_Gold.Text = Resources.Lbl_Gold + (Reg.GoldPath ?? Resources._null);
            bool goldValid = Valid.IsValidGold(Reg.GoldPath);
            if (goldValid)
            {
                CB_GoldOK.Text = Resources.Status_Ok;
                CB_GoldOK.Checked = true;
                Btn_UpdateMPMaps.Enabled = true;
                Btn_GoldSave.Enabled = true;
                BTN_UpdateFrom105.Enabled = (Valid.IsGold105(Reg.GoldPath)) && Reg.GoldHasReg;
                bool patched = Valid.IsExeGold(Reg.GoldPath);
                BTN_Patch106.Enabled = !BTN_UpdateFrom105.Enabled && !patched;
                CB_AllPatched.Text = patched ? Resources.Txt_AllPatchedOK : Resources.Txt_AllPatchedNO;
                CB_AllPatched.Checked = patched;
                Btn_MapInstallerGold.Enabled = true;
                Btn_UpdateHook.Enabled = true;
                Updating = true;
                Cb_EnableHook.Enabled = TaskUpdateHook.IsInstalled(Reg.GoldPath);
                Cb_EnableHook.Checked = TaskUpdateHook.IsEnabled(Reg.GoldPath);
                Updating = false;
            }
            else
            {
                CB_GoldOK.Text = Resources.Status_Invalid;
                CB_GoldOK.Checked = false;
                Btn_UpdateMPMaps.Enabled = false;
                Btn_GoldSave.Enabled = false;
                BTN_Patch106.Enabled = false;
                CB_AllPatched.Text = Resources.Txt_AllPatchedNO;
                CB_AllPatched.Checked = false;
                BTN_UpdateFrom105.Enabled = false;
                Btn_MapInstallerGold.Enabled = false;
                Btn_UpdateHook.Enabled = false;
                Updating = true;
                Cb_EnableHook.Checked = false;
                Cb_EnableHook.Enabled = false;
                Updating = false;
            }
            LBL_HE.Text = Resources.Lbl_HE + (Reg.HEPath ?? Resources._null);
            if (Valid.IsValidHENotConverted(Reg.HEPath))
            {
                CB_HEOk.Text = Resources.Status_Ok;
                CB_HEOk.Checked = true;
                Btn_ConvertHE.Enabled = !goldValid && (string.IsNullOrEmpty(Reg.GoldPath) || (MainUpdater.IsDirNotExistingOrEmpty(Reg.GoldPath) && !MainUpdater.IsSubDirectoryOf(Reg.GoldPath, Reg.HEPath)));
                Btn_MapInstallerHE.Enabled = true;
            }
            else
            {
                CB_HEOk.Text = Resources.Status_Invalid;
                CB_HEOk.Checked = false;
                Btn_ConvertHE.Enabled = false;
                Btn_MapInstallerHE.Enabled = false;
            }
        }

        private void BTN_SetGold_Click(object sender, EventArgs e)
        {
            if (Reg.GoldPath != null)
                Dlg_FolderBrowser.SelectedPath = Reg.GoldPath;
            if (Dlg_FolderBrowser.ShowDialog()==DialogResult.OK)
            {
                Reg.GoldPath = Dlg_FolderBrowser.SelectedPath;
                Log(Resources.Log_SetGold + Reg.GoldPath);
                UpdateInstallation();
            }
        }

        internal void Log(string s)
        {
            TextBox_Output.Text += s + Environment.NewLine;
        }

        private void Btn_UpdateMPMaps_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
        }

        internal bool EasyMode
        {
            get => CB_EasyMode.Checked;
            set => CB_EasyMode.Checked = value;
        }

        public bool ShouldMappackBeDownloaded(string name)
        {
            int index = checkedListBox_Mappacks.FindString(name);
            return checkedListBox_Mappacks.GetItemChecked(index);
        }

        private void CB_ShowLog_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            TextBox_Output.Visible = CB_ShowLog.Checked;
            Size = new Size(CB_ShowLog.Checked ? 1127 : 560, 594);
        }

        private void Btn_GoldSave_Click(object sender, EventArgs e)
        {
            if (EasyMode || MessageBox.Show(Resources.Txt_QuestOverrideReg, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Reg.SetGoldReg();
                Log(Resources.Log_RegGold + Reg.GoldPath);
            }
        }

        private void CB_EasyMode_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            bool hideInEasy = !EasyMode;
            Btn_GoldSave.Visible = hideInEasy;
            CB_DevMode.Visible = hideInEasy;
        }

        private void ComboBox_Reso_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            Resolution sel = ComboBox_Reso.SelectedItem as Resolution;
            Reg.Resolution = sel.RegValue;
            Log(Resources.Log_SetReso + sel.Show);
            if (sel.NeedsDev && !CB_DevMode.Checked)
            {
                if (EasyMode || MessageBox.Show(Resources.Txt_QuestEnableDevModeRes, sel.Show, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    CB_DevMode.Checked = true;
            }
        }

        private void CB_DevMode_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            uint x = CB_DevMode.Checked ? HashCalc.CalcHash(Reg.GetPCName()) : 0;
            Log(Resources.Log_SetDev + x);
            Reg.DevMode = x;
        }

        private void ComboBox_Langua_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            Language sel = ComboBox_Langua.SelectedItem as Language;
            Reg.Language = sel.RegValue;
            Log(Resources.Log_SetLang + sel.Show);
        }

        private void BTN_SetHE_Click(object sender, EventArgs e)
        {
            if (Reg.HEPath != null)
                Dlg_FolderBrowser.SelectedPath = Reg.HEPath;
            if (Dlg_FolderBrowser.ShowDialog() == DialogResult.OK)
            {
                Reg.HEPath = Dlg_FolderBrowser.SelectedPath;
                Log(Resources.Log_SetHE + Reg.HEPath);
                UpdateInstallation();
            }
        }

        private void Btn_ConvertHE_Click(object sender, EventArgs e)
        {
            TaskConvertHE t = new TaskConvertHE()
            {
                MM = this
            };
            Prog.ShowWorkDialog(t, this);
            CheckStatus(t.Status);
            UpdateInstallation();
        }

        private void CB_ShowIntro_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            Reg.ShowIntroVideo = CB_ShowIntro.Checked;
            Log(Resources.Log_SetVideo + CB_ShowIntro.Checked.ToString());
        }

        private void BTN_DBG_HashFile_Click(object sender, EventArgs e)
        {
            if (Dlg_OpenFile.ShowDialog() == DialogResult.OK)
            {
                Log(Path.GetFileName(Dlg_OpenFile.FileName) + " " + Valid.GetFileHash(Dlg_OpenFile.FileName));
            }
        }

        private void BTN_UpdateFrom105_Click(object sender, EventArgs e)
        {
            TaskUpdateGoldFrom105 t = new TaskUpdateGoldFrom105()
            {
                MM = this
            };
            Prog.ShowWorkDialog(t, this);
            UpdateInstallation();
            CheckStatus(t.Status);
        }

        private void CheckStatus(int status)
        {
            if (status != Status_OK)
            {
                MessageBox.Show(Resources.Txt_ErrMsg, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (!CB_ShowLog.Checked)
                    CB_ShowLog.Checked = true;
            }
        }

        private void BTN_Patch106_Click(object sender, EventArgs e)
        {
            TaskUpdate106 t = new TaskUpdate106()
            {
                MM = this
            };
            Prog.ShowWorkDialog(t, this);
            UpdateInstallation();
            CheckStatus(t.Status);
        }

        private void HandleMap(string file, string path, out string name)
        {
            name = "";
            try
            {
                if (Path.GetExtension(file) == ".zip")
                {
                    using (ZipArchive a = ZipFile.OpenRead(file))
                    {
                        foreach (ZipArchiveEntry e in a.Entries)
                        {
                            if (e.IsFolder())
                                continue;
                            if (Path.GetExtension(e.Name) == ".s5x")
                            {
                                string extractfile = Path.Combine(path, e.Name);
                                e.ExtractToFile(extractfile, true);
                                HandleMap(extractfile, path, out string n);
                                name += "\n" + n;
                                File.Delete(extractfile);
                            }
                        }
                    }
                }
                else if (Path.GetExtension(file) == ".s5x")
                {
                    string outPath = GetMapFileExtraPath(file, out name);
                    if (outPath == null)
                        return;
                    outPath = Path.Combine(path, outPath, Path.GetFileName(file));
                    File.Copy(file, outPath, true);
                    Log(Resources.Log_InstallMap + outPath);
                }
            }
            catch (Exception e)
            {
                Log(e.ToString());
            }
        }

        private string GetMapFileExtraPath(string file, out string name)
        {
            try
            {
                BbaArchive a = new BbaArchive();
                a.ReadBba(file, InfoInternalPath);
                XDocument doc = XDocument.Load(a.GetFileByName(InfoInternalPath).GetStream());
                a.Clear();
                int key = int.Parse(doc.Element("root").Element("Key").Value);
                string outPath = "base\\shr\\maps\\user";
                string into = Resources.Txt_Extra0;
                if (key == 2)
                {
                    outPath = "extra2\\shr\\maps\\user";
                    into = Resources.Txt_Extra2;
                }
                else if (key == 1)
                {
                    outPath = "extra1\\shr\\maps\\user";
                    into = Resources.Txt_Extra1;
                }
                name = Regex.Replace(doc.Element("root").Element("Name").Value, "@color:[0-9]{1,3}(,[0-9]{1,3})*", "").Trim() + Resources.Txt_MapInto + into;
                return outPath;
            }
            catch (Exception e)
            {
                Log(e.ToString());
                name = "";
                return null;
            }
        }

        private void Btn_MapInstallerGold_Click(object sender, EventArgs e)
        {
            QueryMapInstall(Reg.GoldPath);
        }

        private void QueryMapInstall(string path)
        {
            Dlg_OpenFile.Filter = "Maps|*.s5x;*.zip|Anything|*.*";
            Dlg_OpenFile.CheckFileExists = true;
            Dlg_OpenFile.Multiselect = true;
            if (Dlg_OpenFile.ShowDialog() == DialogResult.OK)
            {
                string txt = Resources.Log_InstallMap;
                foreach (string f in Dlg_OpenFile.FileNames)
                {
                    HandleMap(f, path, out string name);
                    txt = txt + "\n" + name;
                }
                MessageBox.Show(txt);
            }
        }

        private void Btn_MapInstallerHE_Click(object sender, EventArgs e)
        {
            QueryMapInstall(Reg.HEPath);
        }

        private void Btn_UpdateHook_Click(object sender, EventArgs e)
        {
            TaskUpdateHook t = new TaskUpdateHook()
            {
                MM = this
            };
            Prog.ShowWorkDialog(t, this);
            UpdateInstallation();
            CheckStatus(t.Status);
        }

        private void Cb_EnableHook_CheckedChanged(object sender, EventArgs e)
        {
            if (Updating)
                return;
            TaskUpdateHook.SetEnabled(Reg.GoldPath, Cb_EnableHook.Checked);
            Log(Resources.Log_SetHookEnabled + Cb_EnableHook.Checked);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void checkedListBox_Mappacks_SelectedIndexChanged(object sender, EventArgs e)
        {
            // update registry
            int idx = checkedListBox_Mappacks.FindString("EMS");
            Reg.DownloadMappackEMS = checkedListBox_Mappacks.GetItemChecked(idx);
            idx = checkedListBox_Mappacks.FindString("Speedwar");
            Reg.DownloadMappackSpeedwar = checkedListBox_Mappacks.GetItemChecked(idx);
            idx = checkedListBox_Mappacks.FindString("BS");
            Reg.DownloadMappackBS = checkedListBox_Mappacks.GetItemChecked(idx);
        }

        private void BTN_UpdateMappacks_Click(object sender, EventArgs e)
        {
            TaskUpdateMPMaps t = new TaskUpdateMPMaps
            {
                MM = this
            };
            Prog.ShowWorkDialog(t, this);
            CheckStatus(t.Status);
        }
    }
}
