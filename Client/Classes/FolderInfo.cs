using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Classes
{
    class FolderInfo
    {
        public string FolderName { get; set; }
        public List<FileInfo> ListedFiles { get; set; }
        public FolderInfo()
        {

        }
        public FolderInfo(string foldername)
        {
            FolderName = foldername;
            ListedFiles = new List<FileInfo>();
        }
    }
}
