using System;
using System.Threading;
using System.Threading.Tasks;

namespace Challenge
{
    /// <summary>
    /// Задача.
    /// </summary>
    public interface ITask
    {
        /// <summary>
        /// Исполнить задачу.
        /// </summary>
        /// <returns>Task, завершаемый по мере исполнения задачи.</returns>
        Task Execute();
    }

    public class CustomTask : ITask
    {
        private readonly Priority _priority;

        public CustomTask(Priority priority)
        {
            _priority = priority;
        }
        
        public Task Execute()
        {
            Console.WriteLine(_priority);
            return Task.Delay(2000);
        }
    }
}