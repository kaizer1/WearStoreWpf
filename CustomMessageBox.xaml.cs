using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WearStoreWpf
{
    /// <summary>
    /// Логика взаимодействия для CustomMessageBox.xaml
    /// </summary>
   
    public partial class CustomMessageBox : Window
    {
       
        public string Message { get; set; }
        public bool ShowCloseButton { get; set; }
        public bool ShowYesNoButtons { get; set; }

        public CustomMessageBox(string message)
        {
            InitializeComponent();
            Message = message;
            DataContext = this;

            

            if (message.Contains("?"))
            {
                CloseBtn.Visibility = Visibility.Hidden;
            }
            else {
                YesBtn.Visibility = Visibility.Hidden;
                NoBtn.Visibility = Visibility.Hidden;
            }
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
    }
