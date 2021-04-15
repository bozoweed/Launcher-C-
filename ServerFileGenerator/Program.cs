using ServerFileGenerator.Classes;
using ServerFileGenerator.ConfigManager;
using ServerFileGenerator.Logger;
using ServerFileGenerator.ReaderManager;
using ServerFileGenerator.ThreadManager;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Timers;

namespace ServerFileGenerator
{
    class Program
    {
        public bool Stopped = false;
        internal LoggerManager LogManage;
        internal ThreadManager420 ThreadManager = new ThreadManager420();
        internal Config420 Config;
        internal ReaderMain Reader = new ReaderMain();
        Timer Debug = new Timer(1000);

        static void Main(string[] args)
        {
         
            new Program().start();
        }

        public void Log(string msg)
        {
            LogManage.AddCustomLog("System", msg, ConsoleColor.White);
        }

        public void LogError(Exception msg)
        {
            LogManage.AddCustomLog("System", msg.ToString(), ConsoleColor.Red);
        }

        public void LogWarning(string msg)
        {
            LogManage.AddCustomLog("System", msg, ConsoleColor.Yellow);
        }
        public MemoryCleanUp Mem = new MemoryCleanUp();
        public void start()
        {
            

            Config = new Config420(this);
            Config.SaveConfig();
            LogManage = new LoggerManager(this);
            ThreadManager.StartServerThread(this);
            Reader.Init(this);
            Debug.Elapsed += cleanup;
            Debug.Start();
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;// Si on ferme le serveur on appel la fonction
            Console.CancelKeyPress += OnProcessExit;
            while (!Stopped)
            {
                Log("Enter The Folder Name To Scan or Exit:");
                string read = Console.ReadLine();
                switch (read) {
                    case "Exit":
                        Log("Process kill");
                        ThreadManager.Stopped = true;
                        ThreadManager.WaitForEnd();
                        LogManage.Stop = true;
                        Stopped = true;
                        break;
                    default:
                        Log("Start Reading Folder");
                            Reader.AllFileToCheckCount(@".\" + read);     
                            Mem.EmptyWorkingSetFunction();
                            Mem.ClearFileSystemCache(false);
                            Reader.SaveConfig(read);                        
                        break;
                }
            }

        }

        private async void cleanup(object sender, ElapsedEventArgs e)
        {
            Mem.EmptyWorkingSetFunction();
            Mem.ClearFileSystemCache(false);
            await Task.Yield();
        }

        private void OnProcessExit(object sender, EventArgs e)
        {
            ThreadManager.Stopped = true;
            ThreadManager.WaitForEnd();
            LogManage.Stop = true;
            Stopped = true;
        }

    }
}
