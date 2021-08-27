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
    [Scraper("https://www.bhic.nl/memorix/genealogy/search/deeds/*")]
    public partial class BHIC : Picturae
    {
        public BHIC()
            : base("Brabants Historisch Informatie Centrum", "https://www.bhic.nl/")
        {
        }

        protected override void Customize(Dictionary<string, string> fields)
        {
        }
    }
}