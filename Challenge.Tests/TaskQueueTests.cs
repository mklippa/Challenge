using System;
using Challenge.Lib;
using Moq;
using NUnit.Framework;

namespace Challenge.Tests
{
    public class TaskQueueTests
    {
        private TaskQueue _taskQueue;

        [SetUp]
        public void Setup()
        {
            _taskQueue = new TaskQueue();
        }

        [Test]
        public void IsEmpty_True_IfQueueIsEmpty()
        {
            // Arrange

            // Act
            var actual = _taskQueue.IsEmpty;

            // Assert
            Assert.AreEqual(true, _taskQueue.IsEmpty);
        }

        [Test]
        public void IsEmpty_False_IfQueueIsNotEmpty()
        {
            // Arrange
            _taskQueue.Enqueue(default(Priority), null);

            // Act
            var actual = _taskQueue.IsEmpty;

            // Assert
            Assert.AreEqual(false, actual);
        }

        [Test]
        public void Enqueue_AddTaskToQueue()
        {
            // Arrange

            // Act
            _taskQueue.Enqueue(default(Priority), null);

            // Assert
            Assert.AreEqual(false, _taskQueue.IsEmpty);
        }

        [Test]
        public void Dequeue_RemoveAndReturnTaskOfQueue_IfQueueIsNotEmpty()
        {
            // Arrange
            var task = Mock.Of<ITask>();
            _taskQueue.Enqueue(default(Priority), task);

            // Act
            var actual = _taskQueue.Dequeue();

            // Assert
            Assert.AreEqual(true, _taskQueue.IsEmpty);
            Assert.AreEqual(task, actual);
        }

        [Test]
        public void Dequeue_ThrowException_IfQueueIsEmpty()
        {
            // Arrange

            // Act

            // Assert
            Assert.Throws<InvalidOperationException>(() => _taskQueue.Dequeue());
        }

        [Test]
        public void Dequeue_ReturnHighLow_IfLowHighWereEnqueued()
        {
            // Arrange
            var lowPriorityTask = Mock.Of<ITask>();
            var highPriorityTask = Mock.Of<ITask>();
            _taskQueue.Enqueue(Priority.Low, lowPriorityTask);
            _taskQueue.Enqueue(Priority.High, highPriorityTask);

            // Act
            var firstTask = _taskQueue.Dequeue();
            var secondTask = _taskQueue.Dequeue();

            // Assert
            Assert.AreEqual(highPriorityTask, firstTask);
            Assert.AreEqual(lowPriorityTask, secondTask);
        }

        [Test]
        public void Dequeue_ReturnHighNormal_IfNormalHighWereEnqueued()
        {
            // Arrange
            var normalPriorityTask = Mock.Of<ITask>();
            var highPriorityTask = Mock.Of<ITask>();
            _taskQueue.Enqueue(Priority.Normal, normalPriorityTask);
            _taskQueue.Enqueue(Priority.High, highPriorityTask);

            // Act
            var firstTask = _taskQueue.Dequeue();
            var secondTask = _taskQueue.Dequeue();

            // Assert
            Assert.AreEqual(highPriorityTask, firstTask);
            Assert.AreEqual(normalPriorityTask, secondTask);
        }

        [Test]
        public void Dequeue_ReturnNormalLow_IfLowNormalWereEnqueued()
        {
            // Arrange
            var normalPriorityTask = Mock.Of<ITask>();
            var lowPriorityTask = Mock.Of<ITask>();
            _taskQueue.Enqueue(Priority.Low, lowPriorityTask);
            _taskQueue.Enqueue(Priority.Normal, normalPriorityTask);

            // Act
            var firstTask = _taskQueue.Dequeue();
            var secondTask = _taskQueue.Dequeue();

            // Assert
            Assert.AreEqual(normalPriorityTask, firstTask);
            Assert.AreEqual(lowPriorityTask, secondTask);
        }

        [Test]
        public void Dequeue_ReturnHighLow_IfHighLowWereEnqueued()
        {
            // Arrange
            var highPriorityTask = Mock.Of<ITask>();
            var lowPriorityTask = Mock.Of<ITask>();
            _taskQueue.Enqueue(Priority.High, highPriorityTask);
            _taskQueue.Enqueue(Priority.Low, lowPriorityTask);

            // Act
            var firstTask = _taskQueue.Dequeue();
            var secondTask = _taskQueue.Dequeue();

            // Assert
            Assert.AreEqual(highPriorityTask, firstTask);
            Assert.AreEqual(lowPriorityTask, secondTask);
        }

        [Test]
        public void Dequeue_ReturnNormalLow_IfNormalLowWereEnqueued()
        {
            // Arrange
            var normalPriorityTask = Mock.Of<ITask>();
            var lowPriorityTask = Mock.Of<ITask>();
            _taskQueue.Enqueue(Priority.Normal, normalPriorityTask);
            _taskQueue.Enqueue(Priority.Low, lowPriorityTask);

            // Act
            var firstTask = _taskQueue.Dequeue();
            var secondTask = _taskQueue.Dequeue();

            // Assert
            Assert.AreEqual(normalPriorityTask, firstTask);
            Assert.AreEqual(lowPriorityTask, secondTask);
        }

        [Test]
        public void Dequeue_ReturnHighNormal_IfHighNormalWereEnqueued()
        {
            // Arrange
            var highPriorityTask = Mock.Of<ITask>();
            var normalPriorityTask = Mock.Of<ITask>();
            _taskQueue.Enqueue(Priority.High, highPriorityTask);
            _taskQueue.Enqueue(Priority.Normal, normalPriorityTask);

            // Act
            var firstTask = _taskQueue.Dequeue();
            var secondTask = _taskQueue.Dequeue();

            // Assert
            Assert.AreEqual(highPriorityTask, firstTask);
            Assert.AreEqual(normalPriorityTask, secondTask);
        }

        [Test]
        public void Dequeue_ReturnHighNormalLow_IfLowHighNormalWereEnqueued()
        {
            // Arrange
            var lowPriorityTask = Mock.Of<ITask>();
            var highPriorityTask = Mock.Of<ITask>();
            var normalPriorityTask = Mock.Of<ITask>();
            _taskQueue.Enqueue(Priority.Low, lowPriorityTask);
            _taskQueue.Enqueue(Priority.High, highPriorityTask);
            _taskQueue.Enqueue(Priority.Normal, normalPriorityTask);

            // Act
            var firstTask = _taskQueue.Dequeue();
            var secondTask = _taskQueue.Dequeue();
            var thirdTask = _taskQueue.Dequeue();

            // Assert
            Assert.AreEqual(highPriorityTask, firstTask);
            Assert.AreEqual(normalPriorityTask, secondTask);
            Assert.AreEqual(lowPriorityTask, thirdTask);
        }

        [Test]
        public void Dequeue_ReturnHighHighHighHighHigh_IfHighHighHighHighHighWereEnqueued()
        {
            // Arrange
            var highPriorityTask1 = Mock.Of<ITask>();
            var highPriorityTask2 = Mock.Of<ITask>();
            var highPriorityTask3 = Mock.Of<ITask>();
            var highPriorityTask4 = Mock.Of<ITask>();
            var highPriorityTask5 = Mock.Of<ITask>();
            _taskQueue.Enqueue(Priority.High, highPriorityTask1);
            _taskQueue.Enqueue(Priority.High, highPriorityTask2);
            _taskQueue.Enqueue(Priority.High, highPriorityTask3);
            _taskQueue.Enqueue(Priority.High, highPriorityTask4);
            _taskQueue.Enqueue(Priority.High, highPriorityTask5);

            // Act
            var task1 = _taskQueue.Dequeue();
            var task2 = _taskQueue.Dequeue();
            var task3 = _taskQueue.Dequeue();
            var task4 = _taskQueue.Dequeue();
            var task5 = _taskQueue.Dequeue();

            // Assert
            Assert.AreEqual(highPriorityTask1, task1);
            Assert.AreEqual(highPriorityTask2, task2);
            Assert.AreEqual(highPriorityTask3, task3);
            Assert.AreEqual(highPriorityTask4, task4);
            Assert.AreEqual(highPriorityTask5, task5);
        }
    }
}