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
using TechliyApp.MVVM.Model;
using System.CodeDom;
using System.Reflection;
using Google.Protobuf.Collections;
using Squirrel;
using Squirrel.SimpleSplat;
using System;
using System.Net.NetworkInformation;


//https://github.com/clowd/Clowd.Squirrel

public class Program
{

    private static FireStoreFunctions cloud = new FireStoreFunctions();

    public static int NumberOfReads { get; set; }
    public static int NumberOfWrites { get; set; }

    private static async Task Main(string[] args)
    {
        SquirrelAwareApp.HandleEvents(onInitialInstall: OnAppInstall, onAppUninstall: OnAppUninstall, onEveryRun: OnAppRun);

        Console.WriteLine($"Techliy Client V{FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion.ToString()} Started...\n");

        Console.WriteLine("Checking internet Connection...");

        if (new Ping().Send("www.google.com").Status != IPStatus.Success)
            throw new Exception("You must be connected to the internet to used this app.");

        var OS = new OperatingSystemInfo();

        InitializePC(OS);

        if (!await cloud.Exists("Clients", Environment.MachineName,  nameof(ClientPC.Activated) , true))
            throw new Exception("This App is not Activated.");

      

        await UpdateMyApp();

      

        

     


       


        RunInBackground(TimeSpan.FromSeconds(3), () => cloud.SendData(
            new Dictionary<string, object> {
                {"RamInUse", ClientFunctions.getRam().ToString("##.##") + "%" },  
                {"UpTime", ClientFunctions.UpTime.ToString() },
                {"DateUpdated" , DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)}
            },
            "Clients",
            Environment.MachineName));


        Console.ReadLine();

        #region Background tasks
        async Task RunInBackground(TimeSpan timeSpan, Action action)
        {
            var periodicTimer = new PeriodicTimer(timeSpan);
            while (await periodicTimer.WaitForNextTickAsync())
            {
                
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                
                Console.WriteLine("Writes: " + NumberOfWrites + " and " + "Reads: " +  NumberOfReads);
                action();
            }
        }
        #endregion
    }


    private static async Task UpdateMyApp()
    {
        using var mgr = new GithubUpdateManager(@"https://github.com/youssef-alchaer/techliyClient");

        /* mgr.CheckForUpdate();
         Console.WriteLine(mgr.CheckForUpdate); */

      

        if (!mgr.IsInstalledApp)
        {
            return;
        }
        var newVersion = await mgr.UpdateApp();

        // optionally restart the app automatically, or ask the user if/when they want to restart
        if (newVersion != null)
        {
            UpdateManager.RestartApp();
          
        }
    }

    public static async void InitializePC(OperatingSystemInfo os)
    {
        Console.WriteLine("Initializing PC...");

        //check if PC exists, If so we do NOT need to InitializePC
        if (await cloud.Exists("Clients", Environment.MachineName , nameof(ClientPC.MachineName), Environment.MachineName))
        {
           Console.WriteLine(Environment.MachineName + " was found in the Database");
           Console.WriteLine();
           Console.WriteLine();


            return;
        }

        Console.WriteLine("Creating New Client '" + Environment.MachineName + "' and adding to the Database");

        var NewPC = new ClientPC()
        {
            Activated = true,
            DateCreated = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
            DateUpdated = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
            MachineName = Environment.MachineName,
            ProcessorType = os.ProcessorName,
            OperatingSystem = os.Caption,
            OperatingSystemArchitecture = os.OperatingSystemArchitecture
         
        };



        /*            var info = new Dictionary<string, object>() {
            {nameof(NewPC.MachineName), NewPC.MachineName },
            {nameof(NewPC.Description) , NewPC.Description },
            {nameof(NewPC.OperatingSystem), NewPC.OperatingSystem },
            {nameof(NewPC.ProcessorType), NewPC.ProcessorType },
            {nameof(NewPC.OperatingSystemArchitecture) , NewPC.OperatingSystemArchitecture },
            {nameof(NewPC.DateCreated) , NewPC.DateCreated },
            {nameof(NewPC.DateUpdated) , NewPC.DateUpdated },
            {nameof(NewPC.RamUsage) , "0.0" },
         };*/

        Dictionary<string, object> dictionary = NewPC.GetType().GetProperties()
        .ToDictionary(x => x.Name, x => x.GetValue(NewPC) ?? new object());


        await cloud.SendData(dictionary, "Clients", Environment.MachineName);

        
    }




    private static void OnAppInstall(SemanticVersion version, IAppTools tools)
    {
        tools.CreateShortcutForThisExe(ShortcutLocation.StartMenu | ShortcutLocation.Desktop);
    }

    private static void OnAppUninstall(SemanticVersion version, IAppTools tools)
    {
        tools.RemoveShortcutForThisExe(ShortcutLocation.StartMenu | ShortcutLocation.Desktop);
    }

    private static void OnAppRun(SemanticVersion version, IAppTools tools, bool firstRun)
    {
        tools.SetProcessAppUserModelId();
        // show a welcome message when the app is first installed
        if (firstRun) Console.WriteLine("Thanks for installing my application!");
    }
}

#region Background tasks

#endregion
