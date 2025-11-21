using SmartTaskScheduler.Library.Core.Contracts;
using SmartTaskScheduler.Library.Core.Verification;
using SmartTaskScheduler.Library.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTaskScheduler.Library.Core.Services
{
    public class TaskSchedulerService
    {
        private readonly List<TaskItem> _tasks = new();
        private readonly TaskVerificationEngine _verifier = new();

        public IReadOnlyList<TaskItem> Tasks => _tasks.AsReadOnly();

        public bool AddTask(TaskItem task)
        {
            // Проверка предусловий из контракта
            if (!ValidatePreConditions(OperationContracts.AddTask.PreConditions, task))
                return false;

            _tasks.Add(task);
            return true;
        }

        public VerifiedFilterResult FilterTasks(
            string customFilter = "",
            bool byDeadline = false,
            bool byPriority = false)
        {
            var result = new VerifiedFilterResult();

            // Верификация операции
            result.Verification = _verifier.VerifyFilterOperation(
                customFilter, _tasks, byDeadline, byPriority);

            // Применение фильтра
            result.FilteredTasks = ApplyFilters(_tasks, customFilter, byDeadline, byPriority);

            return result;
        }

        private List<TaskItem> ApplyFilters(
            IEnumerable<TaskItem> tasks,
            string customFilter,
            bool byDeadline,
            bool byPriority)
        {
            var query = tasks.AsEnumerable();

            if (byDeadline)
                query = query.Where(t => t.IsOverdue || t.IsUrgent);

            if (byPriority)
                query = query.Where(t => t.Priority == 1);

            if (!string.IsNullOrEmpty(customFilter))
                query = ApplyCustomFilter(query, customFilter);

            return query.ToList();
        }

        private IEnumerable<TaskItem> ApplyCustomFilter(IEnumerable<TaskItem> tasks, string filter)
        {
            // Упрощенная реализация кастомного фильтра
            foreach (var task in tasks)
            {
                if (EvaluateBooleanCondition(task, filter))
                    yield return task;
            }
        }

        private bool EvaluateBooleanCondition(TaskItem task, string condition)
        {
            // Базовая оценка булевых условий
            return condition switch
            {
                var c when c.Contains("Priority == 1") => task.Priority == 1,
                var c when c.Contains("IsOverdue") => task.IsOverdue,
                var c when c.Contains("IsUrgent") => task.IsUrgent,
                _ => true
            };
        }

        private bool ValidatePreConditions(string preConditions, object context)
        {
            // Упрощенная валидация предусловий
            return !string.IsNullOrEmpty(preConditions); // Заглушка
        }
    }

    public class VerifiedFilterResult
    {
        public List<TaskItem> FilteredTasks { get; set; } = new();
        public VerificationResult Verification { get; set; } = new();
    }
}

