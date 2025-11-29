using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartTaskScheduler.Library.Core.Models;
using SmartTaskScheduler.Library.Core.Services;
using SmartTaskScheduler.Library.Core.Verification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace SmartTaskScheduler.Tests
{
    [TestClass]
    public class TaskSchedulerUnitTests
    {
        // ===== ТЕСТЫ МОДЕЛЕЙ =====
        [TestClass]
        public class TaskItemTests
        {
            [TestMethod]
            public void TaskItem_IsOverdue_WhenDeadlineInPast_ReturnsTrue()
            {
                // Arrange
                var task = new TaskItem { Deadline = DateTime.Now.AddHours(-1) };

                // Act & Assert
                Assert.IsTrue(task.IsOverdue);
            }

            [TestMethod]
            public void TaskItem_IsOverdue_WhenDeadlineInFuture_ReturnsFalse()
            {
                // Arrange
                var task = new TaskItem { Deadline = DateTime.Now.AddHours(1) };

                // Act & Assert
                Assert.IsFalse(task.IsOverdue);
            }

            [TestMethod]
            public void TaskItem_IsUrgent_WhenHighPriorityAndNearDeadline_ReturnsTrue()
            {
                // Arrange
                var task = new TaskItem
                {
                    Priority = 1,
                    Deadline = DateTime.Now.AddHours(12)
                };

                // Act & Assert
                Assert.IsTrue(task.IsUrgent);
            }

            [TestMethod]
            public void TaskItem_IsUrgent_WhenLowPriority_ReturnsFalse()
            {
                // Arrange
                var task = new TaskItem
                {
                    Priority = 3,
                    Deadline = DateTime.Now.AddHours(12)
                };

                // Act & Assert
                Assert.IsFalse(task.IsUrgent);
            }

            [TestMethod]
            public void TaskItem_Properties_SetCorrectly()
            {
                // Arrange & Act
                var task = new TaskItem
                {
                    Title = "Test",
                    Description = "Description",
                    Priority = 2,
                    Deadline = new DateTime(2024, 1, 1),
                    IsCompleted = true
                };

                // Assert
                Assert.AreEqual("Test", task.Title);
                Assert.AreEqual("Description", task.Description);
                Assert.AreEqual(2, task.Priority);
                Assert.AreEqual(new DateTime(2024, 1, 1), task.Deadline);
                Assert.IsTrue(task.IsCompleted);
            }
        }

        // ===== ТЕСТЫ СЕРВИСА =====
        [TestClass]
        public class TaskSchedulerServiceTests
        {
            private TaskSchedulerService _service;

            [TestInitialize]
            public void Setup()
            {
                // Используем временный файл для тестов
                var tempFilePath = Path.GetTempFileName();
                _service = CreateTestService(tempFilePath);
            }

            private TaskSchedulerService CreateTestService(string filePath)
            {
                // Создаем сервис с временным файлом через рефлексию
                var service = new TaskSchedulerService();
                var dataServiceField = typeof(TaskSchedulerService).GetField("_dataService",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                if (dataServiceField != null)
                {
                    var testDataService = new TestTaskDataService(filePath);
                    dataServiceField.SetValue(service, testDataService);
                }

                // Очищаем задачи
                ClearAllTasks(service);
                return service;
            }

            [TestMethod]
            public void AddTask_WithValidTask_AddsToCollection()
            {
                // Arrange
                var initialCount = _service.Tasks.Count;
                var task = CreateValidTask();

                // Act
                var result = _service.AddTask(task);

                // Assert
                Assert.IsTrue(result);
                Assert.AreEqual(initialCount + 1, _service.Tasks.Count);
                Assert.IsTrue(_service.Tasks.Contains(task));
            }

            [TestMethod]
            public void AddTask_WithDuplicateTask_AddsBothTasks()
            {
                // Arrange
                var initialCount = _service.Tasks.Count;
                var task1 = CreateValidTask();
                var task2 = CreateValidTask();

                // Act
                _service.AddTask(task1);
                _service.AddTask(task2);

                // Assert
                Assert.AreEqual(initialCount + 2, _service.Tasks.Count);
            }

            [TestMethod]
            public void Tasks_Property_ReturnsReadOnlyCollection()
            {
                // Act
                var tasks = _service.Tasks;

                // Assert
                Assert.IsInstanceOfType(tasks, typeof(IReadOnlyList<TaskItem>));
            }

            [TestMethod]
            public void FilterTasks_WithNoFilters_ReturnsAllTasks()
            {
                // Arrange
                var initialCount = _service.Tasks.Count;
                var task1 = CreateValidTask();
                var task2 = CreateValidTask();
                _service.AddTask(task1);
                _service.AddTask(task2);

                // Act
                var result = _service.FilterTasks("", false, false);

                // Assert
                Assert.AreEqual(initialCount + 2, result.FilteredTasks.Count);
            }
        }

        // ===== ТЕСТЫ ФИЛЬТРАЦИИ =====
        [TestClass]
        public class FilteringTests
        {
            private TaskSchedulerService _service;

            [TestInitialize]
            public void Setup()
            {
                // Используем временный файл для тестов
                var tempFilePath = Path.GetTempFileName();
                _service = CreateTestService(tempFilePath);
            }

            private TaskSchedulerService CreateTestService(string filePath)
            {
                var service = new TaskSchedulerService();
                var dataServiceField = typeof(TaskSchedulerService).GetField("_dataService",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                if (dataServiceField != null)
                {
                    var testDataService = new TestTaskDataService(filePath);
                    dataServiceField.SetValue(service, testDataService);
                }

                ClearAllTasks(service);
                return service;
            }

            [TestMethod]
            public void FilterByDeadline_ReturnsOnlyOverdueTasks()
            {
                // Arrange
                var overdue = new TaskItem
                {
                    Deadline = DateTime.Now.AddDays(-1),
                    Priority = 2,
                    IsCompleted = false
                };
                var future = new TaskItem
                {
                    Deadline = DateTime.Now.AddDays(1),
                    Priority = 2
                };
                _service.AddTask(overdue);
                _service.AddTask(future);

                // Act
                var result = _service.FilterTasks("", byDeadline: true, byPriority: false);

                // Assert
                Assert.AreEqual(1, result.FilteredTasks.Count);
                Assert.IsTrue(result.FilteredTasks.Any(t => t.IsOverdue || t.IsUrgent));
            }

            [TestMethod]
            public void FilterByPriority_ReturnsOnlyHighPriorityTasks()
            {
                // Arrange
                var highPriority = new TaskItem
                {
                    Deadline = DateTime.Now.AddDays(1),
                    Priority = 1,
                    IsCompleted = false
                };
                var lowPriority = new TaskItem
                {
                    Deadline = DateTime.Now.AddDays(1),
                    Priority = 3
                };
                _service.AddTask(highPriority);
                _service.AddTask(lowPriority);

                // Act - используем фильтрацию по конкретному приоритету
                var result = _service.FilterTasks(
                    customFilter: "",
                    byDeadline: false,
                    byPriority: true,
                    priorityFilter: "1");

                // Assert
                Assert.AreEqual(1, result.FilteredTasks.Count);
                Assert.IsTrue(result.FilteredTasks.Any(t => t.Priority == 1));
            }

            [TestMethod]
            public void FilterByPriority_WithGreaterThanCondition_ReturnsMatchingTasks()
            {
                // Arrange
                var highPriority = new TaskItem
                {
                    Deadline = DateTime.Now.AddDays(1),
                    Priority = 3
                };
                var lowPriority = new TaskItem
                {
                    Deadline = DateTime.Now.AddDays(1),
                    Priority = 1
                };
                _service.AddTask(highPriority);
                _service.AddTask(lowPriority);

                // Act - фильтрация по приоритету > 2
                var result = _service.FilterTasks(
                    customFilter: "",
                    byDeadline: false,
                    byPriority: true,
                    priorityFilter: ">2");

                // Assert
                Assert.AreEqual(1, result.FilteredTasks.Count);
                Assert.IsTrue(result.FilteredTasks.Any(t => t.Priority > 2));
            }

            [TestMethod]
            public void FilterWithCustomCondition_ReturnsMatchingTasks()
            {
                // Arrange
                var urgentTask = new TaskItem
                {
                    Title = "Urgent",
                    Deadline = DateTime.Now.AddHours(10),
                    Priority = 1
                };
                var normalTask = new TaskItem
                {
                    Title = "Normal",
                    Deadline = DateTime.Now.AddDays(5),
                    Priority = 3
                };
                _service.AddTask(urgentTask);
                _service.AddTask(normalTask);

                // Act
                var result = _service.FilterTasks("Priority == 1", false, false);

                // Assert
                Assert.AreEqual(1, result.FilteredTasks.Count);
                Assert.IsTrue(result.FilteredTasks.Contains(urgentTask));
                Assert.IsFalse(result.FilteredTasks.Contains(normalTask));
            }

            [TestMethod]
            public void MultipleFilterCalls_DoNotInterfere()
            {
                // Arrange
                var task1 = new TaskItem
                {
                    Deadline = DateTime.Now.AddDays(-1), // Просроченная
                    Priority = 1,
                    IsCompleted = false
                };
                var task2 = new TaskItem
                {
                    Deadline = DateTime.Now.AddDays(1), // Не просроченная
                    Priority = 3
                };
                _service.AddTask(task1);
                _service.AddTask(task2);

                // Act
                var result1 = _service.FilterTasks("", true, false); // Только просроченные
                var result2 = _service.FilterTasks("", false, true, priorityFilter: "1"); // Только приоритет 1

                // Assert
                Assert.AreEqual(1, result1.FilteredTasks.Count); // Только task1 (просроченная)
                Assert.AreEqual(1, result2.FilteredTasks.Count); // Только task1 (приоритет 1)
            }

            [TestMethod]
            public void FilterTasks_WithEmptyService_ReturnsEmptyList()
            {
                // Act
                var result = _service.FilterTasks("", true, true);

                // Assert
                Assert.AreEqual(0, result.FilteredTasks.Count);
            }
        }

        // ===== ТЕСТЫ ВЕРИФИКАЦИИ =====
        [TestClass]
        public class VerificationTests
        {
            private TaskVerificationEngine _verifier;

            [TestInitialize]
            public void Setup()
            {
                _verifier = new TaskVerificationEngine();
            }

            [TestMethod]
            public void VerifyFilterOperation_WithEmptyTasks_ReturnsValidResult()
            {
                // Arrange
                var emptyTasks = Array.Empty<TaskItem>();

                // Act
                var result = _verifier.VerifyFilterOperation("Priority == 1", emptyTasks, true, false);

                // Assert
                Assert.IsNotNull(result);
                Assert.IsFalse(string.IsNullOrEmpty(result.BooleanFunction));
                Assert.IsFalse(string.IsNullOrEmpty(result.Invariant));
                Assert.IsFalse(string.IsNullOrEmpty(result.WpResult));
                Assert.IsTrue(result.WpCalculation.Count > 0);
            }

            [TestMethod]
            public void VerifyFilterOperation_WithNullFilter_HandlesCorrectly()
            {
                // Arrange
                var tasks = new[] { CreateValidTask() };

                // Act
                var result = _verifier.VerifyFilterOperation(null, tasks, false, true);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual("Без кастомного фильтра", result.BooleanFunction);
            }

            [TestMethod]
            public void VerifyFilterOperation_WithComplexFilter_ParsesCorrectly()
            {
                // Arrange
                var tasks = new[] { CreateValidTask() };
                var complexFilter = "Priority == 1 & IsOverdue == true";

                // Act
                var result = _verifier.VerifyFilterOperation(complexFilter, tasks, true, true);

                // Assert
                Assert.IsTrue(result.BooleanFunction.Contains("∧")); // Должен заменить & на ∧
            }

            [TestMethod]
            public void VerificationResult_DefaultValues_AreCorrect()
            {
                // Arrange & Act
                var result = new VerificationResult();

                // Assert
                Assert.AreEqual(string.Empty, result.BooleanFunction);
                Assert.AreEqual(string.Empty, result.Invariant);
                Assert.AreEqual(string.Empty, result.WpResult);
                Assert.IsNotNull(result.WpCalculation);
                Assert.AreEqual(0, result.WpCalculation.Count);
            }
        }

        // ===== ТЕСТЫ ГРАНИЧНЫХ СЛУЧАЕВ =====
        [TestClass]
        public class EdgeCaseTests
        {
            [TestMethod]
            public void TaskItem_WithMinimalData_WorksCorrectly()
            {
                // Arrange & Act
                var task = new TaskItem
                {
                    Title = "Minimal",
                    Deadline = DateTime.MinValue,
                    Priority = 1
                };

                // Assert
                Assert.IsTrue(task.IsOverdue); // MinValue всегда в прошлом
            }

            [TestMethod]
            public void FilterTasks_WithAllFiltersFalse_ReturnsAllTasks()
            {
                // Arrange
                var service = CreateTestService(Path.GetTempFileName());
                var task = CreateValidTask();
                service.AddTask(task);

                // Act
                var result = service.FilterTasks("", false, false);

                // Assert
                Assert.AreEqual(1, result.FilteredTasks.Count);
            }
        }

        // ===== ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ И КЛАССЫ =====
        private static TaskItem CreateValidTask()
        {
            return new TaskItem
            {
                Title = "Valid Task",
                Description = "Valid Description",
                Deadline = DateTime.Now.AddDays(1),
                Priority = 2
            };
        }

        // Метод для очистки всех задач из сервиса
        private static void ClearAllTasks(TaskSchedulerService service)
        {
            var tasksField = typeof(TaskSchedulerService).GetField("_tasks",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (tasksField != null)
            {
                var tasks = tasksField.GetValue(service) as List<TaskItem>;
                tasks?.Clear();
            }
        }

        // Создание тестового сервиса
        private static TaskSchedulerService CreateTestService(string filePath)
        {
            var service = new TaskSchedulerService();
            var dataServiceField = typeof(TaskSchedulerService).GetField("_dataService",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (dataServiceField != null)
            {
                var testDataService = new TestTaskDataService(filePath);
                dataServiceField.SetValue(service, testDataService);
            }

            ClearAllTasks(service);
            return service;
        }

        // Тестовый DataService, который использует временный файл
        public class TestTaskDataService : TaskDataService
        {
            public TestTaskDataService(string filePath) : base(filePath)
            {
            }
        }
    }
}