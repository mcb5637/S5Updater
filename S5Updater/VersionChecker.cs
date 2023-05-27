using S5Updater.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace S5Updater
{
    [Flags]
    public enum VersionInfo
    {
        Unknown = 0,
        Patch1_5 = 1,
        Patch1_6 = 2,
        Patch1_6_Win10 = 4,
        HE = 8,
        Coverted_HE = 16,
    }
    [Flags]
    public enum Extras
    {
        None = 0,
        Base = 1,
        Extra1 = 2,
        Extra2 = 4,
    }

    public class VersionFileHash
    {
        public string Hash;
        public VersionInfo VersionInfo;
    }
    public class VersionFileInfo
    {
        public string RelativePath;
        public List<VersionFileHash> Versions = new List<VersionFileHash>();
        public VersionInfo InvalidVersion = VersionInfo.Unknown, NotFoundVersion = VersionInfo.Unknown;
        public Extras NotFoundExtras;
    }
    public class VersionData
    {
        public List<VersionFileInfo> Files = new List<VersionFileInfo>();
    }

    class VersionChecker : IUpdaterTask
    {
        internal VersionData Data;

        internal void StoreTo(string file)
        {
            XmlSerializer seri = new XmlSerializer(typeof(VersionData));
            using (TextWriter w = new StreamWriter(file))
            {
                seri.Serialize(w, Data);
            }
        }
        internal void LoadFrom(string file)
        {
            XmlSerializer seri = new XmlSerializer(typeof(VersionData));
            using (TextReader w = new StreamReader(file))
            {
                Data = (VersionData)seri.Deserialize(w);
            }
        }
        internal void FixFileInfoCase()
        {
            List<VersionFileInfo> newf = new List<VersionFileInfo>();
            foreach (var f in Data.Files)
            {
                string fn = f.RelativePath.ToLower();
                if (fn.Contains("shr\\maps\\user"))
                    continue;
                var nf = newf.FirstOrDefault(x => x.RelativePath == fn);
                if (nf == null)
                {
                    newf.Add(f);
                    f.RelativePath = fn;
                }
                else
                {
                    foreach (var fha in f.Versions)
                    {
                        var nfh = nf.Versions.FirstOrDefault(x => x.Hash == fha.Hash);
                        if (nfh == null)
                        {
                            nf.Versions.Add(fha);
                        }
                        else
                        {
                            nfh.VersionInfo |= fha.VersionInfo;
                        }
                    }
                    nf.InvalidVersion |= f.InvalidVersion;
                    nf.NotFoundVersion |= f.NotFoundVersion;
                    nf.NotFoundExtras |= f.NotFoundExtras;
                }
            }
            Data.Files = newf;
        }
        internal void FixNotAllVersionExtra()
        {
            foreach (var f in Data.Files)
            {
                if (f.RelativePath.StartsWith("extra"))
                {
                    VersionInfo i = VersionInfo.Unknown;
                    foreach (var fha in f.Versions)
                    {
                        i |= fha.VersionInfo;
                    }
                    i &= VersionInfo.Patch1_5 | VersionInfo.Patch1_6 | VersionInfo.Patch1_6_Win10;
                    if (i != (VersionInfo.Patch1_5 | VersionInfo.Patch1_6 | VersionInfo.Patch1_6_Win10))
                    {
                        f.NotFoundExtras = Extras.Base | Extras.Extra1 | Extras.Extra2;
                    }
                }
            }
        }
        static internal string AnalyzeLog(string path, string suspected, string req)
        {
            string r = "";
            using (TextReader read = new StreamReader(path))
            {
                string l = read.ReadLine();
                while (l != null)
                {
                    if (l.Contains(req) && !l.Contains(suspected))
                        r += l + "\n";
                    l = read.ReadLine();
                }
            }
            return r;
        }
        internal void AddHashesForVersion(string path, VersionInfo thisversion, VersionInfo invalidversion, VersionInfo notfoundversion)
        {
            if (Data == null)
                Data = new VersionData();

            void TravDir(DirectoryInfo dir, string rel)
            {
                if (rel.EndsWith("shr\\maps\\user"))
                    return;
                foreach (FileInfo f in dir.EnumerateFiles())
                {
                    string h = InstallValidator.GetFileHash(f.FullName);
                    VersionFileInfo vi = Data.Files.Find((VersionFileInfo vil) => vil.RelativePath.Equals(Path.Combine(rel, f.Name)));
                    if (vi == null)
                    {
                        Extras ex = Extras.Base | Extras.Extra1 | Extras.Extra2;
                        if (rel.StartsWith("extra1"))
                            ex = Extras.Base | Extras.Extra2;
                        else if (rel.StartsWith("extra2"))
                            ex = Extras.Base | Extras.Extra1;
                        vi = new VersionFileInfo()
                        {
                            RelativePath = Path.Combine(rel, f.Name),
                            NotFoundExtras = ex,
                            InvalidVersion = invalidversion,
                            NotFoundVersion = notfoundversion,
                        };
                        Data.Files.Add(vi);
                    }
                    VersionFileHash fh = vi.Versions.Find((VersionFileHash vil) => vil.Hash.Equals(h));
                    if (fh == null)
                    {
                        fh = new VersionFileHash() { Hash = h };
                        vi.Versions.Add(fh);
                    }
                    fh.VersionInfo |= thisversion;
                }
                foreach (DirectoryInfo d in dir.EnumerateDirectories())
                {
                    TravDir(d, Path.Combine(rel, d.Name));
                }
            }
            TravDir(new DirectoryInfo(path), "");

            foreach (VersionFileInfo fil in Data.Files)
            {
                string p = Path.Combine(path, fil.RelativePath);
                if (!File.Exists(p))
                    fil.NotFoundVersion |= thisversion;
            }
        }
        internal VersionInfo CheckVersion(string path, out Extras e, ProgressDialog.ReportProgressDel rd, TextWriter log)
        {
            VersionInfo r = VersionInfo.Patch1_5 | VersionInfo.Patch1_6 | VersionInfo.Patch1_6_Win10 | VersionInfo.HE | VersionInfo.Coverted_HE;
            e = Extras.Base | Extras.Extra1 | Extras.Extra2;
            int curr = 0;
            log.WriteLine($"version check for {path}");
            foreach (VersionFileInfo vi in Data.Files)
            {
                log.Write($"file {vi.RelativePath} -> ");
                rd(curr * 100 / Data.Files.Count, null);
                rd(-1, vi.RelativePath);
                string p = Path.Combine(path, vi.RelativePath);
                string h = InstallValidator.GetFileHash(p);
                if (h == null)
                {
                    r &= vi.NotFoundVersion;
                    e &= vi.NotFoundExtras;
                    log.WriteLine($"not found {vi.NotFoundVersion}, {vi.NotFoundExtras}");
                }
                else
                {
                    VersionFileHash fh = vi.Versions.Find((VersionFileHash vil) => vil.Hash.Equals(h));
                    if (fh == null)
                    {
                        r &= vi.InvalidVersion;
                        log.WriteLine($"no hash found {vi.InvalidVersion} {h}");
                    }
                    else
                    {
                        r &= fh.VersionInfo;
                        log.WriteLine($"hash for {fh.VersionInfo}");
                    }
                }
                curr++;
            }
            log.WriteLine($"done: {r}, {e}");
            return r;
        }

        internal MainMenu MM;
        internal int Status;
        internal string WorkPath;
        internal const string DataFile = "./VersionInfo.xml";
        internal const string LogFile = "./VersionLog.txt";
        internal string Response;
        public void Work(ProgressDialog.ReportProgressDel r)
        {
            Status = MainMenu.Status_OK;
            try
            {
                using (TextWriter w = new StreamWriter(LogFile, false))
                {
                    r(0, Resources.Txt_VersionCheckStart);
                    LoadFrom(DataFile);
                    r(0, string.Format(Resources.Txt_VersionCheckCount, Data.Files.Count));
                    VersionInfo i = CheckVersion(WorkPath, out Extras e, r, w);
                    Response = string.Format(Resources.Txt_VersioncheckDone, i.ToString(), e.ToString());
                    r(100, Response);
                }
            }
            catch (Exception ex)
            {
                r(0, ex.ToString());
                Status = MainMenu.Status_Error;
            }
        }
    }
}
