using System;

namespace Challenge
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
             * 1.High
    2.High
    3.High
    4.High
    5.Normal
    6.Low
    7.Normal
    8.High
             */
            var p = new PriorityQueue();
            p.Enqueue(Priority.High, 0);
            p.Enqueue(Priority.High, 0);
            p.Enqueue(Priority.High, 0);
            p.Enqueue(Priority.High, 0);
            p.Enqueue(Priority.Normal, 1);
            p.Enqueue(Priority.Low, 2);
            p.Enqueue(Priority.Normal, 1);
            p.Enqueue(Priority.High, 0);
            
            Console.WriteLine((Priority)p.Dequeue());
            Console.WriteLine((Priority)p.Dequeue());
            Console.WriteLine((Priority)p.Dequeue());
            Console.WriteLine((Priority)p.Dequeue());
            Console.WriteLine((Priority)p.Dequeue());
            Console.WriteLine((Priority)p.Dequeue());
            Console.WriteLine((Priority)p.Dequeue());
            Console.WriteLine((Priority)p.Dequeue());
        }
    }
}