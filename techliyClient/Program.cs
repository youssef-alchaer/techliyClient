﻿using Google.Cloud.Firestore;
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


//https://github.com/clowd/Clowd.Squirrel

public class Program
{

        private static fireStoreFunctions cloud = new fireStoreFunctions();

    public static int NumberOfReads { get; set; }
    public static int NumberOfWrites { get; set; }



    private static async Task Main(string[] args)
    {
        SquirrelAwareApp.HandleEvents(
      onInitialInstall: OnAppInstall,
      onAppUninstall: OnAppUninstall,
      onEveryRun: OnAppRun);

    


    FileVersionInfo versioninfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

        Console.WriteLine("Techliy V" + versioninfo);

        Console.WriteLine("YAAAAA welcome to da new version\n");

        await UpdateMyApp();


        Console.WriteLine("Techliy Client Started...\n");
       Console.WriteLine("YAAAAA...\n");

        //  await CheckForUpdates();

        var OS = new OperatingSystemInfo();

        

        InitializePC(OS);

        Console.WriteLine();


#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

         RunInBackground(TimeSpan.FromSeconds(1), () => cloud.SendData(
            new Dictionary<string, object> {
                {"RamInUse", ClientFunctions.getRam().ToString("##.##") + "%" },  
                {"UpTime", ClientFunctions.UpTime.ToString() },
                {"DateUpdated" , DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)}

            },
            "Clients",
            Environment.MachineName));

        var c = Environment.ProcessorCount;
        Console.WriteLine(Environment.UserName);

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        Console.Read();




        //cloud.sendData(new Dictionary<string, object> { { "Ram Useage", percent.ToString() } }, "Clients", "AMD Desktop");







        Console.ReadLine();

        #region Background tasks
        async Task RunInBackground(TimeSpan timeSpan, Action action)
        {
            var periodicTimer = new PeriodicTimer(timeSpan);
            while (await periodicTimer.WaitForNextTickAsync())
            {
            
                Console.WriteLine(NumberOfWrites + " and " + NumberOfReads);
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
        Console.WriteLine("Initializing PC");

        //check if PC exists, If so we do NOT need to InitializePC
        if (await cloud.Exists("Clients", "MachineName", Environment.MachineName))
        {
           Console.WriteLine(Environment.MachineName + " was found in the Database");
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


    private static async Task CheckForUpdates()
    {

        var manager = await UpdateManager.GitHubUpdateManager(@"https://github.com/youssef-alchaer/techliyClient/releases/");


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
