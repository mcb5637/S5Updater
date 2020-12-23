using LibGit2Sharp;
using S5Updater.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace S5Updater
{
    class TaskUpdateMPMaps : IUpdaterTask
    {
        internal MainMenu MM;
        internal int Status;

        internal readonly string[] Exclude = new string[] { ".git", ".gitignore", ".gitmodules", ".gitattributes" };

        public void Work(ProgressDialog.ReportProgressDel r)
        {
            Status = MainMenu.Status_OK;
            r(0, Resources.TaskMPMap_Start);
            HandleRepo(Path.Combine(MM.Reg.GoldPath, "extra2\\shr\\maps\\user\\EMSGit"), "master", "https://github.com/MadShadow-/EMS.git", r);
            HandleRepo(Path.Combine(MM.Reg.GoldPath, "extra2\\shr\\maps\\user\\EMS\\tools\\s5CommunityLib"), "master", "https://github.com/mcb5637/s5CommunityLib.git", r);
            HandleRepo(Path.Combine(MM.Reg.GoldPath, "extra2\\shr\\maps\\user\\speedwar"), "master", "https://github.com/MadShadow-/speedwar.git", r);
            HandleRepo(Path.Combine(MM.Reg.GoldPath, "extra2\\shr\\maps\\user\\Balancing_Stuff_in_Dev"), "master", "https://github.com/GhoulMadness/Balancing-Stuff.git", r);
            HandleRepo(Path.Combine(MM.Reg.GoldPath, "extra2\\shr\\maps\\user\\Balancing_Stuff_Maps"), "main", "https://github.com/GhoulMadness/Balancing_Stuff_Maps.git", r);
            try
            {
                r(0, Resources.TaskMPMap_CopyFiles);
                MainUpdater.Copy(Path.Combine(MM.Reg.GoldPath, "extra2\\shr\\maps\\user\\EMSGit"), Path.Combine(MM.Reg.GoldPath, "extra2\\shr\\maps\\user"), Exclude);
                MainUpdater.Copy(Path.Combine(MM.Reg.GoldPath, "extra2\\shr\\maps\\user\\Balancing_Stuff_Maps"), Path.Combine(MM.Reg.GoldPath, "extra2\\shr\\maps\\user"), Exclude);
                r(100, Resources.Done);
            }
            catch (Exception e)
            {
                r(0, e.ToString());
                Status = MainMenu.Status_Error;
            }
        }

        private void HandleRepo(string repo, string branch, string url, ProgressDialog.ReportProgressDel r)
        {
            try
            {
                string reponame = Path.GetFileName(repo);
                r(0, Resources.TaskMPMap_Repo + reponame);
                if (!Repository.IsValid(repo))
                {
                    r(0, Resources.TaskMPMap_Cloning);
                    CloneRepo(repo, branch, url, r);
                    r(100, Resources.Done);
                }
                else
                {
                    r(0, Resources.TaskMPMap_Fetch);
                    using (Repository rep = new Repository(repo))
                    {
                        FetchRepo(r, rep);
                        StatusOptions statusopt = new StatusOptions
                        {
                            ExcludeSubmodules = false,
                            IncludeUntracked = true
                        };
                        RepositoryStatus stat = rep.RetrieveStatus(statusopt);
                        if (stat.IsDirty)
                        {
                            if (MM.EasyMode || MessageBox.Show(string.Format(Resources.TaskMPMap_ErrDirty, reponame), reponame, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                r(0, Resources.TaskMPMap_DirtyDeleting);
                                rep.Reset(ResetMode.Hard, rep.Head.Tip);
                            }
                            else
                            {
                                r(0, Resources.TaskMPMap_DirtyKeep);
                                Status = MainMenu.Status_Error;
                                return;
                            }
                        }
                        r(0, Resources.TaskMPMap_Checkout);
                        CheckoutRepo(branch, r, rep);
                        r(100, Resources.Done);
                    }
                }
            }
            catch (Exception e)
            {
                r(0, e.ToString());
                Status = MainMenu.Status_Error;
            }
        }

        private void CheckoutRepo(string branch, ProgressDialog.ReportProgressDel r, Repository rep)
        {
            CheckoutOptions checkoutopt = new CheckoutOptions
            {
                OnCheckoutProgress = (path, com, tot) =>
                {
                    r(tot == 0 ? 100 : com * 100 / tot, null);
                },
                CheckoutModifiers = CheckoutModifiers.Force
            };
            if (!rep.Head.CanonicalName.Equals(branch))
                Commands.Checkout(rep, rep.Branches[branch], checkoutopt);
            rep.Reset(ResetMode.Hard, rep.Head.TrackedBranch.Tip, checkoutopt);
            r(100, Resources.TaskMPMap_LatestComm + rep.Head.Tip.Message);
        }

        private void FetchRepo(ProgressDialog.ReportProgressDel r, Repository rep)
        {
            FetchOptions fetchopt = new FetchOptions
            {
                OnTransferProgress = (t) =>
                {
                    r(t.TotalObjects == 0 ? 100 : t.ReceivedObjects * 100 / t.TotalObjects, null);
                    return true;
                },
                OnProgress = (s) =>
                {
                    r(-1, s);
                    return true;
                }
            };
            Commands.Fetch(rep, "origin", rep.Network.Remotes["origin"].FetchRefSpecs.Select((x) => x.Specification), fetchopt, null);
        }

        private void CloneRepo(string repo, string branch, string url, ProgressDialog.ReportProgressDel r)
        {
            if (Directory.Exists(repo))
                Directory.Delete(repo, true);
            CloneOptions cloneopt = new CloneOptions
            {
                RecurseSubmodules = true,
                BranchName = branch,
                OnTransferProgress = (t) =>
                {
                    r(t.TotalObjects == 0 ? 100 : t.ReceivedObjects * 100 / t.TotalObjects, null);
                    return true;
                },
                OnCheckoutProgress = (path, com, tot) =>
                {
                    r(tot == 0 ? 100 : com * 100 / tot, null);
                },
                OnProgress = (s) =>
                {
                    r(-1, s);
                    return true;
                }
            };
            Repository.Clone(url, repo, cloneopt);
        }
    }
}
