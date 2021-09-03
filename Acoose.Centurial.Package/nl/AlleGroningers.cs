using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Acoose.Genealogy.Extensibility.Data;
using Acoose.Genealogy.Extensibility.Data.References;
using Acoose.Genealogy.Extensibility.Web;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

namespace Acoose.Centurial.Package.nl
{
    [Scraper("https://www.allegroningers.nl/zoeken-op-naam/deeds/*")]
    public partial class AlleGroningers : Picturae
    {
        public AlleGroningers()
            : base("AlleGroningers", "https://www.allegroningers.nl/")
        {
        }

        protected override void Customize(HtmlNode container, Dictionary<string, string> fields)
        {
            // record type
            if (this.RecordType == null && this.EventType == Package.EventType.Baptism)
            {
                // record type
                this.RecordType = RecordType.DoopTrouwBegraaf;
                this.RecordPlace = fields.Get("register.metadata.plaats");
                this.Organization = fields.Get("register.metadata.archiefnaam");
            }

            // archive
            this.ArchiveName = "Groninger Archieven";
            this.ArchivePlace = "Groningen";
        }
    }
}