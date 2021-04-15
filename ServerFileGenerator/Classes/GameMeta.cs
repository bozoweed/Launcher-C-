using System;
using System.Collections.Generic;
using System.Text;

namespace ServerFileGenerator.Classes
{
    class GameMeta
    {
        public Dictionary<string, GameMetaInfo> GameList { get; set; } = new Dictionary<string, GameMetaInfo>();
        
        public GameMeta()
        {

        }


        public GameMetaInfo GetGameMeta(string name)
        {
            if (!GameList.ContainsKey(name))
                GameList.Add(name, new GameMetaInfo());
            return GameList[name];
        }
    }
}
