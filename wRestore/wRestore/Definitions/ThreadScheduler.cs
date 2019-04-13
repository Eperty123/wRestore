using System;
using System.Threading;

namespace wRestore.Definitions
{
    public class ThreadScheduler
    {
        public string Name { get; set; }
        public bool IsBackground { get; set; }
        private Thread CurrentThread { get; set; }
        private ManualResetEvent State { get; set; }

        public ThreadScheduler()
        {
            State = new ManualResetEvent(false);
        }

        public void Start(Action action)
        {
            // Don't do anything unless this thread is still running.
            if (IsRunning()) return;

            // Do something if above is false.
            State.Set();
            Thread t = new Thread(() =>
            {
                action();
            });
            t.Name = Name;
            t.IsBackground = IsBackground;
            t.Start();
            CurrentThread = t;
        }

        public bool IsRunning()
        {
            if (this == null || CurrentThread == null) return false;
            return CurrentThread.IsAlive;
        }

        public void Stop()
        {
            State.Reset();
            if (CurrentThread != null)
                CurrentThread.Abort();
        }
    }
}
