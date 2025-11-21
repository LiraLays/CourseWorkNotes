using SmartTaskScheduler.Library.Core.Models;
using SmartTaskScheduler.Library.Core.Services;
using SmartTaskScheduler.Library.Core.Verification;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System;

namespace WpfTaskScheduler.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly TaskSchedulerService _schedulerService;
        private string _currentFilter = "";
        private bool _filterByDeadline = true;
        private bool _filterByPriority = true;
        private string _verificationLog = "";
        private string _newTaskTitle = "";
        private DateTime _newTaskDeadline = DateTime.Now.AddDays(1);
        private int _newTaskPriority = 2;
        private string _errorMessage = "";

        public MainViewModel()
        {
            _schedulerService = new TaskSchedulerService();

            // Инициализация демо-данными
            InitializeDemoData();

            // Первоначальная фильтрация
            RefreshFilteredTasks();
        }

        // Коллекции для привязки
        public ObservableCollection<TaskItem> AllTasks { get; } = new ObservableCollection<TaskItem>();
        public ObservableCollection<TaskItem> FilteredTasks { get; } = new ObservableCollection<TaskItem>();
        public ObservableCollection<string> VerificationSteps { get; } = new ObservableCollection<string>();

        // Свойства для новой задачи
        public string NewTaskTitle
        {
            get => _newTaskTitle;
            set
            {
                _newTaskTitle = value;
                OnPropertyChanged();
                ClearErrorMessage();
            }
        }

        public DateTime NewTaskDeadline
        {
            get => _newTaskDeadline;
            set
            {
                _newTaskDeadline = value;
                OnPropertyChanged();
                ClearErrorMessage();
            }
        }

        public int NewTaskPriority
        {
            get => _newTaskPriority;
            set
            {
                _newTaskPriority = value;
                OnPropertyChanged();
                ClearErrorMessage();
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        // Свойства для привязки к UI
        public string CurrentFilter
        {
            get => _currentFilter;
            set
            {
                _currentFilter = value;
                OnPropertyChanged();
                RefreshFilteredTasks();
            }
        }

        public bool FilterByDeadline
        {
            get => _filterByDeadline;
            set
            {
                _filterByDeadline = value;
                OnPropertyChanged();
                RefreshFilteredTasks();
            }
        }

        public bool FilterByPriority
        {
            get => _filterByPriority;
            set
            {
                _filterByPriority = value;
                OnPropertyChanged();
                RefreshFilteredTasks();
            }
        }

        public string VerificationLog
        {
            get => _verificationLog;
            set
            {
                _verificationLog = value;
                OnPropertyChanged();
            }
        }

        // Команды для привязки к кнопкам
        public void AddNewTask()
        {
            if (!ValidateNewTask())
                return;

            try { 
                var newTask = new TaskItem
                {
                    Title = $"Новая задача {AllTasks.Count + 1}",
                    Description = "Описание задачи",
                    Deadline = DateTime.Now.AddDays(AllTasks.Count % 3), // Чередуем дедлайны
                    Priority = (AllTasks.Count % 4) + 1 // Приоритеты 1-4
                };

                if (_schedulerService.AddTask(newTask))
                {
                    AllTasks.Add(newTask);
                    RefreshFilteredTasks();
                }
            } catch (Exception ex)
            {
                ErrorMessage = $"Ошибка: {ex.Message}";
            }
        }

        public void AddQuickTask()
        {
            var newTask = new TaskItem
            {
                Title = $"Быстрая задача {AllTasks.Count + 1}",
                Description = "Автоматически созданная задача",
                Deadline = DateTime.Now.AddDays(AllTasks.Count % 3),
                Priority = (AllTasks.Count % 4) + 1
            };

            try
            {
                if (_schedulerService.AddTask(newTask))
                {
                    AllTasks.Add(newTask);
                    RefreshFilteredTasks();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при создании быстрой задачи: {ex.Message}";
            }
        }

        public void MarkAsCompleted(TaskItem task)
        {
            if (task != null)
            {
                task.IsCompleted = true;
                RefreshFilteredTasks();
            }
        }

        public void ShowVerificationDetails()
        {
            try
            {
                var result = _schedulerService.FilterTasks(CurrentFilter, FilterByDeadline, FilterByPriority);

                VerificationSteps.Clear();
                VerificationLog = $"Результат верификации:\n" +
                                 $"Булева функция: {result.Verification.BooleanFunction}\n" +
                                 $"Инвариант: {result.Verification.Invariant}\n" +
                                 $"WP-результат: {result.Verification.WpResult}\n\n" +
                                 $"Шаги расчета WP:";

                foreach (var step in result.Verification.WpCalculation)
                {
                    VerificationSteps.Add(step);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка верификации: {ex.Message}";
            }
        }

        public void ClearVerification()
        {
            VerificationSteps.Clear();
            VerificationLog = "Верификация не выполнена";
        }

        // Валидация
        public bool ValidateNewTask()
        {
            if (string.IsNullOrWhiteSpace(NewTaskTitle))
            {
                ErrorMessage = "Название задачи не может быть пустым";
                return false;
            }

            if (NewTaskDeadline <= DateTime.Now)
            {
                ErrorMessage = "Дедлайн должен быть в будущем";
                return false;
            }

            if (NewTaskPriority < 1 || NewTaskPriority > 4)
            {
                ErrorMessage = "Приоритет должен быть от 1 до 4";
                return false;
            }

            ErrorMessage = "";
            return true;
        }

        private void ClearErrorMessage()
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ErrorMessage = "";
            }
        }

        private void ResetNewTaskForm()
        {
            NewTaskTitle = "";
            NewTaskDeadline = DateTime.Now.AddDays(1);
            NewTaskPriority = 2;
        }

        // Приватные методы
        // Приватные методы
        private void RefreshFilteredTasks()
        {
            try
            {
                var result = _schedulerService.FilterTasks(CurrentFilter, FilterByDeadline, FilterByPriority);

                FilteredTasks.Clear();
                foreach (var task in result.FilteredTasks)
                {
                    FilteredTasks.Add(task);
                }

                ShowVerificationDetails();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка фильтрации: {ex.Message}";
                FilteredTasks.Clear();
            }
        }

        private void InitializeDemoData()
        {
            var demoTasks = new[]
            {
                new TaskItem {
                    Title = "Важная встреча",
                    Description = "Обсуждение проекта",
                    Deadline = DateTime.Now.AddHours(2),
                    Priority = 1
                },
                new TaskItem {
                    Title = "Просроченная задача",
                    Description = "Нужно было сделать вчера",
                    Deadline = DateTime.Now.AddDays(-1),
                    Priority = 2
                },
                new TaskItem {
                    Title = "Обычная задача",
                    Description = "Ничего срочного",
                    Deadline = DateTime.Now.AddDays(5),
                    Priority = 3
                }
            };

            foreach (var task in demoTasks)
            {
                try
                {
                    if (_schedulerService.AddTask(task))
                    {
                        AllTasks.Add(task);
                    }
                }
                catch (Exception ex)
                {
                    // Игнорируем ошибки демо-данных
                    System.Diagnostics.Debug.WriteLine($"Ошибка добавления демо-задачи: {ex.Message}");
                }
            }
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}