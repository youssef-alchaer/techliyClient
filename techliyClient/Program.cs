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

public class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Techliy Client Started");

        fireStoreFunctions cloud = new fireStoreFunctions();

        InitializePC();

        async void InitializePC()
        {
            var os = new OperatingSystemInfo();

            var info = new Dictionary<string, object>() {
    {nameof(os.Caption), os.Caption },
    {nameof(os.ProcessorName), os.ProcessorName },
    {nameof(os.OperatingSystemArchitecture) , os.OperatingSystemArchitecture }
    };

            var exists = await cloud.sendData(info, "Clients", Environment.MachineName);

            if (exists) Console.WriteLine(Environment.MachineName + " was found in the Database");
            else Console.WriteLine(Environment.MachineName + " was NOT found in the Database");


        }

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

        await RunInBackground(TimeSpan.FromSeconds(2),
            action:
            () => cloud.sendData(new Dictionary<string, object>
            {
         {"RamUsage", ClientFunctions.getRam().ToString() }
            },
            "Clients",
            Environment.MachineName)
            );

        Console.Read();




        //cloud.sendData(new Dictionary<string, object> { { "Ram Useage", percent.ToString() } }, "Clients", "AMD Desktop");







        Console.ReadLine();
    }
}

#region Background tasks

#endregion
