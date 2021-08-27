using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Acoose.Genealogy.Extensibility.Data;
using Acoose.Genealogy.Extensibility.Data.References;
using Acoose.Genealogy.Extensibility.Web;
using Newtonsoft.Json.Linq;

namespace Acoose.Centurial.Package.nl
{
    [Scraper("https://archief.amsterdam/indexen/deeds/*")]
    public partial class StadsarchiefAmsterdam : Picturae
    {
        public StadsarchiefAmsterdam()
            : base("Stadsarchief Amsterdam", "https://archief.amsterdam/")
        {
        }

        protected override void Customize(Dictionary<string, string> fields)
        {
        }
    }
}