using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Challenge
{
    public class TaskScheduler : ITaskScheduler
    {
        

        public void Initialize(int parallelTaskNumber)
        {
            //_parallelTaskNumber = parallelTaskNumber;
        }

        public bool Schedule(ITask task, Priority priority)
        {
            throw new System.NotImplementedException();
            
        }

        public Task Stop(CancellationToken token)
        {
            throw new System.NotImplementedException();
        }
    }
}