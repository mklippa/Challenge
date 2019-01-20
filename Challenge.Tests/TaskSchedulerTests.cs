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
        // complete all scheduled tasks in random order  when there are more parallel tasks
        public async Task TestAsync()
        {
            // Arrange
            var expected = new[] { 1, 2, 3 };

            var actual = new List<int>();
            var task1 = new TestTask(started: actual, order: 1);
            var task2 = new TestTask(started: actual, order: 2);
            var task3 = new TestTask(started: actual, order: 3);

            _taskScheduler.Initialize(3);

            // Act
            _taskScheduler.Schedule(task1, Priority.Low);
            _taskScheduler.Schedule(task2, Priority.Normal);
            _taskScheduler.Schedule(task3, Priority.High);
            await _taskScheduler.Stop(CancellationToken.None);

            // Assert
            CollectionAssert.AreEquivalent(expected, actual);
        }

        // 1 x ||, 5 tasks, first - long, 2-5 correct order
        [Test]
        public async Task Test2()
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
        // 5 x ||, 5 tasks, first - long (not necessary), 1-5 random order, but devided in half
        [Test]
        public async Task Test3()
        {
            // Arrange
            var expectedFirstPortion = new[] { 1, 2, 3, 4 };
            var expectedSecondPortion = new[] { 5, 6, 7, 8 };

            var actual = new List<int>();
            // the first portion
            var task1 = new TestTask(started: actual, order: 1);
            var task2 = new TestTask(started: actual, order: 2);
            var task3 = new TestTask(started: actual, order: 3);
            var task4 = new TestTask(started: actual, order: 4);
            // the second portion
            var task5 = new TestTask(started: actual, order: 5);
            var task6 = new TestTask(started: actual, order: 6);
            var task7 = new TestTask(started: actual, order: 7);
            var task8 = new TestTask(started: actual, order: 8);
            // parallelTaskNumber == portion size
            _taskScheduler.Initialize(4);

            // Act
            // schedule the first portion
            _taskScheduler.Schedule(task1, Priority.High);
            _taskScheduler.Schedule(task2, Priority.High);
            _taskScheduler.Schedule(task3, Priority.Normal);
            _taskScheduler.Schedule(task4, Priority.High);
            // schdule the second portion
            _taskScheduler.Schedule(task5, Priority.High);
            _taskScheduler.Schedule(task6, Priority.Low);
            _taskScheduler.Schedule(task7, Priority.Normal);
            _taskScheduler.Schedule(task8, Priority.High);
            await _taskScheduler.Stop(CancellationToken.None);

            // Assert
            CollectionAssert.AreEquivalent(expectedFirstPortion, actual.Take(4));
            CollectionAssert.AreEquivalent(expectedSecondPortion, actual.Skip(4));
        } 

        // cancell - отменили сразу после запуска
        [Test]
        public async Task TestF00()
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
            try {
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

        // cancell - успели несколько запустить, но не успули ничего закончить
        [Test]
        public async Task TestBar()
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
            try {
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
            Assert.AreEqual(2, actualStarted.Count());
            Assert.Zero(actualCompleted.Count());
        }

        // cancell - успели несколько запустить и успели чуть меньше закончить
        [Test]
        public async Task Test4()
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
            try {
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

        // cancell - успели все запустить и все закончили
        [Test]
        public async Task TestBux()
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
            try {
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
    }
}