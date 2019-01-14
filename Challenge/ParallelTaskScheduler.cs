using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Challenge
{
    public class ParallelTaskScheduler : System.Threading.Tasks.TaskScheduler
    {
        private readonly int _maxDegreeOfParallelism;
        private volatile int _runningOrQueuedCount;
        private readonly LinkedList<Task> tasks = new LinkedList<Task>();

        public ParallelTaskScheduler(int maxDegreeOfParallelism)
        {
            if (maxDegreeOfParallelism < 1)
                throw new ArgumentOutOfRangeException("maxDegreeOfParallelism");

            _maxDegreeOfParallelism = maxDegreeOfParallelism;
        }

        public override int MaximumConcurrencyLevel => _maxDegreeOfParallelism;

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            var lockTaken = false;
            try
            {
                Monitor.TryEnter(tasks, ref lockTaken);
                if (lockTaken) return tasks.ToArray();
                else throw new NotSupportedException();
            }
            finally { if (lockTaken) Monitor.Exit(tasks); }
        }

        protected override void QueueTask(Task task)
        {
            lock (tasks) tasks.AddLast(task);

            if (_runningOrQueuedCount < _maxDegreeOfParallelism)
            {
                _runningOrQueuedCount++;
                RunTasks();
            }
        }

        private void RunTasks()
        {
            ThreadPool.UnsafeQueueUserWorkItem(_ =>
            {
                List<Task> taskList = new List<Task>();

                while (true)
                {
                    lock (tasks)
                    {
                        if (tasks.Count == 0)
                        {
                            _runningOrQueuedCount--;
                            break;
                        }

                        var t = tasks.First.Value;
                        taskList.Add(t);
                        tasks.RemoveFirst();
                    }
                }

                if (taskList.Count == 1)
                {
                    base.TryExecuteTask(taskList[0]);
                }
                else if (taskList.Count > 0)
                    {
                        var batches = taskList.GroupBy(
                            task => taskList.IndexOf(task) / _maxDegreeOfParallelism);

                        foreach (var batch in batches)
                        {
                            batch.AsParallel().ForAll(task =>
                                base.TryExecuteTask(task));
                        }
                    }

            }, null);
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return false;
        }
    }
}