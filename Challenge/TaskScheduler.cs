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
            int maxConcurrency=parallelTaskNumber;
            var messages = new List<string>();
            using(SemaphoreSlim concurrencySemaphore = new SemaphoreSlim(maxConcurrency))
            {
                List<Task> tasks = new List<Task>();
                foreach(var msg in messages)
                {
                    concurrencySemaphore.Wait(cancellationToken:null);

                    var t = Task.Factory.StartNew(() =>
                    {

                        try
                        {
                            Process(msg);
                        }
                        finally
                        {
                            concurrencySemaphore.Release();
                        }
                    });

                    tasks.Add(t);
                }

                Task.WaitAll(tasks.ToArray());
            }

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