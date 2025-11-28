using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartTaskScheduler.Library.Core.Models;
using SmartTaskScheduler.Library.Core.Services;
using SmartTaskScheduler.Library.Core.Verification;
using WpfTaskScheduler.ViewModels;
using System;
using System.Linq;

namespace SmartTaskScheduler.Tests
{
    [TestClass]
    public class IncorrectViewModelTests
    {
        // ===== НЕПРАВИЛЬНЫЕ ТЕСТЫ ДЛЯ ViewModel =====
        [TestClass]
        public class IncorrectMainViewModelTests
        {
            [TestMethod]
            public void MainViewModel_AddTask_WithNullTitle_Succeeds() // НЕПРАВИЛЬНО!
            {
                // Arrange
                var vm = new MainViewModel();
                vm.NewTaskTitle = null; // Null название

                // Act & Assert - ЗАВЕДОМО НЕВЕРНОЕ УТВЕРЖДЕНИЕ
                vm.AddNewTask(); // Должен был сработать Validation
                Assert.AreEqual(1, vm.AllTasks.Count); // Но мы утверждаем что задача не добавилась
                // НА САМОМ ДЕЛЕ: задача не должна добавляться из-за валидации в AddNewTask()
            }

            [TestMethod]
            public void MainViewModel_AddTask_WithPastDeadline_Succeeds() // НЕПРАВИЛЬНО!
            {
                // Arrange
                var vm = new MainViewModel();
                vm.NewTaskTitle = "Test";
                vm.NewTaskDeadline = DateTime.Now.AddDays(-1); // Прошедшая дата

                // Act & Assert - ЗАВЕДОМО НЕВЕРНОЕ УТВЕРЖДЕНИЕ
                vm.AddNewTask(); // Должен был сработать Validation
                Assert.AreEqual(1, vm.AllTasks.Count); // Но мы утверждаем что задача добавилась
                // НА САМОМ ДЕЛЕ: задача не должна добавляться из-за валидации в AddNewTask()
            }

            [TestMethod]
            public void MainViewModel_AddTask_WithInvalidPriority_Succeeds() // НЕПРАВИЛЬНО!
            {
                // Arrange
                var vm = new MainViewModel();
                vm.NewTaskTitle = "Test";
                vm.NewTaskPriority = 999; // Невалидный приоритет

                // Act & Assert - ЗАВЕДОМО НЕВЕРНОЕ УТВЕРЖДЕНИЕ
                vm.AddNewTask(); // Должен был сработать Validation
                Assert.AreEqual(1, vm.AllTasks.Count); // Но мы утверждаем что задача добавилась
                // НА САМОМ ДЕЛЕ: задача не должна добавляться из-за валидации в AddNewTask()
            }
        }

        // ===== НЕПРАВИЛЬНЫЕ ТЕСТЫ ДЛЯ ВАЛИДАЦИИ =====
        [TestClass]
        public class IncorrectValidationTests
        {
            [TestMethod]
            public void MainViewModel_ValidateNewTask_AcceptsEmptyTitle() // НЕПРАВИЛЬНО!
            {
                // Arrange
                var vm = new MainViewModel();
                vm.NewTaskTitle = "";

                // Act & Assert - ЗАВЕДОМО НЕВЕРНОЕ УТВЕРЖДЕНИЕ
                Assert.IsTrue(vm.ValidateNewTask()); // Утверждаем что пустое название проходит валидацию
                // НА САМОМ ДЕЛЕ: ValidateNewTask() должен вернуть false для пустого названия
            }

            [TestMethod]
            public void MainViewModel_ValidateNewTask_AcceptsPastDeadline() // НЕПРАВИЛЬНО!
            {
                // Arrange
                var vm = new MainViewModel();
                vm.NewTaskTitle = "Test";
                vm.NewTaskDeadline = DateTime.Now.AddHours(-1); // Прошедшее время

                // Act & Assert - ЗАВЕДОМО НЕВЕРНОЕ УТВЕРЖДЕНИЕ
                Assert.IsTrue(vm.ValidateNewTask()); // Утверждаем что прошедший дедлайн проходит валидацию
                // НА САМОМ ДЕЛЕ: ValidateNewTask() должен вернуть false для прошедшего дедлайна
            }

            [TestMethod]
            public void MainViewModel_ValidateNewTask_AcceptsInvalidPriority() // НЕПРАВИЛЬНО!
            {
                // Arrange
                var vm = new MainViewModel();
                vm.NewTaskTitle = "Test";
                vm.NewTaskPriority = 0; // Невалидный приоритет

                // Act & Assert - ЗАВЕДОМО НЕВЕРНОЕ УТВЕРЖДЕНИЕ
                Assert.IsTrue(vm.ValidateNewTask()); // Утверждаем что невалидный приоритет проходит валидацию
                // НА САМОМ ДЕЛЕ: ValidateNewTask() должен вернуть false для приоритета вне диапазона 1-4
            }
        }

        // ===== НЕПРАВИЛЬНЫЕ ТЕСТЫ ДЛЯ СВОЙСТВ =====
        [TestClass]
        public class IncorrectPropertyTests
        {
            [TestMethod]
            public void MainViewModel_ErrorMessage_IsNullAfterSuccessfulOperation() // НЕПРАВИЛЬНО!
            {
                // Arrange
                var vm = new MainViewModel();
                vm.NewTaskTitle = "Valid Task";
                vm.NewTaskDeadline = DateTime.Now.AddDays(1);
                vm.NewTaskPriority = 2;

                // Act
                vm.AddNewTask(); // Успешное добавление

                // Assert - ЗАВЕДОМО НЕВЕРНОЕ УТВЕРЖДЕНИЕ
                Assert.IsNull(vm.ErrorMessage); // Утверждаем что ErrorMessage становится null
                // НА САМОМ ДЕЛЕ: после успешного добавления ErrorMessage = "Задача успешно добавлена!"
            }

            [TestMethod]
            public void MainViewModel_NewTaskProperties_KeepInvalidValues() // НЕПРАВИЛЬНО!
            {
                // Arrange
                var vm = new MainViewModel();
                vm.NewTaskTitle = "Test";
                vm.NewTaskDeadline = DateTime.Now.AddDays(-1); // Невалидное значение

                // Act
                var isValid = vm.ValidateNewTask(); // Должно вернуть false

                // Assert - ЗАВЕДОМО НЕВЕРНОЕ УТВЕРЖДЕНИЕ
                Assert.AreEqual(DateTime.Now.AddDays(-1), vm.NewTaskDeadline); // Утверждаем что невалидное значение сохраняется
                // НА САМОМ ДЕЛЕ: невалидные значения могут быть сброшены или обработаны
            }
        }
    }
}