using Lab1_TaskScheduler.Utils;
using System;
using Xunit;

namespace Lab1_TaskScheduler.Tests
{
	public class GuardTests
	{
		[Fact]
		public void Requires_WithTrueCondition_ShouldNotThrowException()
		{
			// Arrange
			var condition = true;

			// Act & Assert
			var exception = Record.Exception(() => Guard.Requires(condition, "Сообщение"));
			Assert.Null(exception);
		}

		[Fact]
		public void Requires_WithFalseCondition_ShouldThrowArgumentException()
		{
			// Arrange
			var condition = false;
			var errorMessage = "Тестовое сообщение об ошибке";

			// Act & Assert
			var exception = Assert.Throws<ArgumentException>(() =>
				Guard.Requires(condition, errorMessage));
			Assert.Contains(errorMessage, exception.Message);
			Assert.Contains("Pre-condition failed", exception.Message);
		}

		[Fact]
		public void Requires_WithCustomException_ShouldThrowSpecifiedException()
		{
			// Arrange
			var condition = false;

			// Act & Assert
			var exception = Assert.Throws<InvalidOperationException>(() =>
				Guard.Requires<InvalidOperationException>(condition, "Кастомная ошибка"));
			Assert.Contains("Кастомная ошибка", exception.Message);
		}
	}
}