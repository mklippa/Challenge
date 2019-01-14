using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Challenge
{
    public class TaskScheduler : ITaskScheduler
    {
        private ParallelTaskScheduler _taskScheduler;
        public void Initialize(int parallelTaskNumber)
        {
            _taskScheduler = new ParallelTaskScheduler(parallelTaskNumber);
        }

        public bool Schedule(ITask task, Priority priority)
        {
            task.Execute().Start(_taskScheduler);
        }

        public Task Stop(CancellationToken token)
        {
            
        }
    }
}