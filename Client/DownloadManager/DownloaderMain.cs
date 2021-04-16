using Client.ReaderManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading;
using Client.Classes;
using FileInfo = Client.Classes.FileInfo;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Client.DownloadManager
{
    class DownloaderMain
    {
        Form1 Base;
        bool busy = false;
        public GameMeta AllGameMeta;
        public DownloaderMain(Form1 bas)
        {
            Base = bas;
            AllGameMeta = GetGamesMeta();
        }

        private GameInfo GetGamesInfo(string game)
        {
            using (WebClient wc = new WebClient())
            {
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                return JsonSerializer.Deserialize<GameInfo>(wc.DownloadString(Base.Config.Config().Api + game + ".json"), options); ;
            }
        }

        private GameMeta GetGamesMeta()
        {
            using (WebClient wc = new WebClient())
            {
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                return JsonSerializer.Deserialize<GameMeta>(wc.DownloadString(Base.Config.Config().Api + "api.json"), options); ;
            }
        }

        public bool CheckGameUpdate(string game)
        {
            if (busy)
                return true;
            busy = true;
            if (!Directory.Exists(@".\" + game))
            {
                busy = false;
                return true;
            }
            GameInfo gameInfo = GetGamesInfo(game);
            if (gameInfo.Version != Base.Config.GetGameVersion(game))
            {
                busy = false;
                return true;
            }
            if(CountMissingFile(game, gameInfo) >0)
            {
                busy = false;
                return true;
            }
            /*if (checkfolder)
            {
                int count = CheckLocalGameInfo(game, gameInfo).Count;
                busy = false;
                return count > 0;
            }*/
            busy = false;
            return false;
        }

        private int CountMissingFile(string game, GameInfo gameInfo)
        {
            int count = 0;           
            List<FolderInfo> RemoteGameInfo = gameInfo.RemoteGameInfo;
            
                foreach (FolderInfo folder in RemoteGameInfo)
                {
                    if (!Directory.Exists(folder.FolderName))
                        Directory.CreateDirectory(folder.FolderName);
                    foreach (FileInfo file in folder.ListedFiles)
                        if (!File.Exists(@".\" +folder.FolderName + "/" + file.Name))
                            count++;
                }
            
            return count;
        }

        private List<DownloadFileInfo> CheckLocalGameInfo(string game, GameInfo gameInfo)
        {
            List<DownloadFileInfo> List = new List<DownloadFileInfo>();

            List<FolderInfo> RemoteGameInfo = gameInfo.RemoteGameInfo;
            if (!Directory.Exists(@".\" + game))
            {
                foreach (FolderInfo folder in RemoteGameInfo)
                {
                    if (!Directory.Exists(folder.FolderName))
                        Directory.CreateDirectory(folder.FolderName);
                    foreach (FileInfo file in folder.ListedFiles)
                        List.Add(new DownloadFileInfo(folder.FolderName, file.Name, file.Size));
                }
                return List;
            }

            List<FolderInfo> LocalGameInfo = Base.Reader.AllFileToCheckCount(@".\" + game);
            foreach (FolderInfo folder in RemoteGameInfo)
            {
                if (!Directory.Exists(folder.FolderName))
                    Directory.CreateDirectory(folder.FolderName);
                foreach (FileInfo file in folder.ListedFiles)
                    if (UpdateFile(LocalGameInfo, folder, file))
                        List.Add(new DownloadFileInfo(folder.FolderName, file.Name, file.Size));

            }
            foreach (FolderInfo folder in LocalGameInfo)
            {
                FolderInfo remote = GetFolderInfo(RemoteGameInfo, folder);
                if (remote != null)
                {
                    foreach (FileInfo file in folder.ListedFiles)
                    {
                        if (GetFileInfo(remote, file) == null)
                        {
                            if (File.Exists(folder.FolderName + "/" + file.Name))
                                File.Delete(folder.FolderName + "/" + file.Name);
                        }
                    }
                }
                else
                {
                    if (Directory.Exists(folder.FolderName))
                        Directory.Delete(folder.FolderName, true);
                }
            }
            RemoteGameInfo.Clear();
            LocalGameInfo.Clear();
            return List;
        }

        private static void GrantAccess(string file)
        {
            if (Directory.Exists(file))
            {
               Directory.CreateDirectory(file);
            }
            DirectoryInfo dInfo = new DirectoryInfo(file);
            DirectorySecurity dSecurity = dInfo.GetAccessControl();
            dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
            dInfo.SetAccessControl(dSecurity);

        }

        private void DownloadFile(string dire, string file, bool displayProgressBar)
        {
            using (WebClient webClient = new WebClient())
            {
                if (displayProgressBar)
                {

                    DisplayDownloadProgress(0).Wait();
                    //TaskCompletionSource<bool> s_tcs = new TaskCompletionSource<bool>();
                    webClient.DownloadProgressChanged += (sender, e) =>
                    {
                       
                            UpdateDownloadProgress((int)e.BytesReceived, (int)e.TotalBytesToReceive);

                            /*if (e.BytesReceived == e.TotalBytesToReceive)
                                s_tcs.SetResult(true);*/
                    };

                }
                if (File.Exists(dire + "/" + file))
                    File.Delete(dire + "/" + file);
                GrantAccess(dire);
                webClient.DownloadFileTaskAsync(new Uri(Base.Config.Config().Api + dire + "/" + file + ".zip"), dire + "/" + file + ".zip").Wait();

                /*if (size == 0)
                    s_tcs.SetResult(true);
                s_tcs.Task.Wait();*/
                ZipFile.ExtractToDirectory(dire + "/" + file + ".zip", dire + "/");
                File.Delete(dire + "/" + file + ".zip");

            }


        }

        private FolderInfo GetFolderInfo(List<FolderInfo> localFolderList, FolderInfo remoteFolder)
        {
            foreach (FolderInfo localFolder in localFolderList)
            {
                if (localFolder.FolderName == remoteFolder.FolderName)
                {
                    return localFolder;
                }
            }
            return null;
        }

        private FileInfo GetFileInfo(FolderInfo folder, FileInfo file)
        {
            foreach (FileInfo fil in folder.ListedFiles)
            {
                if (fil.Name == file.Name)
                    return fil;
            }
            return null;
        }

        private bool UpdateFile(List<FolderInfo> localFolderList, FolderInfo remoteFolder, FileInfo remoteFile)
        {
            FolderInfo LocalFolder = GetFolderInfo(localFolderList, remoteFolder);
            if (LocalFolder == null)
            {
                return true;
            }
            FileInfo info = GetFileInfo(LocalFolder, remoteFile);
            if (info == null)
            {
                return true;
            }

            return info.Hashe != remoteFile.Hashe;
        }


        private Task<bool> DisplayDownloadProgress(int max)
        {
            TaskCompletionSource<bool> s_tcs = new TaskCompletionSource<bool>();
            Action act = () =>
            {
                Base.DownloadBarGUI.MarqueeAnimationSpeed = 0;
                Base.DownloadBarGUI.Value = 0;
                Base.DownloadBarGUI.Minimum = 0;
                Base.DownloadBarGUI.Maximum = max;
                Base.DownloadBarGUI.Visible = true;
                s_tcs.SetResult(true);
            };
            Base.Invoke(act);
            return s_tcs.Task;
        }

        private void HideDownloadProgress()
        {
            Action act = () =>
            {
                Base.DownloadBarGUI.Visible = false;
            };
            Base.Invoke(act);
        }

        private void UpdateDownloadProgress(int value, int max)
        {
            Action act = () =>
            {
                try {
                    using (Graphics gr = Base.DownloadBarGUI.CreateGraphics())
                    {
                        if(max > Base.DownloadBarGUI.Maximum)
                        Base.DownloadBarGUI.Maximum = max;
                        Base.DownloadBarGUI.Value = value;
                        int percent = (int)(((double)(Base.DownloadBarGUI.Value - Base.DownloadBarGUI.Minimum) /
                        (double)(Base.DownloadBarGUI.Maximum - Base.DownloadBarGUI.Minimum)) * 100);
                        //gr.DrawString((text!="" ? text: Base.StepBarGUI.Value + "/" + Base.StepBarGUI.Maximum + "(" + percent + "%)"),
                        gr.DrawString("",
                        SystemFonts.DefaultFont,
                        Brushes.Black,
                        new PointF(Base.DownloadBarGUI.Width / 2 - (gr.MeasureString(percent.ToString() + "%",
                            SystemFonts.DefaultFont).Width / 2.0F),
                        Base.DownloadBarGUI.Height / 2 - (gr.MeasureString(percent.ToString() + "%",
                            SystemFonts.DefaultFont).Height / 2.0F)));
                    }
                }catch(Exception a)
                {

                }
            };
            Base.Invoke(act);
        }


        private void DisplayStepProgress(int max)
        {
            Action act = () =>
            {
                Base.StepBarGUI.Value = 0;
                Base.StepBarGUI.Minimum = 0;
                Base.StepBarGUI.Maximum = max;
                Base.StepBarGUI.Visible = true;
            };
            Base.Invoke(act);
        }

        private void HideStepProgress()
        {
            Action act = () =>
            {
                Base.StepBarGUI.Visible = false;
            };
            Base.Invoke(act);
        }



        private void UpdateStepProgress(string text = "")
        {
            Action act = () =>
            {
                using (Graphics gr = Base.StepBarGUI.CreateGraphics())
                {
                    Base.StepBarGUI.Value++;
                    int percent = (int)(((double)(Base.StepBarGUI.Value - Base.StepBarGUI.Minimum) /
                    (double)(Base.StepBarGUI.Maximum - Base.StepBarGUI.Minimum)) * 100);
                    //gr.DrawString((text!="" ? text: Base.StepBarGUI.Value + "/" + Base.StepBarGUI.Maximum + "(" + percent + "%)"),
                    gr.DrawString("",
                    SystemFonts.DefaultFont,
                    Brushes.Black,
                    new PointF(Base.StepBarGUI.Width / 2 - (gr.MeasureString(percent.ToString() + "%",
                        SystemFonts.DefaultFont).Width / 2.0F),
                    Base.StepBarGUI.Height / 2 - (gr.MeasureString(percent.ToString() + "%",
                        SystemFonts.DefaultFont).Height / 2.0F)));
                }
            };
            Base.Invoke(act);
        }


        public void Download(string game)
        {
            if (busy)
                return;
            busy = true;
            Base.ThreadManager.AddAsyncTask(() =>
            {

                Base.ResetTextBox();
                Base.AddTextBox("Récupération des métadonnée");
                GameInfo gameInfo = GetGamesInfo(game);
                Base.AddTextBox("Vérification des fichier");
                List<DownloadFileInfo> ListDl = CheckLocalGameInfo(game, gameInfo);
                DisplayStepProgress(ListDl.Count);
                int count = 0;
                int launched = 0;
                int totalCount = ListDl.Count;


                Base.AddTextBox("Téléchargement des petits fichier");
                System.Timers.Timer debug = new System.Timers.Timer(1000);
               
                foreach (DownloadFileInfo dl in ListDl)
                {
                    
                    if (Base.ToMo(dl.Size) < 10)
                    {
                        dl.Downloaded = true;
                        launched++;
                        Base.ThreadManager.AddAsyncTask(() =>
                        {
                            DownloadFile(dl.Directory, dl.FileName, false);
                            //ListDl.Remove(dl);
                            UpdateStepProgress();
                            count++;
                        });
                    }
                }
                while (launched > count)
                {
                    Thread.Sleep(1000);

                    int percent = (int)(count /
                     (double)(totalCount) * 100);
                    Base.AddTextBox("Téléchargement fini a " + percent + "% | Ficher télécharger " + count + "/" + totalCount + " | left queue " + Base.ThreadManager.AllTask.Count());
                }
                foreach (DownloadFileInfo dl in ListDl)
                {
                    if (!dl.Downloaded)
                    {
                        Base.AddTextBox("Téléchargement de " + dl.Directory + "/" + dl.FileName);
                        DownloadFile(dl.Directory, dl.FileName, true);
                        UpdateStepProgress();
                        count++;

                        int percent = (int)(count /
                         (double)(totalCount) * 100);
                        Base.AddTextBox("Téléchargement fini a " + percent + "% | Ficher télécharger " + count + "/" + totalCount);
                    }
                }
                ListDl.Clear();
                busy = false;
                HideDownloadProgress();
                HideStepProgress();
                Base.AddTextBox("Téléchargement términé");
                Base.Config.SetGameVersion(game, gameInfo.Version);
                Base.ChangeDownloadButtonText("Jouer");

            });
        }
    }

}
