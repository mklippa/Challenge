using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Challenge.Lib;
using Moq;
using NUnit.Framework;

namespace Challenge.Tests
{
    public class TaskSchedulerTests
    {
        private Challenge.Lib.TaskScheduler _taskScheduler = new Challenge.Lib.TaskScheduler();

        [SetUp]
        public void Setup()
        {
            _taskScheduler = new Challenge.Lib.TaskScheduler();
        }

        [Test]
        public void Initialize_ThrowException_IfParallelTaskNumberLessThenOrEqualTo0()
        {
            // Arrange

            // Act

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => _taskScheduler.Initialize(0));
        }

        [Test]
        public void Initialize_ThrowException_IfItWasAlreadyCalled()
        {
            // Arrange
            _taskScheduler.Initialize(1);

            // Act

            // Assert
            Assert.Throws<InvalidOperationException>(() => _taskScheduler.Initialize(1));
        }

        [Test]
        public void Schedule_ReturnFalse_IfTaskSchedulerWasNotInitialized()
        {
            // Arrange

            // Act
            var actual = _taskScheduler.Schedule(Mock.Of<ITask>(), default(Priority));

            // Assert
            Assert.AreEqual(false, actual);
        }

        [Test]
        public void Schedule_ReturnFalse_IfTaskSchedulerWasStopped()
        {
            // Arrange
            _taskScheduler.Initialize(1);
            _taskScheduler.Stop(CancellationToken.None);

            // Act
            var actual = _taskScheduler.Schedule(Mock.Of<ITask>(), default(Priority));

            // Assert
            Assert.AreEqual(false, actual);
        }

        [Test]
        public void Stop_ThrowException_IfTaskSchedulerWasNotInitialized()
        {
            // Arrange

            // Act

            // Assert
            Assert.Throws<InvalidOperationException>(() => _taskScheduler.Stop(CancellationToken.None));
        }

        [Test]
        public void Stop_ThrowException_IfTaskSchedulerWasAlreadyStopped()
        {
            // Arrange
            _taskScheduler.Initialize(1);
            _taskScheduler.Stop(CancellationToken.None);

            // Act

            // Assert
            Assert.Throws<InvalidOperationException>(() => _taskScheduler.Stop(CancellationToken.None));
        }

        [Test]
        public void Schedule_ReturnTrue_IfTaskSchedulerWasInitialized()
        {
            // Arrange
            _taskScheduler.Initialize(1);

            // Act
            var actual = _taskScheduler.Schedule(Mock.Of<ITask>(), default(Priority));

            // Assert
            Assert.AreEqual(true, actual);
        }

        [Test]
        public async Task Schedule_RunsAllTasksInRandomOrder_IfParallelismDegreeIsGreaterThenOrEqualToTasksCount_Async()
        {
            // Arrange
            var expected = new[] { 1, 2, 3 };

            var actual = new List<int>();
            var task1 = new TestTask(started: actual, order: 1);
            var task2 = new TestTask(started: actual, order: 2);
            var task3 = new TestTask(started: actual, order: 3);

            _taskScheduler.Initialize(parallelTaskNumber: 3);

            // Act
            _taskScheduler.Schedule(task1, Priority.Low);
            _taskScheduler.Schedule(task2, Priority.Normal);
            _taskScheduler.Schedule(task3, Priority.High);
            await _taskScheduler.Stop(CancellationToken.None);

            // Assert
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public async Task Schedule_RunsAllTasksInStrictOrder_IfParallelismDegreeIsEqualToOne_Async()
        {
            // Arrange
            var expected = new[] { 1, 2, 3, 5, 4, 8, 7, 6 };

            var actual = new List<int>();
            var task1 = new TestTask(started: actual, order: 1);
            var task2 = new TestTask(started: actual, order: 2);
            var task3 = new TestTask(started: actual, order: 3);
            var task4 = new TestTask(started: actual, order: 4);
            var task5 = new TestTask(started: actual, order: 5);
            var task6 = new TestTask(started: actual, order: 6);
            var task7 = new TestTask(started: actual, order: 7);
            var task8 = new TestTask(started: actual, order: 8);
            _taskScheduler.Initialize(1);

            // Act 
            _taskScheduler.Schedule(task1, Priority.High);
            _taskScheduler.Schedule(task2, Priority.High);
            _taskScheduler.Schedule(task3, Priority.High);
            _taskScheduler.Schedule(task4, Priority.High);
            _taskScheduler.Schedule(task5, Priority.Normal);
            _taskScheduler.Schedule(task6, Priority.Low);
            _taskScheduler.Schedule(task7, Priority.Normal);
            _taskScheduler.Schedule(task8, Priority.High);
            await _taskScheduler.Stop(CancellationToken.None);

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public async Task Schedule_OrdersTasksButRunsThemInRandomOrderWithinBatch_IfParallelismDegreeIsNotOne_First_Async()
        {
            // Arrange
            // Input:    Output:
            // 1. High   1. High
            // 2. High   2. High
            // 3. High   3. High
            // 4. High   5. Normal
            // 5. Normal 4. High
            // 6. Low    8. High
            // 7. Normal 7. Normal
            // 8. High   6. Low
            var expectedFirstBatch = new[] { 1, 2, 3, 5 };
            var expectedSecondBatch = new[] { 4, 8, 7, 6 };

            var actual = new List<int>();
            // the first batch
            var task1 = new TestTask(started: actual, order: 1);
            var task2 = new TestTask(started: actual, order: 2);
            var task3 = new TestTask(started: actual, order: 3);
            var task4 = new TestTask(started: actual, order: 4);
            // the second batch
            var task5 = new TestTask(started: actual, order: 5);
            var task6 = new TestTask(started: actual, order: 6);
            var task7 = new TestTask(started: actual, order: 7);
            var task8 = new TestTask(started: actual, order: 8);
            // parallelTaskNumber == batch size
            _taskScheduler.Initialize(4);

            // Act
            // schedule the first batch
            _taskScheduler.Schedule(task1, Priority.High);
            _taskScheduler.Schedule(task2, Priority.High);
            _taskScheduler.Schedule(task3, Priority.High);
            _taskScheduler.Schedule(task4, Priority.High);
            // schdule the second batch
            _taskScheduler.Schedule(task5, Priority.Normal);
            _taskScheduler.Schedule(task6, Priority.Low);
            _taskScheduler.Schedule(task7, Priority.Normal);
            _taskScheduler.Schedule(task8, Priority.High);
            await _taskScheduler.Stop(CancellationToken.None);

            // Assert
            CollectionAssert.AreEquivalent(expectedFirstBatch, actual.Take(4));
            CollectionAssert.AreEquivalent(expectedSecondBatch, actual.Skip(4));
        }

        [Test]
        public async Task Schedule_OrdersTasksButRunsThemInRandomOrderWithinBatch_IfParallelismDegreeIsNotOne_Second_Async()
        {
            // Arrange
            // Input:    Output:
            // 1. Low    2. High
            // 2. High   5. High
            // 3. Normal 7. High
            // 4. Low    3. Normal
            // 5. High   8. High
            // 6. Normal 9. High
            // 7. High   6. Normal
            // 8. High   1. Low
            // 9. High   4. Low
            var expectedFirstBatch = new[] { 2, 5, 7 };
            var expectedSecondBatch = new[] { 3, 8, 9 };
            var expectedThirdBatch = new[] { 6, 1, 4 };

            var actual = new List<int>();
            // the first batch
            var task1 = new TestTask(started: actual, order: 1);
            var task2 = new TestTask(started: actual, order: 2);
            var task3 = new TestTask(started: actual, order: 3);
            // the second batch
            var task4 = new TestTask(started: actual, order: 4);
            var task5 = new TestTask(started: actual, order: 5);
            var task6 = new TestTask(started: actual, order: 6);

            // the third batch
            var task7 = new TestTask(started: actual, order: 7);
            var task8 = new TestTask(started: actual, order: 8);
            var task9 = new TestTask(started: actual, order: 9);

            // parallelTaskNumber == batch size
            _taskScheduler.Initialize(parallelTaskNumber: 3);

            // Act
            // schedule the first batch
            _taskScheduler.Schedule(task1, Priority.Low);
            _taskScheduler.Schedule(task2, Priority.High);
            _taskScheduler.Schedule(task3, Priority.Normal);

            // schdule the second batch
            _taskScheduler.Schedule(task4, Priority.Low);
            _taskScheduler.Schedule(task5, Priority.High);
            _taskScheduler.Schedule(task6, Priority.Normal);

            // schedule the third batch
            _taskScheduler.Schedule(task7, Priority.High);
            _taskScheduler.Schedule(task8, Priority.High);
            _taskScheduler.Schedule(task9, Priority.High);
            await _taskScheduler.Stop(CancellationToken.None);

            // Assert
            System.Diagnostics.Debug.WriteLine(string.Join(", ", actual));
            CollectionAssert.AreEquivalent(expectedFirstBatch, actual.Take(3));
            CollectionAssert.AreEquivalent(expectedSecondBatch, actual.Skip(3).Take(3));
            CollectionAssert.AreEquivalent(expectedThirdBatch, actual.Skip(6));
        }

        [Test]
        public async Task Stop_CancelsAllTasksBeforeRun_IfCancellationWasCalledImmediately_Async()
        {
            // Arrange
            var actualStarted = new List<int>();
            var task1 = new TestTask(started: actualStarted, order: 1, delay: 3);
            var task2 = new TestTask(started: actualStarted, order: 2, delay: 3);
            _taskScheduler.Initialize(parallelTaskNumber: 2);
            var cts = new CancellationTokenSource();

            // Act 
            _taskScheduler.Schedule(task1, Priority.High);
            _taskScheduler.Schedule(task2, Priority.High);
            try
            {
                var stoppingTask = _taskScheduler.Stop(cts.Token);
                cts.Cancel();
                await stoppingTask;
            }
            catch (OperationCanceledException)
            {
                // do nothing when tasks were cancelled
            }
            finally
            {
                cts.Dispose();
            }

            // Assert
            Assert.Zero(actualStarted.Count());
        }

        [Test]
        public async Task Stop_CancelsAllRunningTasks_IfCancellationWasCalledBeforeTheirCompletition_Async()
        {
            // Arrange
            const int delayBeforeCancellation = 1000;
            var actualStarted = new List<int>();
            var actualCompleted = new List<int>();
            var task1 = new TestTask(started: actualStarted, completed: actualCompleted, order: 1, delay: 3);
            var task2 = new TestTask(started: actualStarted, completed: actualCompleted, order: 2, delay: 3);
            var task3 = new TestTask(started: actualStarted, completed: actualCompleted, order: 3, delay: 3);
            var task4 = new TestTask(started: actualStarted, completed: actualCompleted, order: 4, delay: 3);
            _taskScheduler.Initialize(parallelTaskNumber: 2);
            var cts = new CancellationTokenSource();

            // Act 
            _taskScheduler.Schedule(task1, Priority.High);
            _taskScheduler.Schedule(task2, Priority.High);
            _taskScheduler.Schedule(task3, Priority.High);
            _taskScheduler.Schedule(task4, Priority.High);
            try
            {
                var stoppingTask = _taskScheduler.Stop(cts.Token);
                await Task.Delay(delayBeforeCancellation);
                cts.Cancel();
                await stoppingTask;
            }
            catch (OperationCanceledException)
            {
                // do nothing when tasks were cancelled
            }
            finally
            {
                cts.Dispose();
            }

            // Assert
            System.Diagnostics.Debug.WriteLine($"Started: {actualStarted.Count()}");
            System.Diagnostics.Debug.WriteLine($"Completed: {actualCompleted.Count()}");
            Assert.AreEqual(2, actualStarted.Count());
            Assert.Zero(actualCompleted.Count());
        }

        [Test]
        public async Task Stop_CancelsSomeRunningTasks_IfCancellationWasCalledBeforeTheirCompletition_Async()
        {
            // Arrange
            const int delayBeforeCancellation = 4000;
            var actualStarted = new List<int>();
            var actualCompleted = new List<int>();
            var task1 = new TestTask(started: actualStarted, completed: actualCompleted, order: 1, delay: 3);
            var task2 = new TestTask(started: actualStarted, completed: actualCompleted, order: 2, delay: 3);
            var task3 = new TestTask(started: actualStarted, completed: actualCompleted, order: 3, delay: 3);
            var task4 = new TestTask(started: actualStarted, completed: actualCompleted, order: 4, delay: 3);
            var task5 = new TestTask(started: actualStarted, completed: actualCompleted, order: 5, delay: 3);
            var task6 = new TestTask(started: actualStarted, completed: actualCompleted, order: 6, delay: 3);
            _taskScheduler.Initialize(parallelTaskNumber: 2);
            var cts = new CancellationTokenSource();

            // Act 
            _taskScheduler.Schedule(task1, Priority.High);
            _taskScheduler.Schedule(task2, Priority.High);
            _taskScheduler.Schedule(task3, Priority.High);
            _taskScheduler.Schedule(task4, Priority.High);
            _taskScheduler.Schedule(task5, Priority.Normal);
            _taskScheduler.Schedule(task6, Priority.Low);
            try
            {
                var stoppingTask = _taskScheduler.Stop(cts.Token);
                await Task.Delay(delayBeforeCancellation);
                cts.Cancel();
                await stoppingTask;
            }
            catch (OperationCanceledException)
            {
                // do nothing when tasks were cancelled
            }
            finally
            {
                cts.Dispose();
            }

            // Assert
            Assert.AreEqual(4, actualStarted.Count());
            Assert.AreEqual(2, actualCompleted.Count());
        }

        [Test]
        public async Task Stop_IgnoreAllRunningTasks_IfCancellationWasCalledAfterTheirCompletition_Async()
        {
            // Arrange
            const int delayBeforeCancellation = 4000;
            var actualStarted = new List<int>();
            var actualCompleted = new List<int>();
            var task1 = new TestTask(started: actualStarted, completed: actualCompleted, order: 1, delay: 3);
            var task2 = new TestTask(started: actualStarted, completed: actualCompleted, order: 2, delay: 3);
            _taskScheduler.Initialize(parallelTaskNumber: 2);
            var cts = new CancellationTokenSource();

            // Act 
            _taskScheduler.Schedule(task1, Priority.High);
            _taskScheduler.Schedule(task2, Priority.High);
            try
            {
                var stoppingTask = _taskScheduler.Stop(cts.Token);
                await Task.Delay(delayBeforeCancellation);
                cts.Cancel();
                await stoppingTask;
            }
            catch (OperationCanceledException)
            {
                // do nothing when tasks were cancelled
            }
            finally
            {
                cts.Dispose();
            }

            // Assert
            CollectionAssert.AreEquivalent(actualStarted, actualCompleted);
        }

        private class TestTask : ITask
        {
            private readonly List<int> _started;
            private readonly List<int> _completed;
            private readonly int _order;
            private readonly int _delay;

            public TestTask(
                List<int> started = null,
                List<int> completed = null,
                int order = 0,
                int delay = 1)
            {
                _started = started ?? new List<int>();
                _completed = completed ?? new List<int>();
                _order = order;
                _delay = delay;
            }

            public Task Execute()
            {
                lock (_started) _started.Add(_order);
                // System.Diagnostics.Debug.WriteLine($"{_order} started {DateTime.Now.ToString("mm:ss:ffff")}");
                return Task.Run(() =>
                {
                    Task.Delay(_delay * 1000).Wait();
                    lock (_completed) _completed.Add(_order);
                    // System.Diagnostics.Debug.WriteLine($"{_order} finished {DateTime.Now.ToString("mm:ss:ffff")}");
                });
            }
        }
    }
}