using Google.Cloud.Firestore;
using Google.Type;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace techliyClient
{
    public class fireStoreFunctions
    {
        

        //set GOOGLE_APPLICATION_CREDENTIALS="C:\Users\ysf\source\Repos\techliyClient\techliyClient\techliy37272303f98c730b41.json"

        private readonly string path = AppDomain.CurrentDomain.BaseDirectory + @"techliy-firebase-adminsdk-f61b6-6d09b4d37e.json";

        private FirestoreDb db;

        public fireStoreFunctions()
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
            db = FirestoreDb.Create("techliy");
        }

       public async void sendData(Dictionary<string, object> data , string Collection , string Document)
        {
           DocumentReference docRef = db.Collection(Collection).Document(Document);
            try
            {
                await docRef.UpdateAsync(data);
            }
            catch (Exception)
            {
                Console.WriteLine("Document does not exsist, creating new one..");
                await docRef.SetAsync(data);
            }
        }


    }
}
