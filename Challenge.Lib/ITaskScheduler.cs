using System.Threading;
using System.Threading.Tasks;

namespace Challenge.Lib
{
    /// <summary>
    /// Планировщик задач.
    /// </summary>
    public interface ITaskScheduler
    {
        /// <summary>
        /// Инициализирует планировщик. Остальные методы можно вызвать только после инициализации.
        /// </summary>
        /// <param name="parallelTaskNumber">Число одновременно исполняемых задач.</param>
        void Initialize(int parallelTaskNumber);

        /// <summary>
        /// Запланировать задачу на исполнение.
        /// </summary>
        /// <param name="task">Исполняемая задача.</param>
        /// <param name="priority">Приоритет исполнения.</param>
        /// <returns>Признак успешной постановки на исполнение. В случае, если планировщик остановлен (вызван метод Stop), возвращает false. Иначе - true.</returns>
        bool Schedule(ITask task, Priority priority);

        /// <summary>
        /// Останавливает планировщик.
        /// </summary>
        /// <param name="token">Токен отмены ожидания исполнения задач.</param>
        /// <returns>Task, завершаемый по мере исполнения всех запланированных задач.</returns>
        Task Stop(CancellationToken token);
    }
}