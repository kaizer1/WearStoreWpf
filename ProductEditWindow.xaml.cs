using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Path = System.IO.Path;

namespace WearStoreWpf
{

    /// <summary>
    /// Логика взаимодействия для ProductEditWindow.xaml
    /// </summary>
    public partial class ProductEditWindow : Window
    {

        public string CloseReason { get; private set; }
        string imagePath ;
        string newProductKey = Guid.NewGuid().ToString();
        public Dictionary<string, int> stockBySize { get; set; } = new Dictionary<string, int>();
        Product currentProduct;
        public ICommand AddCommand { get; }
        private ObservableCollection<KeyValuePair<string, int>> _stockBySizeObservable;
        public ObservableCollection<KeyValuePair<string, int>> StockBySizeObservable
        {
            get
            {
                if (_stockBySizeObservable == null)
                {
                    _stockBySizeObservable = new ObservableCollection<KeyValuePair<string, int>>(stockBySize);
                }
                return _stockBySizeObservable;
            }
        }


        public ProductEditWindow(Product product = null)
        {
            InitializeComponent();
            DataContext = this;
            currentProduct = product;
            AddCommand = new DelegateCommand(AddItem);
            System.Drawing.Bitmap bitmap = Properties.Resources.emptyImage;
            System.Windows.Media.Imaging.BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            pictureBox.Source = bitmapSource;
            Icon = ConvertIconToImageSource(Properties.Resources.icon);
            if (currentProduct != null)
            {
                FillUserData();
            }
            FillCategories();
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
        public async void FillCategories()
        {
            var firebase = new FirebaseClientHelper();
            var cats = await firebase.GetCategory();
            foreach (var category in cats)
            {
                CategoryComboBox.Items.Add(category);
            }
        }
        private void FillUserData()
        {
            nameTextBox.Text = currentProduct.Name;
            descriptionTextBox.Text = currentProduct.Description;
            priceTextBox.Text = currentProduct.Price.ToString();
            CategoryComboBox.SelectedValue = currentProduct.Category.ToString();
            discountTextBox.Text = currentProduct.Discount.ToString();
            pictureBox.Source = currentProduct.Image;

            if (currentProduct.StockBySize is JObject stockObject)
            {
                // Преобразуем JObject в словарь и добавляем в ObservableCollection
                Dictionary<string, int> stockDictionary = stockObject.ToObject<Dictionary<string, int>>();

                // Обновляем _stockBySizeObservable перед добавлением новых элементов
                _stockBySizeObservable = new ObservableCollection<KeyValuePair<string, int>>();

                foreach (var pair in stockDictionary)
                {
                    string size = pair.Key.ToString();
                    int quantity = pair.Value;
                    _stockBySizeObservable.Add(new KeyValuePair<string, int>(size, quantity));
                }
            }
        }


        private void AddItem(object parameter)
        {
            if (!int.TryParse(quantityTextBox.Text, out int quantity))
            {
                new CustomMessageBox("Пожалуйста, введите корректное количество.").ShowDialog();
                return;
            }

            stockBySize.Add(sizeTextBox.Text, quantity);
             _stockBySizeObservable.Add(new KeyValuePair<string, int>(sizeTextBox.Text, quantity));

            sizeTextBox.Clear();
            quantityTextBox.Clear();
        }
        private async void UploadImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Изображения (*.png;)|*.png"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(openFileDialog.FileName);
                System.Windows.Media.Imaging.BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                    bitmap.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
                pictureBox.Source = bitmapSource;
                imagePath = openFileDialog.FileName;
            }

        }

       

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (imagePath != null)
            {
                string projectId = "shopbase-b8fc9";
                string bucketName = "shopbase-b8fc9.appspot.com";
                string localFilePath = imagePath;
                string remoteFileName = newProductKey + Path.GetExtension(imagePath);
                FirebaseStorageUploader uploader = new FirebaseStorageUploader(projectId, bucketName);
                await uploader.UploadFileAsync(localFilePath, remoteFileName, Path.GetExtension(imagePath).Replace(".", ""));
            }

            FirebaseClientHelper firebase = new FirebaseClientHelper();
            try
            {
                if (currentProduct == null)
                {
                    int discount = 0;
                    if (!string.IsNullOrEmpty(discountTextBox.Text))
                    {
                        discount = Convert.ToInt32(discountTextBox.Text);
                    }
                    var product = new Product
                    {
                        Key = newProductKey,
                        Name = nameTextBox.Text,
                        Description = descriptionTextBox.Text,
                        Price = Convert.ToDouble(priceTextBox.Text),
                        Discount = discount,
                        Category = CategoryComboBox.SelectedValue.ToString(),
                        StockBySize = stockBySize

                    };
                    await firebase.NewProduct(product);
                    CloseReason = "Saved";
                    this.Close();
                    new CustomMessageBox($"Товар: {product.Name}, добавлен").ShowDialog();
                }else
                {
                    int discount = 0;
                    if (!string.IsNullOrEmpty(discountTextBox.Text))
                    {
                        discount = Convert.ToInt32(discountTextBox.Text);
                    }
                    currentProduct.Name = nameTextBox.Text;
                    currentProduct.Description = descriptionTextBox.Text;
                    currentProduct.Price = Convert.ToDouble(priceTextBox.Text);
                    currentProduct.Category = CategoryComboBox.SelectedValue.ToString();
                    currentProduct.Discount = discount;
                    currentProduct.StockBySize = stockBySize;
                    await firebase.EditProduct(currentProduct);
                    CloseReason = "Saved";
                    this.Close();
                }
               
            }
            catch (Exception ex)
            {
                new CustomMessageBox($"Ошибка при сохранении пользователя: {ex.Message}").ShowDialog();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
    public class DelegateCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;

        public void Execute(object parameter) => _execute(parameter);
    }
    public class FirebaseStorageUploader
    {
        private readonly string _projectId;
        private readonly string _bucketName;

        public FirebaseStorageUploader(string projectId, string bucketName)
        {
            _projectId = projectId;
            _bucketName = bucketName;
        }
        public async Task<BitmapImage> DownloadFileAsync(string fileName)
        {
            GoogleCredential credential = GoogleCredential.FromFile("shopbase-b8fc9-firebase-adminsdk-fp736-94ba20aaef.json");
            StorageClient storage = StorageClient.Create(credential);

            using (var memoryStream = new MemoryStream())
            {
                // Загружаем файл из Firebase Storage в MemoryStream
                await storage.DownloadObjectAsync(_bucketName, fileName + ".png", memoryStream);
                memoryStream.Position = 0; // Перемещаем указатель потока в начало

                // Создаем объект BitmapImage из MemoryStream
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

               
                // Возвращаем BitmapImage
                return bitmapImage;
            }
        }
        public async Task UploadFileAsync(string localFilePath, string remoteFileName, string type)
        {
            GoogleCredential credential = GoogleCredential.FromFile("shopbase-b8fc9-firebase-adminsdk-fp736-94ba20aaef.json");
            StorageClient storage = StorageClient.Create(credential);

            using (var fileStream = File.OpenRead(localFilePath))
            {
                await storage.UploadObjectAsync(
                    bucket: _bucketName,
                    objectName: remoteFileName,
                    contentType: "image/"+type,
                    source: fileStream
                );
            }
        }

    }
   

}
