﻿using System;
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
            // archive
            record.ArchiveName = "Regionaal Archief Tilburg";
            record.ArchivePlace= "Tilburg";

            // dtp
            if (record.RecordType == RecordType.DoopTrouwBegraaf)
            {
                // assume register.metadata.naam "Inv.nr. 32 - Oosterhout - trouwboek 1730-1741 (nederduits-gereformeerde gemeente)"
                if (fields.TryGetValue("register.metadata.naam", out var name))
                {
                    // find "trouwboek 1730-1741 (nederduits-gereformeerde gemeente)"
                    var value = Regex.Split(name, " - ")
                        .Select(x => x.Trim())
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Last();

                    // split
                    var parts = value.Split('(');
                    if (parts.Length == 2)
                    {
                        // done
                        record.Label = parts[0].TrimAll();
                        record.Organization = parts[1].TrimEnd(')').TrimAll();
                    }
                }
            }
        }
    }
}