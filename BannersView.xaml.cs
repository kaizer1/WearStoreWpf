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
using System.Windows.Shapes;

namespace WearStoreWpf
{


    public partial class BannersView : Page
    {
        private FirebaseClientHelper firebaseHelper = new FirebaseClientHelper();

        public BannersView()
        {
            InitializeComponent();
            Loaded += BannersViewPage_Loaded;
        }


        private async void BannersViewPage_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadBannersAsync();
        }


        private async Task LoadBannersAsync()
        {
            //var users = await firebaseHelper.GetDataAsync<User>("Users");
            var banners = await firebaseHelper.GetDataAsyncBanners();
            Console.WriteLine(" and load banners ! ");
            foreach (var banner in banners)
            {
                Console.WriteLine($"my bannImage = {banner.ImageSrc}");
                var ImImre = await GetImageFromStorage(banner.ImageSrc);
                banner.Image = ImImre;
            }

            BannersDataList.ItemsSource = banners;
        }


        private async Task<BitmapImage> GetImageFromStorage(string imageName)
        {
            string projectId = "shopbase-b8fc9";
            string bucketName = "shopbase-b8fc9.appspot.com";
            FirebaseStorageUploader uploader = new FirebaseStorageUploader(projectId, bucketName);
            var imageStream = await uploader.DownloadFileAsync(imageName);
            Console.WriteLine("ok load images ! ");
            return imageStream;
        }


        
        private async void Button_Click_Load_Image(object sender, RoutedEventArgs e)
        {

        }

    }
}
