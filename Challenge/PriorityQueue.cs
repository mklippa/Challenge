using System.Collections.Generic;
using System.Linq;

namespace Challenge
{
    public class PriorityQueue<P, V>
    {
        private readonly SortedDictionary<P, Queue<V>> list = new SortedDictionary<P, Queue<V>>();

        public bool IsEmpty => !list.Any();

        public void Enqueue(P priority, V value)
        {
            if (!list.TryGetValue(priority, out var q))
            {
                q = new Queue<V>();
                list.Add(priority, q);
            }

            q.Enqueue(value);
        }

        public V Dequeue()
        {
            // will throw if there isn’t any first element!
            var pair = list.First();
            var v = pair.Value.Dequeue();
            if (pair.Value.Count == 0) // nothing left of the top priority.
                list.Remove(pair.Key);
            return v;
        }
    }
}