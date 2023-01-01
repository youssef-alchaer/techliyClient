// See https://aka.ms/new-console-template for more information
using Google.Cloud.Firestore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Management;
using techliyClient;
using System.Diagnostics;
using techliyClient.Functions;
using System.Drawing;


//https://github.com/clowd/Clowd.Squirrel

Console.WriteLine("Techliy Client Started");

fireStoreFunctions cloud = new fireStoreFunctions();

var os = new OperatingSystemInfo();

var info = new Dictionary<string, string>() { 
    {nameof(os.Caption), os.Caption }, 
    {nameof(os.ProcessorName), os.ProcessorName }, 
    {nameof(os.OperatingSystemArchitecture) , os.OperatingSystemArchitecture } 
};

cloud.sendData(info, "Clients");

Console.WriteLine();

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


Console.WriteLine();

 /*await RunInBackground(TimeSpan.FromSeconds(2), 
     action: 
     () => cloud.sendData(new Dictionary<string, object> 
     {
         {"Ram Useage", ClientFunctions.getRam().ToString() } 
     },
     "Clients", 
     "AMD Desktop")
     );*/

Console.Read();



//cloud.sendData(new Dictionary<string, object> { { "Ram Useage", percent.ToString() } }, "Clients", "AMD Desktop");







Console.ReadLine();