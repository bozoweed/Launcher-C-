
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client.ThreadManager
{
    class ThreadFunctionAsyncCallBack420 : ThreadFunction420
    {

        Action Func { get; set; }
        Action CallBack { get; set; }
        Form1 Base;
        public ThreadFunctionAsyncCallBack420(Form1 Bas, int id, Action func, Action callBack): base(Bas, id)
        {
            Func = func;
            Base = Bas;
            CallBack = callBack;
        }

        public override void Start()
        {
            
            Func();
            CallBack();
        }


    }
}
