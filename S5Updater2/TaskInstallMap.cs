using bbaLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace S5Updater2
{
    internal class TaskInstallMap : IUpdaterTask
    {
        internal required MainWindow MM;
        internal Status Status;
        internal required string TargetPath;
        internal required string[] Files;
        internal required bool AllowModPacks;

        public Task Work(ProgressDialog.ReportProgressDel r)
        {
            Status = Status.Ok;
            try
            {
                foreach (var file in Files)
                    HandleMap(file, r);
            }
            catch (Exception e)
            {
                r(0, 100, e.ToString(), e.ToString());
                Status = Status.Error;
            }
            return Task.CompletedTask;
        }

        private void HandleMap(string file, ProgressDialog.ReportProgressDel r)
        {
            try
            {
                if (Path.GetExtension(file) == ".zip")
                {
                    using ZipArchive a = ZipFile.OpenRead(file);
                    foreach (ZipArchiveEntry e in a.Entries)
                    {
                        if (e.IsFolder())
                            continue;
                        if (Path.GetExtension(e.Name) == ".s5x")
                        {
                            string extractfile = Path.Combine(TargetPath, e.Name);
                            e.ExtractToFile(extractfile, true);
                            HandleMap(extractfile, r);
                            File.Delete(extractfile);
                        }
                        else if (e.Name.Equals("info.xml", StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (new XmlSerializer(typeof(S5MapInfo)).Deserialize(e.Open()) is not S5MapInfo i)
                                continue;
                            string outPath = GetMapFileExtraPath(i);
                            if (outPath == null)
                                continue;
                            string? dir = Path.GetDirectoryName(e.FullName);
                            if (dir == null)
                                continue;
                            outPath = Path.Combine(TargetPath, outPath, Path.GetFileName(dir));
                            if (Directory.Exists(outPath))
                                Directory.Delete(outPath, true);
                            Directory.CreateDirectory(outPath);
                            foreach (ZipArchiveEntry e2 in a.Entries)
                            {
                                if (e2.IsFolder())
                                    continue;
                                if (Path.GetDirectoryName(e2.FullName) == dir)
                                {
                                    e2.ExtractToFile(Path.Combine(outPath, e2.Name), true);
                                }
                            }
                            r(0, 100, Res.Log_InstallMap + outPath, Res.Log_InstallMap + outPath);
                        }
                    }
                }
                else if (Path.GetExtension(file) == ".s5x")
                {
                    using BbaArchive a = new();
                    a.ReadBba(file);
                    S5MapInfo? i = a.MapInfo;
                    if (i == null)
                        return;
                    string outPath = GetMapFileExtraPath(i);
                    a.Clear();
                    if (outPath == null)
                        return;
                    outPath = Path.Combine(TargetPath, outPath);
                    Directory.CreateDirectory(outPath);
                    outPath = Path.Combine(outPath, Path.GetFileName(file));
                    File.Copy(file, outPath, true);
                    r(0, 100, Res.Log_InstallMap + outPath, Res.Log_InstallMap + outPath);
                }
                else if (AllowModPacks && Path.GetExtension(file) == ".bba")
                {
                    using BbaArchive a = new();
                    a.ReadBba(file);
                    string? modname = Path.GetFileNameWithoutExtension(file);
                    if (modname == null)
                        return;
                    S5ModPackInfo? i = a.GetModPackInfo(modname);
                    if (i == null)
                        return;
                    string outPath = Path.Combine(TargetPath, "ModPacks");
                    Directory.CreateDirectory(outPath);
                    outPath = Path.Combine(outPath, Path.GetFileName(file));
                    File.Copy(file, outPath, true);
                    r(0, 100, Res.Log_InstallModPack + outPath, Res.Log_InstallModPack + outPath);
                }
            }
            catch (Exception e)
            {
                r(0, 100, e.ToString(), e.ToString());
            }
        }

        private static string GetMapFileExtraPath(S5MapInfo doc)
        {
            int key = doc.Key.FirstOrDefault(0);
            string outPath = "base\\shr\\maps\\user";
            if (key == 2)
            {
                outPath = "extra2\\shr\\maps\\user";
            }
            else if (key == 1)
            {
                outPath = "extra1\\shr\\maps\\user";
            }
            return outPath;
        }
    }
}
