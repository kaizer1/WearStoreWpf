//using Firebase.Database;
//using Firebase.Database.Query;

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Google.Cloud.Firestore;
using Google.Apis.Auth.OAuth2;
using System.ComponentModel.Composition.Primitives;
using System.Net;





namespace WearStoreWpf
{

  

       public class FirebaseClientHelper
    {

  


        FirestoreDb db;
        private const string FirebaseUrl = "https://shopbase-b8fc9-default-rtdb.europe-west1.firebasedatabase.app/"; // Замените на URL вашей базы данных
       // private const string AuthSecret = "YOUR_FIREBASE_AUTH_SECRET"; // Опционально, если ваша база защищена
        //private FirebaseClient firebaseClient;

        public FirebaseClientHelper()
        {

            //export GOOGLE_APPLICATION_CREDENTIALS = "path/to/your/keyfile.json";

            //GoogleCredential credential = GoogleCredential.FromFile("shopbase-b8fc9-firebase-adminsdk-fp736-94ba20aaef.json");

            //Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credential);

            
            
            string path = @"Q:\DownloadsMain\TelegramTemp\WearStoreWpf\WearStoreWpf\bin\Debug\shopbase-b8fc9-firebase-adminsdk-fp736-94ba20aaef.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);

            db = FirestoreDb.Create("shopbase-b8fc9"); // project 

            //firebaseClient = new FirebaseClient(
               // FirebaseUrl,
               // new FirebaseOptions
               // {
                    //AuthTokenAsyncFactory = () => Task.FromResult(AuthSecret)
              //  });
        }
        // Метод для добавления данных
        public async Task AddDataAsync<T>(string childNode, T data)
        {
            DocumentReference docRef = db.Collection("").Document(childNode);
            await docRef.SetAsync(data);
                
                // firebaseClient
                // .Child(childNode)
                // .PostAsync(data);
        }

        // Метод для получения данных
        public async Task<List<T>> GetDataAsync<T>(string childNode) where T : IHasKey, new()
        {
            CollectionReference userRef = db.Collection(childNode);

            var items = await userRef.GetSnapshotAsync();

             foreach(DocumentSnapshot item in items.Documents)
            {
                Dictionary<string, object> documentDictionary = item.ToDictionary();

                Console.WriteLine(" my doc == ", documentDictionary.Values);
                // documentDictionary.ContainsKey;
                //documentDictionary.Keys. = documentDictionary.ContainsKey;

            }

            //    .Child(childNode)
            //    .OnceAsync<T>();

            // Сохранение ключа каждого элемента в объекте
            // foreach (var item in items)
            // {
            //    item.Object.Key = item.Key;
            //}

            return new List<T>();
          
            //return items.Select(item => item.Object).ToList();
        }


        public async Task NewUser(User user)
        {
            var key = Guid.NewGuid().ToString();
            CollectionReference userRef = db.Collection("users"); 
           
            await userRef.Document(key).SetAsync(user);
            //await firebaseClient.Child("users").Child(key).PutAsync(user);
        }

        public async Task EditUser(User user)
        {

            var key = Guid.NewGuid().ToString();
            CollectionReference userRef = db.Collection("users");

            await userRef.Document(user.Key).SetAsync(user);


            //  await firebaseClient.Child("users").Child(user.Key).PutAsync(user);
        }

        public async Task DeleteUserAsync(User user)
        {

            var key = Guid.NewGuid().ToString();
            CollectionReference userRef = db.Collection("users");

            await userRef.Document(key).DeleteAsync();

          //  await firebaseClient.Child("users").Child(user.Key).DeleteAsync();
        }

        
        public async Task NewProduct(Product product)
        {
            var key = product.Key;


            //var key = Guid.NewGuid().ToString();
            CollectionReference userRef = db.Collection("products");

            await userRef.Document(key).SetAsync(product);
            //await firebaseClient.Child("products").Child(key).PutAsync(product);
        }
        public async Task EditProduct(Product product)
        {
            CollectionReference userRef = db.Collection("products");

            await userRef.Document(product.Key).SetAsync(product);
            //await firebaseClient.Child("products").Child(product.Key).PutAsync(product);
        }
        public async Task DeleteProduct(Product product)
        {
            CollectionReference userRef = db.Collection("products");

            await userRef.Document(product.Key).DeleteAsync();
            //await firebaseClient.Child("products").Child(product.Key).DeleteAsync();
        }

        public async Task NewOrder(Order order)
        {
            var key = order.Key;

            CollectionReference userRef = db.Collection("orders");

            await userRef.Document(key).SetAsync(order);

           // await firebaseClient.Child("orders").Child(key).PutAsync(order);

            //уменьшение количества товара на складе
            var products = await GetDataAsync<Product>("products");
            foreach (var prod in order.Items)
            {
                var newProd = products.FirstOrDefault(x => x.Key == prod.ProductKey);
                if (newProd != null)
                {
                    if (newProd.StockBySize is Newtonsoft.Json.Linq.JObject stockObject)
                    {
                        var stockBySize = stockObject.ToObject<Dictionary<string, int>>();

                        string size = prod.Size;
                        if (stockBySize.ContainsKey(size))
                        {
                            int currentQuantity = stockBySize[size];
                            var newQuantity = Math.Max(currentQuantity - prod.Quantity, 0); // Принудительно делаем новое количество неотрицательным
                            stockBySize[size] = newQuantity;
                        }
                        newProd.StockBySize = Newtonsoft.Json.Linq.JObject.FromObject(stockBySize);

                        await EditProduct(newProd);

                    }
                }
            }



        }
        public async Task EditOrder(Order order)
        {
            CollectionReference userRef = db.Collection("orders");

            await userRef.Document(order.Key).SetAsync(order);

          //  await firebaseClient.Child("orders").Child(order.Key).PutAsync(order);
        }
        public async Task DeleteOrder(Order order)
        {

            CollectionReference userRef = db.Collection("orders");

            await userRef.Document(order.Key).DeleteAsync();

            //await firebaseClient.Child("orders").Child(order.Key).DeleteAsync();
            //увелечение количества товара на складе
            var products = await GetDataAsync<Product>("products");
            foreach (var prod in order.Items)
            {
                var newProd = products.FirstOrDefault(x => x.Key == prod.ProductKey);
                if (newProd != null)
                {
                    if (newProd.StockBySize is Newtonsoft.Json.Linq.JObject stockObject)
                    {
                        var stockBySize = stockObject.ToObject<Dictionary<string, int>>();

                        string size = prod.Size;
                        if (stockBySize.ContainsKey(size))
                        {
                            int currentQuantity = stockBySize[size];
                            var newQuantity = Math.Max(currentQuantity + prod.Quantity, 0); // Принудительно делаем новое количество неотрицательным
                            stockBySize[size] = newQuantity;
                        }
                        newProd.StockBySize = Newtonsoft.Json.Linq.JObject.FromObject(stockBySize);

                        await EditProduct(newProd);

                    }
                }
            }
        }

        public async Task SetCategory(Categories categories)
        {
            var key = Guid.NewGuid().ToString();

            CollectionReference userRef = db.Collection("categories");

            await userRef.Document(key).SetAsync(categories);

            //await firebaseClient.Child("categories").Child(key).PutAsync(categories);
        }
        public async Task<List<string>> GetCategory()
        {
            var categories = await GetDataAsync<Categories>("categories");
            List<String> list = new List<string>();

            foreach (var el in categories)
                list.Add(el.Name);
            return list;
        }
    }
}