using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Challenge
{
    public class PriorityQueue
    {
        private readonly SortedDictionary<Priority, Queue<ITask>> list = new SortedDictionary<Priority, Queue<ITask>>();

        private int limit = 3;
        
        public bool IsEmpty => !list.Any();

        public void Enqueue(Priority priority, ITask task)
        {
            if (!list.TryGetValue(priority, out var tasks))
            {
                tasks = new Queue<ITask>();
                list.Add(priority, tasks);
            }

            tasks.Enqueue(task);
        }

        public ITask Dequeue()
        {
            Priority priority = Priority.Normal;
            Queue<ITask> tasks;
            if (limit == 0)
            {
                limit = 3;
                if (!list.TryGetValue(priority, out tasks))
                {
                    tasks = list.First().Value;
                    priority = list.First().Key;
                }
            }
            else
            {
                tasks = list.First().Value;
                priority = list.First().Key;
            }
            var v = tasks.Dequeue();
            if (tasks.Count == 0) 
                list.Remove(priority);
            limit--;
            return v;
        }
    }
}