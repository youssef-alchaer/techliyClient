using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechliyApp.MVVM.Model
{
    public class ClientPC
    {
        public string MachineName { get; set; }

        public string OperatingSystem { get; set; }

        public string ProcessorType { get; set; }

        public string OperatingSystemArchitecture { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }

        public bool Activated { get; set; }

        
       
    }
}
