using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Classes
{
    class GameInfo
    {
        public int Version { get; set; }
        public List<FolderInfo> RemoteGameInfo { get; set; }
        public GameInfo()
        {

        }
    }
}
