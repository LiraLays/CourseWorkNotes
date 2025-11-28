using SmartTaskScheduler.Library.Core.Models;
using SmartTaskScheduler.Library.Core.Services;
using SmartTaskScheduler.Library.Core.Verification;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Input;

namespace WpfTaskScheduler.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly TaskSchedulerService _schedulerService;
        private string _currentFilter = "";
        private string _deadlineFilter = "";
        private int _priorityFilter = 0;
        private bool _filterByDeadline = false;
        private bool _filterByPriority = false;
        private string _verificationLog = "";
        private string _newTaskTitle = "";
        private DateTime _newTaskDeadline = DateTime.Now.AddDays(1);
        private int _newTaskPriority = 2;
        private string _errorMessage = "";
        private bool _isSuccessMessage = false;
        private TaskItem _selectedTask;

        public MainViewModel()
        {
            _schedulerService = new TaskSchedulerService();

            // Инициализация команд
            AddNewTaskCommand = new RelayCommand(AddNewTask);
            AddQuickTaskCommand = new RelayCommand(AddQuickTask);
            MarkAsCompletedCommand = new RelayCommand<TaskItem>(MarkAsCompleted);
            ShowVerificationDetailsCommand = new RelayCommand(ShowVerificationDetails);
            ClearVerificationCommand = new RelayCommand(ClearVerification);

            // Инициализация демо-данными (теперь загружаются из файла)
            // InitializeDemoData();

            // Инициализация сортировки
            InitializeSorting();

            // Первоначальная фильтрация
            RefreshFilteredTasks();
        }

        // Команды
        public ICommand AddNewTaskCommand { get; }
        public ICommand AddQuickTaskCommand { get; }
        public ICommand MarkAsCompletedCommand { get; }
        public ICommand ShowVerificationDetailsCommand { get; }
        public ICommand ClearVerificationCommand { get; }

        // Коллекции для привязки
        public ObservableCollection<TaskItem> AllTasks { get; } = new ObservableCollection<TaskItem>();
        public ObservableCollection<TaskItem> FilteredTasks { get; } = new ObservableCollection<TaskItem>();
        public ObservableCollection<string> VerificationSteps { get; } = new ObservableCollection<string>();

        // Свойство для отображения отсортированных задач
        public ICollectionView AllTasksView => CollectionViewSource.GetDefaultView(AllTasks);

        // Коллекция приоритетов
        public ObservableCollection<PriorityItem> PriorityOptions { get; } = new ObservableCollection<PriorityItem>
        {
            new PriorityItem { Value = 1, DisplayName = "Не срочно" },
            new PriorityItem { Value = 2, DisplayName = "Умеренно" },
            new PriorityItem { Value = 3, DisplayName = "Срочно" },
            new PriorityItem { Value = 4, DisplayName = "Крайне срочно" }
        };

        // Коллекция приоритетов фильтрации
        public ObservableCollection<PriorityItem> PriorityFilterOptions { get; } = new ObservableCollection<PriorityItem>
        {
            new PriorityItem { Value = 0, DisplayName = "Все приоритеты" },
            new PriorityItem { Value = 1, DisplayName = "Не срочно" },
            new PriorityItem { Value = 2, DisplayName = "Умеренно" },
            new PriorityItem { Value = 3, DisplayName = "Срочно" },
            new PriorityItem { Value = 4, DisplayName = "Крайне срочно" }
        };

        

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
                OnPropertyChanged(nameof(PriorityDisplay)); // Уведомляем об изменении отображаемого текста
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
                // Опрелелаение типа сообщения
                IsSuccessMessage = !string.IsNullOrEmpty(value) &&
                                  (value.Contains("успешно") ||
                                   value.Contains("добавлен") ||
                                   value.Contains("выполнен") ||
                                   value.Contains("Быстрая задача"));
            }
        }

        public bool IsSuccessMessage
        {
            get => _isSuccessMessage;
            set
            {
                _isSuccessMessage = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsErrorMessage)); // Уведомляем об изменении противоположного свойства
            }
        }

        public bool IsErrorMessage => !string.IsNullOrEmpty(ErrorMessage) && !IsSuccessMessage;

        public TaskItem SelectedTask
        {
            get => _selectedTask;
            set
            {
                _selectedTask = value;
                OnPropertyChanged();
            }
        }

        // Свойство для отображения текста приоритета
        public string PriorityDisplay => PriorityOptions.FirstOrDefault(p => p.Value == NewTaskPriority)?.DisplayName;

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

        public string DeadlineFilter
        {
            get => _deadlineFilter;
            set
            {
                _deadlineFilter = value;
                OnPropertyChanged();
                RefreshFilteredTasks();
            }
        }

        public int PriorityFilter
        {
            get => _priorityFilter;
            set
            {
                _priorityFilter = value;
                OnPropertyChanged();
                // Автоматическое включение фильтрации по приоритету при выборе значения
                if (value > 0 && !FilterByPriority)
                {
                    FilterByPriority = true;
                }
                else
                { 
                    RefreshFilteredTasks();
                }
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
                // Если выключили фильтрацию по приоритету - сбрасываем выбранный приоритет
                if (!value)
                {
                    PriorityFilter = 0;
                }
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

        // Методы команд

        private void InitializeSorting()
        {
            AllTasksView.SortDescriptions.Add(new SortDescription("IsCompleted", ListSortDirection.Ascending));
            AllTasksView.SortDescriptions.Add(new SortDescription("Deadline", ListSortDirection.Ascending));
        }

        public void AddNewTask()
        {
            if (!ValidateNewTask())
                return;

            try
            {
                var newTask = new TaskItem
                {
                    Title = NewTaskTitle,
                    Description = "Описание задачи",
                    Deadline = NewTaskDeadline,
                    Priority = NewTaskPriority
                };

                if (_schedulerService.AddTask(newTask))
                {
                    AllTasks.Add(newTask);
                    RefreshFilteredTasks();
                    ResetNewTaskForm();
                    ErrorMessage = "Задача успешно добавлена!";
                }
            }
            catch (Exception ex)
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
                    ErrorMessage = "Быстрая задача добавлена!";
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
                ErrorMessage = $"Задача '{task.Title}' выполнена!";
            }
        }

        public void ShowVerificationDetails()
        {
            try
            {
                var result = _schedulerService.FilterTasks(CurrentFilter, FilterByDeadline, FilterByPriority);

                VerificationSteps.Clear();
                VerificationLog = $"Результат верификации:\n" +
                                 $"Булева функция (Лаб. 4): {result.Verification.BooleanFunction}\n" +
                                 $"Инвариант цикла (Лаб. 3): {result.Verification.Invariant}\n" +
                                 $"WP-результат (Лаб. 2): {result.Verification.WpResult}";

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
            ErrorMessage = "";
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
        private void RefreshFilteredTasks()
        {
            try
            {
                // Преобразование PriorityFilter обратно в строку для сервиса
                string priorityFilterValue = _priorityFilter > 0 ? _priorityFilter.ToString() : "";

                var result = _schedulerService.FilterTasks(CurrentFilter, FilterByDeadline, FilterByPriority, DeadlineFilter, priorityFilterValue);

                // Очистка AllTasks и добавляем только отфильтрованные задачи
                AllTasks.Clear();
                foreach (var task in result.FilteredTasks)
                {
                    AllTasks.Add(task);
                }

                ShowVerificationDetails();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка фильтрации: {ex.Message}";
                AllTasks.Clear();
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
                        // Добавляем в AllTasks напрямую
                        AllTasks.Add(task);
                    }
                }
                catch (Exception ex)
                {
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

    // RelayCommand implementation
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;
        public void Execute(object parameter) => _execute();
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke((T)parameter) ?? true;
        public void Execute(object parameter) => _execute((T)parameter);
    }

    public class PriorityItem
    {
        public int Value { get; set; }
        public string DisplayName { get; set; }
    }

    public class PriorityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int priority)
            {
                return priority switch
                {
                    1 => "Не срочно",
                    2 => "Умеренно",
                    3 => "Срочно",
                    4 => "Крайне срочно",
                    _ => priority.ToString()
                };
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}