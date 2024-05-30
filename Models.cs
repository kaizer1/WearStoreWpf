using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Media.Imaging;
namespace WearStoreWpf
{
    public interface IHasKey
    {
        string Key { get; set; }
    }

    public class User : IHasKey
    {
        [JsonIgnore] 
        public string Key { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; } 
    }
    public class Categories : IHasKey
    {
        [JsonIgnore]
        public string Key { get; set; }
        public string Name { get; set; }
       
    }
    public class Product : IHasKey
    {
        [JsonIgnore]
        public string Key { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Discount { get; set; }
        public double Price { get; set; }

        public string ImageString { get; set; }
        public string Category { get; set; } // Категория продукта
        public object StockBySize { get; set; }  // Количество товаров по размерам
        [JsonIgnore]
        public BitmapImage Image { get; set; } = null;

        [JsonIgnore]
        public double DiscountedPrice
        {
            get { return Price * (1 - (Discount / 100.0)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class Order : IHasKey
    {
        [JsonIgnore]
        public string Key { get; set; }
        
        public string UserKey { get; set; }
        [JsonIgnore]
        public string UserName { get; set; }

        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
        public string Status { get; set; } // Например, New, Processing, Shipped, Delivered
        public double TotalAmount { get; set; } //Итоговая стоимость
    }

    public class OrderItem
    {
        public string ProductKey { get; set; }
        [JsonIgnore]
        public string ProductName { get; set; } = null;
        public int Quantity { get; set; }
        public string Size { get; set; }
        public double Price { get; set; }
    }

}