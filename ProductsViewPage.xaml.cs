//using Google.Cloud.Storage.V1;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
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
            //var products = await firebaseHelper.GetDataAsync<Product>("Products");
            var products = await firebaseHelper.GetDataAsyncProduct();
            progressBar.Visibility = Visibility.Visible;
            progressBar.Maximum = products.Count;
            progressBar.Value = 0;
            foreach (var product in products)
            {
                //progressBar.Value++;
                //BitmapImage image = await GetImageFromStorage(product.ImageString); // was product.key ImageString
                //product.Image = image;

                if (product.ImageString != null)
                {

                    Bitmap imagee = Base64StringToBitmap(product.ImageString);
                    BitmapImage finalA = ConvertBIM(imagee);

                    product.Image = finalA;
                }


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




        private BitmapImage ConvertBIM(Bitmap bmp)
        {

            var bitmapData = bmp.LockBits(
              new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
              System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);

            var bitmapSource = BitmapSource.Create(
               bitmapData.Width, bitmapData.Height, 96, 96, PixelFormats.Bgr24, null,
               bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

            bmp.UnlockBits(bitmapData);
            return bitmapSource as BitmapImage;
        }

        private BitmapImage Bitmap2BitmapImage(Bitmap bitmap)
        {
            BitmapSource i = Imaging.CreateBitmapSourceFromHBitmap(
                           bitmap.GetHbitmap(),
                           IntPtr.Zero,
                           Int32Rect.Empty,
                           BitmapSizeOptions.FromEmptyOptions());
            return (BitmapImage)i;
        }


        //private BitmapImage Bitmap2BitmapImage(Bitmap bitmap)
        //{
        //    IntPtr hBitmap = bitmap.GetHbitmap();
        //    BitmapImage retval;

        //    try
        //    {
        //        retval = (BitmapImage)Imaging.CreateBitmapSourceFromHBitmap(
        //                     hBitmap,
        //                     IntPtr.Zero,
        //                     Int32Rect.Empty,
        //                     BitmapSizeOptions.FromEmptyOptions());
        //    }
        //    finally
        //    {
        //        //DeleteObject(hBitmap);
        //    }

        //    return retval;
        //}


        public Bitmap Base64StringToBitmap( string
                                       base64String)
        {
            Bitmap bmpReturn = null;


            byte[] byteBuffer = Convert.FromBase64String(base64String);
            MemoryStream memoryStream = new MemoryStream(byteBuffer);


            memoryStream.Position = 0;


            bmpReturn = (Bitmap)Bitmap.FromStream(memoryStream);


            memoryStream.Close();
            memoryStream = null;
            byteBuffer = null;


            return bmpReturn;
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
