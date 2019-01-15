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
}