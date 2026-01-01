using LibGit2Sharp;
using MsBox.Avalonia;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace S5Updater2
{
    internal class MapPack
    {
        public required string DisplayName;
        public required string RegKey;
        public required MapPackSource[] Sources;
        public required IMapPackCopy[] Copies;

        public override string ToString()
        {
            return DisplayName;
        }
    }
    internal class MapPackSource
    {
        public required string URL;
        public required string Branch;
        public required string TargetFolder;
    }
    internal interface IMapPackCopy
    {
        void CopyTo(MainWindow mm, ProgressDialog.ReportProgressDel r);
    }
    internal class MapPackCopyFolder : IMapPackCopy
    {
        public required string From, To;
        public string[] ExcludeAppend = [];
        public void CopyTo(MainWindow mm, ProgressDialog.ReportProgressDel r)
        {
            if (mm.Reg.GoldPath == null)
                throw new NullReferenceException();
            MainUpdater.Copy(Path.Combine(mm.Reg.GoldPath, From), Path.Combine(mm.Reg.GoldPath, To), TaskUpdateMPMaps.Exclude.Concat(ExcludeAppend), r);
        }
    }
    internal class MapPackCopyBba : IMapPackCopy
    {
        public required string From, To;
        public readonly string[] ExcludeAppend = [];
        public void CopyTo(MainWindow mm, ProgressDialog.ReportProgressDel r)
        {
            if (mm.Reg.GoldPath == null)
                throw new NullReferenceException();
            MainUpdater.PackDirToBba(Path.Combine(mm.Reg.GoldPath, From), Path.Combine(mm.Reg.GoldPath, To), TaskUpdateMPMaps.Exclude.Concat(ExcludeAppend), r);
        }
    }
    internal class MapPackCopyMultiBba : IMapPackCopy
    {
        public required string From, To;
        public readonly string[] ExcludeAppend = [];
        public void CopyTo(MainWindow mm, ProgressDialog.ReportProgressDel r)
        {
            if (mm.Reg.GoldPath == null)
                throw new NullReferenceException();
            foreach (DirectoryInfo f in new DirectoryInfo(Path.Combine(mm.Reg.GoldPath, From)).EnumerateDirectories())
            {
                MainUpdater.PackDirToBba(f.FullName, Path.Combine(mm.Reg.GoldPath, To, f.Name + ".bba"), TaskUpdateMPMaps.Exclude.Concat(ExcludeAppend), r);
            }
        }
    }

    internal class TaskUpdateMPMaps : IUpdaterTask
    {
        internal required MainWindow MM;
        internal Status Status;

        internal static readonly string[] Exclude = [".git", ".gitignore", ".gitmodules", ".gitattributes"];
        internal static readonly MapPack[] Packs = [
            new MapPack() {
                DisplayName = "EMS",
                RegKey = "EMS",
                Sources = [
                    new MapPackSource() {
                        URL = "https://github.com/MadShadow-/EMS.git",
                        Branch = "master",
                        TargetFolder = "extra2/shr/maps/user/EMSGit",
                    },
                    new MapPackSource() {
                        URL = "https://github.com/mcb5637/s5CommunityLib.git",
                        Branch = "master",
                        TargetFolder = "extra2/shr/maps/user/EMS/tools/s5CommunityLib",
                    },
                ],
                Copies = [
                    new MapPackCopyFolder() {
                        From = "extra2/shr/maps/user/EMSGit",
                        To = "extra2/shr/maps/user",
                    },
                ],
            },
            new MapPack() {
                DisplayName = "Speedwar",
                RegKey = "Speedwar",
                Sources = [
                    new MapPackSource() {
                        URL = "https://github.com/MadShadow-/speedwar.git",
                        Branch = "master",
                        TargetFolder = "extra2/shr/maps/user/speedwar",
                    },
                ],
                Copies = [],
            },
            new MapPack() {
                DisplayName = "BS",
                RegKey = "BS",
                Sources = [
                    new MapPackSource() {
                        URL = "https://github.com/GhoulMadness/Balancing-Stuff.git",
                        Branch = "master",
                        TargetFolder = "extra2/shr/maps/user/Balancing_Stuff_in_Dev",
                    },
                    new MapPackSource() {
                        URL = "https://github.com/GhoulMadness/Balancing_Stuff_Maps.git",
                        Branch = "main",
                        TargetFolder = "extra2/shr/maps/user/Balancing_Stuff_Maps",
                    },
                ],
                Copies = [
                    new MapPackCopyFolder() {
                        From = "extra2/shr/maps/user/Balancing_Stuff_Maps",
                        To = "extra2/shr/maps/user",
                    },
                ],
            },
            new MapPack() {
                DisplayName = "Stronghold",
                RegKey = "Stronghold",
                Sources = [
                    new MapPackSource() {
                        URL = "https://github.com/totalwarANGEL1993/stronghold_s5mp_release.git",
                        Branch = "master",
                        TargetFolder = "extra2/shr/maps/user/StrongholdMP",
                    },
                ],
                Copies = [
                    new MapPackCopyFolder() {
                        From = "extra2/shr/maps/user/StrongholdMP",
                        To = "extra2/shr/maps/user",
                    },
                ],
            },
            new MapPack() {
                DisplayName = "RandomChaos",
                RegKey = "RandomChaos",
                Sources = [
                    new MapPackSource() {
                        URL = "https://github.com/RobbiTheFox/S5RandomChaos.git",
                        Branch = "main",
                        TargetFolder = "extra2/shr/maps/user/RandomChaos",
                    },
                ],
                Copies = [
                    new MapPackCopyFolder() {
                        From = "extra2/shr/maps/user/RandomChaos/ModsMP",
                        To = "MP_SettlerServer/Mods",
                        ExcludeAppend = ["randomchaos"],
                    },
                    new MapPackCopyBba() {
                        From = "extra2/shr/maps/user/RandomChaos/ModsMP/randomchaos/randomchaos",
                        To = "MP_SettlerServer/Mods/randomchaos/randomchaos.bba",
                    },
                    new MapPackCopyFolder() {
                        From = "extra2/shr/maps/user/RandomChaos/ModsSP",
                        To = "CSinglePlayer/Mods",
                        ExcludeAppend = ["randomchaos"],
                    },
                    new MapPackCopyFolder() {
                        From = "extra2/shr/maps/user/RandomChaos/ModsSP/randomchaos",
                        To = "CSinglePlayer/Mods/randomchaos",
                        ExcludeAppend = ["randomchaos"],
                    },
                    new MapPackCopyBba() {
                        From = "extra2/shr/maps/user/RandomChaos/ModsSP/randomchaos/randomchaos",
                        To = "CSinglePlayer/Mods/randomchaos/randomchaos.bba",
                    },
                ],
            },
            new MapPack() {
                DisplayName = "MPW",
                RegKey = "MPW",
                Sources = [
                    new MapPackSource() {
                        URL = "https://github.com/Novator12/Multiplayer-Workover-MPW.git",
                        Branch = "master",
                        TargetFolder = "extra2/shr/maps/user/MPW",
                    },
                ],
                Copies = [
                    new MapPackCopyFolder() {
                        From = "extra2/shr/maps/user/MPW/Settlers5/MP_SettlerServer/Mods",
                        To = "MP_SettlerServer/Mods",
                    },
                    new MapPackCopyFolder() {
                        From = "extra2/shr/maps/user/MPW/Settlers5/CSinglePlayer/Mods",
                        To = "CSinglePlayer/Mods",
                    },
                    new MapPackCopyMultiBba() {
                        From = "extra2/shr/maps/user/MPW/bba",
                        To = "MP_SettlerServer/Mods/MPW/Ingame",
                    },
                ],
            },
        ];

        public async Task Work(ProgressDialog.ReportProgressDel r)
        {
            Status = Status.Ok;
            r(0, 100, Res.TaskMPMap_Start, Res.TaskMPMap_Start);

            try
            {
                if (MM.Reg.GoldPath == null)
                    throw new NullReferenceException();
                await MM.EnsureWriteAccess(MM.Reg.GoldPath);
                foreach (MapPack p in Packs.Where(MM.GetMapPackUpdateFromSettings))
                    HandleMapPack(p, r);
                
                r(100, 100, Res.Done, Res.Done);
            }
            catch (Exception e)
            {
                r(0, 100, e.ToString(), e.ToString());
                Status = Status.Error;
            }
        }

        private void HandleMapPack(MapPack p, ProgressDialog.ReportProgressDel r)
        {
            try
            {
                if (MM.Reg.GoldPath == null)
                    throw new NullReferenceException();
                foreach (MapPackSource s in p.Sources)
                {
                    HandleRepo(Path.Combine(MM.Reg.GoldPath, s.TargetFolder), s.Branch, s.URL, r);
                }
                r(0, 100, Res.TaskMPMap_CopyFiles, Res.TaskMPMap_CopyFiles);
                foreach (IMapPackCopy c in p.Copies)
                {
                    c.CopyTo(MM, r);
                }
            }
            catch (Exception e)
            {
                r(0, 100, e.ToString(), e.ToString());
                Status = Status.Error;
            }
        }

        private async void HandleRepo(string repo, string branch, string url, ProgressDialog.ReportProgressDel r)
        {
            try
            {
                string reponame = Path.GetFileName(repo);
                r(0, 100, Res.TaskMPMap_Repo + reponame, Res.TaskMPMap_Repo + reponame);
                if (!Repository.IsValid(repo))
                {
                    r(0, 100, Res.TaskMPMap_Cloning, Res.TaskMPMap_Cloning);
                    CloneRepo(repo, branch, url, r);
                    r(100, 100, Res.Done, Res.Done);
                }
                else
                {
                    r(0, 100, Res.TaskMPMap_Fetch, Res.TaskMPMap_Fetch);
                    using Repository rep = new(repo);
                    FetchRepo(r, rep, branch);
                    StatusOptions statusopt = new()
                    {
                        ExcludeSubmodules = false,
                        IncludeUntracked = true
                    };
                    RepositoryStatus stat = rep.RetrieveStatus(statusopt);
                    if (stat.IsDirty)
                    {
                        if (MM.EasyMode || await Messagebox(string.Format(Res.TaskMPMap_ErrDirty, reponame)))
                        {
                            r(0, 100, Res.TaskMPMap_DirtyDeleting, Res.TaskMPMap_DirtyDeleting);
                            foreach (StatusEntry f in stat.Untracked)
                            {
                                File.Delete(Path.Combine(repo, f.FilePath));
                            }
                            rep.Reset(ResetMode.Hard, rep.Head.Tip);
                        }
                        else
                        {
                            r(0, 100, Res.TaskMPMap_DirtyKeep, Res.TaskMPMap_DirtyKeep);
                            Status = Status.Error;
                            return;
                        }
                    }
                    r(0, 100, Res.TaskMPMap_Checkout, Res.TaskMPMap_Checkout);
                    CheckoutRepo(branch, r, rep);
                    r(100, 100, Res.Done, Res.Done);
                }
            }
            catch (Exception e)
            {
                r(0, 100, e.ToString(), e.ToString());
                Status = Status.Error;
            }

            async Task<bool> Messagebox(string q)
            {
                var box = MessageBoxManager.GetMessageBoxStandard("", q, MsBox.Avalonia.Enums.ButtonEnum.YesNo);
                return await box.ShowAsync() == MsBox.Avalonia.Enums.ButtonResult.Yes;
            }
        }

        private void CheckoutRepo(string branch, ProgressDialog.ReportProgressDel r, Repository rep)
        {
            CheckoutOptions checkoutopt = new()
            {
                OnCheckoutProgress = (path, com, tot) =>
                {
                    r(com, tot, path, null);
                },
                CheckoutModifiers = CheckoutModifiers.Force
            };
            if (!rep.Head.CanonicalName.Equals(branch))
                Commands.Checkout(rep, rep.Branches[branch], checkoutopt);
            rep.Reset(ResetMode.Hard, rep.Head.TrackedBranch.Tip, checkoutopt);
            r(100, 100, Res.TaskMPMap_LatestComm + rep.Head.Tip.Message, Res.TaskMPMap_LatestComm + rep.Head.Tip.Message);
        }

        private void FetchRepo(ProgressDialog.ReportProgressDel r, Repository rep, string branch)
        {
            FetchOptions fetchopt = new()
            {
                OnTransferProgress = (t) =>
                {
                    r(t.ReceivedObjects, t.TotalObjects, null, null);
                    return true;
                },
                OnProgress = (s) =>
                {
                    r(-1, 0, s, null);
                    return true;
                },
                Depth = 1,
            };
            Commands.Fetch(rep, "origin", [$"+refs/heads/{branch}:refs/remotes/origin/{branch}"], fetchopt, null);
        }

        private void CloneRepo(string repo, string branch, string url, ProgressDialog.ReportProgressDel r)
        {
            if (Directory.Exists(repo))
                Directory.Delete(repo, true);
            CloneOptions cloneopt = new()
            {
                RecurseSubmodules = true,
                BranchName = branch,
                OnCheckoutProgress = (path, com, tot) =>
                {
                    r(com, tot, path, null);
                },
                FetchOptions =
                {
                    OnProgress = (s) =>
                    {
                        r(-1, 0, s, null);
                        return true;
                    },
                    OnTransferProgress = (t) =>
                    {
                        r(t.ReceivedObjects, t.TotalObjects, null, null);
                        return true;
                    },
                    Depth = 1
                }
            };
            Repository.Clone(url, repo, cloneopt);
        }
    }
}
