using Newtonsoft.Json;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WearStoreWpf
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Console.WriteLine("start project work  ! ");
          

            Icon = ConvertIconToImageSource(Properties.Resources.icon);

            LoadUsers(); // 
        }
        private ImageSource ConvertIconToImageSource(System.Drawing.Icon icon)
        {
            using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
            {
                icon.Save(memoryStream);
                memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }
        private async void LoadUsers()
        {
            FirebaseClientHelper firebase = new FirebaseClientHelper();
            Console.WriteLine("Pre load users ! ");
            //var users = await firebase.GetDataAsync<User>("Users"); // was users
            var users = await firebase.GetDataAsyncUsers();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new UsersViewPage());

        }

        private void ProdView(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProductsViewPage());
        }

        private void OrderView(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new OrdersViewPage());

        }

        private void OrderViewBanners(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new BannersView());
        }
    }

   

}
