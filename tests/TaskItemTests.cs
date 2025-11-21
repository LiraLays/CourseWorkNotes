using Lab1_TaskScheduler.Models;
using System;
using System.ComponentModel;
using Xunit;

namespace Lab1_TaskScheduler.Tests
{
	public class TaskItemTests
	{
		[Fact]
		public void TaskItem_PropertyChanged_ShouldBeRaisedWhenPropertyChanges()
		{
			// Arrange
			var task = new TaskItem();
			var propertyChangedRaised = false;
			string changedPropertyName = "";

			task.PropertyChanged += (sender, e) =>
			{
				propertyChangedRaised = true;
				changedPropertyName = e.PropertyName;
			};

			// Act
			task.Title = "Новое название";

			// Assert
			Assert.True(propertyChangedRaised);
			Assert.Equal(nameof(TaskItem.Title), changedPropertyName);
		}

		[Fact]
		public void TaskItem_DeadlineDisplay_ShouldReturnFormattedString()
		{
			// Arrange
			var deadline = new DateTime(2024, 1, 15, 14, 30, 0);
			var task = new TaskItem { Deadline = deadline };

			// Act
			var display = task.DeadlineDisplay;

			// Assert
			Assert.Equal("15.01.2024", display);
		}

		[Fact]
		public void TaskItem_PriorityDisplay_ShouldReturnFormattedString()
		{
			// Arrange
			var task = new TaskItem { Priority = 4 };

			// Act
			var display = task.PriorityDisplay;

			// Assert
			Assert.Equal("Приоритет: Не срочно (4)", display);
		}

		[Fact]
		public void TaskItem_ToString_ShouldReturnCorrectFormat()
		{
			// Arrange
			var task = new TaskItem
			{
				Title = "Тестовая задача",
				Priority = 3,
				Deadline = new DateTime(2024, 1, 15, 10, 0, 0)
			};

			// Act
			var result = task.ToString();

			// Assert
			Assert.Equal("Тестовая задача (Приоритет: Низкий (3), Дедлайн: 15.01.2024)", result);
		}
	}
}