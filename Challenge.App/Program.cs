using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Challenge.Lib;
using DemoTaskScheduler = Challenge.Lib.TaskScheduler;

namespace Challenge.App
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (!int.TryParse(args.First(), out var n))
                return;

            var scheduler = new DemoTaskScheduler();
            var random = new Random(DateTime.Now.Millisecond);
            var tasks = Enumerable.Range(0, n).Select(i => new DemoTask((Priority)random.Next(3)));
            scheduler.Initialize(1);
            foreach (var task in tasks)
            {
                scheduler.Schedule(task, task.Priority);
            }
            var completitionTask = scheduler.Stop(CancellationToken.None);
            completitionTask.ContinueWith(t => Console.WriteLine("Please press Enter to exit."));
            Console.ReadLine();
        }
    }
}
