using Client.ConfigManager;
using Client.ReaderManager;
using Client.ThreadManager;
using System;
using System.Windows.Forms;
using System.Timers;
using Client.DownloadManager;
using System.Threading.Tasks;
using Client.Classes;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime;
using System.Net;

namespace Client
{
    public partial class Form1 : Form
    {
        public bool Stopped = false;
        internal ThreadManager420 ThreadManager = new ThreadManager420();
        internal Config420 Config;
        internal SteamUser SteamInfo;
        internal ReaderMain Reader = new ReaderMain();
        internal DownloaderMain Downloader;
        internal ProgressBar StepBarGUI;
        internal ProgressBar DownloadBarGUI;
        internal RichTextBox TextBoxGUI;
        internal Button DownloadButtonUI;
        int Version = 23;
        private MemoryCleanUp Mem = new MemoryCleanUp();
        System.Timers.Timer Debug = new System.Timers.Timer(1000);
        public Form1()
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            Config = new Config420(this);
            CheckUpdate();
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;// Si on ferme le serveur on appel la fonction
            ThreadManager.StartServerThread(this);
            Reader.Init(this);
            Downloader =new DownloaderMain(this);
            Debug.Elapsed += (o, b) =>
            {
                Mem.EmptyWorkingSetFunction();
                Mem.ClearFileSystemCache(false);
                GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                GC.Collect();

            };
            Debug.Start();
            InitializeComponent();
            List<Object> ListObject = new List<object>(); 
            foreach(GameMetaInfo info in Downloader.AllGameMeta.GameList.Values)
            {
                ListObject.Add(info.Label);
            }
            StepBarGUI = StepBar;
            DownloadBarGUI = DownloadBar;
            TextBoxGUI = TextBox;
            DownloadButtonUI = DownloadButton;
            ListGame.Items.AddRange(ListObject.ToArray());

        }
       

        internal void CheckUpdate()
        {
            using(WebClient web = new WebClient())
            {

                int CloudVersion = int.Parse(web.DownloadString(new Uri(Config.Config().Api + "version.txt")));
                if(CloudVersion > Version)
                {
                    Process update = new Process();
                    update.StartInfo.FileName = "updater.exe";
                    update.StartInfo.Arguments = Process.GetCurrentProcess().ProcessName + ".exe";
                    update.Start();
                    Application.Exit();
                }
            }
        }

        internal long ToMo(long size)
        {
            return size / (1000 * 1000);
        }
        internal void SelectGame(GameMetaInfo gameInfo)
        {
            GameLabel.Text = gameInfo.Label;
            GameLabel.Visible = true;
            TextBox.Visible = true;
            DownloadButtonUI.Visible = true;
            ThreadManager.AddAsyncTask(async () =>
            {
                await Task.Delay(500);

                ResetTextBox();
                AddTextBox("Vérification des fichier");
                ChangeDownloadButtonText("Vérification de jeux");
                if (Downloader.CheckGameUpdate(gameInfo.Game))
                {
                    ChangeDownloadButtonText("Mise à jour");
                }
                else
                {
                    ChangeDownloadButtonText("Jouer");
                }
            });
        }

        internal void ChangeDownloadButtonText(string msg)
        {
            Action act = () =>
            {
                DownloadButtonUI.Text = msg;
            };
            Invoke(act);
        }

        internal void ResetTextBox()
        {
            Action act = () =>
            {
                TextBoxGUI.Text = "";
            };
            this.Invoke(act);
        }
        internal void AddTextBox(string text)
        {
            Action act = () =>
            {
                TextBoxGUI.AppendText(TextBoxGUI.Text == "" ? text : Environment.NewLine + text);

                TextBoxGUI.ScrollToCaret();
            };
            this.Invoke(act);
        }

        internal void LogWarning(string e)
        {

            MessageBox.Show(e, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        internal void LogError(Exception e)
        {
            MessageBox.Show(e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        internal void LogError(string e)
        {
            MessageBox.Show(e, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GameMetaInfo gameInfo = Downloader.AllGameMeta.GameList.Values.ToList()[ListGame.SelectedIndex];
            if (Downloader.CheckGameUpdate(gameInfo.Game))
            {
                Downloader.Download(gameInfo.Game);
                return;
            }
            if(!SteamInfo.WroteIdentitity(@"./" + gameInfo.Game))
            {
                LogWarning("Impossible de récupérer votre compte steam , merci de relanecr ou lancer steam");
                return;
            }
            ProcessStartInfo startInfo = new ProcessStartInfo(@"./" + gameInfo.Game + gameInfo.ExeName);
            startInfo.Arguments = gameInfo.LaunchCommand;
            startInfo.WorkingDirectory = Environment.CurrentDirectory + "/" + gameInfo.Game;
            Process.Start(startInfo);


        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectGame(Downloader.AllGameMeta.GameList.Values.ToList()[ListGame.SelectedIndex]);
        }

        private void OnProcessExit(object sender, EventArgs e)
        {
            ThreadManager.Stopped = true;
            ThreadManager.WaitForEnd();
            Stopped = true;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {

            SteamInfo = new SteamUser(this);
        }
    }
}
