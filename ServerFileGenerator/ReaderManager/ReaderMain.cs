using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using ServerFileGenerator.Classes;

namespace ServerFileGenerator.ReaderManager
{
    class ReaderMain
    {
        Program Base;
        List<FolderInfo> AllFolder = new List<FolderInfo>();
        List<FileToReadInfo> AllFileToReadInfo = new List<FileToReadInfo>();
        System.Timers.Timer Debug = new System.Timers.Timer(1000);


        long maxBuffer = 0;
        long bufferUsed = 0;
        int exes = 0;
        public void Init(Program bas)
        {
            Base = bas;
            maxBuffer = Base.Config.Config().MaxRamGoToUse * ((1000 * 1000) * 1000);
            Debug.Elapsed += (ob, obj) => {
               Console.Title = ("File Left: " + (AllFileToReadInfo.Count) + " | " + exes + " Read file by Seconde | Thread Running " + Base.ThreadManager.AllThread.Count+" | Ram Allocated "+ ToMo(bufferUsed)+" Mo");
                exes = 0;
            };
        }

        long GetBufferLeft()
        {
            return maxBuffer - bufferUsed;
        }
       

        private long ToMo(long size)
        {
            return size / (1000 * 1000);
        }

        private long AllocateBuffer(long size)
        {
            long sizemo = ToMo(size);
            long leftmo = ToMo(GetBufferLeft()); 

            if (sizemo > 750)
            {
                if (leftmo > 750)
                    return Math.Min(size, GetBufferLeft());
                return 0;
            }
            if (sizemo > 500)
            {
                if (leftmo > 500) 
                    return Math.Min(size, GetBufferLeft());
                return 0;
            }
            if (sizemo > 250)
            {
                if (leftmo > 250)
                    return Math.Min(size, GetBufferLeft());
                return 0;
            }
            if (sizemo > 100)
            {
                if (leftmo > 100)
                    return Math.Min(size, GetBufferLeft());
                return 0;
            }
            if (sizemo > 50)
            {
                if (leftmo > 50)
                    return Math.Min(size, GetBufferLeft());
                return 0;
            }
            return Math.Min(size, GetBufferLeft());
            //return Math.Min(size, Math.Min(1024*1024, GetBufferLeft()));
        }
        public List<FolderInfo> AllFileToCheckCount(string originalPath)
        {
            if (Directory.Exists(originalPath))
            {
                bufferUsed = 0;
                ReadFile(originalPath);
                //Base.Title = "Queu Left : " + (AllFileToReadInfo.Count) + " | " + exes + " Request Seconde | Thread Running " + Base.ThreadManager.AllThread.Count + " | Thread Task Queu " + Base.ThreadManager.AllTask.Count + " | Used mem " + (CurrentmemorryUsed / ((1024 * 1024))) + " Mb | FreeMemory " + (GetFreeMem() / ((1024 * 1024)) + " Mb");
                exes = 0;
                Debug.Start();
                while (AllFileToReadInfo.Count > 0)
                {


                    for (int i =0; i< AllFileToReadInfo.Count; i++)
                    {
                        FileToReadInfo info = AllFileToReadInfo[i];
                        if (info.State == 0  )
                        {
                            long bufferSize = AllocateBuffer(info.Size);
                            if( info.Size  == 0 || bufferSize > 0)
                            {

                                bufferUsed += bufferSize;
                               
                                info.Run(Base, bufferSize);
                            }
                        }
                        else if( info.State == 2)
                        {
                            exes++;
                            AllFileToReadInfo.Remove(info);
                            bufferUsed -= info.BufferSize;

                        }
                    }
                    //Console.Title = "Queu Left : " + (AllFileToReadInfo.Count) + " | " + exes + " Request Seconde | Thread Running " + Base.ThreadManager.AllThread.Count + " | Thread Task Queu " + Base.ThreadManager.AllTask.Count + " | Used mem " + (CurrentmemorryUsed/((1024*1024))) + " Mb | FreeMemory " + (GetFreeMem() / ((1024 * 1024))+" Mb") ;

                }
                Debug.Stop();
                AllFileToReadInfo.Clear();
            }
            else
            {
               // Base.Log("Folder " + originalPath + " Not Found");
            }
            return AllFolder;
        }



        public void ReadFile(string originalPath)
        {
            if (Directory.Exists(originalPath))
            {
                Directory.CreateDirectory(@".\ToUpload" + originalPath);
                FolderInfo folderInfo = new FolderInfo(originalPath);
                foreach (string Dir in Directory.GetDirectories(originalPath))
                {
                   
                        ReadFile(Dir);
                }

                foreach (string File in Directory.GetFiles(originalPath))
                {
                    System.IO.FileInfo info = new System.IO.FileInfo(File);
                  
                    //info.CopyTo(@".\ToUpload" + File);
                    AllFileToReadInfo.Add(new FileToReadInfo((buffer) =>
                    {
                       
                        ReadFile(folderInfo, File, info, buffer);

                    }, info.Length));
                }
                AllFolder.Add(folderInfo);
            }
            else
            {
               // Base.Log("Folder " + originalPath + " Not Found");
            }
        }

        protected string getFileMd5(string filePath, long size, long bufferSize)
        {
            if (!File.Exists(filePath))
            {
                Base.LogWarning(filePath + " not exist");
                return "errormd5";
            }
            ZipFile(filePath, @".\ToUpload" + filePath);
            if (size == 0)
                using (MD5 md5 = MD5.Create())
                {
                    string md = BitConverter.ToString(md5.ComputeHash(File.ReadAllBytes(filePath))).Replace("-", "");

                    return md;
                }


            return getFileMd5Big(filePath, bufferSize);
            //}
            //}
        }


        private void ZipFile(string FilePath, string toDir)
        {
            System.IO.FileInfo file = new System.IO.FileInfo(FilePath);
            using (var sourceFileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var destinationStream = new FileStream(toDir + ".zip", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    using (var archive = new ZipArchive(destinationStream, ZipArchiveMode.Create, true))
                    {
                        var File = archive.CreateEntry(file.Name, CompressionLevel.Optimal);
                        using (var entryStream = File.Open())
                        {
                            var fileStream = sourceFileStream;
                            fileStream.CopyTo(entryStream);
                        }
                    }
                }
            }
        }

        protected string getFileMd5Big(string filePath, long size)
        {
            if (!File.Exists(filePath))
            {
                Base.LogWarning(filePath + " not exist");
                return "errormd5";
            }
            try
            {
                using (MD5 md5 = MD5.Create())
                {
                    //using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, (int) size))
                    using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {

                        byte[] block = new byte[size];

                        int length;

                        while ((length = stream.Read(block, 0, block.Length)) > 0)
                        {
                            md5.TransformBlock(block, 0, length, null, 0);
                        }
                        md5.TransformFinalBlock(block, 0, 0);
                        //return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "");

                    }

                    return BitConverter.ToString(md5.Hash).Replace("-", "");
                        //return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "");
                    //}
                }
            }
            catch (Exception e)
            {
                Base.LogError(e);
                return "errormd5";
            }
        }
        public void ReadFile(FolderInfo folderInfo, string File, System.IO.FileInfo info, long buffer)
        {
            string md5 = getFileMd5(File, info.Length, buffer);
            lock (folderInfo.ListedFiles)
                folderInfo.ListedFiles.Add(new Classes.FileInfo(
                               info.Name, md5
                           ,
                           info.Length
                           ));
            //Base.AddTextBox(File);
                Base.Log(File);

        }
        private GameInfo GetGameInfo(string name, JsonSerializerOptions options)
        {
            GameInfo gameInfo;
            if (File.Exists(@Base.Config.Config().FolderApi + @".\" + name + ".json"))
            {
                gameInfo = JsonSerializer.Deserialize<GameInfo>(File.ReadAllText(@Base.Config.Config().FolderApi + @".\" + name + ".json"), options);
                File.Delete(@Base.Config.Config().FolderApi + @".\" + name + ".json");
            }
            else
            {
                gameInfo = new GameInfo();
            }
            return gameInfo;
        }


        private GameMeta GetGameMeta(JsonSerializerOptions options)
        {
            GameMeta gameMeta;
            if (File.Exists(@Base.Config.Config().FolderApi + @"\api.json"))
            {
                gameMeta = JsonSerializer.Deserialize<GameMeta>(File.ReadAllText(@Base.Config.Config().FolderApi + @"\api.json"), options);
                File.Delete(@Base.Config.Config().FolderApi + @"\api.json");
            }
            else
            {
                gameMeta = new GameMeta();
            }
            return gameMeta;
        }

        private void MoveToDir(string original, string target)
        {
            if (Directory.Exists(target))
                Directory.Delete(target, true);
            if (Path.GetPathRoot(original) == Path.GetPathRoot(target))
            {
                Directory.Move(original, target);
            }
            else
            {
                CopyDirectoryRecursive(original, target);
                Directory.Delete(original, true);
            }
        }

        private void CopyDirectoryRecursive(string source, string target)
        {
            foreach (string dirPath in Directory.GetDirectories(source, "*",    SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(source, target));

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(source, "*.*",
                SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(source, target), true);


        }

        public void SaveConfig(string name)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            GameInfo gameInfo = GetGameInfo(name, options);
            gameInfo.Version++;
            MoveToDir(@".\ToUpload\" + name, @Base.Config.Config().FolderApi + @"\" + name);
            gameInfo.RemoteGameInfo.Clear();
           /* for (int i = AllFolder.Count - 1; i > 0; i--)
            {
                gameInfo.RemoteGameInfo.Add(AllFolder.ElementAt(i));
            }*/
           foreach(FolderInfo info in AllFolder)
            {
                gameInfo.RemoteGameInfo.Add(info);
            }
            GameMeta gameMeta = GetGameMeta(options);
            GameMetaInfo gameMetaInfo = gameMeta.GetGameMeta(name);
            gameMetaInfo.Game = name;
            File.WriteAllText(@Base.Config.Config().FolderApi + @"\api.json", JsonSerializer.Serialize(gameMeta, options));
            File.WriteAllText(@Base.Config.Config().FolderApi + @"\" + name + ".json", JsonSerializer.Serialize(gameInfo, options));
            AllFolder.Clear();
            gameInfo.RemoteGameInfo.Clear();
        }


    }
}
