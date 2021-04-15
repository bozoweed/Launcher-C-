using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Classes
{
    class FileToReadInfo
    {
         Action<long> Func { get; set; }
        public int State { get; set; } = 0;
        public long Size { get; set; } = 0;
        public long BufferSize { get; set; } = 0;
        public FileToReadInfo(Action<long> func, long size)
        {
            Func = func;
            Size = size;
        }

        public void Run(Form1 Base, long buffer)
        {
            State = 1;
            BufferSize = buffer;
            Base.ThreadManager.AddAsyncTask(() =>
            {
                Func(BufferSize);
                State = 2;
            });
        }
    }
}
