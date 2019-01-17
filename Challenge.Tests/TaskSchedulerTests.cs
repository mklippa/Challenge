using System;
using System.Collections.Generic;
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

            var task1 = new Mock<ITask>();
            var task2 = new Mock<ITask>();
            var task3 = new Mock<ITask>();
            var actual = new List<int>();
            task1.Setup(t => t.Execute()).Returns(Task.Run(() => { Thread.Sleep(1000); lock(actual) actual.Add(1); }));
            task2.Setup(t => t.Execute()).Returns(Task.Run(() => { Thread.Sleep(1000); lock(actual) actual.Add(2); }));
            task3.Setup(t => t.Execute()).Returns(Task.Run(() => { Thread.Sleep(1000); lock(actual) actual.Add(3); }));

            _taskScheduler.Initialize(3);

            // Act
            _taskScheduler.Schedule(task1.Object, Priority.Low);
            _taskScheduler.Schedule(task2.Object, Priority.Normal);
            _taskScheduler.Schedule(task3.Object, Priority.High);
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

            var task1 = new Mock<ITask>();
            var task2 = new Mock<ITask>();
            var task3 = new Mock<ITask>();
            var task4 = new Mock<ITask>();
            var task5 = new Mock<ITask>();
            var task6 = new Mock<ITask>();
            var task7 = new Mock<ITask>();
            var task8 = new Mock<ITask>();
            var actual = new List<int>();
            task1.Setup(t => t.Execute()).Returns(Task.Run(() => { lock(actual) actual.Add(1); Thread.Sleep(1000); }));
            task2.Setup(t => t.Execute()).Returns(Task.Run(() => { lock(actual) actual.Add(2); Thread.Sleep(1000); }));
            task3.Setup(t => t.Execute()).Returns(Task.Run(() => { lock(actual) actual.Add(3); Thread.Sleep(1000); }));
            task4.Setup(t => t.Execute()).Returns(Task.Run(() => { lock(actual) actual.Add(4); Thread.Sleep(1000); }));
            task5.Setup(t => t.Execute()).Returns(Task.Run(() => { lock(actual) actual.Add(5); Thread.Sleep(1000); }));
            task6.Setup(t => t.Execute()).Returns(Task.Run(() => { lock(actual) actual.Add(6); Thread.Sleep(1000); }));
            task7.Setup(t => t.Execute()).Returns(Task.Run(() => { lock(actual) actual.Add(7); Thread.Sleep(1000); }));
            task8.Setup(t => t.Execute()).Returns(Task.Run(() => { lock(actual) actual.Add(8); Thread.Sleep(1000); }));
            _taskScheduler.Initialize(1);

            // Act 
            _taskScheduler.Schedule(task1.Object, Priority.High);
            _taskScheduler.Schedule(task2.Object, Priority.High);
            _taskScheduler.Schedule(task3.Object, Priority.High);
            _taskScheduler.Schedule(task4.Object, Priority.High);
            _taskScheduler.Schedule(task5.Object, Priority.Normal);
            _taskScheduler.Schedule(task6.Object, Priority.Low);
            _taskScheduler.Schedule(task7.Object, Priority.Normal);
            _taskScheduler.Schedule(task8.Object, Priority.High);
            await _taskScheduler.Stop(CancellationToken.None);

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }
        // 5 x ||, 5 tasks, first - long (not necessary), 1-5 random order

        // cancell - выполнятся не все
    }
}