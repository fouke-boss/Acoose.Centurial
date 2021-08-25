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
    [Scraper("https://www.regionaalarchieftilburg.nl/zoek-een-persoon/deeds/*")]
    public partial class RegionaalArchiefTilburg : Picturae
    {
        public RegionaalArchiefTilburg()
            : base("Regionaal Archief Tilburg", "https://www.regionaalarchieftilburg.nl//")
        {
        }

        protected override void Customize(Record record, Dictionary<string, string> fields)
        {
        }
    }
}