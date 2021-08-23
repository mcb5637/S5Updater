using S5Updater.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace S5Updater
{
    class TaskConvertHE : IUpdaterTask
    {
        internal MainMenu MM;
        internal int Status;

        public void Work(ProgressDialog.ReportProgressDel r)
        {
            Status = MainMenu.Status_OK;
            SetupPath();
            if (Status != MainMenu.Status_OK)
                return;
            CopyInstall(r);
            if (Status != MainMenu.Status_OK)
                return;
            PatchExe(r);
            if (Status != MainMenu.Status_OK)
                return;
            PatchData(r);
            if (Status != MainMenu.Status_OK)
                return;
            PatchFonts(r);
        }

        private void PatchFonts(ProgressDialog.ReportProgressDel r)
        {
            try
            {
                string patchfile = Path.Combine(MM.Reg.GoldPath, "Tmp_font.zip");
                r(0, Resources.TaskConvert_DownloadFont);
                MainUpdater.DownlaodFile("https://cdn.discordapp.com/attachments/276409631746686976/788789883300741140/Schriftarten.zip", patchfile, r);
                r(100, Resources.Done);
                r(0, Resources.TaskConvert_PatchFont);
                string[] extras = new string[] { "base", "extra1", "extra2" };
                using (ZipArchive a = ZipFile.OpenRead(patchfile))
                {
                    foreach (ZipArchiveEntry e in a.Entries)
                    {
                        int filenium = a.Entries.Count;
                        int count = 0;
                        if (e.IsFolder())
                        {
                            count++;
                            continue;
                        }
                        r(count * 100 / filenium, null);
                        foreach (string s in extras)
                        {
                            string destinationFileName = Path.Combine(MM.Reg.GoldPath, s, "shr\\menu\\Fonts", e.FullName);
                            Directory.CreateDirectory(Path.GetDirectoryName(destinationFileName));
                            e.ExtractToFile(destinationFileName, true);
                        }
                        count++;
                    }
                }
                File.Delete(patchfile);
                r(100, Resources.Done);
            }
            catch (Exception e)
            {
                r(0, e.ToString());
                Status = MainMenu.Status_Error;
            }
        }

        private void PatchData(ProgressDialog.ReportProgressDel r)
        {
            try
            {
                string patchfile = Path.Combine(MM.Reg.GoldPath, "Tmp_data.zip");
                r(0, Resources.TaskConvert_DownloadFileData);
                MainUpdater.DownlaodFile("https://dedk.de/wiki/lib/exe/fetch.php?media=multiplayer:help:hemodification.zip", patchfile, r);
                r(100, Resources.Done);
                r(0, Resources.TaskConvert_PatchData);
                File.Delete(Path.Combine(MM.Reg.GoldPath, "base\\shr\\menu\\Projects\\mainmenu.xml"));
                File.Delete(Path.Combine(MM.Reg.GoldPath, "extra1\\shr\\menu\\Projects\\mainmenu.xml"));
                File.Delete(Path.Combine(MM.Reg.GoldPath, "extra2\\shr\\menu\\Projects\\mainmenu.xml"));
                Directory.Delete(Path.Combine(MM.Reg.GoldPath, "base\\shr\\config"), true);
                Directory.Delete(Path.Combine(MM.Reg.GoldPath, "base\\shr\\Script"), true);
                Directory.Delete(Path.Combine(MM.Reg.GoldPath, "extra1\\shr\\config"), true);
                Directory.Delete(Path.Combine(MM.Reg.GoldPath, "extra1\\shr\\Script"), true);
                Directory.Delete(Path.Combine(MM.Reg.GoldPath, "extra2\\shr\\config"), true);
                Directory.Delete(Path.Combine(MM.Reg.GoldPath, "extra2\\shr\\Script"), true);
                using (ZipArchive a = ZipFile.OpenRead(patchfile))
                {
                    int filenium = a.Entries.Count;
                    int count = 0;
                    Regex gui = new Regex("^HEModification/(base|extra1|extra2)/mainmenu.xml");
                    Regex config = new Regex("^HEModification/(base|extra1|extra2)/config/");
                    Regex script = new Regex("^HEModification/(base|extra1|extra2)/[Ss]cript/");
                    foreach (ZipArchiveEntry e in a.Entries)
                    {
                        if (e.IsFolder())
                        {
                            count++;
                            continue;
                        }
                        string extrapath = null;
                        if (e.FullName.StartsWith("HEModification/base"))
                            extrapath = "base\\shr";
                        else if (e.FullName.StartsWith("HEModification/extra1"))
                            extrapath = "extra1\\shr";
                        else if (e.FullName.StartsWith("HEModification/extra2"))
                            extrapath = "extra2\\shr";
                        else
                        {
                            r(count * 100 / filenium, "err: " + e.FullName);
                            count++;
                            continue;
                        }
                        string filepath = null;
                        if (gui.IsMatch(e.FullName))
                            filepath = "menu\\Projects\\mainmenu.xml";
                        else if (config.IsMatch(e.FullName))
                            filepath = Path.Combine("config", config.Replace(e.FullName, "").Replace("/", "\\"));
                        else if (script.IsMatch(e.FullName))
                            filepath = Path.Combine("Script", script.Replace(e.FullName, "").Replace("/", "\\"));
                        else
                        {
                            r(count * 100 / filenium, "err: " + e.FullName);
                            count++;
                            continue;
                        }
                        r(count * 100 / filenium, null);
                        string destinationFileName = Path.Combine(MM.Reg.GoldPath, extrapath, filepath);
                        Directory.CreateDirectory(Path.GetDirectoryName(destinationFileName));
                        e.ExtractToFile(destinationFileName, true);
                        count++;
                    }
                }
                File.Delete(patchfile);
                r(100, Resources.Done);
            }
            catch (Exception e)
            {
                r(0, e.ToString());
                Status = MainMenu.Status_Error;
            }
        }

        private void PatchExe(ProgressDialog.ReportProgressDel r)
        {
            TaskUpdate106 t = new TaskUpdate106()
            {
                MM = MM
            };
            t.Work(r);
            Status = t.Status;
        }

        private void CopyInstall(ProgressDialog.ReportProgressDel r)
        {
            r(0, Resources.TaskConvert_CopyingInstall);
            MainUpdater.Copy(MM.Reg.HEPath, MM.Reg.GoldPath, Array.Empty<string>());
            string goldbin = Path.Combine(MM.Reg.GoldPath, "bin");
            MainUpdater.Copy(goldbin, Path.Combine(MM.Reg.GoldPath, "extra1\\bin"), Array.Empty<string>());
            MainUpdater.Copy(goldbin, Path.Combine(MM.Reg.GoldPath, "extra2\\bin"), Array.Empty<string>());
            r(100, Resources.Done);
        }

        private void SetupPath()
        {
            if (string.IsNullOrEmpty(MM.Reg.GoldPath))
            {
                MM.Reg.GoldPath = Path.Combine(MainUpdater.GetParentDir(MM.Reg.HEPath), "SetthersHoK_ConvertedGold");
            }
            if (!MM.Reg.GoldHasReg && (MM.EasyMode || MessageBox.Show(Resources.TaskConvert_QstSetReg + MM.Reg.GoldPath, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes))
            {
                MM.Reg.SetGoldReg();
            }
        }
    }
}
