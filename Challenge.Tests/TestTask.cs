using System.Collections.Generic;
using System.Threading.Tasks;
using Challenge.Lib;

namespace Challenge.Tests
{
    internal class TestTask : ITask
    {
        private readonly List<int> _started;
        private readonly List<int> _completed;
        private readonly int _order;
        private readonly int _delay;

        public TestTask(
            List<int> started = null, 
            List<int> completed = null, 
            int order = 0, 
            int delay = 1)
        {
            _started = started ?? new List<int>();
            _completed = completed ?? new List<int>();
            _order = order;
            _delay = delay;
        }

        public Task Execute()
        {
            return Task.Run(() => 
            {
                lock (_started) _started.Add(_order); 
                Task.Delay(_delay * 1000).Wait();
                lock (_completed) _completed.Add(_order);
            });
        }
    }
}