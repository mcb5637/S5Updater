using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Text.RegularExpressions;
using MsBox.Avalonia;
using System.Resources;
using System.Xml.XPath;

namespace S5Updater2
{
    partial class TaskConvertHE : IUpdaterTask
    {
        internal required MainWindow MM;
        internal Status Status;

        public async Task Work(ProgressDialog.ReportProgressDel r)
        {
            Status = Status.Ok;
            SetupPath();
            if (Status != Status.Ok)
                return;
            CopyInstall(r);
            if (Status != Status.Ok)
                return;
            await PatchExe(r);
            if (Status != Status.Ok)
                return;
            await PatchData(r);
            if (Status != Status.Ok)
                return;
            await PatchFonts(r);
            if (Status != Status.Ok)
                return;
            CreateSortcuts(r);
        }

        private async Task PatchFonts(ProgressDialog.ReportProgressDel r)
        {
            try
            {
                if (MM.Reg.GoldPath == null)
                    throw new NullReferenceException();
                if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName != "de" || !MM.EasyMode)
                {
                    var box = MessageBoxManager.GetMessageBoxStandard("", MM.EasyMode ? Res.TaskConvert_QstRu : Res.TaskConvert_QstRuFull, MsBox.Avalonia.Enums.ButtonEnum.YesNo);
                    if (await box.ShowAsync() == MsBox.Avalonia.Enums.ButtonResult.Yes)
                    {
                        r(0, 100, Res.TaskConvert_SkipFont, Res.TaskConvert_SkipFont);
                        return;
                    }
                }

                string patchfile = Path.Combine(MM.Reg.GoldPath, "Tmp_font.zip");
                r(0, 100, Res.TaskConvert_DownloadFont, Res.TaskConvert_DownloadFont);
                await MainUpdater.DownloadFile("https://github.com/mcb5637/s5HEfonts/archive/refs/heads/master.zip", patchfile, r);
                r(100, 100, Res.Done, Res.Done);
                r(0, 100, Res.TaskConvert_PatchFont, Res.TaskConvert_PatchFont);
                string[] extras = ["base", "extra1", "extra2"];
                using (ZipArchive a = ZipFile.OpenRead(patchfile))
                {
                    int filenium = a.Entries.Count;
                    int count = 0;
                    foreach (ZipArchiveEntry e in a.Entries)
                    {
                        if (e.IsFolder())
                        {
                            count++;
                            continue;
                        }
                        r(count, filenium, e.FullName, null);
                        foreach (string s in extras)
                        {
                            string destinationFileName = Path.Combine(MM.Reg.GoldPath, s, "shr\\menu\\Fonts", Path.GetFileName(e.FullName));
                            string? path = Path.GetDirectoryName(destinationFileName);
                            if (path != null)
                                Directory.CreateDirectory(path);
                            e.ExtractToFile(destinationFileName, true);
                        }
                        count++;
                    }
                }
                File.Delete(patchfile);
                r(100, 100, Res.Done, Res.Done);
            }
            catch (Exception e)
            {
                r(0, 100, e.ToString(), e.ToString());
                Status = Status.Error;
            }
        }

        private async Task PatchData(ProgressDialog.ReportProgressDel r)
        {
            try
            {
                if (MM.Reg.GoldPath == null)
                    throw new NullReferenceException();
                string patchfile = Path.Combine(MM.Reg.GoldPath, "Tmp_data.zip");
                r(0, 100, Res.TaskConvert_DownloadFileData, Res.TaskConvert_DownloadFileData);
                await MainUpdater.DownloadFile("https://github.com/mcb5637/s5HEmodification/archive/refs/heads/master.zip", patchfile, r);
                r(100, 100, Res.Done, Res.Done);
                r(0, 100, Res.TaskConvert_PatchData, Res.TaskConvert_PatchData);
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
                    Regex gui = GUIRegex();
                    Regex config = ConfigRegex();
                    Regex script = ScriptRegex();
                    foreach (ZipArchiveEntry e in a.Entries)
                    {
                        if (e.IsFolder())
                        {
                            count++;
                            continue;
                        }
                        string extrapath;
                        if (e.FullName.StartsWith("s5HEmodification-master/base"))
                            extrapath = "base\\shr";
                        else if (e.FullName.StartsWith("s5HEmodification-master/extra1"))
                            extrapath = "extra1\\shr";
                        else if (e.FullName.StartsWith("s5HEmodification-master/extra2"))
                            extrapath = "extra2\\shr";
                        else
                        {
                            r(count, filenium, "err: " + e.FullName, "err: " + e.FullName);
                            count++;
                            continue;
                        }
                        string filepath;
                        if (gui.IsMatch(e.FullName))
                            filepath = "menu\\Projects\\mainmenu.xml";
                        else if (config.IsMatch(e.FullName))
                            filepath = Path.Combine("config", config.Replace(e.FullName, "").Replace("/", "\\"));
                        else if (script.IsMatch(e.FullName))
                            filepath = Path.Combine("Script", script.Replace(e.FullName, "").Replace("/", "\\"));
                        else
                        {
                            r(count, filenium, "err: " + e.FullName, "err: " + e.FullName);
                            count++;
                            continue;
                        }
                        r(count, filenium, e.FullName, null);
                        string destinationFileName = Path.Combine(MM.Reg.GoldPath, extrapath, filepath);
                        string? path = Path.GetDirectoryName(destinationFileName);
                        if (path != null)
                            Directory.CreateDirectory(path);
                        e.ExtractToFile(destinationFileName, true);
                        count++;
                    }
                }
                File.Delete(patchfile);
                r(100, 100, Res.Done, Res.Done);
            }
            catch (Exception e)
            {
                r(0, 100, e.ToString(), e.ToString());
                Status = Status.Error;
            }
        }

        private async Task PatchExe(ProgressDialog.ReportProgressDel r)
        {
            TaskUpdate106 t = new()
            {
                MM = MM
            };
            await t.Work(r);
            Status = t.Status;
        }

        private void CopyInstall(ProgressDialog.ReportProgressDel r)
        {
            if (MM.Reg.GoldPath == null)
                throw new NullReferenceException();
            if (MM.Reg.HEPath == null)
                throw new NullReferenceException();
            r(0, 100, Res.TaskConvert_CopyingInstall, Res.TaskConvert_CopyingInstall);
            MainUpdater.Copy(MM.Reg.HEPath, MM.Reg.GoldPath, [], r);
            string goldbin = Path.Combine(MM.Reg.GoldPath, "bin");
            MainUpdater.Copy(goldbin, Path.Combine(MM.Reg.GoldPath, "extra1\\bin"), [], r);
            MainUpdater.Copy(goldbin, Path.Combine(MM.Reg.GoldPath, "extra2\\bin"), [], r);
            r(100, 100, Res.Done, Res.Done);
        }

        private async void SetupPath()
        {
            if (MM.Reg.HEPath == null)
                throw new NullReferenceException();
            if (string.IsNullOrEmpty(MM.Reg.GoldPath))
            {
                MM.Reg.GoldPath = Path.Combine(MainUpdater.GetParentDir(MM.Reg.HEPath), "SetthersHoK_ConvertedGold");
            }
            if (!MM.Reg.GoldHasReg && (MM.EasyMode || await regquest()))
            {
                MM.Reg.SetGoldReg();
            }

            async Task<bool> regquest()
            {
                var box = MessageBoxManager.GetMessageBoxStandard("", Res.TaskConvert_QstSetReg + MM.Reg.GoldPath, MsBox.Avalonia.Enums.ButtonEnum.YesNo);
                return await box.ShowAsync() == MsBox.Avalonia.Enums.ButtonResult.Yes;
            }
        }

        private async void CreateSortcuts(ProgressDialog.ReportProgressDel r)
        {
            if (MM.Reg.GoldPath == null)
                throw new NullReferenceException();
            if (!OperatingSystem.IsWindows())
                return;
            string p = MM.Reg.GoldPath;
            Action<string> log = (s) =>
            {
                r(0, 100, s, s);
            };
            if (await messagebox(Res.TaskConvert_QstLink))
            {
                MainUpdater.CreateLinkPS(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Settlers HoK Base.lnk"),
                    Path.Combine(p, "bin/settlershok.exe"), "", log);
                MainUpdater.CreateLinkPS(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Settlers HoK Extra1.lnk"),
                    Path.Combine(p, "extra1/bin/settlershok.exe"), "", log);
                MainUpdater.CreateLinkPS(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Settlers HoK Extra2.lnk"),
                    Path.Combine(p, "extra2/bin/settlershok.exe"), "", log);
            }
            if (await messagebox(Res.TaskConvert_QstLinkEditor))
            {
                MainUpdater.CreateLinkPS(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Settlers HoK Extra1 Editor.lnk"),
                    Path.Combine(p, "extra1/bin/shokmapeditor.exe"), "", log);
                MainUpdater.CreateLinkPS(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Settlers HoK Extra2 Editor.lnk"),
                    Path.Combine(p, "extra2/bin/shokmapeditor.exe"), "", log);
            }

            async Task<bool> messagebox(string q)
            {
                var box = MessageBoxManager.GetMessageBoxStandard("", q, MsBox.Avalonia.Enums.ButtonEnum.YesNo);
                return await box.ShowAsync() == MsBox.Avalonia.Enums.ButtonResult.Yes;
            }
        }

        [GeneratedRegex("^s5HEmodification-master/(base|extra1|extra2)/mainmenu.xml")]
        private static partial Regex GUIRegex();
        [GeneratedRegex("^s5HEmodification-master/(base|extra1|extra2)/config/")]
        private static partial Regex ConfigRegex();
        [GeneratedRegex("^s5HEmodification-master/(base|extra1|extra2)/[Ss]cript/")]
        private static partial Regex ScriptRegex();
    }
}
