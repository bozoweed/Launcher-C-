using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Classes
{
    class DownloadFileInfo
    {
        public string Directory;
        public string FileName;
        public long Size;
        public bool Downloaded = false;


        public DownloadFileInfo(string dir, string file, long size)
        {
            Directory = dir;
            FileName = file;
            Size = size;
        }

    }
}
