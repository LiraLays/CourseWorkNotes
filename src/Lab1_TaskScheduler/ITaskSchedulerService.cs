using Lab1_TaskScheduler.Models;
using System.Collections.Generic;

namespace Lab1_TaskScheduler.Services
{
	public interface ITaskSchedulerService
	{
		IReadOnlyList<TaskItem> Tasks { get; }
		bool AddTask(TaskItem task);
		bool MoveTask(TaskItem task, DateTime newDeadline);
		IReadOnlyList<TaskItem> FilterTasks(bool byDeadline, bool byPriority);
	}
}