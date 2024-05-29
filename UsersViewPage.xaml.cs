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
    /// Логика взаимодействия для UsersViewPage.xaml
    /// </summary>
    public partial class UsersViewPage : Page
    {
        private FirebaseClientHelper firebaseHelper = new FirebaseClientHelper();

        public UsersViewPage()
        {
            InitializeComponent();
            Loaded += UsersViewPage_Loaded;
        }

        private async void UsersViewPage_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadUsersAsync();
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            

        }
        private async Task LoadUsersAsync()
        {
            var users = await firebaseHelper.GetDataAsync<User>("users");
            UsersDataGrid.ItemsSource = users;
        }

        private async void UsersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem is User selectedUser)
            {
                // Передайте selectedUser в конструктор UserEditPage
                
                UserEditWindow taskWindow = new UserEditWindow(selectedUser);
                taskWindow.ShowDialog();
                string closeReason = taskWindow.CloseReason;
                if (closeReason == "Saved")
                {
                    await LoadUsersAsync();
                }
            }
        }

        
        private async void Button_Click_Delete(object sender, RoutedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem is User selectedUser)
            {
                // Подтверждение действия пользователя
                var customMessageBox = new CustomMessageBox("Вы уверены, что хотите удалить выбранного пользователя?");
                var result = customMessageBox.ShowDialog();
               // var result = MessageBox.Show("Вы уверены, что хотите удалить выбранного пользователя?", "Подтверждение удаления", MessageBoxButton.YesNo);
                if (result.HasValue && result.Value)
                {
                   
                    // Предполагается, что firebaseHelper имеет метод DeleteUserAsync для удаления пользователя
                    await firebaseHelper.DeleteUserAsync(selectedUser);
                    await LoadUsersAsync(); // Обновление списка пользователей после удаления
                }
            }
            else
            {
                new CustomMessageBox("Пожалуйста, выберите пользователя для удаления.").ShowDialog();
            }
        }

        private async void Button_Click_Edit(object sender, RoutedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem is User selectedUser)
            {
                var editWindow = new UserEditWindow(selectedUser);
                editWindow.ShowDialog();
                string closeReason = editWindow.CloseReason;
                if (closeReason == "Saved")
                {
                    await LoadUsersAsync();
                }
            }
            else
            {
                new CustomMessageBox("Пожалуйста, выберите пользователя для редактирования.").ShowDialog();
            }
        }

        private async void Button_Click_NewUser(object sender, RoutedEventArgs e)
        {
            UserEditWindow taskWindow = new UserEditWindow();
            taskWindow.ShowDialog();
            string closeReason = taskWindow.CloseReason;
            if (closeReason == "Saved")
            {
                await LoadUsersAsync();
            }
        }
    }
}
