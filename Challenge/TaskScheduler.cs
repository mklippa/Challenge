using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Challenge
{
    public class TaskScheduler : ITaskScheduler
    {
        PriorityQueue _queue = new PriorityQueue();
        private int _parallelTaskNumber = 0;

        public void Initialize(int parallelTaskNumber)
        {
            _parallelTaskNumber = parallelTaskNumber;
        }

        public void Start()
        {
            if (_parallelTaskNumber == 0)
            {
                throw new Exception();
            }
            
            using(var concurrencySemaphore = new SemaphoreSlim(_parallelTaskNumber))
            {
                List<Task> tasks = new List<Task>();
                while(!_queue.IsEmpty)
                {
                    concurrencySemaphore.Wait(CancellationToken.None);

                    try
                    {
                        var task = _queue.Dequeue().Execute();
                        tasks.Add(task);
                    }
                    finally 
                    {
                            concurrencySemaphore.Release();
                    }
                }

                Task.WaitAll(tasks.ToArray(),CancellationToken.None);
            }
        }

        public bool Schedule(ITask task, Priority priority)
        {
            _queue.Enqueue(priority, task);
            return true;
        }

        public Task Stop(CancellationToken token)
        {
            throw new System.NotImplementedException();
        }
    }
}