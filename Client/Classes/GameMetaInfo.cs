using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Classes
{
    class GameMetaInfo
    {
        public string Label { get; set; } = "";
        public string Game { get; set; } = "";
        public string ExeName { get; set; } = "";
        public string LaunchCommand { get; set; } = "";

        public GameMetaInfo()
        {

        }
    }
}
