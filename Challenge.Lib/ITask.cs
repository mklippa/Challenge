using System.Threading.Tasks;

namespace Challenge.Lib
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