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
    [Scraper("https://allefriezen.nl/zoeken/deeds/*")]
    public partial class AlleFriezen : Picturae
    {
        public AlleFriezen()
            : base("AlleFriezen", "https://allefriezen.nl/")
        {
        }

        protected override void Customize(HtmlNode container, Dictionary<string, string> fields)
        {
            // record type
            if (this.RecordType == RecordType.DoopTrouwBegraaf)
            {
                // kerknaam
                this.Organization = string.Join(" ", (fields.Get("register.metadata.naam") ?? "")
                    .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .Skip(1));
            }

            // archive
            var archief = fields.Get("register.metadata.archiefnaam").Split('-');
            this.ArchiveName = archief.Last().Trim();
            this.CollectionName = string.Join("-", archief.Reverse().Skip(1).Reverse());
        }
    }
}