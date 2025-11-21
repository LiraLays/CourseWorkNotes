using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Lab1_TaskScheduler.Models
{
	public class TaskItem : INotifyPropertyChanged
	{
		private string _title = string.Empty;
		private string _description = string.Empty;
		private DateTime _deadline;
		private int _priority;
		private bool _isCompleted;

		private string GetPriorityDisplay()
		{
			return Priority switch
			{
				1 => $"Высокий ({Priority})",
				2 => $"Средний ({Priority})",
				3 => $"Низкий ({Priority})",
				4 => $"Не срочно ({Priority})",
				5 => $"Очень низкий ({Priority})", // на случай если у вас 5 приоритетов
				_ => $"Приоритет {Priority}"
			};
		}

		public string Title
		{
			get => _title;
			set { _title = value; OnPropertyChanged(); }
		}

		public string Description
		{
			get => _description;
			set { _description = value; OnPropertyChanged(); }
		}

		public DateTime Deadline
		{
			get => _deadline;
			set { _deadline = value; OnPropertyChanged(); OnPropertyChanged(nameof(DeadlineDisplay)); }
		}

		public int Priority
		{
			get => _priority;
			set
			{
				_priority = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(PriorityDisplay)); // Добавьте эту строку
			}
		}

		public DateTime CreatedDate { get; set; } = DateTime.Now;

		public bool IsCompleted
		{
			get => _isCompleted;
			set { _isCompleted = value; OnPropertyChanged(); }
		}

		// Дополнительные свойства для отображения
		public string DeadlineDisplay => Deadline.ToString("dd.MM.yyyy");
		public string PriorityDisplay => $"Приоритет: {GetPriorityDisplay()}";

		public override string ToString()
		{
			return $"{Title} (Приоритет: {GetPriorityDisplay()}, Дедлайн: {Deadline:dd.MM.yyyy})";
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}