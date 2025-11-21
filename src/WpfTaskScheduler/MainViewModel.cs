using Lab1_TaskScheduler.Models;
using Lab1_TaskScheduler.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfTaskScheduler.ViewModels
{
	public class MainViewModel : INotifyPropertyChanged
	{
		private readonly ITaskSchedulerService _taskService;
		private string _preConditionStatus = "Предусловие: не проверено";
		private string _postConditionStatus = "Постусловие: не выполнено";
		private string _currentOperation = "";
		private bool _preConditionMet;
		private bool _postConditionMet;

		public MainViewModel(ITaskSchedulerService taskService)
		{
			_taskService = taskService;
			Tasks = new ObservableCollection<TaskItem>(_taskService.Tasks);
		}

		public ObservableCollection<TaskItem> Tasks { get; set; }

		public string PreConditionStatus
		{
			get => _preConditionStatus;
			set
			{
				_preConditionStatus = value;
				OnPropertyChanged();
			}
		}

		public string PostConditionStatus
		{
			get => _postConditionStatus;
			set
			{
				_postConditionStatus = value;
				OnPropertyChanged();
			}
		}

		public bool PreConditionMet
		{
			get => _preConditionMet;
			set
			{
				_preConditionMet = value;
				OnPropertyChanged();
			}
		}

		public bool PostConditionMet
		{
			get => _postConditionMet;
			set
			{
				_postConditionMet = value;
				OnPropertyChanged();
			}
		}

		public void UpdatePreCondition(bool isMet, string message = "")
		{
			PreConditionMet = isMet;
			PreConditionStatus = isMet ? "Предусловие: выполнено" : $"Предусловие: не выполнено ({message})";
		}

		public void UpdatePostCondition(bool isMet, string message = "")
		{
			PostConditionMet = isMet;
			PostConditionStatus = isMet ? "Постусловие: выполнено" : $"Постусловие: не выполнено ({message})";
		}

		public void ResetConditions()
		{
			PreConditionStatus = "Предусловие: не проверено";
			PostConditionStatus = "Постусловие: не выполнено";
			PreConditionMet = false;
			PostConditionMet = false;
		}

		public void RefreshTasks()
		{
			Tasks.Clear();
			foreach (var task in _taskService.Tasks)
			{
				Tasks.Add(task);
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}