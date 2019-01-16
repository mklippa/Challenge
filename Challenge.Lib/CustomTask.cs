using System;
using System.Threading;
using System.Threading.Tasks;

namespace Challenge
{
    public class CustomTask : ITask
    {
        private readonly int _duration;

        public CustomTask(int duration)
        {
            this._duration = duration;
        }
        
        public Task Execute()
        {
            return Task.Run(() =>
            {
                Write($"Running {_duration} seconds");
                Thread.Sleep(_duration * 1000);
                Console.WriteLine($"{_duration}-second task is finished");
            });
        }

        private void Write(string msg)
        {
            Console.WriteLine($"{DateTime.Now} on Thread {Thread.CurrentThread.ManagedThreadId} -- {msg}");
        }
    }
}