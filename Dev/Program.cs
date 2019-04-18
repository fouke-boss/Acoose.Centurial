using Acoose.Genealogy.Extensibility.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev
{
    class Program
    {
        // development and testing code
        static void Main(string[] args)
        {
            var akte = Acoose.Centurial.Package.nl.A2A.Akte.LoadXml(@"C:\Users\Fouke\Desktop\A2A Registratie.xml");
            var info = akte.GetInfo();
        }
    }
}