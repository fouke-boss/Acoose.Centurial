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
    [Scraper("https://allefriezen.nl/zoeken/deeds/*")]
    public partial class AlleFriezen : Picturae
    {
        public AlleFriezen()
            : base("AlleFriezen", "https://allefriezen.nl/")
        {
        }

        protected override void Improve(Record record, Dictionary<string, string> fields)
        {
            // record type
            switch (record.RecordType)
            {
                case RecordType.Church:
                    record.Organization = string.Join(" ", (fields.Get("register.metadata.naam") ?? "")
                        .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                        .Skip(1));
                    break;
            }

            // archive
            var archief = fields.Get("register.metadata.archiefnaam").Split('-');
            record.ArchiveName = archief.Last().Trim();
            record.CollectionName = string.Join("-", archief.Reverse().Skip(1).Reverse());
        }
    }
}