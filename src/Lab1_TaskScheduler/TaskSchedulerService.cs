using Lab1_TaskScheduler.Models;
using Lab1_TaskScheduler.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Lab1_TaskScheduler.Services
{
	public class TaskSchedulerService : ITaskSchedulerService
	{
		private readonly List<TaskItem> _tasks = new List<TaskItem>();

		public IReadOnlyList<TaskItem> Tasks => _tasks.AsReadOnly();

		public bool AddTask(TaskItem task)
		{
			// Pre-conditions
			Guard.Requires(task != null, "Задача не может быть null");
			Guard.Requires(!string.IsNullOrWhiteSpace(task.Title), "Название задачи не может быть пустым");
			Guard.Requires(task.Deadline > DateTime.Now, "Дедлайн должен быть в будущем");
			Guard.Requires(task.Priority >= 1 && task.Priority <= 4, "Приоритет должен быть от 1 до 4");

			bool result = false;

			try
			{
				_tasks.Add(task);
				result = true;
			}
			finally
			{
				// Post-condition
				Debug.Assert(_tasks.Contains(task), "Задача должна быть добавлена в список");
			}

			return result;
		}

		public bool MoveTask(TaskItem task, DateTime newDeadline)
		{
			// Pre-conditions
			Guard.Requires(task != null, "Задача не может быть null");
			Guard.Requires(_tasks.Contains(task), "Задача должна существовать в списке");
			Guard.Requires(newDeadline > DateTime.Now, "Новый дедлайн должен быть в будущем");

			bool result = false;
			DateTime oldDeadline = task.Deadline;

			try
			{
				task.Deadline = newDeadline;
				result = true;
			}
			finally
			{
				// Post-condition
				Debug.Assert(task.Deadline == newDeadline, "Дедлайн задачи должен быть изменен");
				Debug.Assert(oldDeadline != newDeadline, "Дедлайн должен быть изменен на новое значение");
			}

			return result;
		}

		public IReadOnlyList<TaskItem> FilterTasks(bool byDeadline, bool byPriority)
		{
			// Pre-conditions
			Guard.Requires(byDeadline || byPriority, "Должен быть выбран хотя бы один критерий фильтрации");

			IEnumerable<TaskItem> filteredTasks = _tasks;

			if (byDeadline)
			{
				// Фильтруем задачи с дедлайном на сегодня или ранее (просроченные)
				filteredTasks = filteredTasks.Where(t => t.Deadline.Date <= DateTime.Today && !t.IsCompleted);
			}

			if (byPriority)
			{
				// Фильтруем задачи с высоким приоритетом (4-5)
				filteredTasks = filteredTasks.Where(t => t.Priority >= 4 && !t.IsCompleted);
			}

			var result = filteredTasks.ToList();

			// Post-condition
			Debug.Assert(result != null, "Результат фильтрации не должен быть null");

			if (byDeadline)
			{
				Debug.Assert(!result.Any(t => t.Deadline.Date > DateTime.Today),
					"При фильтрации по дедлайну все задачи должны иметь дедлайн не позже сегодняшнего дня");
			}

			if (byPriority)
			{
				Debug.Assert(!result.Any(t => t.Priority < 4),
					"При фильтрации по приоритету все задачи должны иметь приоритет 4");
			}

			return result.AsReadOnly();
		}
	}
}