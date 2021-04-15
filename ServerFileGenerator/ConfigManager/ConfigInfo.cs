using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ServerFileGenerator.ConfigManager
{
    class ConfigInfo
    {        
        public int Thread { set; get; } = Environment.ProcessorCount;
        public int ThreadSleep { set; get; } = 0;
        public long MaxRamGoToUse { set; get; } = 4;
        public string FolderApi { get; set; } = @"C:\xampp\htdocs\api\launcher\game";
        public bool Debug { set; get; } = false;
        public ConfigInfo()
        {

        }
    }
}
