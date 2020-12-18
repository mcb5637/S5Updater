
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
            this.LBL_Gold = new System.Windows.Forms.Label();
            this.BTN_SetGold = new System.Windows.Forms.Button();
            this.Dlg_FolderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.TextBox_Output = new System.Windows.Forms.TextBox();
            this.CB_GoldOK = new System.Windows.Forms.CheckBox();
            this.GroupBox_Updates = new System.Windows.Forms.GroupBox();
            this.Btn_UpdateMPMaps = new System.Windows.Forms.Button();
            this.GroupBox_Installation.SuspendLayout();
            this.GroupBox_Updates.SuspendLayout();
            this.SuspendLayout();
            // 
            // GroupBox_Installation
            // 
            this.GroupBox_Installation.Controls.Add(this.CB_GoldOK);
            this.GroupBox_Installation.Controls.Add(this.BTN_SetGold);
            this.GroupBox_Installation.Controls.Add(this.LBL_Gold);
            this.GroupBox_Installation.Location = new System.Drawing.Point(12, 12);
            this.GroupBox_Installation.Name = "GroupBox_Installation";
            this.GroupBox_Installation.Size = new System.Drawing.Size(514, 100);
            this.GroupBox_Installation.TabIndex = 0;
            this.GroupBox_Installation.TabStop = false;
            this.GroupBox_Installation.Text = "GroupBox_Installation";
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
            // BTN_SetGold
            // 
            this.BTN_SetGold.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BTN_SetGold.Location = new System.Drawing.Point(433, 19);
            this.BTN_SetGold.Name = "BTN_SetGold";
            this.BTN_SetGold.Size = new System.Drawing.Size(75, 23);
            this.BTN_SetGold.TabIndex = 1;
            this.BTN_SetGold.Text = "button1";
            this.BTN_SetGold.UseVisualStyleBackColor = true;
            this.BTN_SetGold.Click += new System.EventHandler(this.BTN_SetGold_Click);
            // 
            // TextBox_Output
            // 
            this.TextBox_Output.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBox_Output.Location = new System.Drawing.Point(532, 12);
            this.TextBox_Output.Multiline = true;
            this.TextBox_Output.Name = "TextBox_Output";
            this.TextBox_Output.ReadOnly = true;
            this.TextBox_Output.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TextBox_Output.Size = new System.Drawing.Size(567, 531);
            this.TextBox_Output.TabIndex = 1;
            this.TextBox_Output.WordWrap = false;
            // 
            // CB_GoldOK
            // 
            this.CB_GoldOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CB_GoldOK.AutoSize = true;
            this.CB_GoldOK.Enabled = false;
            this.CB_GoldOK.Location = new System.Drawing.Point(347, 23);
            this.CB_GoldOK.Name = "CB_GoldOK";
            this.CB_GoldOK.Size = new System.Drawing.Size(83, 17);
            this.CB_GoldOK.TabIndex = 2;
            this.CB_GoldOK.Text = "CB_GoldOK";
            this.CB_GoldOK.UseVisualStyleBackColor = true;
            // 
            // GroupBox_Updates
            // 
            this.GroupBox_Updates.Controls.Add(this.Btn_UpdateMPMaps);
            this.GroupBox_Updates.Location = new System.Drawing.Point(12, 118);
            this.GroupBox_Updates.Name = "GroupBox_Updates";
            this.GroupBox_Updates.Size = new System.Drawing.Size(514, 233);
            this.GroupBox_Updates.TabIndex = 2;
            this.GroupBox_Updates.TabStop = false;
            this.GroupBox_Updates.Text = "groupBox1";
            // 
            // Btn_UpdateMPMaps
            // 
            this.Btn_UpdateMPMaps.Location = new System.Drawing.Point(6, 19);
            this.Btn_UpdateMPMaps.Name = "Btn_UpdateMPMaps";
            this.Btn_UpdateMPMaps.Size = new System.Drawing.Size(75, 23);
            this.Btn_UpdateMPMaps.TabIndex = 0;
            this.Btn_UpdateMPMaps.Text = "button1";
            this.Btn_UpdateMPMaps.UseVisualStyleBackColor = true;
            this.Btn_UpdateMPMaps.Click += new System.EventHandler(this.Btn_UpdateMPMaps_Click);
            // 
            // MainMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1111, 555);
            this.Controls.Add(this.GroupBox_Updates);
            this.Controls.Add(this.TextBox_Output);
            this.Controls.Add(this.GroupBox_Installation);
            this.Name = "MainMenu";
            this.Text = "MainMenu";
            this.Load += new System.EventHandler(this.MainMenu_Load);
            this.GroupBox_Installation.ResumeLayout(false);
            this.GroupBox_Installation.PerformLayout();
            this.GroupBox_Updates.ResumeLayout(false);
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
    }
}