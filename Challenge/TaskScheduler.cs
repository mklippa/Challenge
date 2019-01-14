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
        private readonly BlockingCollection<ITask> _tasks = new BlockingCollection<ITask>();

        public void Initialize(int parallelTaskNumber)
        {
            if (parallelTaskNumber < 1)
                throw new ArgumentOutOfRangeException(nameof(parallelTaskNumber));

            _maxDegreeOfParallelism = parallelTaskNumber;
        }

        public bool Schedule(ITask task, Priority priority)
        {
            _tasks.Add(task);

            if (_runningOrQueuedCount < _maxDegreeOfParallelism)
            {
                _runningOrQueuedCount++;
                RunTasks();
            }

            return true;
        }

        private void RunTasks()
        {
            ThreadPool.UnsafeQueueUserWorkItem(_ =>
            {
                List<ITask> taskList = new List<ITask>();

                while (true)
                {
                    lock (_tasks)
                    {
                        if (_tasks.Count == 0)
                        {
                            _runningOrQueuedCount--;
                            break;
                        }

                        var t = _tasks.Take();
                        taskList.Add(t);
                    }
                }

                if (taskList.Count == 1)
                {
                    taskList[0].Execute().Start();
                }
                else if (taskList.Count > 0)
                {
                    var batches = taskList.GroupBy(
                        task => taskList.IndexOf(task) / _maxDegreeOfParallelism);

                    foreach (var batch in batches)
                    {
                        batch.AsParallel().ForAll(task => task.Execute().Start());
                    }
                }
            }, null);
        }

        public Task Stop(CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}