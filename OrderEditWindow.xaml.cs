using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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

namespace WearStoreWpf
{
    /// <summary>
    /// Логика взаимодействия для OrderEditWindow.xaml
    /// </summary>
    /// 

    public partial class OrderEditWindow : Window
    {
        private Order newOrder = new Order();
        private FirebaseClientHelper firebaseHelper = new FirebaseClientHelper();
        private Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
        private Dictionary<string, string> keyValueProduct = new Dictionary<string, string>();
        private List<Product> products = new List<Product>();
        public ObservableCollection<OrderItem> orderItems { get; set; }
        private Order currentOrder;
        public string CloseReason { get; private set; }
        public OrderEditWindow(Order order = null)
        {
            InitializeComponent();
            Icon = ConvertIconToImageSource(Properties.Resources.icon);
            LoadComponents();
            currentOrder = order;
            orderItems = new ObservableCollection<OrderItem>();
            newOrder.TotalAmount = 0;
            DataContext = this;

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
        public async void LoadComponents()
        {
            
            var users = await firebaseHelper.GetDataAsync<User>("users");
                foreach (var el in users)
                    keyValuePairs.Add( el.Name, el.Key);
            UserComboBox.ItemsSource = keyValuePairs.Keys;

            StatusComboBox.Items.Add("Оформлен");
            StatusComboBox.Items.Add("Доставляется");
            StatusComboBox.Items.Add("Доставлен");
            StatusComboBox.Items.Add("Отменен");
            TotalAmountTextBox.Text = "0";
            products = await firebaseHelper.GetDataAsync<Product>("products");
            foreach (var el in products)
                keyValueProduct.Add(el.Name, el.Key);
            ProductComboBox.ItemsSource = keyValueProduct.Keys;

            if (currentOrder!=null)
            {
                newOrder = currentOrder;
                UserComboBox.SelectedItem = keyValuePairs.FirstOrDefault(x => x.Value == currentOrder.UserKey).Key;
                StatusComboBox.SelectedItem = currentOrder.Status;
                TotalAmountTextBox.Text = currentOrder.TotalAmount.ToString();

                orderItems.Clear();
                foreach (var item in currentOrder.Items)
                {
                    string productName = keyValueProduct.FirstOrDefault(x => x.Value == item.ProductKey).Key;
                    OrderItem newItem = new OrderItem
                    {
                        ProductKey = item.ProductKey,
                        ProductName = productName,
                        Quantity = item.Quantity,
                        Size = item.Size,
                        Price = item.Price
                    };
                    orderItems.Add(newItem);
                }
            }

        }

        private async void AddProduct(object sender, RoutedEventArgs e)
        {
            string selectedKey = ProductComboBox.SelectedItem.ToString();
            string selectedValue = keyValueProduct[selectedKey];
            var currentProductKey = products.FirstOrDefault(x => x.Key == selectedValue);
           
            OrderItem newItem = new OrderItem { ProductKey = selectedValue,ProductName= currentProductKey.Name, Quantity = Convert.ToInt32(CountTextBox.Text), Size= SizeComboBox.SelectedValue.ToString(), Price = currentProductKey.Price* Convert.ToInt32(CountTextBox.Text) };
            
            newOrder.Items.Add(newItem);
            orderItems.Add(newItem);

            double totalPrice = 0;
            foreach (var el in orderItems)
                totalPrice += el.Price;
            double discount=0;
            if (UserComboBox.SelectedItem != null)
            {
                string selectedKey1 = UserComboBox.SelectedItem.ToString();
                string selectedValue1 =keyValuePairs[selectedKey1];
                var users = await firebaseHelper.GetDataAsync<User>("users");
            }
            double discountAmount = (totalPrice * discount) / 100;

            // Расчет итоговой цены
            double finalPrice = totalPrice - discountAmount;

            TotalAmountTextBox.Text = finalPrice.ToString();
        }

        private async void ProductComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProductComboBox.SelectedItem != null)
            {
                string selectedKey = ProductComboBox.SelectedItem.ToString();
                string selectedValue = keyValueProduct[selectedKey];

                SizeComboBox.Items.Clear();
               var currentProduct = products.FirstOrDefault(x => x.Key == selectedValue);
                if (currentProduct.StockBySize is JObject stockObject)
                {
                    // Преобразуем JObject в словарь
                    Dictionary<string, int> stockDictionary = stockObject.ToObject<Dictionary<string, int>>();

                    // Теперь вы можете использовать stockDictionary для доступа к данным о запасах
                    foreach (var pair in stockDictionary)
                    {
                        string size = pair.Key;
                        int quantity = pair.Value;
                        SizeComboBox.Items.Add(size);
                    }
                }
            }
        }

        private async void SaveOrderd(object sender, RoutedEventArgs e)
        {
            FirebaseClientHelper firebase = new FirebaseClientHelper();
            string selectedKey = UserComboBox.SelectedItem.ToString();
            string selectedValue = keyValuePairs[selectedKey];
            if (currentOrder == null)
            {
                
                await firebase.NewOrder(new Order { Key= Guid.NewGuid().ToString(), UserKey = selectedValue , Items= newOrder.Items, Status= StatusComboBox.SelectedValue.ToString(), TotalAmount=Convert.ToDouble(TotalAmountTextBox.Text)});
                CloseReason = "Saved";
                this.Close();
                new CustomMessageBox($"Заказ создан").ShowDialog();
            }
            else
            {
                await firebase.EditOrder(new Order { Key = newOrder.Key, UserKey = selectedValue, Items = newOrder.Items, Status = StatusComboBox.SelectedValue.ToString(), TotalAmount = Convert.ToDouble(TotalAmountTextBox.Text) });
                CloseReason = "Saved";
                this.Close();
                new CustomMessageBox($"Заказ изменен").ShowDialog();
            }
        }
    }

   
}
