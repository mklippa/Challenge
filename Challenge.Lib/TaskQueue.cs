using System.Collections.Generic;
using System.Linq;

namespace Challenge.Lib
{
    public class TaskQueue
    {
        private const int Limit = 3;
        private readonly SortedDictionary<Priority, Queue<ITask>> _prioritizedTasks = new SortedDictionary<Priority, Queue<ITask>>();
        private readonly int _normalPosition = Limit + 1;
        private int _counter = 0;
        public bool IsEmpty => !_prioritizedTasks.Any();

        public void Enqueue(Priority priority, ITask task)
        {
            if (!_prioritizedTasks.TryGetValue(priority, out var tasks))
            {
                tasks = new Queue<ITask>();
                _prioritizedTasks.Add(priority, tasks);
            }

            tasks.Enqueue(task);
        }

        public ITask Dequeue()
        {
            _counter = _counter % _normalPosition + 1;

            var priority = Priority.Normal;
            if (_counter != _normalPosition || !_prioritizedTasks.TryGetValue(priority, out var tasks))
            {
                var first = _prioritizedTasks.First();
                tasks = first.Value;
                priority = first.Key;
            }

            var task = tasks.Dequeue();
            if (!tasks.Any())
                _prioritizedTasks.Remove(priority);

            if (IsEmpty)
                _counter = 0;

            return task;
        }
    }
}