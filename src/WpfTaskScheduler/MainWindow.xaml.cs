using Microsoft.VisualBasic;
using SmartTaskScheduler.Contracts;
using SmartTaskScheduler.Models;
using SmartTaskScheduler.Services;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using WpfTaskScheduler.ViewModels;

namespace WpfTaskScheduler
{
	public partial class MainWindow : Window
	{

		private MainViewModel _viewModel;
		private ITaskSchedulerService _taskService;

		public MainWindow()
		{
			InitializeComponent();



        }


        private void checkBox_Checked_2(object sender, RoutedEventArgs e)
        {
			groupBox.Visibility = ((bool)checkBoxFiltr.IsChecked ? Visibility.Visible : Visibility.Collapsed) ;
        }

        private void checkBox_Checked_1(object sender, RoutedEventArgs e)
        {

        }
    }
}