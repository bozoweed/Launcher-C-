using System;
using System.Collections.Generic;
using System.Text;

namespace ServerFileGenerator.Classes
{
    class GameInfo
    {
        public int Version { get; set; } = 0;
        public List<FolderInfo> RemoteGameInfo { get; set; } = new List<FolderInfo>();
        public GameInfo()
        {

        }
    }
}
