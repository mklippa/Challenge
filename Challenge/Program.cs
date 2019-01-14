using System;
using System.Threading;

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
//            var p = new PriorityQueue();
//            p.Enqueue(Priority.High, 0);
//            p.Enqueue(Priority.High, 0);
//            p.Enqueue(Priority.High, 0);
//            p.Enqueue(Priority.High, 0);
//            p.Enqueue(Priority.Normal, 1);
//            p.Enqueue(Priority.Low, 2);
//            p.Enqueue(Priority.Normal, 1);
//            p.Enqueue(Priority.High, 0);
//            
//            Console.WriteLine((Priority)p.Dequeue());
//            Console.WriteLine((Priority)p.Dequeue());
//            Console.WriteLine((Priority)p.Dequeue());
//            Console.WriteLine((Priority)p.Dequeue());
//            Console.WriteLine((Priority)p.Dequeue());
//            Console.WriteLine((Priority)p.Dequeue());
//            Console.WriteLine((Priority)p.Dequeue());
//            Console.WriteLine((Priority)p.Dequeue());
            var ts = new TaskScheduler();
            ts.Initialize(5);
            ts.Schedule(new CustomTask(1), Priority.High);
            ts.Schedule(new CustomTask(2), Priority.High);
            ts.Schedule(new CustomTask(3), Priority.High);
            ts.Schedule(new CustomTask(4), Priority.High);
            ts.Schedule(new CustomTask(5), Priority.Normal);
            ts.Schedule(new CustomTask(6), Priority.Low);
            ts.Schedule(new CustomTask(7), Priority.Normal);
            ts.Schedule(new CustomTask(8), Priority.High);
            //ts.Stop(new CancellationToken());

            Console.ReadLine();
        }
    }
}