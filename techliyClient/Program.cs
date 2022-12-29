// See https://aka.ms/new-console-template for more information
using Google.Cloud.Firestore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Management;
using techliyClient;
using System.Diagnostics;

Console.WriteLine("Techliy Client Started");

fireStoreFunctions cloud = new fireStoreFunctions();

#region Background tasks
async Task RunInBackground(TimeSpan timeSpan, Action action)
{
    var periodicTimer = new PeriodicTimer(timeSpan);
    while (await periodicTimer.WaitForNextTickAsync())
    {
        action();
    }
}
#endregion


RunInBackground(TimeSpan.FromSeconds(2), () => getRam(cloud));


cloud.sendData(new Dictionary<string, object> { { "Ram Useage", percent.ToString() } }, "Clients", "AMD Desktop");







Console.ReadLine();