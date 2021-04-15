using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Updater.ConfigManager;

namespace Updater
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().run(args);
        }

        internal void run(string[] args)
        {
            if (args.Length == 1)
            {
                Config420 config = new Config420(this);
                string link = config.Config().Api + "/launcher/";
                string proccessename = args[0];
                /*
                Process.Start("taskkill", "/F /IM " + proccessename);
                while (Process.GetProcessesByName(proccessename.Replace(".exe", "")).Length > 0)
                    Thread.Sleep(1);
                */
                foreach (Process process in Process.GetProcessesByName(proccessename.Replace(".exe", "")))
                {
                    process.Kill();
                    process.WaitForExit();
                }
                WebRequest request = WebRequest.Create(link);
                WebResponse response = request.GetResponse();
                List<Task> Dl = new List<Task>();
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    string result = reader.ReadToEnd();
                    foreach (string str in result.Split("<a href=\""))
                    {
                        if (str.Split("\">")[0].Contains("Client"))
                            Dl.Add(DlFile(link, str.Split("\">")[0]));
                    }
                    Task.WhenAll(Dl).Wait();
                    if (proccessename != "Client.exe")
                    {
                        if (File.Exists(Environment.CurrentDirectory + "\\" + proccessename))
                        {
                            File.Delete(Environment.CurrentDirectory + "\\" + proccessename);
                        }
                        if (File.Exists(Environment.CurrentDirectory + "\\Client.exe"))
                        {
                            File.Move(Environment.CurrentDirectory + "\\Client.exe", Environment.CurrentDirectory + "\\" + proccessename);
                        }
                    }
                    Process.Start(Environment.CurrentDirectory + "\\" + proccessename);
                    /*if (matches.Count == 0)
                    {
                        Console.WriteLine("parse failed.");
                        return;
                    }

                    foreach (Match match in matches)
                    {
                        if (!match.Success) { continue; }
                        
                    }*/
                }
            }
        }
        
        internal Task DlFile(string link, string file)
        {

            using(WebClient client = new WebClient())
            {
                return client.DownloadFileTaskAsync(new Uri(link + file), "./" + file);
            }
        }

    }
}
