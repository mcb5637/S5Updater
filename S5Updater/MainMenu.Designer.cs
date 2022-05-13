
namespace S5Updater
{
    partial class MainMenu
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.GroupBox_Installation = new System.Windows.Forms.GroupBox();
            this.BTN_SetHE = new System.Windows.Forms.Button();
            this.LBL_HE = new System.Windows.Forms.Label();
            this.CB_HEOk = new System.Windows.Forms.CheckBox();
            this.Btn_GoldSave = new System.Windows.Forms.Button();
            this.CB_GoldOK = new System.Windows.Forms.CheckBox();
            this.BTN_SetGold = new System.Windows.Forms.Button();
            this.LBL_Gold = new System.Windows.Forms.Label();
            this.Dlg_FolderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.TextBox_Output = new System.Windows.Forms.TextBox();
            this.GroupBox_Updates = new System.Windows.Forms.GroupBox();
            this.Cb_EnableHook = new System.Windows.Forms.CheckBox();
            this.Btn_UpdateHook = new System.Windows.Forms.Button();
            this.Btn_MapInstallerGold = new System.Windows.Forms.Button();
            this.CB_AllPatched = new System.Windows.Forms.CheckBox();
            this.BTN_Patch106 = new System.Windows.Forms.Button();
            this.BTN_UpdateFrom105 = new System.Windows.Forms.Button();
            this.Btn_UpdateMPMaps = new System.Windows.Forms.Button();
            this.GroupBox_Settings = new System.Windows.Forms.GroupBox();
            this.BTN_DBG_HashFile = new System.Windows.Forms.Button();
            this.CB_EasyMode = new System.Windows.Forms.CheckBox();
            this.CB_ShowLog = new System.Windows.Forms.CheckBox();
            this.GroupBox_Registry = new System.Windows.Forms.GroupBox();
            this.CB_ShowIntro = new System.Windows.Forms.CheckBox();
            this.ComboBox_Langua = new System.Windows.Forms.ComboBox();
            this.LBL_Langua = new System.Windows.Forms.Label();
            this.CB_DevMode = new System.Windows.Forms.CheckBox();
            this.LBL_Reso = new System.Windows.Forms.Label();
            this.ComboBox_Reso = new System.Windows.Forms.ComboBox();
            this.GroupBox_Convert = new System.Windows.Forms.GroupBox();
            this.Btn_MapInstallerHE = new System.Windows.Forms.Button();
            this.Btn_ConvertHE = new System.Windows.Forms.Button();
            this.Dlg_OpenFile = new System.Windows.Forms.OpenFileDialog();
            this.TabControl_Main = new System.Windows.Forms.TabControl();
            this.tabPageGeneral = new System.Windows.Forms.TabPage();
            this.GroupBox_GoldDev = new System.Windows.Forms.GroupBox();
            this.Cb_EnableDebugger = new System.Windows.Forms.CheckBox();
            this.Btn_UpdateDebugger = new System.Windows.Forms.Button();
            this.tabPageMaps = new System.Windows.Forms.TabPage();
            this.CheckedListBox_Mappacks = new System.Windows.Forms.CheckedListBox();
            this.BTN_UpdateMappacks = new System.Windows.Forms.Button();
            this.label_MapDownload = new System.Windows.Forms.Label();
            this.BTN_DBG_Xml = new System.Windows.Forms.Button();
            this.Btn_ExtVersionCheckGold = new System.Windows.Forms.Button();
            this.Btn_ExtVersionCheckHE = new System.Windows.Forms.Button();
            this.GroupBox_Installation.SuspendLayout();
            this.GroupBox_Updates.SuspendLayout();
            this.GroupBox_Settings.SuspendLayout();
            this.GroupBox_Registry.SuspendLayout();
            this.GroupBox_Convert.SuspendLayout();
            this.TabControl_Main.SuspendLayout();
            this.tabPageGeneral.SuspendLayout();
            this.GroupBox_GoldDev.SuspendLayout();
            this.tabPageMaps.SuspendLayout();
            this.SuspendLayout();
            // 
            // GroupBox_Installation
            // 
            this.GroupBox_Installation.Controls.Add(this.BTN_SetHE);
            this.GroupBox_Installation.Controls.Add(this.LBL_HE);
            this.GroupBox_Installation.Controls.Add(this.CB_HEOk);
            this.GroupBox_Installation.Controls.Add(this.Btn_GoldSave);
            this.GroupBox_Installation.Controls.Add(this.CB_GoldOK);
            this.GroupBox_Installation.Controls.Add(this.BTN_SetGold);
            this.GroupBox_Installation.Controls.Add(this.LBL_Gold);
            this.GroupBox_Installation.Location = new System.Drawing.Point(6, 6);
            this.GroupBox_Installation.Name = "GroupBox_Installation";
            this.GroupBox_Installation.Size = new System.Drawing.Size(514, 120);
            this.GroupBox_Installation.TabIndex = 0;
            this.GroupBox_Installation.TabStop = false;
            this.GroupBox_Installation.Text = "GroupBox_Installation";
            // 
            // BTN_SetHE
            // 
            this.BTN_SetHE.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BTN_SetHE.Location = new System.Drawing.Point(331, 90);
            this.BTN_SetHE.Name = "BTN_SetHE";
            this.BTN_SetHE.Size = new System.Drawing.Size(75, 23);
            this.BTN_SetHE.TabIndex = 6;
            this.BTN_SetHE.Text = "button1";
            this.BTN_SetHE.UseVisualStyleBackColor = true;
            this.BTN_SetHE.Click += new System.EventHandler(this.BTN_SetHE_Click);
            // 
            // LBL_HE
            // 
            this.LBL_HE.AutoSize = true;
            this.LBL_HE.Location = new System.Drawing.Point(6, 74);
            this.LBL_HE.Name = "LBL_HE";
            this.LBL_HE.Size = new System.Drawing.Size(35, 13);
            this.LBL_HE.TabIndex = 5;
            this.LBL_HE.Text = "label1";
            // 
            // CB_HEOk
            // 
            this.CB_HEOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CB_HEOk.AutoSize = true;
            this.CB_HEOk.Enabled = false;
            this.CB_HEOk.Location = new System.Drawing.Point(242, 94);
            this.CB_HEOk.Name = "CB_HEOk";
            this.CB_HEOk.Size = new System.Drawing.Size(80, 17);
            this.CB_HEOk.TabIndex = 4;
            this.CB_HEOk.Text = "checkBox1";
            this.CB_HEOk.UseVisualStyleBackColor = true;
            // 
            // Btn_GoldSave
            // 
            this.Btn_GoldSave.Location = new System.Drawing.Point(412, 40);
            this.Btn_GoldSave.Name = "Btn_GoldSave";
            this.Btn_GoldSave.Size = new System.Drawing.Size(96, 23);
            this.Btn_GoldSave.TabIndex = 3;
            this.Btn_GoldSave.Text = "button1";
            this.Btn_GoldSave.UseVisualStyleBackColor = true;
            this.Btn_GoldSave.Click += new System.EventHandler(this.Btn_GoldSave_Click);
            // 
            // CB_GoldOK
            // 
            this.CB_GoldOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CB_GoldOK.AutoSize = true;
            this.CB_GoldOK.Enabled = false;
            this.CB_GoldOK.Location = new System.Drawing.Point(242, 44);
            this.CB_GoldOK.Name = "CB_GoldOK";
            this.CB_GoldOK.Size = new System.Drawing.Size(83, 17);
            this.CB_GoldOK.TabIndex = 2;
            this.CB_GoldOK.Text = "CB_GoldOK";
            this.CB_GoldOK.UseVisualStyleBackColor = true;
            // 
            // BTN_SetGold
            // 
            this.BTN_SetGold.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BTN_SetGold.Location = new System.Drawing.Point(331, 40);
            this.BTN_SetGold.Name = "BTN_SetGold";
            this.BTN_SetGold.Size = new System.Drawing.Size(75, 23);
            this.BTN_SetGold.TabIndex = 1;
            this.BTN_SetGold.Text = "button1";
            this.BTN_SetGold.UseVisualStyleBackColor = true;
            this.BTN_SetGold.Click += new System.EventHandler(this.BTN_SetGold_Click);
            // 
            // LBL_Gold
            // 
            this.LBL_Gold.AutoSize = true;
            this.LBL_Gold.Location = new System.Drawing.Point(6, 24);
            this.LBL_Gold.Name = "LBL_Gold";
            this.LBL_Gold.Size = new System.Drawing.Size(35, 13);
            this.LBL_Gold.TabIndex = 0;
            this.LBL_Gold.Text = "label1";
            // 
            // TextBox_Output
            // 
            this.TextBox_Output.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBox_Output.Location = new System.Drawing.Point(534, 12);
            this.TextBox_Output.MaximumSize = new System.Drawing.Size(571, 570);
            this.TextBox_Output.MinimumSize = new System.Drawing.Size(571, 570);
            this.TextBox_Output.Multiline = true;
            this.TextBox_Output.Name = "TextBox_Output";
            this.TextBox_Output.ReadOnly = true;
            this.TextBox_Output.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TextBox_Output.Size = new System.Drawing.Size(571, 570);
            this.TextBox_Output.TabIndex = 1;
            this.TextBox_Output.WordWrap = false;
            // 
            // GroupBox_Updates
            // 
            this.GroupBox_Updates.Controls.Add(this.Btn_ExtVersionCheckGold);
            this.GroupBox_Updates.Controls.Add(this.Cb_EnableHook);
            this.GroupBox_Updates.Controls.Add(this.Btn_UpdateHook);
            this.GroupBox_Updates.Controls.Add(this.Btn_MapInstallerGold);
            this.GroupBox_Updates.Controls.Add(this.CB_AllPatched);
            this.GroupBox_Updates.Controls.Add(this.BTN_Patch106);
            this.GroupBox_Updates.Controls.Add(this.BTN_UpdateFrom105);
            this.GroupBox_Updates.Controls.Add(this.Btn_UpdateMPMaps);
            this.GroupBox_Updates.Location = new System.Drawing.Point(6, 132);
            this.GroupBox_Updates.Name = "GroupBox_Updates";
            this.GroupBox_Updates.Size = new System.Drawing.Size(514, 136);
            this.GroupBox_Updates.TabIndex = 2;
            this.GroupBox_Updates.TabStop = false;
            this.GroupBox_Updates.Text = "goldupdate";
            // 
            // Cb_EnableHook
            // 
            this.Cb_EnableHook.AutoSize = true;
            this.Cb_EnableHook.Location = new System.Drawing.Point(191, 110);
            this.Cb_EnableHook.Name = "Cb_EnableHook";
            this.Cb_EnableHook.Size = new System.Drawing.Size(80, 17);
            this.Cb_EnableHook.TabIndex = 5;
            this.Cb_EnableHook.Text = "checkBox1";
            this.Cb_EnableHook.UseVisualStyleBackColor = true;
            this.Cb_EnableHook.CheckedChanged += new System.EventHandler(this.Cb_EnableHook_CheckedChanged);
            // 
            // Btn_UpdateHook
            // 
            this.Btn_UpdateHook.Location = new System.Drawing.Point(6, 106);
            this.Btn_UpdateHook.Name = "Btn_UpdateHook";
            this.Btn_UpdateHook.Size = new System.Drawing.Size(179, 23);
            this.Btn_UpdateHook.TabIndex = 4;
            this.Btn_UpdateHook.Text = "button1";
            this.Btn_UpdateHook.UseVisualStyleBackColor = true;
            this.Btn_UpdateHook.Click += new System.EventHandler(this.Btn_UpdateHook_Click);
            // 
            // Btn_MapInstallerGold
            // 
            this.Btn_MapInstallerGold.Location = new System.Drawing.Point(331, 77);
            this.Btn_MapInstallerGold.Name = "Btn_MapInstallerGold";
            this.Btn_MapInstallerGold.Size = new System.Drawing.Size(177, 23);
            this.Btn_MapInstallerGold.TabIndex = 0;
            this.Btn_MapInstallerGold.Text = "button1";
            this.Btn_MapInstallerGold.UseVisualStyleBackColor = true;
            this.Btn_MapInstallerGold.Click += new System.EventHandler(this.Btn_MapInstallerGold_Click);
            // 
            // CB_AllPatched
            // 
            this.CB_AllPatched.AutoSize = true;
            this.CB_AllPatched.Enabled = false;
            this.CB_AllPatched.Location = new System.Drawing.Point(331, 23);
            this.CB_AllPatched.Name = "CB_AllPatched";
            this.CB_AllPatched.Size = new System.Drawing.Size(80, 17);
            this.CB_AllPatched.TabIndex = 3;
            this.CB_AllPatched.Text = "checkBox1";
            this.CB_AllPatched.UseVisualStyleBackColor = true;
            // 
            // BTN_Patch106
            // 
            this.BTN_Patch106.Location = new System.Drawing.Point(6, 48);
            this.BTN_Patch106.Name = "BTN_Patch106";
            this.BTN_Patch106.Size = new System.Drawing.Size(179, 23);
            this.BTN_Patch106.TabIndex = 2;
            this.BTN_Patch106.Text = "button1";
            this.BTN_Patch106.UseVisualStyleBackColor = true;
            this.BTN_Patch106.Click += new System.EventHandler(this.BTN_Patch106_Click);
            // 
            // BTN_UpdateFrom105
            // 
            this.BTN_UpdateFrom105.Location = new System.Drawing.Point(6, 19);
            this.BTN_UpdateFrom105.Name = "BTN_UpdateFrom105";
            this.BTN_UpdateFrom105.Size = new System.Drawing.Size(179, 23);
            this.BTN_UpdateFrom105.TabIndex = 1;
            this.BTN_UpdateFrom105.Text = "button1";
            this.BTN_UpdateFrom105.UseVisualStyleBackColor = true;
            this.BTN_UpdateFrom105.Click += new System.EventHandler(this.BTN_UpdateFrom105_Click);
            // 
            // Btn_UpdateMPMaps
            // 
            this.Btn_UpdateMPMaps.Location = new System.Drawing.Point(6, 77);
            this.Btn_UpdateMPMaps.Name = "Btn_UpdateMPMaps";
            this.Btn_UpdateMPMaps.Size = new System.Drawing.Size(179, 23);
            this.Btn_UpdateMPMaps.TabIndex = 0;
            this.Btn_UpdateMPMaps.Text = "button1";
            this.Btn_UpdateMPMaps.UseVisualStyleBackColor = true;
            this.Btn_UpdateMPMaps.Click += new System.EventHandler(this.Btn_UpdateMPMaps_Click);
            // 
            // GroupBox_Settings
            // 
            this.GroupBox_Settings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.GroupBox_Settings.Controls.Add(this.BTN_DBG_Xml);
            this.GroupBox_Settings.Controls.Add(this.BTN_DBG_HashFile);
            this.GroupBox_Settings.Controls.Add(this.CB_EasyMode);
            this.GroupBox_Settings.Controls.Add(this.CB_ShowLog);
            this.GroupBox_Settings.Location = new System.Drawing.Point(6, 501);
            this.GroupBox_Settings.Name = "GroupBox_Settings";
            this.GroupBox_Settings.Size = new System.Drawing.Size(514, 48);
            this.GroupBox_Settings.TabIndex = 3;
            this.GroupBox_Settings.TabStop = false;
            this.GroupBox_Settings.Text = "groupBox1";
            // 
            // BTN_DBG_HashFile
            // 
            this.BTN_DBG_HashFile.Location = new System.Drawing.Point(302, 15);
            this.BTN_DBG_HashFile.Name = "BTN_DBG_HashFile";
            this.BTN_DBG_HashFile.Size = new System.Drawing.Size(75, 23);
            this.BTN_DBG_HashFile.TabIndex = 2;
            this.BTN_DBG_HashFile.Text = "hash file";
            this.BTN_DBG_HashFile.UseVisualStyleBackColor = true;
            this.BTN_DBG_HashFile.Click += new System.EventHandler(this.BTN_DBG_HashFile_Click);
            // 
            // CB_EasyMode
            // 
            this.CB_EasyMode.AutoSize = true;
            this.CB_EasyMode.Checked = true;
            this.CB_EasyMode.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CB_EasyMode.Location = new System.Drawing.Point(142, 19);
            this.CB_EasyMode.Name = "CB_EasyMode";
            this.CB_EasyMode.Size = new System.Drawing.Size(80, 17);
            this.CB_EasyMode.TabIndex = 1;
            this.CB_EasyMode.Text = "checkBox2";
            this.CB_EasyMode.UseVisualStyleBackColor = true;
            this.CB_EasyMode.CheckedChanged += new System.EventHandler(this.CB_EasyMode_CheckedChanged);
            // 
            // CB_ShowLog
            // 
            this.CB_ShowLog.AutoSize = true;
            this.CB_ShowLog.Location = new System.Drawing.Point(6, 19);
            this.CB_ShowLog.Name = "CB_ShowLog";
            this.CB_ShowLog.Size = new System.Drawing.Size(80, 17);
            this.CB_ShowLog.TabIndex = 0;
            this.CB_ShowLog.Text = "checkBox1";
            this.CB_ShowLog.UseVisualStyleBackColor = true;
            this.CB_ShowLog.CheckedChanged += new System.EventHandler(this.CB_ShowLog_CheckedChanged);
            // 
            // GroupBox_Registry
            // 
            this.GroupBox_Registry.Controls.Add(this.CB_ShowIntro);
            this.GroupBox_Registry.Controls.Add(this.ComboBox_Langua);
            this.GroupBox_Registry.Controls.Add(this.LBL_Langua);
            this.GroupBox_Registry.Controls.Add(this.CB_DevMode);
            this.GroupBox_Registry.Controls.Add(this.LBL_Reso);
            this.GroupBox_Registry.Controls.Add(this.ComboBox_Reso);
            this.GroupBox_Registry.Location = new System.Drawing.Point(6, 274);
            this.GroupBox_Registry.Name = "GroupBox_Registry";
            this.GroupBox_Registry.Size = new System.Drawing.Size(514, 77);
            this.GroupBox_Registry.TabIndex = 4;
            this.GroupBox_Registry.TabStop = false;
            this.GroupBox_Registry.Text = "reg";
            // 
            // CB_ShowIntro
            // 
            this.CB_ShowIntro.AutoSize = true;
            this.CB_ShowIntro.Location = new System.Drawing.Point(326, 48);
            this.CB_ShowIntro.Name = "CB_ShowIntro";
            this.CB_ShowIntro.Size = new System.Drawing.Size(80, 17);
            this.CB_ShowIntro.TabIndex = 5;
            this.CB_ShowIntro.Text = "checkBox1";
            this.CB_ShowIntro.UseVisualStyleBackColor = true;
            this.CB_ShowIntro.CheckedChanged += new System.EventHandler(this.CB_ShowIntro_CheckedChanged);
            // 
            // ComboBox_Langua
            // 
            this.ComboBox_Langua.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBox_Langua.FormattingEnabled = true;
            this.ComboBox_Langua.Location = new System.Drawing.Point(67, 46);
            this.ComboBox_Langua.Name = "ComboBox_Langua";
            this.ComboBox_Langua.Size = new System.Drawing.Size(121, 21);
            this.ComboBox_Langua.TabIndex = 4;
            this.ComboBox_Langua.SelectedIndexChanged += new System.EventHandler(this.ComboBox_Langua_SelectedIndexChanged);
            // 
            // LBL_Langua
            // 
            this.LBL_Langua.AutoSize = true;
            this.LBL_Langua.Location = new System.Drawing.Point(6, 49);
            this.LBL_Langua.Name = "LBL_Langua";
            this.LBL_Langua.Size = new System.Drawing.Size(35, 13);
            this.LBL_Langua.TabIndex = 3;
            this.LBL_Langua.Text = "label1";
            // 
            // CB_DevMode
            // 
            this.CB_DevMode.AutoSize = true;
            this.CB_DevMode.Location = new System.Drawing.Point(326, 21);
            this.CB_DevMode.Name = "CB_DevMode";
            this.CB_DevMode.Size = new System.Drawing.Size(80, 17);
            this.CB_DevMode.TabIndex = 2;
            this.CB_DevMode.Text = "checkBox1";
            this.CB_DevMode.UseVisualStyleBackColor = true;
            this.CB_DevMode.CheckedChanged += new System.EventHandler(this.CB_DevMode_CheckedChanged);
            // 
            // LBL_Reso
            // 
            this.LBL_Reso.AutoSize = true;
            this.LBL_Reso.Location = new System.Drawing.Point(6, 22);
            this.LBL_Reso.Name = "LBL_Reso";
            this.LBL_Reso.Size = new System.Drawing.Size(35, 13);
            this.LBL_Reso.TabIndex = 1;
            this.LBL_Reso.Text = "label1";
            // 
            // ComboBox_Reso
            // 
            this.ComboBox_Reso.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBox_Reso.FormattingEnabled = true;
            this.ComboBox_Reso.Location = new System.Drawing.Point(67, 19);
            this.ComboBox_Reso.Name = "ComboBox_Reso";
            this.ComboBox_Reso.Size = new System.Drawing.Size(121, 21);
            this.ComboBox_Reso.TabIndex = 0;
            this.ComboBox_Reso.SelectedIndexChanged += new System.EventHandler(this.ComboBox_Reso_SelectedIndexChanged);
            // 
            // GroupBox_Convert
            // 
            this.GroupBox_Convert.Controls.Add(this.Btn_ExtVersionCheckHE);
            this.GroupBox_Convert.Controls.Add(this.Btn_MapInstallerHE);
            this.GroupBox_Convert.Controls.Add(this.Btn_ConvertHE);
            this.GroupBox_Convert.Location = new System.Drawing.Point(6, 357);
            this.GroupBox_Convert.Name = "GroupBox_Convert";
            this.GroupBox_Convert.Size = new System.Drawing.Size(514, 81);
            this.GroupBox_Convert.TabIndex = 5;
            this.GroupBox_Convert.TabStop = false;
            this.GroupBox_Convert.Text = "converhe";
            // 
            // Btn_MapInstallerHE
            // 
            this.Btn_MapInstallerHE.Location = new System.Drawing.Point(331, 48);
            this.Btn_MapInstallerHE.Name = "Btn_MapInstallerHE";
            this.Btn_MapInstallerHE.Size = new System.Drawing.Size(177, 23);
            this.Btn_MapInstallerHE.TabIndex = 1;
            this.Btn_MapInstallerHE.Text = "button1";
            this.Btn_MapInstallerHE.UseVisualStyleBackColor = true;
            this.Btn_MapInstallerHE.Click += new System.EventHandler(this.Btn_MapInstallerHE_Click);
            // 
            // Btn_ConvertHE
            // 
            this.Btn_ConvertHE.Location = new System.Drawing.Point(9, 19);
            this.Btn_ConvertHE.Name = "Btn_ConvertHE";
            this.Btn_ConvertHE.Size = new System.Drawing.Size(179, 23);
            this.Btn_ConvertHE.TabIndex = 0;
            this.Btn_ConvertHE.Text = "button1";
            this.Btn_ConvertHE.UseVisualStyleBackColor = true;
            this.Btn_ConvertHE.Click += new System.EventHandler(this.Btn_ConvertHE_Click);
            // 
            // TabControl_Main
            // 
            this.TabControl_Main.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.TabControl_Main.Controls.Add(this.tabPageGeneral);
            this.TabControl_Main.Controls.Add(this.tabPageMaps);
            this.TabControl_Main.Location = new System.Drawing.Point(0, 0);
            this.TabControl_Main.MaximumSize = new System.Drawing.Size(533, 587);
            this.TabControl_Main.MinimumSize = new System.Drawing.Size(533, 587);
            this.TabControl_Main.Name = "TabControl_Main";
            this.TabControl_Main.SelectedIndex = 0;
            this.TabControl_Main.Size = new System.Drawing.Size(533, 587);
            this.TabControl_Main.TabIndex = 0;
            // 
            // tabPageGeneral
            // 
            this.tabPageGeneral.Controls.Add(this.GroupBox_GoldDev);
            this.tabPageGeneral.Controls.Add(this.GroupBox_Settings);
            this.tabPageGeneral.Controls.Add(this.GroupBox_Convert);
            this.tabPageGeneral.Controls.Add(this.GroupBox_Registry);
            this.tabPageGeneral.Controls.Add(this.GroupBox_Installation);
            this.tabPageGeneral.Controls.Add(this.GroupBox_Updates);
            this.tabPageGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabPageGeneral.Name = "tabPageGeneral";
            this.tabPageGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageGeneral.Size = new System.Drawing.Size(525, 561);
            this.tabPageGeneral.TabIndex = 0;
            this.tabPageGeneral.Text = "tabPage1";
            this.tabPageGeneral.UseVisualStyleBackColor = true;
            // 
            // GroupBox_GoldDev
            // 
            this.GroupBox_GoldDev.Controls.Add(this.Cb_EnableDebugger);
            this.GroupBox_GoldDev.Controls.Add(this.Btn_UpdateDebugger);
            this.GroupBox_GoldDev.Location = new System.Drawing.Point(6, 444);
            this.GroupBox_GoldDev.Name = "GroupBox_GoldDev";
            this.GroupBox_GoldDev.Size = new System.Drawing.Size(514, 51);
            this.GroupBox_GoldDev.TabIndex = 6;
            this.GroupBox_GoldDev.TabStop = false;
            this.GroupBox_GoldDev.Text = "golddev";
            // 
            // Cb_EnableDebugger
            // 
            this.Cb_EnableDebugger.AutoSize = true;
            this.Cb_EnableDebugger.Location = new System.Drawing.Point(191, 23);
            this.Cb_EnableDebugger.Name = "Cb_EnableDebugger";
            this.Cb_EnableDebugger.Size = new System.Drawing.Size(80, 17);
            this.Cb_EnableDebugger.TabIndex = 1;
            this.Cb_EnableDebugger.Text = "checkBox1";
            this.Cb_EnableDebugger.UseVisualStyleBackColor = true;
            this.Cb_EnableDebugger.CheckedChanged += new System.EventHandler(this.Cb_EnableDebugger_CheckedChanged);
            // 
            // Btn_UpdateDebugger
            // 
            this.Btn_UpdateDebugger.Location = new System.Drawing.Point(6, 19);
            this.Btn_UpdateDebugger.Name = "Btn_UpdateDebugger";
            this.Btn_UpdateDebugger.Size = new System.Drawing.Size(179, 23);
            this.Btn_UpdateDebugger.TabIndex = 0;
            this.Btn_UpdateDebugger.Text = "button1";
            this.Btn_UpdateDebugger.UseVisualStyleBackColor = true;
            this.Btn_UpdateDebugger.Click += new System.EventHandler(this.Btn_UpdateDebugger_Click);
            // 
            // tabPageMaps
            // 
            this.tabPageMaps.Controls.Add(this.CheckedListBox_Mappacks);
            this.tabPageMaps.Controls.Add(this.BTN_UpdateMappacks);
            this.tabPageMaps.Controls.Add(this.label_MapDownload);
            this.tabPageMaps.Location = new System.Drawing.Point(4, 22);
            this.tabPageMaps.Name = "tabPageMaps";
            this.tabPageMaps.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMaps.Size = new System.Drawing.Size(525, 561);
            this.tabPageMaps.TabIndex = 1;
            this.tabPageMaps.Text = "tabPage2";
            this.tabPageMaps.UseVisualStyleBackColor = true;
            // 
            // CheckedListBox_Mappacks
            // 
            this.CheckedListBox_Mappacks.CheckOnClick = true;
            this.CheckedListBox_Mappacks.FormattingEnabled = true;
            this.CheckedListBox_Mappacks.Items.AddRange(new object[] {
            "EMS",
            "BS",
            "Speedwar"});
            this.CheckedListBox_Mappacks.Location = new System.Drawing.Point(10, 36);
            this.CheckedListBox_Mappacks.Name = "CheckedListBox_Mappacks";
            this.CheckedListBox_Mappacks.Size = new System.Drawing.Size(509, 49);
            this.CheckedListBox_Mappacks.TabIndex = 3;
            this.CheckedListBox_Mappacks.SelectedIndexChanged += new System.EventHandler(this.CheckedListBox_Mappacks_SelectedIndexChanged);
            // 
            // BTN_UpdateMappacks
            // 
            this.BTN_UpdateMappacks.Location = new System.Drawing.Point(10, 91);
            this.BTN_UpdateMappacks.Name = "BTN_UpdateMappacks";
            this.BTN_UpdateMappacks.Size = new System.Drawing.Size(509, 23);
            this.BTN_UpdateMappacks.TabIndex = 2;
            this.BTN_UpdateMappacks.Text = "BTN_UpdateMappacks";
            this.BTN_UpdateMappacks.UseVisualStyleBackColor = true;
            this.BTN_UpdateMappacks.Click += new System.EventHandler(this.BTN_UpdateMappacks_Click);
            // 
            // label_MapDownload
            // 
            this.label_MapDownload.AutoSize = true;
            this.label_MapDownload.Location = new System.Drawing.Point(7, 7);
            this.label_MapDownload.Name = "label_MapDownload";
            this.label_MapDownload.Size = new System.Drawing.Size(57, 13);
            this.label_MapDownload.TabIndex = 1;
            this.label_MapDownload.Text = "Mappacks";
            // 
            // BTN_DBG_Xml
            // 
            this.BTN_DBG_Xml.Location = new System.Drawing.Point(400, 15);
            this.BTN_DBG_Xml.Name = "BTN_DBG_Xml";
            this.BTN_DBG_Xml.Size = new System.Drawing.Size(75, 23);
            this.BTN_DBG_Xml.TabIndex = 3;
            this.BTN_DBG_Xml.Text = "add versxml";
            this.BTN_DBG_Xml.UseVisualStyleBackColor = true;
            this.BTN_DBG_Xml.Click += new System.EventHandler(this.BTN_DBG_Xml_Click);
            // 
            // Btn_ExtVersionCheckGold
            // 
            this.Btn_ExtVersionCheckGold.Location = new System.Drawing.Point(331, 48);
            this.Btn_ExtVersionCheckGold.Name = "Btn_ExtVersionCheckGold";
            this.Btn_ExtVersionCheckGold.Size = new System.Drawing.Size(177, 23);
            this.Btn_ExtVersionCheckGold.TabIndex = 6;
            this.Btn_ExtVersionCheckGold.Text = "button1";
            this.Btn_ExtVersionCheckGold.UseVisualStyleBackColor = true;
            this.Btn_ExtVersionCheckGold.Click += new System.EventHandler(this.Btn_ExtVersionCheckGold_Click);
            // 
            // Btn_ExtVersionCheckHE
            // 
            this.Btn_ExtVersionCheckHE.Location = new System.Drawing.Point(331, 19);
            this.Btn_ExtVersionCheckHE.Name = "Btn_ExtVersionCheckHE";
            this.Btn_ExtVersionCheckHE.Size = new System.Drawing.Size(177, 23);
            this.Btn_ExtVersionCheckHE.TabIndex = 2;
            this.Btn_ExtVersionCheckHE.Text = "button1";
            this.Btn_ExtVersionCheckHE.UseVisualStyleBackColor = true;
            this.Btn_ExtVersionCheckHE.Click += new System.EventHandler(this.Btn_ExtVersionCheckHE_Click);
            // 
            // MainMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1117, 596);
            this.Controls.Add(this.TextBox_Output);
            this.Controls.Add(this.TabControl_Main);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainMenu";
            this.Text = "MainMenu";
            this.Load += new System.EventHandler(this.MainMenu_Load);
            this.GroupBox_Installation.ResumeLayout(false);
            this.GroupBox_Installation.PerformLayout();
            this.GroupBox_Updates.ResumeLayout(false);
            this.GroupBox_Updates.PerformLayout();
            this.GroupBox_Settings.ResumeLayout(false);
            this.GroupBox_Settings.PerformLayout();
            this.GroupBox_Registry.ResumeLayout(false);
            this.GroupBox_Registry.PerformLayout();
            this.GroupBox_Convert.ResumeLayout(false);
            this.TabControl_Main.ResumeLayout(false);
            this.tabPageGeneral.ResumeLayout(false);
            this.GroupBox_GoldDev.ResumeLayout(false);
            this.GroupBox_GoldDev.PerformLayout();
            this.tabPageMaps.ResumeLayout(false);
            this.tabPageMaps.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox GroupBox_Installation;
        private System.Windows.Forms.Label LBL_Gold;
        private System.Windows.Forms.Button BTN_SetGold;
        private System.Windows.Forms.FolderBrowserDialog Dlg_FolderBrowser;
        private System.Windows.Forms.TextBox TextBox_Output;
        private System.Windows.Forms.CheckBox CB_GoldOK;
        private System.Windows.Forms.GroupBox GroupBox_Updates;
        private System.Windows.Forms.Button Btn_UpdateMPMaps;
        private System.Windows.Forms.GroupBox GroupBox_Settings;
        private System.Windows.Forms.CheckBox CB_EasyMode;
        private System.Windows.Forms.CheckBox CB_ShowLog;
        private System.Windows.Forms.Button Btn_GoldSave;
        private System.Windows.Forms.GroupBox GroupBox_Registry;
        private System.Windows.Forms.Label LBL_Reso;
        private System.Windows.Forms.ComboBox ComboBox_Reso;
        private System.Windows.Forms.CheckBox CB_DevMode;
        private System.Windows.Forms.ComboBox ComboBox_Langua;
        private System.Windows.Forms.Label LBL_Langua;
        private System.Windows.Forms.Button BTN_SetHE;
        private System.Windows.Forms.Label LBL_HE;
        private System.Windows.Forms.CheckBox CB_HEOk;
        private System.Windows.Forms.GroupBox GroupBox_Convert;
        private System.Windows.Forms.Button Btn_ConvertHE;
        private System.Windows.Forms.CheckBox CB_ShowIntro;
        private System.Windows.Forms.Button BTN_DBG_HashFile;
        private System.Windows.Forms.OpenFileDialog Dlg_OpenFile;
        private System.Windows.Forms.Button BTN_UpdateFrom105;
        private System.Windows.Forms.Button BTN_Patch106;
        private System.Windows.Forms.CheckBox CB_AllPatched;
        private System.Windows.Forms.Button Btn_MapInstallerGold;
        private System.Windows.Forms.Button Btn_MapInstallerHE;
        private System.Windows.Forms.Button Btn_UpdateHook;
        private System.Windows.Forms.CheckBox Cb_EnableHook;
        private System.Windows.Forms.TabControl TabControl_Main;
        private System.Windows.Forms.TabPage tabPageGeneral;
        private System.Windows.Forms.TabPage tabPageMaps;
        private System.Windows.Forms.Label label_MapDownload;
        private System.Windows.Forms.Button BTN_UpdateMappacks;
        private System.Windows.Forms.CheckedListBox CheckedListBox_Mappacks;
        private System.Windows.Forms.GroupBox GroupBox_GoldDev;
        private System.Windows.Forms.Button Btn_UpdateDebugger;
        private System.Windows.Forms.CheckBox Cb_EnableDebugger;
        private System.Windows.Forms.Button BTN_DBG_Xml;
        private System.Windows.Forms.Button Btn_ExtVersionCheckGold;
        private System.Windows.Forms.Button Btn_ExtVersionCheckHE;
    }
}