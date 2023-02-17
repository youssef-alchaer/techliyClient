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
using TechliyApp.MVVM.Model;

namespace techliyClient
{
    public class FireStoreFunctions
    {
        

        //set GOOGLE_APPLICATION_CREDENTIALS="C:\Users\ysf\source\Repos\techliyClient\techliyClient\techliy37272303f98c730b41.json"

        private readonly string path = AppDomain.CurrentDomain.BaseDirectory + @"techliy-firebase-adminsdk-f61b6-6d09b4d37e.json";

        private FirestoreDb db;

        public FireStoreFunctions()
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
            db = FirestoreDb.Create("techliy");
        }

       public async Task SendData(Dictionary<string, object> data , string Collection , string Document )
        {
            Program.NumberOfWrites++;

            DocumentReference docRef = db.Collection(Collection).Document(Document);
            try
            {
                await docRef.UpdateAsync(data);
            }
            catch (Exception)
            {
                foreach (var item in data)
                    Console.WriteLine($"Adding New Field '{item.Key}' with value '{item.Value}' ");

                await docRef.SetAsync(data);
            }


        }


        /// <summary>
        /// Checks if any document inside of a collection has a Feild that equals the value
        /// </summary>
        /// <param name="Collection"> any collection Like 'Clients' </param>
        /// <param name="Field"> Like 'MachineName'</param>
        /// <param name="Value"> value of the Feild to be checked</param>
        /// <returns></returns>
        public async Task<bool> Exists(string Collection, string Document ,  string Field ,  object Value)
        {
            Program.NumberOfReads++;

            CollectionReference docRef = db.Collection(Collection);

            Query query = docRef.WhereEqualTo(Field, Value).WhereEqualTo(nameof(ClientPC.MachineName) , Document);

            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

            
            return querySnapshot.Any();

        }

    }
}
