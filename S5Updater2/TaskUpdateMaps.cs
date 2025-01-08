using bbaLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S5Updater2
{
    internal class MapUpdate
    {
        internal required string Name, File;

        public override string ToString()
        {
            return Name;
        }
    }
    internal class TaskUpdateMaps : IUpdaterTask
    {
        internal required MainWindow MM;
        internal Status Status;
        internal required IEnumerable<MapUpdate> Maps;
        internal required TaskUpdateMapsType Type;

        public async Task Work(ProgressDialog.ReportProgressDel r)
        {
            Status = Status.Ok;
            try
            {
                if (MM.Reg.GoldPath == null)
                    throw new NullReferenceException();
                foreach (MapUpdate n in Maps)
                {
                    r(0, 100, n.Name, n.Name);
                    TaskUpdateMapsType.Info? i = Type.GetInfo(n.File);
                    if (i == null)
                        continue;
                    if (i.VersionURL == null || i.UpdateURL == null)
                        continue;
                    if (i.Version == await MainUpdater.DownloadFileString(i.VersionURL, r))
                    {
                        r(0, 100, "not needed", "not needed");
                        continue;
                    }
                    string tmp = Path.Combine(MM.Reg.GoldPath, n.Name + Path.GetExtension(i.UpdateURL));
                    await MainUpdater.DownloadFile(i.UpdateURL, tmp, r);
                    TaskInstallMap inst = new()
                    {
                        MM = MM,
                        Files = [tmp],
                        TargetPath = MM.Reg.GoldPath,
                        AllowModPacks = true,
                        OnlyName = n.Name,
                    };
                    await inst.Work(r);
                    File.Delete(tmp);
                    if (inst.Status != Status.Ok)
                        Status = inst.Status;
                }
            }
            catch (Exception e)
            {
                r(0, 100, e.ToString(), e.ToString());
                Status = Status.Error;
            }
        }
    }
    internal class TaskScanMaps : IUpdaterTask
    {
        internal required MainWindow MM;
        internal Status Status;
        internal List<MapUpdate> Maps = [];
        internal required TaskUpdateMapsType Type;

        public Task Work(ProgressDialog.ReportProgressDel r)
        {
            Status = Status.Ok;
            try
            {
                if (MM.Reg.GoldPath == null)
                    throw new NullReferenceException();
                Maps.Clear();
                foreach (string sp in Type.SearchPath)
                {
                    string outPath = Path.Combine(MM.Reg.GoldPath, sp);
                    if (!Directory.Exists(outPath))
                        continue;
                    DirectoryInfo dirinfo = new(outPath);
                    foreach (FileInfo mp in dirinfo.EnumerateFiles())
                    {
                        r(0, 100, mp.Name, null);
                        handle(mp);
                    }
                    if (Type.SearchDirs)
                    {
                        foreach (DirectoryInfo di in dirinfo.EnumerateDirectories())
                        {
                            r(0, 100, di.Name, null);
                            handle(di);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                r(0, 100, e.ToString(), e.ToString());
                Status = Status.Error;
            }
            return Task.CompletedTask;

            void handle(FileSystemInfo fi)
            {
                TaskUpdateMapsType.Info? i = Type.GetInfo(fi.FullName);
                if (i == null)
                    return;
                if (i.VersionURL == null || i.UpdateURL == null)
                    return;
                string n = Path.GetFileNameWithoutExtension(fi.FullName);
                Maps.Add(new MapUpdate()
                {
                    File = fi.FullName,
                    Name = n,
                });
            }
        }
    }
    internal abstract class TaskUpdateMapsType
    {
        internal class Info
        {
            internal required string? VersionURL;
            internal required string? UpdateURL;
            internal required string Version;
        }
        internal abstract Info? GetInfo(string path);
        internal abstract string[] SearchPath { get; }
        internal abstract bool SearchDirs { get; }
    }
    internal class TaskUpdateMapsTypeModPack : TaskUpdateMapsType
    {
        internal override string[] SearchPath => ["ModPacks"];
        internal override bool SearchDirs => false;

        internal override Info? GetInfo(string path)
        {
            if (Path.GetExtension(path) != ".bba")
                return null;
            using BbaArchive a = new();
            a.ReadBba(path);
            string n = Path.GetFileNameWithoutExtension(path);
            S5ModPackInfo? i = a.GetModPackInfo(n);
            if (i == null)
                return null;
            return new Info()
            {
                VersionURL = i.VersionURL,
                UpdateURL = i.UpdateURL,
                Version = i.Version,
            };
        }
    }
    internal class TaskUpdateMapsTypeMap : TaskUpdateMapsType
    {
        internal override string[] SearchPath => ["base\\shr\\maps\\user", "extra1\\shr\\maps\\user", "extra2\\shr\\maps\\user"];
        internal override bool SearchDirs => true;

        internal override Info? GetInfo(string path)
        {
            using BbaArchive a = new();
            try
            {
                if (Directory.Exists(path))
                {
                    a.AddFileFromFilesystem(Path.Combine(path, "info.xml"), BbaArchive.InfoXML);
                }
                else
                {
                    if (Path.GetExtension(path) != ".s5x")
                        return null;
                    a.ReadBba(path);
                }
                S5MapInfo? i = a.MapInfo;
                if (i == null)
                    return null;
                return new Info()
                {
                    VersionURL = i.VersionURL,
                    UpdateURL = i.UpdateURL,
                    Version = i.GUID.Data,
                };
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }
    }
}
