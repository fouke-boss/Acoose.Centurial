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
    [Scraper("https://archief.amsterdam/indexen/deeds/*")]
    public partial class StadsarchiefAmsterdam : Picturae
    {
        public StadsarchiefAmsterdam()
            : base("Stadsarchief Amsterdam", "https://archief.amsterdam/")
        {
        }

        protected override void Customize(HtmlNode container, Dictionary<string, string> fields)
        {
            // archief
            this.ArchiveName = "Stadsarchief Amsterdam";
            this.ArchivePlace = "Amsterdam";

            // record type
            if (this.RecordType == RecordType.DoopTrouwBegraaf)
            {
                // init
                var bijzonderheden = container
                    .Descendants("p").WithAttribute("ng-if", "deed.metadata.diversen")
                    .Descendants("span")
                    .Select(x => x.GetChildText())
                    .FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(bijzonderheden))
                {
                    // split
                    var parts = bijzonderheden.Split(':')
                        .Select(x => x.TrimAll())
                        .ToArray();
                    if (parts.Length == 2)
                    {
                        if (parts[0].ToLower() == "kerk")
                        {
                            this.Organization = parts[1];
                        }
                        else if (parts[0].ToLower() == "begraafplaats")
                        {
                            // begraafplaats
                            this.RecordType = RecordType.Cemetery;
                            this.Organization = parts[1];
                        }
                    }
                }

                // deed.metadata.nummer is per abuis gevuld met het inventarisnummer
                this.Number = null;
            }
        }
    }
}