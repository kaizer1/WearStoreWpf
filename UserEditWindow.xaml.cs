using Google.Apis.Storage.v1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WearStoreWpf
{
    /// <summary>
    /// Логика взаимодействия для UserEditWindow.xaml
    /// </summary>
    public partial class UserEditWindow : Window
    {
        private User currentUser;
        public string CloseReason { get; private set; }
        // Измените конструктор, чтобы он принимал объект User
        public  UserEditWindow(User user = null) // Позволяет передавать существующего пользователя или null для создания нового
        {
            InitializeComponent();
            Icon = ConvertIconToImageSource(Properties.Resources.icon);
            currentUser = user;
            if (currentUser != null)
            {
                FillUserData();
                OrdersLabel.Visibility = Visibility.Visible;
                OrdersDataGrid.Visibility = Visibility.Visible;
                
            }
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
        private async void FillUserData()
        {
            // Заполните поля интерфейса данными из currentUser
            NameTextBox.Text = currentUser.Name;
            EmailTextBox.Text = currentUser.Email;
            RoleTextBox.Text = currentUser.Role;


            FirebaseClientHelper firebase = new FirebaseClientHelper();
            var allorders = await firebase.GetDataAsync<Order>("orders");
            var orders = allorders.Where(order => order.UserKey == currentUser.Key).ToList();
            OrdersDataGrid.ItemsSource = orders;
        }
        private void OrdersDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedOrder = (Order)OrdersDataGrid.SelectedItem;
            var editWindow = new OrderEditWindow(selectedOrder);
            editWindow.ShowDialog();
        }


        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            FirebaseClientHelper firebase = new FirebaseClientHelper();
            try
            {
                if (currentUser == null)
                {
                    var user = new User
                    {
                        Name = NameTextBox.Text,
                        Email = EmailTextBox.Text,
                        Role = RoleTextBox.Text
                    };
                    await firebase.NewUser(user);
                    CloseReason = "Saved";
                    this.Close();
                    new CustomMessageBox($"Пользователь: {user.Name}, добавлен").ShowDialog();
                }
                else
                {
                    currentUser.Name = NameTextBox.Text;
                    currentUser.Email = EmailTextBox.Text;
                    currentUser.Role = RoleTextBox.Text;
                    await firebase.EditUser(currentUser);
                    CloseReason = "Saved";
                    this.Close();
                    new CustomMessageBox($"Пользователь: {currentUser.Name}, изменен").ShowDialog();
                }
            }
            catch (Exception ex)
            {
                new CustomMessageBox($"Ошибка при сохранении пользователя: {ex.Message}").ShowDialog();
            }


        }
    }
}
