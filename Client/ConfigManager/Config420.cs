using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Client.ConfigManager
{
    class Config420
    {
        Form1 Base;
        ConfigInfo Info;
        string file = @".\Config.json";
        JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
        public Config420 (Form1 bas)
        {
            Base = bas;
            if (!File.Exists(file))
            {
                Info = new ConfigInfo();
                SaveConfig();
            }
            else
            {
                Info = JsonSerializer.Deserialize<ConfigInfo>(File.ReadAllText(file), options);   
               
            }
        }

        public ConfigInfo Config()
        {
            return Info;
        }

        public int GetGameVersion(string game)
        {
            return Info.GameVersion.ContainsKey(game) ? Info.GameVersion[game] : 0;
        }

        public void SetGameVersion(string game, int version)
        {
            if (!Info.GameVersion.ContainsKey(game))
            {
                Info.GameVersion.Add(game, version);
            }else
                Info.GameVersion[game] = version;
            SaveConfig();
        }


        public void SaveConfig()
        {
            if (File.Exists(file))
                File.Delete(file);
            File.WriteAllText(file, JsonSerializer.Serialize(Info, options)) ;

        }
    }
}
