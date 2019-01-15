using System;
using System.Threading;

namespace Challenge
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
                1.High
                2.High
                3.High
                4.High
                5.Normal
                6.Low
                7.Normal
                8.High
             */
             var q = new TaskQueue();

            q.Enqueue(Priority.High, new CustomTask((int)Priority.High));
            q.Enqueue(Priority.High, new CustomTask((int)Priority.High));
            q.Enqueue(Priority.High, new CustomTask((int)Priority.High));
            q.Enqueue(Priority.High, new CustomTask((int)Priority.High));
            q.Enqueue(Priority.Normal, new CustomTask((int)Priority.Normal));
            q.Enqueue(Priority.Low, new CustomTask((int)Priority.Low));
            q.Enqueue(Priority.Normal, new CustomTask((int)Priority.Normal));
            q.Enqueue(Priority.High, new CustomTask((int)Priority.High));

            while (!q.IsEmpty)
            {
                Console.WriteLine((Priority)int.Parse(q.Dequeue().ToString()));
            }
        }
    }
}