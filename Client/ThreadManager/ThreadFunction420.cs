using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client.ThreadManager
{
    class ThreadFunction420
    {

        public int Id { get; set; } = -1;
        Form1 Base;
        public DateTime CreatedAt = DateTime.Now;

        public ThreadFunction420(Form1 bas, int id) 
        {
            Id = id;
            Base = bas;
        }

        public virtual void Start()
        {
            Base.LogWarning("Thread Request Was Empty");
        } 
    }
}
