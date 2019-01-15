using System;
using System.Threading;
using System.Threading.Tasks;

namespace Challenge
{
    public class CustomTask : ITask
    {
        private readonly int _number;

        public CustomTask(int number)
        {
            this._number = number;
        }
        
        public Task Execute()
        {
            return Task.Run(() => { Write($"Running {_number} seconds"); Thread.Sleep(_number * 1000); });
        }

        void Write(string msg)
        {
            Console.WriteLine(DateTime.Now.ToString() + " on Thread " + Thread.CurrentThread.ManagedThreadId.ToString() + " -- " + msg);
        }
    }
}