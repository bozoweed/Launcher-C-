using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Classes
{
    class GameMeta
    {
        public Dictionary<string, GameMetaInfo> GameList { get; set; } = new Dictionary<string, GameMetaInfo>();
        public GameMeta()
        {

        }
    }
}
