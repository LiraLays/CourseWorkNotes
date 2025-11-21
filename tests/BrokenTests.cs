using Lab1_TaskScheduler.Models;
using Lab1_TaskScheduler.Services;
using System;
using Xunit;

namespace Lab1_TaskScheduler.Tests
{
	public class BrokenTests
	{
		[Fact]
		public void AddTask_WithPastDeadline_ShouldSuccess_ButActuallyFails()
		{
			// Arrange
			var service = new TaskSchedulerService();
			var invalidTask = new TaskItem
			{
				Title = "Задача с прошедшим дедлайном",
				Description = "Эта задача должна вызвать ошибку, но тест ожидает успех",
				Deadline = DateTime.Now.AddDays(-1), // Дедлайн в прошлом - НЕВАЛИДНО!
				Priority = 3
			};

			// Act
			var result = service.AddTask(invalidTask); // Этот вызов ДОЛЖЕН выбросить исключение!

			// Assert
			// НЕПРАВИЛЬНОЕ утверждение - мы ожидаем успех, но на самом деле должна быть ошибка
			Assert.True(result); // ЭТО УТВЕРЖДЕНИЕ НЕВЕРНОЕ!
			Assert.Contains(invalidTask, service.Tasks); // И ЭТО ТОЖЕ НЕВЕРНОЕ!

			// Правильный тест должен был бы выглядеть так:
			// var exception = Assert.Throws<ArgumentException>(() => service.AddTask(invalidTask));
			// Assert.Contains("Дедлайн должен быть в будущем", exception.Message);
		}

		[Fact]
		public void MoveTask_ToYesterday_ShouldWork_ButActuallyFails()
		{
			// Arrange
			var service = new TaskSchedulerService();
			var task = new TaskItem
			{
				Title = "Тестовая задача",
				Deadline = DateTime.Now.AddDays(2),
				Priority = 3
			};

			service.AddTask(task);
			var yesterday = DateTime.Now.AddDays(-1); // Вчера - НЕВАЛИДНАЯ дата!

			// Act
			var result = service.MoveTask(task, yesterday); // ДОЛЖЕН выбросить исключение!

			// Assert
			// НЕПРАВИЛЬНОЕ утверждение - мы ожидаем успех
			Assert.True(result); // ЭТО НЕПРАВИЛЬНО!
			Assert.Equal(yesterday, task.Deadline); // И ЭТО НЕПРАВИЛЬНО!

			// Правильный тест:
			// var exception = Assert.Throws<ArgumentException>(() => service.MoveTask(task, yesterday));
			// Assert.Contains("Новый дедлайн должен быть в будущем", exception.Message);
		}

		[Fact]
		public void FilterTasks_WithNoCriteria_ShouldReturnAllTasks_ButActuallyFails()
		{
			// Arrange
			var service = new TaskSchedulerService();
			var task1 = new TaskItem { Title = "Задача 1", Deadline = DateTime.Now.AddDays(1), Priority = 2 };
			var task2 = new TaskItem { Title = "Задача 2", Deadline = DateTime.Now.AddDays(2), Priority = 4 };

			service.AddTask(task1);
			service.AddTask(task2);

			// Act & Assert
			// НЕПРАВИЛЬНО: вызываем фильтрацию без критериев, что должно вызвать исключение
			var result = service.FilterTasks(byDeadline: false, byPriority: false); // ДОЛЖЕН БЫТЬ EXCEPTION!

			// Неправильные утверждения:
			Assert.Equal(2, result.Count); // ЭТО НЕПРАВИЛЬНО!
			Assert.Contains(task1, result); // ЭТО НЕПРАВИЛЬНО!
			Assert.Contains(task2, result); // ЭТО НЕПРАВИЛЬНО!

			// Правильный тест:
			// var exception = Assert.Throws<ArgumentException>(() => service.FilterTasks(false, false));
			// Assert.Contains("Должен быть выбран хотя бы один критерий фильтрации", exception.Message);
		}

		[Fact]
		public void TaskItem_WithPriorityZero_ShouldBeValid_ButActuallyFails()
		{
			// Arrange
			var task = new TaskItem
			{
				Title = "Задача с нулевым приоритетом",
				Deadline = DateTime.Now.AddDays(1),
				Priority = 0 // НЕВАЛИДНЫЙ приоритет!
			};

			var service = new TaskSchedulerService();

			// Act & Assert
			// НЕПРАВИЛЬНО: пытаемся добавить задачу с приоритетом 0
			var result = service.AddTask(task); // ДОЛЖЕН БЫТЬ EXCEPTION!

			Assert.True(result); // ЭТО НЕПРАВИЛЬНО!
			Assert.Equal(0, task.Priority); // ЭТО НЕПРАВИЛЬНО!

			// Правильный тест:
			// var exception = Assert.Throws<ArgumentException>(() => service.AddTask(task));
			// Assert.Contains("Приоритет должен быть от 1 до 5", exception.Message);
		}

		[Fact]
		public void AddNullTask_ShouldWork_ButActuallyFails()
		{
			// Arrange
			var service = new TaskSchedulerService();
			TaskItem nullTask = null!; // Явно передаем null - ОЧЕВИДНАЯ ОШИБКА!

			// Act & Assert
			// НЕПРАВИЛЬНО: пытаемся добавить null
			var result = service.AddTask(nullTask); // ДОЛЖЕН БЫТЬ EXCEPTION!

			Assert.True(result); // ЭТО СОВЕРШЕННО НЕПРАВИЛЬНО!

			// Правильный тест:
			// var exception = Assert.Throws<ArgumentException>(() => service.AddTask(nullTask));
			// Assert.Contains("Задача не может быть null", exception.Message);
		}
	}
}