using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Challenge
{
    public class TaskScheduler : ITaskScheduler
    {
        private int _maxDegreeOfParallelism;
        private volatile int _runningOrQueuedCount;
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

            _maxDegreeOfParallelism = parallelTaskNumber;
            _runningOrQueuedCount = 0;
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

            if (_runningOrQueuedCount < _maxDegreeOfParallelism)
            {
                _runningOrQueuedCount++;
                RunTasks();
            }

            return true;
        }

        private void RunTasks()
        {
            _tasks.Add(Task.Run(() =>
            {
                var taskList = new List<ITask>();

                while (true)
                {
                    lock (_taskQueue)
                    {
                        if (_taskQueue.IsEmpty)
                        {
                            _runningOrQueuedCount--;
                            break;
                        }

                        var t = _taskQueue.Dequeue();
                        taskList.Add(t);
                    }
                }

                if (_token.IsCancellationRequested)
                {
                    _token.ThrowIfCancellationRequested();
                }

                if (taskList.Count == 1)
                {
                    taskList[0].Execute();
                }
                else if (taskList.Count > 0)
                {
                    var batches = taskList.GroupBy(
                        task => taskList.IndexOf(task) / _maxDegreeOfParallelism);

                    foreach (var batch in batches)
                    {
                        batch.AsParallel().ForAll(task => task.Execute());
                    }
                }
            }, _token));
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