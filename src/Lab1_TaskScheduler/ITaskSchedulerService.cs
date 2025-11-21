using SmartTaskScheduler.Models;
using System.Collections.Generic;

namespace SmartTaskScheduler.Services
{
	public interface ITaskSchedulerService
	{
		IReadOnlyList<TaskItem> Tasks { get; }
		bool AddTask(TaskItem task);
		bool MoveTask(TaskItem task, DateTime newDeadline);
		IReadOnlyList<TaskItem> FilterTasks(bool byDeadline, bool byPriority);
	}
}