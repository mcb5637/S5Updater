using S5Updater.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace S5Updater
{
    public partial class MainMenu : Form
    {
        internal static readonly int Status_OK = 0;
        internal static readonly int Status_Error = 1;

        internal RegistryHandler Reg;
        internal ProgressDialog Prog;
        internal DevHashCalc HashCalc;

        private readonly string[] Resolutions = new string[] {"default", "select", "1920 x 1080 x 32"};
        private readonly bool[] ResolutionNeedsDev = new bool[] { false, false, true };
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

            Text = Resources.TitleMainMenu;
            GroupBox_Installation.Text = Resources.TitleInstallation;
            BTN_SetGold.Text = Resources.Txt_SetGold;
            GroupBox_Updates.Text = Resources.TitleUpdate;
            Btn_UpdateMPMaps.Text = Resources.Txt_UpdateMP;
            CB_EasyMode.Text = Resources.Txt_EasyMode;
            CB_ShowLog.Text = Resources.Txt_ShowLog;
            Btn_GoldSave.Text = Resources.Txt_GoldSetReg;
            GroupBox_Settings.Text = Resources.TitleSettings;
            GroupBox_Registry.Text = Resources.TitleReg;
            LBL_Reso.Text = Resources.Lbl_Reso;
            Updating = true;
            ComboBox_Reso.Items.AddRange(Resolutions);
            string r = Reg.Resolution;
            int i = Array.IndexOf(Resolutions, r);
            if (i == -1)
                i = 0;
            ComboBox_Reso.SelectedIndex = i;
            CB_DevMode.Text = Resources.Txt_DevMode;
            CB_DevMode.Checked = HashCalc.CalcHash(Reg.GetPCName()) == Reg.DevMode;
            Updating = false;

            CB_ShowLog_CheckedChanged(null, null);
            CB_EasyMode_CheckedChanged(null, null);

            Reg.LoadGoldPathFromRegistry();
            Log(Resources.Log_SetGold + Reg.GoldPath);
            UpdateInstallation();
        }

        private void UpdateInstallation()
        {
            LBL_Gold.Text = Resources.Lbl_Gold + (Reg.GoldPath ?? Resources._null);
            if (Reg.IsGoldValid())
            {
                CB_GoldOK.Text = Resources.Status_Ok;
                CB_GoldOK.Checked = true;
                Btn_UpdateMPMaps.Enabled = true;
                Btn_GoldSave.Enabled = !Reg.GoldHasReg;
            }
            else
            {
                CB_GoldOK.Text = Resources.Status_Invalid;
                CB_GoldOK.Checked = false;
                Btn_UpdateMPMaps.Enabled = false;
                Btn_GoldSave.Enabled = false;
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
            TaskUpdateMPMaps t = new TaskUpdateMPMaps
            {
                MM = this
            };
            Prog.ShowWorkDialog(t, this);
        }

        internal bool EasyMode
        {
            get => CB_EasyMode.Checked;
            set => CB_EasyMode.Checked = value;
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
            Reg.SetGoldReg();
            Log(Resources.Log_RegGold + Reg.GoldPath);
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
            string sel = ComboBox_Reso.SelectedItem as string;
            if (sel.Equals("default"))
                sel = "0";
            Reg.Resolution = sel;
            Log(Resources.Log_SetReso + sel);
            if (ResolutionNeedsDev[ComboBox_Reso.SelectedIndex] && !CB_DevMode.Checked)
            {
                if (EasyMode || MessageBox.Show(Resources.Txt_QuestEnableDevModeRes, sel, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
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
    }
}
