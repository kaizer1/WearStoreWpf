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
using System.Diagnostics;
using System.Windows.Forms.VisualStyles;
using Grpc.Core;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using Google.Cloud.Firestore.V1;
using Google.Apis.Util;
//using System.Windows.Documents;




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


        public static double ConvertToDouble(string Value)
        {
            if (Value == null)
            {
                return 0;
            }
            else
            {
                double OutVal;
                double.TryParse(Value, out OutVal);

                if (double.IsNaN(OutVal) || double.IsInfinity(OutVal))
                {
                    return 0;
                }
                return OutVal;
            }
        }


        public async Task<List<Order>> GetDataAsyncOrders()
        {

            List<Order> orders = new List<Order>();
            CollectionReference userRef = db.Collection("Orders");

            var items = await userRef.GetSnapshotAsync();

            foreach (DocumentSnapshot item in items.Documents)
            {

                Order testUser = new Order();
                Dictionary<string, object> documentDictionary = item.ToDictionary();

                testUser.Key = item.Id;

               

                foreach (var person in documentDictionary)
                {
                    List<OrderItem> orItems = new List<OrderItem>();

                    if (person.Key == "address")
                    {
                       

                        var dsfkj = person.Value as Dictionary<string, object>;

                        foreach( var ii in dsfkj)
                        {
                          
                            if(ii.Key == "Name")
                            {
                                testUser.UserName = ii.Value.ToString();
                            }
                        }
                        
                    }
                    

                    if(person.Key == "status")
                    {
                        testUser.Status = person.Value.ToString();
                    }

                    if(person.Key == "totalAmount")
                    {
                        // double 
                        double DirectExpense = ConvertToDouble(person.Value.ToString());

                        //testUser.TotalAmount = person.Value.ToString();   
                    }


                    if (person.Key == "userId")
                    {
                        testUser.UserKey = person.Value.ToString();
                    }




                    if(person.Key == "items")
                    {

                        Console.WriteLine(" sPP =  " + person.Value.ToString());

                        var kdfj = person.Value as List<Object>;

                       // foreach (var ii in kdfj)
                       // {
                         //   Console.WriteLine(" mt sdf = " + ii.ToString());
                       // }

                        Console.WriteLine(" my values == " + kdfj.Count);

                        foreach (var ii in kdfj)
                        {
                            
                            var sdlfkjj  = ii as Dictionary<string, object>;

                            foreach (var dkfjjjdkkd in sdlfkjj)
                            {

                                OrderItem itemCurrent = new OrderItem();

                                if (dkfjjjdkkd.Key == "productId")
                                {
                                    itemCurrent.ProductKey = dkfjjjdkkd.Value.ToString();
                                }

                                if (dkfjjjdkkd.Key == "brandName")
                                {
                                    itemCurrent.ProductName = dkfjjjdkkd.Value.ToString();
                                }

                                // price
                                if (dkfjjjdkkd.Key == "price")
                                {
                                    itemCurrent.Price = ConvertToDouble(dkfjjjdkkd.Value.ToString());
                                }

                                // quantity
                                if (dkfjjjdkkd.Key == "quantity")
                                {
                                    itemCurrent.Quantity = Int32.Parse(dkfjjjdkkd.Value.ToString());
                                }

                                if (dkfjjjdkkd.Key == "selectedVariation")
                                {

                                    if(dkfjjjdkkd.Value == null)
                                    {
                                        itemCurrent.Size = "";
                                    }
                                    else
                                    {
                                        var sdlkfjs = dkfjjjdkkd.Value as Dictionary<string, object>;

                                        foreach (var jj in sdlkfjs)
                                        {
                                            if (jj.Key == "Size")
                                            {
                                                itemCurrent.Size = jj.Value.ToString();
                                            }
                                        }
                                    }




                                
                                }

                                orItems.Add(itemCurrent);
                            }
                        }

                    }


                    testUser.Items = orItems;
                }
        
                orders.Add(testUser);
            }

                return orders;
        }

        public async Task<List<Product>> GetDataAsyncProduct()
        {

            List<Product> products = new List<Product>();
            CollectionReference userRef = db.Collection("Products");
       
            var items = await userRef.GetSnapshotAsync();

            foreach (DocumentSnapshot item in items.Documents)
            {


                Product testUser = new Product();
                Dictionary<string, object> documentDictionary = item.ToDictionary();

                testUser.Key = item.Id;


                foreach (var person in documentDictionary)
                {
                    //    int        object       BitmapImage
                    //  Discount    StockBySize     Image 

                    if(person.Key == "Brand")
                    {
                        testUser.Name = person.Value.ToString();
                        Console.WriteLine(" my brand valu = " + person.Value.ToString());
                        var dkjf = person.Value as Dictionary<string, object>;
                        foreach (var key in dkjf)
                        {
                           // Console.WriteLine($" df " + key.Key + " and " + key.Value);
                            if(key.Key == "Name")
                            {
                                testUser.Name = key.Value.ToString();
                            }


                            if(key.Key == "Image")
                            {
                                testUser.ImageString = key.Value.ToString();
                            }
                         }


                    }


                    if(person.Key == "Description")
                    {
                        testUser.Description = person.Value.ToString();
                    }


                    if(person.Key == "Price")
                    {
                        //double v = (double) person.Value;
                        testUser.Price = ConvertToDouble(person.Value.ToString());
                    }


                    if(person.Key == "CategoryId")
                    {
                        testUser.Category = person.Value.ToString();
                    }



                }
        

    

                products.Add(testUser);
            }


                return products;
        }

        public async Task<List<User>> GetDataAsyncUsers()
        {

            List<User> users = new List<User>();

            CollectionReference userRef = db.Collection("Users");
            //Console.WriteLine(" in users loading !! 22 ", childNode);
            var items = await userRef.GetSnapshotAsync();


            foreach (DocumentSnapshot item in items.Documents)
            {


                User testUser = new User();
                Dictionary<string, object> documentDictionary = item.ToDictionary();

                testUser.Key = item.Id;

                //foreach (KeyValuePair<string, object> pair in documentDictionary)
                //{


                    foreach (var person in documentDictionary)
                    {

                        if (person.Key == "FirstName")
                        {
                           // Console.WriteLine(" is a name ");
                            testUser.Name = person.Value.ToString();

                        }

                        if(person.Key == "LastName")
                        {
                            testUser.Name += " " + person.Value.ToString();
                        }

                        if(person.Key == "Email")
                        {
                         //   Console.WriteLine(" is a email ");
                            testUser.Email  = person.Value.ToString();
                        }


                        if(person.Key == "PhoneNumber")
                        {
                            testUser.Phone = person.Value.ToString();
                        }

                       // Console.WriteLine(" in doc = " + person.Key + " value = " + person.Value);
                    }
                //}
               users.Add(testUser); 
            }

                return users;
        }









        public async Task<List<T>> GetDataAsync<T>(string childNode) where T : IHasKey, new()
        {

            //Console.WriteLine(" in users loading !! " , childNode);
          //  Console.ReadKey();
            CollectionReference userRef = db.Collection(childNode);
            //Console.WriteLine(" in users loading !! 22 ", childNode);
            var items = await userRef.GetSnapshotAsync();

            T sdf = default;


            Console.WriteLine( " my type == " +  typeof(T).FullName);

            

           

            //var df = items as Object;
            


            List<T> stringFinal = new List<T>();

            //Console.WriteLine(" in users loading !! 33" + childNode);

            //Console.WriteLine(" count = $ " + items.Count);
            foreach (DocumentSnapshot item in items.Documents)
            {


                // Console.WriteLine(" myID = " + item.Id);
                // stringFinal.Add(item.Id.ToString());

                // item.GetValue
                // Object df = item.GetValue("", "");

                //T key = default;



              //  sdf.Key = item.Id;
              //  User asdas = new User();

                if (sdf is WearStoreWpf.User)
                {
                    Console.WriteLine(" my value is User");

                   // T sdff = new WearStoreWpf.User();


                }

                if (sdf is WearStoreWpf.Product)
                {
                    Console.WriteLine(" my value is Product");
                }




                // Console.WriteLine( " sdfasd lksdjflkasdjfklsajdk = " + item.ToString());
                //var dsf = item.TryGetValue("", out key);
                //Console.WriteLine(" my key and = " + key.ToString());

                //stringFinal.Add(item);
                Dictionary<string, object> documentDictionary = item.ToDictionary();

                foreach ( KeyValuePair<string, object> pair in documentDictionary)
                {
                   // Console.WriteLine("dsf ==KKW = "+ pair.Key);
                    //stringFinal.Add(pair)
                    
                    //sdf.Key = pair.Key;
                    
                }

                //var key = documentDictionary.Keys.FirstOrDefault(x => x.Equals("one"));
                //Console.WriteLine(" my key = " + key);
         

                foreach( var person in documentDictionary)
                {

                    if(person.Key == "Name")
                    {
                      //  Console.WriteLine(" is a name ");
                    }

                    //Console.WriteLine(" in doc = " + person.Key + " value = " + person.Value);
                }

               // Console.WriteLine(" my doc count == " + documentDictionary.ToString());
               // Console.WriteLine(" my keys = " +  documentDictionary.Keys);
                
               // Console.WriteLine(" my values == " + documentDictionary.Values);
                

                
                
                //System.Diagnostics.Debug.WriteLine(" my doc 2 ", documentDictionary.Values);
                //Debug.WriteLine("sdf sdfkjkhjsd ", documentDictionary.Values);

                //Debug.Print("aaaaaaaaaaa ");
                //System.Diagnostics.Trace.WriteLine("message sdfkjklsdfj");
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


            return stringFinal as List<T>;
            //return new List<T>();
          
            //return items.Select(item => item.Object).ToList();
        }


        public async Task NewUser(User user)
        {
            var key = Guid.NewGuid().ToString();
            CollectionReference userRef = db.Collection("Users"); 
           
            await userRef.Document(key).SetAsync(user);
            //await firebaseClient.Child("users").Child(key).PutAsync(user);
        }

        public async Task EditUser(User user)
        {

            //Console.WriteLine(" start save users ! ");
            var key = Guid.NewGuid().ToString();
           // Console.WriteLine($"m k = {key}  m Use = {user.Name}");
            CollectionReference userRef = db.Collection("Users");

            string input = user.Name;
            string fiName = "";
            string laName = "";
            int index = input.IndexOf(" ");
            if (index >= 0)
                fiName = input.Substring(0, index);
                laName = input.Substring(index + 1);


            Dictionary<string, object> userRewrite = new Dictionary<string, object>
        {
    { "FirstName", fiName},
    { "LastName", laName },
    { "Email", user.Email },
    { "PhoneNumber", user.Phone }
        };

            await userRef.Document(user.Key).SetAsync(userRewrite);
            

            //  await firebaseClient.Child("users").Child(user.Key).PutAsync(user);
        }

        public async Task DeleteUserAsync(User user)
        {
            var key = Guid.NewGuid().ToString();
            CollectionReference userRef = db.Collection("Users");

            await userRef.Document(user.Key).DeleteAsync();
        }

        
        public async Task NewProduct(Product product)
        {
            var key = product.Key;


            //var key = Guid.NewGuid().ToString();
            CollectionReference userRef = db.Collection("Products");

            await userRef.Document(key).SetAsync(product);
            //await firebaseClient.Child("products").Child(key).PutAsync(product);
        }
        public async Task EditProduct(Product product)
        {
            CollectionReference userRef = db.Collection("Products");

            await userRef.Document(product.Key).SetAsync(product);
            //await firebaseClient.Child("products").Child(product.Key).PutAsync(product);
        }
        public async Task DeleteProduct(Product product)
        {
            CollectionReference userRef = db.Collection("Products");

            await userRef.Document(product.Key).DeleteAsync();
            //await firebaseClient.Child("products").Child(product.Key).DeleteAsync();
        }

        public async Task NewOrder(Order order)
        {
            var key = order.Key;

            CollectionReference userRef = db.Collection("Orders");

            await userRef.Document(key).SetAsync(order);

           // await firebaseClient.Child("orders").Child(key).PutAsync(order);

            //уменьшение количества товара на складе
            var products = await GetDataAsync<Product>("Products");
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

            CollectionReference userRef = db.Collection("Orders");

            await userRef.Document(order.Key).DeleteAsync();

            //await firebaseClient.Child("orders").Child(order.Key).DeleteAsync();
            //увелечение количества товара на складе
            var products = await GetDataAsync<Product>("Products");
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

            CollectionReference userRef = db.Collection("Categories");

            await userRef.Document(key).SetAsync(categories);

            //await firebaseClient.Child("categories").Child(key).PutAsync(categories);
        }
        public async Task<List<string>> GetCategory()
        {
            var categories = await GetDataAsync<Categories>("Categories");
            List<String> list = new List<string>();

            foreach (var el in categories)
                list.Add(el.Name);
            return list;
        }
    }
}