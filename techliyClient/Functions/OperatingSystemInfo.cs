using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace techliyClient.Functions
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]

    public class OperatingSystemInfo
    {
        public string Caption { get; private set; }

        public string OperatingSystemArchitecture { get; private set; }

        public string OperatingSystemServicePack { get; private set; }

        public string ProcessorName { get; private set; }


        public OperatingSystemInfo()
        {
            getOperatingSystemInfo();
            getProcessorInfo();

        }

        public void getOperatingSystemInfo()
        {
            Console.WriteLine("Retriving operating system info");
            //Create an object of ManagementObjectSearcher class and pass query as parameter.
            ManagementObjectSearcher mos = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
            foreach (ManagementObject managementObject in mos.Get())
            {
                if (managementObject["Caption"] != null)
                {
                    //  OperatingSystemArchitecture =  managementObject["Caption"].ToString();   //Display operating system caption

                    Caption = managementObject["Caption"].ToString() ?? "Unknown";
                }
                if (managementObject["OSArchitecture"] != null)
                {
                //    Console.WriteLine("Operating System Architecture  :  " + managementObject["OSArchitecture"].ToString());

                    OperatingSystemArchitecture = managementObject["OSArchitecture"].ToString() ?? "Unknown";
                }
                if (managementObject["CSDVersion"] != null)
                {
                    OperatingSystemServicePack = managementObject["CSDVersion"].ToString() ?? "Unknown";

                }
            }
        }

        public void getProcessorInfo()
        {
            RegistryKey processor_name = Registry.LocalMachine.OpenSubKey(@"Hardware\Description\System\CentralProcessor\0", RegistryKeyPermissionCheck.ReadSubTree);   //This registry entry contains entry for processor info.
            if (processor_name != null)
            {
                if (processor_name.GetValue("ProcessorNameString") != null)
                {
                 //   Console.WriteLine(processor_name.GetValue("ProcessorNameString"));   //Display processor ingo.

                    ProcessorName = processor_name.GetValue("ProcessorNameString").ToString();

                }
            }
        }

    }
}
