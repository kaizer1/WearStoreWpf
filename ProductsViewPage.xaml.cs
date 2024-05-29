//using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для ProductsViewPage.xaml
    /// </summary>
    public partial class ProductsViewPage : Page
    {
        private FirebaseClientHelper firebaseHelper = new FirebaseClientHelper();
        
        public ProductsViewPage()
        {
            InitializeComponent();
            LoadProductsAsync();
        }
        
        private async void LoadProductsAsync()
        {
            productsList.Visibility = Visibility.Hidden;
            var products = await firebaseHelper.GetDataAsync<Product>("products");
            progressBar.Visibility = Visibility.Visible;
            progressBar.Maximum = products.Count;
            progressBar.Value = 0;
            foreach (var product in products)
            {
                progressBar.Value++;
                BitmapImage image = await GetImageFromStorage(product.Key);
                product.Image = image;
                
            }
            progressBar.Visibility = Visibility.Hidden;

            productsList.ItemsSource = products;
            productsList.Visibility = Visibility.Visible;
        }
        private async Task<BitmapImage> GetImageFromStorage(string imageName)
        {
            string projectId = "shopbase-b8fc9";
            string bucketName = "shopbase-b8fc9.appspot.com";
            FirebaseStorageUploader uploader = new FirebaseStorageUploader(projectId, bucketName);
            var imageStream = await uploader.DownloadFileAsync(imageName);
            
            return imageStream;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ProductEditWindow taskWindow = new ProductEditWindow();
            taskWindow.ShowDialog();
            string closeReason = taskWindow.CloseReason;
            if (closeReason == "Saved")
            {
                LoadProductsAsync();
            }
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            if (productsList.SelectedItem is Product selectedProduct)
            {
                var editWindow = new ProductEditWindow(selectedProduct);
                editWindow.ShowDialog(); // Открываем окно модально

                string closeReason = editWindow.CloseReason;
                if (closeReason == "Saved")
                {
                    LoadProductsAsync();
                }
               
            }
            else
            {
                new CustomMessageBox("Пожалуйста, выберите товар для редактирования.").ShowDialog();
            }
        }

        private async void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (productsList.SelectedItem is Product selectedProduct)
            {
                var result = new CustomMessageBox("Вы уверены, что хотите удалить выбранный товар?").ShowDialog();
                if (result.HasValue && result.Value)
                {
                    await firebaseHelper.DeleteProduct(selectedProduct);
                    LoadProductsAsync(); 
                }

            }
            else
            {
                new CustomMessageBox("Пожалуйста, выберите товар для удаления.").ShowDialog();
            }
        }
    }

}
