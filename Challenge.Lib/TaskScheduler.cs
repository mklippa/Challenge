using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Challenge.Lib
{
    public class TaskScheduler : ITaskScheduler
    {
        private SemaphoreSlim _semaphore;
        private TaskQueue _taskQueue;
        private List<Task> _tasks;
        private CancellationToken _token;
        private bool _isAlive;

        public void Initialize(int parallelTaskNumber)
        {
            if (_isAlive)
            {
                throw new InvalidOperationException("Scheduler has already been initialized.");
            }

            if (parallelTaskNumber < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(parallelTaskNumber));
            }

            _semaphore = new SemaphoreSlim(parallelTaskNumber, parallelTaskNumber);
            _taskQueue = new TaskQueue();
            _tasks = new List<Task>();
            _isAlive = true;
        }

        public bool Schedule(ITask task, Priority priority)
        {
            if (!_isAlive)
            {
                return false;
            }

            lock(_taskQueue) _taskQueue.Enqueue(priority, task);

            _tasks.Add(Task.Run(() =>
            {
                _semaphore.Wait(_token);

                try
                {
                    if (_token.IsCancellationRequested)
                    {
                        _token.ThrowIfCancellationRequested();
                    }
                    ITask t;
                    lock (_taskQueue) t = _taskQueue.Dequeue();
                    t.Execute().Wait();
                }
                finally
                {
                    _semaphore.Release();
                }
            }, _token));

            return true;
        }
        public Task Stop(CancellationToken token)
        {
            if(!_isAlive)
            {
                throw new InvalidOperationException("Scheduler is not running.");
            }

            _isAlive = false;
            _token = token;
            return Task.WhenAll(_tasks);
        }
    }
}