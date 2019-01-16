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
        private readonly BlockingCollection<ITask> _taskQueue = new BlockingCollection<ITask>();
        private readonly List<Task> _tasks = new List<Task>();
        private CancellationToken _token;

        public void Initialize(int parallelTaskNumber)
        {
            if (parallelTaskNumber < 1)
                throw new ArgumentOutOfRangeException(nameof(parallelTaskNumber));

            _maxDegreeOfParallelism = parallelTaskNumber;
        }

        public bool Schedule(ITask task, Priority priority)
        {
            _taskQueue.Add(task);

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
                List<ITask> taskList = new List<ITask>();

                while (true)
                {
                    lock (_taskQueue)
                    {
                        if (_taskQueue.Count == 0)
                        {
                            _runningOrQueuedCount--;
                            break;
                        }

                        var t = _taskQueue.Take();
                        taskList.Add(t);
                    }
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
            _token = token;
            return Task.WhenAll(_tasks);
        }
    }
}