using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Classes
{
    class FileInfo
    {
        public string Name { get; set; }
        public string Hashe { get; set; } = "errorMD5";
        public long Size { get; set; } = 0;
        public FileInfo()
        {

        }
        public FileInfo(string name, string hashe, long size)
        {
            Name = name;
            Hashe = hashe;
            Size = size;
        }
    }
}
