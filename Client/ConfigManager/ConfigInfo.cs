using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Client.ConfigManager
{
    class ConfigInfo
    {        
        public int Thread { set; get; } = Environment.ProcessorCount;
        public int ThreadSleep { set; get; } = 0;
        public long MaxRamGoToUse { set; get; } = 4;

        public bool Debug { set; get; } = false;

        public string Api { get; set; } = "http://51.15.15.138/api/launcher/game/";

        public Dictionary<string, int> GameVersion { get; set; } = new Dictionary<string, int>();

        public ConfigInfo()
        {

        }
    }
}
