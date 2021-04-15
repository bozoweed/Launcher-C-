using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace Client.Classes
{
    class SteamUser
    {
        long SteamId=0;
        string Pseudo="";
        Form1 Base;

        string fileName = "steam_settings";
        string FilePseudo = @"\force_account_name.txt";
        string FileId = @"\force_steamid.txt";
        string FileLanguage = @"\force_language.txt";

        public SteamUser( Form1 bas)
        {
            Base = bas;
            Generate();
        }

        private void Generate()
        {

            GetSteamId();
            GetPseudo();
            Action act = () =>
            {

                Base.Text += " | Pseudo " + Pseudo + " | SteamId " + SteamId;
            };
            Base.BeginInvoke(act);
        }

        private void GetPseudo()
        {
            try
            {

                WebRequest request = WebRequest.Create("https://steamidfinder.com/lookup/" + SteamId);
                WebResponse response = request.GetResponse();
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    string result = reader.ReadToEnd();
                    Pseudo = result.Split("<br>name ")[1].Split("</code>")[0].Replace("<code>", "").Trim();
                }
            }
            catch(Exception a)
            {
                Base.LogError(a);
            }
        }

        private void GetSteamId()
        {
            string hex = ((int)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Valve\Steam\ActiveProcess", "ActiveUser", 0)).ToString("x");
            while (hex.Length < 8)
            {
                hex = "0" + hex;
            }
            SteamId = HexToId("1100001" + hex);
        }

        private long HexToId(string hex)
        {
            return long.Parse(hex, System.Globalization.NumberStyles.HexNumber);
        }

        public bool ValideInfo()
        {
            return SteamRunning() && SteamId != 0 && Pseudo != "";
        }

        private bool SteamRunning()
        {
            return Process.GetProcessesByName("steam").Length > 0;
        }

        public bool WroteIdentitity(string gamePath)
        {
            List<string> directory = Base.Reader.GetForlder(gamePath, fileName);
            if (directory != null)
            {
                foreach(string dir in directory)
                {
                    if (File.Exists(dir + FileId))
                        File.Delete(dir + FileId);
                    if (File.Exists(dir + FilePseudo))
                        File.Delete(dir + FilePseudo);
                    if (File.Exists(dir + FileLanguage))
                        File.Delete(dir + FileLanguage);
                    File.WriteAllText(dir + FileId, SteamId.ToString());
                    File.WriteAllText(dir + FilePseudo, Pseudo);
                    File.WriteAllText(dir + FileLanguage, "french");
                }
                return ValideInfo();
            }
            else
            {
                return true;
            }
        }
    }
}
