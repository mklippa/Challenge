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
        public async Task TestAsync()
        {
            // Arrange
            var expected = new[] { 1, 2, 3 };

            var task1 = new Mock<ITask>();
            var task2 = new Mock<ITask>();
            var task3 = new Mock<ITask>();
            var actual = new List<int>();
            task1.Setup(t => t.Execute()).Returns(Task.Run(() => { Thread.Sleep(1000); actual.Add(1); }));
            task2.Setup(t => t.Execute()).Returns(Task.Run(() => { Thread.Sleep(1000); actual.Add(2); }));
            task3.Setup(t => t.Execute()).Returns(Task.Run(() => { Thread.Sleep(1000); actual.Add(3); }));

            _taskScheduler.Initialize(3);

            // Act
            _taskScheduler.Schedule(task1.Object, Priority.Low);
            _taskScheduler.Schedule(task2.Object, Priority.Normal);
            _taskScheduler.Schedule(task3.Object, Priority.High);
            await _taskScheduler.Stop(CancellationToken.None);

            // Assert
            CollectionAssert.AreEquivalent(expected, actual);

        }
    }
}