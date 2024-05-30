using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
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
    public partial class OrdersViewPage : Page
    {
        private FirebaseClientHelper firebaseHelper = new FirebaseClientHelper();

        public OrdersViewPage()
        {
            InitializeComponent();
            Loaded += UsersViewPage_Loaded;
        }

        private async void UsersViewPage_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadOrdersAsync();
        }
       
        private async Task LoadOrdersAsync()
        {
            //var users = await firebaseHelper.GetDataAsync<User>("Users");
            //var orders = await firebaseHelper.GetDataAsync<Order>("Orders");
            var users = await firebaseHelper.GetDataAsyncUsers();
            var orders = await firebaseHelper.GetDataAsyncOrders();
            

            
            foreach(var sdf in users)
            {
                Console.WriteLine(" my users keys = " + sdf.Key);
            }
            
            foreach (var order in orders) {

                Console.WriteLine(" my x.Key " + order.UserKey);

              var user = users.FirstOrDefault(x => x.Key == order.UserKey);
                Console.WriteLine(" my user == " + user.Name);


                order.UserName = user.Name;
            }
            OrdersDataGrid.ItemsSource = orders;
        }

        private void UsersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem is User selectedUser)
            {                
                UserEditWindow taskWindow = new UserEditWindow(selectedUser);
                taskWindow.ShowDialog();
            }
        }

        
        private async void Button_Click_Delete(object sender, RoutedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem is Order selected)
            {
                var result = new CustomMessageBox("Вы уверены, что хотите удалить заказ?").ShowDialog();
                if (result.HasValue && result.Value)
                {
                    await firebaseHelper.DeleteOrder(selected);
                    await LoadOrdersAsync(); // Обновление списка пользователей после удаления
                }
            }
            else
            {
                new CustomMessageBox("Пожалуйста, выберите заказ для удаления.").ShowDialog();
            }
        }

        private async void Button_Click_Edit(object sender, RoutedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem is Order selected)
            {
                var editWindow = new OrderEditWindow(selected);
                editWindow.ShowDialog();
                string closeReason = editWindow.CloseReason;
                if (closeReason == "Saved")
                {
                    await LoadOrdersAsync();
                }
            }
            else
            {
                new CustomMessageBox("Пожалуйста, выберите заказ для редактирования.").ShowDialog();
            }
        }

        private async void Button_Click_NewUser(object sender, RoutedEventArgs e)
        {
            OrderEditWindow taskWindow = new OrderEditWindow();
            taskWindow.ShowDialog();
            string closeReason = taskWindow.CloseReason;
            if (closeReason == "Saved")
            {
                await LoadOrdersAsync();
            }
        }
    }
}
