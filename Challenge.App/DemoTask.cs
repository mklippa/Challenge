using System;
using System.Threading.Tasks;
using Challenge.Lib;

namespace Challenge.App
{
    internal class DemoTask : ITask
    {
        private readonly Priority _priority;

        public DemoTask(Priority priority)
        {
            _priority = priority;
        }

        public Priority Priority => _priority;
        public Task Execute()
        {
            return Task.Run(() =>
            {
                Console.WriteLine(_priority);
                Task.Delay(1000).Wait();
            });
        }
    }
}
