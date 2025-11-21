using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfTaskScheduler
{
    /// <summary>
    /// Interaction logic for AddWindow.xaml
    /// </summary>
    public partial class AddWindow : Window
    {
        public string date;
        public string ladaPriora;
        public string name;
        public MainWindow main;
        public AddWindow()
        {
            InitializeComponent();
            comboBox.Items.Clear();
            foreach (string item in new string[] { "Не срочно (4)", "Низкий (3)", "Средний (2)", "Высокий (1)" })
            {
                comboBox.Items.Add(item);
            }
            
        }
        
        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (datePicker.Text.Replace(" ","") == "" || comboBox.Text.Replace(" ", "") == "" || textBox.Text.Replace(" ", "") == "") { MessageBox.Show("Некорректный ввод.");return;}
            date = datePicker.Text;
            ladaPriora = comboBox.Text.ToCharArray()[^2].ToString();
            name = textBox.Text;
            main.AddTaskSucessful(date, ladaPriora, name);
            Close();
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
