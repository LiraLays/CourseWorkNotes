using Lab1_TaskScheduler.Models;
using Lab1_TaskScheduler.Services;
using System;
using Xunit;

namespace Lab1_TaskScheduler.Tests
{
	public class TaskSchedulerServiceTests
	{
		private readonly TaskSchedulerService _service;
		private readonly TaskItem _validTask;

		public TaskSchedulerServiceTests()
		{
			_service = new TaskSchedulerService();
			_validTask = new TaskItem
			{
				Title = "Тестовая задача",
				Description = "Описание тестовой задачи",
				Deadline = DateTime.Now.AddDays(1),
				Priority = 3
			};
		}

		[Fact]
		public void AddTask_WithValidTask_ShouldAddTaskToCollection()
		{
			// Arrange
			var initialCount = _service.Tasks.Count;

			// Act
			var result = _service.AddTask(_validTask);

			// Assert
			Assert.True(result);
			Assert.Equal(initialCount + 1, _service.Tasks.Count);
			Assert.Contains(_validTask, _service.Tasks);
		}

		[Fact]
		public void AddTask_WithInvalidDeadline_ShouldThrowArgumentException()
		{
			// Arrange
			var invalidTask = new TaskItem
			{
				Title = "Невалидная задача",
				Description = "Задача с прошедшим дедлайном",
				Deadline = DateTime.Now.AddDays(-1),
				Priority = 3
			};

			// Act & Assert
			var exception = Assert.Throws<ArgumentException>(() => _service.AddTask(invalidTask));
			Assert.Contains("Дедлайн должен быть в будущем", exception.Message);
		}

		[Fact]
		public void MoveTask_WithValidParameters_ShouldUpdateDeadline()
		{
			// Arrange
			_service.AddTask(_validTask);
			var newDeadline = DateTime.Now.AddDays(5);

			// Act
			var result = _service.MoveTask(_validTask, newDeadline);

			// Assert
			Assert.True(result);
			Assert.Equal(newDeadline, _validTask.Deadline);
		}
	}
}