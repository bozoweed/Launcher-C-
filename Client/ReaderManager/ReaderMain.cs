using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using Client.Classes;

namespace Client.ReaderManager
{
    class ReaderMain
    {
        Form1 Base;
        List<FolderInfo> AllFolder = new List<FolderInfo>();
        List<FileToReadInfo> AllFileToReadInfo = new List<FileToReadInfo>();
        System.Timers.Timer Debug = new System.Timers.Timer(1000);

        long maxBuffer = 0;
        long bufferUsed = 0;
        int exes = 0;
        public void Init(Form1 bas)
        {
            Base = bas;
            maxBuffer = Base.Config.Config().MaxRamGoToUse * ((1000 * 1000) * 1000);
            Debug.Elapsed += (ob, obj) => {
                Base.AddTextBox("File Left: " + (AllFileToReadInfo.Count) + " | " + exes + " Read file by Seconde | Thread Running " + Base.ThreadManager.AllThread.Count+" | Ram Allocated "+ Base.ToMo(bufferUsed)+" Mo");
                exes = 0;
            };
        }

        long GetBufferLeft()
        {
            return maxBuffer - bufferUsed;
        }
       

        private void DisplayStepProgress(int max)
        {
            Action act = () =>
            {
                Base.StepBarGUI.Value = 0;
                Base.StepBarGUI.Minimum = 0;
                Base.StepBarGUI.Maximum = max;
                Base.StepBarGUI.Visible = true;
            };
            Base.Invoke(act);
        }

        private void HideStepProgress()
        {
            Action act = () =>
            {
                Base.StepBarGUI.Visible = false;
            };
            Base.Invoke(act);
        }



        private void UpdateStepProgress(string text = "")
        {
            Action act = () =>
            {
                using (Graphics gr = Base.StepBarGUI.CreateGraphics())
                {
                    Base.StepBarGUI.Value++;
                    int percent = (int)(((double)(Base.StepBarGUI.Value - Base.StepBarGUI.Minimum) /
                    (double)(Base.StepBarGUI.Maximum - Base.StepBarGUI.Minimum)) * 100);
                    //gr.DrawString((text!="" ? text: Base.StepBarGUI.Value + "/" + Base.StepBarGUI.Maximum + "(" + percent + "%)"),
                    gr.DrawString("",
                    SystemFonts.DefaultFont,
                    Brushes.Black,
                    new PointF(Base.StepBarGUI.Width / 2 - (gr.MeasureString(percent.ToString() + "%",
                        SystemFonts.DefaultFont).Width / 2.0F),
                    Base.StepBarGUI.Height / 2 - (gr.MeasureString(percent.ToString() + "%",
                        SystemFonts.DefaultFont).Height / 2.0F)));
                }
            };
            Base.Invoke(act);
        }

        

        private long AllocateBuffer(long size)
        {
            long sizemo = Base.ToMo(size);
            long leftmo = Base.ToMo(GetBufferLeft()); 

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
                DisplayStepProgress(AllFileToReadInfo.Count);
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
                            UpdateStepProgress();
                            exes++;
                            AllFileToReadInfo.Remove(info);
                            bufferUsed -= info.BufferSize;

                        }
                    }
                    //Console.Title = "Queu Left : " + (AllFileToReadInfo.Count) + " | " + exes + " Request Seconde | Thread Running " + Base.ThreadManager.AllThread.Count + " | Thread Task Queu " + Base.ThreadManager.AllTask.Count + " | Used mem " + (CurrentmemorryUsed/((1024*1024))) + " Mb | FreeMemory " + (GetFreeMem() / ((1024 * 1024))+" Mb") ;

                }
                Debug.Stop();
                HideStepProgress();
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
                FolderInfo folderInfo = new FolderInfo(originalPath);
                foreach (string Dir in Directory.GetDirectories(originalPath))
                {
                   
                        ReadFile(Dir);
                }

                foreach (string File in Directory.GetFiles(originalPath))
                {
                    System.IO.FileInfo info = new System.IO.FileInfo(File);                   
                    AllFileToReadInfo.Add(new FileToReadInfo((buffer) =>
                    {
                       
                        ReadFile(folderInfo, File, info, buffer);

                    }, info.Length));
                }
                AllFolder.Add(folderInfo);
            }
        }

        public List<string> GetForlder(string originalPath, string name)
        {
            if (Directory.Exists(originalPath))
            {
                List<string> alldir = new List<string>();
                foreach (string Dir in Directory.GetDirectories(originalPath))
                {
                    if (Dir.Contains(name))
                        alldir.Add(Dir);
                    List<string> fold = GetForlder(Dir, name);
                    if (fold != null)
                    {
                        foreach (string dir in fold)
                            alldir.Add(dir);
                        fold.Clear();
                    }
                }
                return alldir;
            }
            return null;
        }

        protected string getFileMd5(string filePath, long size, long bufferSize)
        {
            if (!File.Exists(filePath))
            {
                Base.LogWarning(filePath + " not exist");
                return "errormd5";
            }
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
                    using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, (int)size))
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
               // Base.Log(File);

        }

    }
}
