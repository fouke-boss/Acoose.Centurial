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
    [Scraper("https://www.allegroningers.nl/zoeken-op-naam/deeds/*")]
    public partial class AlleGroningers : Picturae
    {
        public AlleGroningers()
            : base("AlleGroningers", "https://www.allegroningers.nl/")
        {
        }

        protected override void Improve(Record record, Dictionary<string, string> fields)
        {
            // per record type
            switch (record.RecordType)
            {
                case RecordType.Church:
                    record.RecordPlace = fields.Get("register.metadata.plaats");
                    record.Organization = fields.Get("register.metadata.archiefnaam");
                    break;
            }

            // archive
            record.ArchiveName = "Groninger Archieven";
            record.ArchivePlace = "Groningen";
        }
    }
}