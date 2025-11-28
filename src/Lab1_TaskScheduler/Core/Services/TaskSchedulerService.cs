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
        private readonly TaskDataService _dataService = new TaskDataService();

        public IReadOnlyList<TaskItem> Tasks => _tasks.AsReadOnly();

        public TaskSchedulerService()
        {
            // Загружаем задачи при создании сервиса
            LoadTasksFromFile();
        }

        public bool AddTask(TaskItem task)
        {
            // Проверка предусловий из контракта
            if (!ValidatePreConditions(OperationContracts.AddTask.PreConditions, task))
                return false;

            _tasks.Add(task);
            // Сохрарание задач после добавления
            SaveTasksToFile();
            return true;
        }

        public VerifiedFilterResult FilterTasks(
            string customFilter = "",
            bool byDeadline = false,
            bool byPriority = false,
            string deadlineFilter = "",
            string priorityFilter = "")
        {
            var result = new VerifiedFilterResult();

            // Верификация операции
            result.Verification = _verifier.VerifyFilterOperation(
                customFilter, _tasks, byDeadline, byPriority);

            // Применение фильтра с новыми параметрами
            result.FilteredTasks = ApplyFilters(_tasks, customFilter, byDeadline, byPriority, deadlineFilter, priorityFilter);

            return result;
        }

        private List<TaskItem> ApplyFilters(
            IEnumerable<TaskItem> tasks,
            string customFilter,
            bool byDeadline,
            bool byPriority,
            string deadlineFilter,
            string priorityFilter)
        {
            var query = tasks.AsEnumerable();

            // Фильтрация по дедлайну с пользовательским условием
            if (byDeadline && !string.IsNullOrWhiteSpace(deadlineFilter))
            {
                query = ApplyDeadlineFilter(query, deadlineFilter);
            }
            else if (byDeadline)
            {
                // Стандартная фильтрация по дедлайну (если не указано конкретное условие)
                query = query.Where(t => t.IsOverdue || t.IsUrgent);
            }

            // Фильтрация по приоритету с пользовательским условием
            if (byPriority && !string.IsNullOrWhiteSpace(priorityFilter))
            {
                query = ApplyPriorityFilter(query, priorityFilter);
            }
            //else if (byPriority)
            //{
            //    // Стандартная фильтрация по приоритету (если не указано конкретное условие)
            //    query = query.Where(t => t.Priority == 1);
            //}

            // Кастомный фильтр
            if (!string.IsNullOrEmpty(customFilter))
                query = ApplyCustomFilter(query, customFilter);

            return query.ToList();
        }

        private IEnumerable<TaskItem> ApplyDeadlineFilter(IEnumerable<TaskItem> tasks, string deadlineFilter)
        {
            try
            {
                var today = DateTime.Today;

                if (deadlineFilter.Trim() == "> сегодня")
                {
                    return tasks.Where(t => t.Deadline.Date > today);
                }
                else if (deadlineFilter.Trim() == "< сегодня")
                {
                    return tasks.Where(t => t.Deadline.Date < today);
                }
                else if (deadlineFilter.Trim() == "== сегодня")
                {
                    return tasks.Where(t => t.Deadline.Date == today);
                }
                else if (DateTime.TryParse(deadlineFilter.Trim(), out DateTime specificDate))
                {
                    return tasks.Where(t => t.Deadline.Date == specificDate.Date);
                }
                else
                {
                    // По умолчанию показываем просроченные задачи
                    return tasks.Where(t => t.Deadline.Date <= today && !t.IsCompleted);
                }
            }
            catch
            {
                // В случае ошибки возвращаем исходный список
                return tasks;
            }
        }

        private IEnumerable<TaskItem> ApplyPriorityFilter(IEnumerable<TaskItem> tasks, string priorityFilter)
        {
            try
            {
                if (int.TryParse(priorityFilter.Trim(), out int priority))
                {
                    return tasks.Where(t => t.Priority == priority);
                }
                else if (priorityFilter.Trim().StartsWith(">"))
                {
                    if (int.TryParse(priorityFilter.Trim().Substring(1).Trim(), out int minPriority))
                    {
                        return tasks.Where(t => t.Priority > minPriority);
                    }
                }
                else if (priorityFilter.Trim().StartsWith("<"))
                {
                    if (int.TryParse(priorityFilter.Trim().Substring(1).Trim(), out int maxPriority))
                    {
                        return tasks.Where(t => t.Priority < maxPriority);
                    }
                }
                else
                {
                    // По умолчанию показываем высокоприоритетные задачи
                    return tasks.Where(t => t.Priority >= 4 && !t.IsCompleted);
                }
            }
            catch
            {
                // В случае ошибки возвращаем исходный список
                return tasks;
            }

            return tasks;
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

        /// <summary>
        /// Загрузка задач из файла
        /// </summary>
        private void LoadTasksFromFile()
        {
            try
            {
                var loadedTasks = _dataService.LoadTasks();
                _tasks.Clear();
                _tasks.AddRange(loadedTasks);
            }
            catch (Exception ex)
            {
                // Логирование ошибки, но не прерывание работы приложения
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки задач: {ex.Message}");
            }
        }

        /// <summary>
        /// Сохранение задач в файл
        /// </summary>
        private void SaveTasksToFile()
        {
            try
            {
                _dataService.SaveTasks(_tasks);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка сохранения задач: {ex.Message}");
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

