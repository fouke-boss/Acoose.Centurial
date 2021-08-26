using Acoose.Genealogy.Extensibility.Data;
using Acoose.Genealogy.Extensibility.Data.References;
using Acoose.Genealogy.Extensibility.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acoose.Centurial.Package.nl
{
    [Scraper("https://www.graftombe.nl/names/info/*")]
    public class Graftombe : MemorialScraper
    {
        public Graftombe()
            : base("Graftombe.nl") 
        {
        }

        protected override void Scan(Context context)
        {
            // init
            var container = context.Body()
                .Elements("div").WithId("container")
                .Elements("div").WithId("container-shadow")
                .Single();
            var breadcrumb = container
                .Descendants("ul").WithClass("breadcrumbs")
                .Elements("li")
                .Select(x => x.GetInnerText())
                .Reverse()
                .ToArray();
            var properties = container
                .Descendants("table").WithClass("names-info")
                .Descendants("tr")
                .ToDictionary(x => x.Element("th").GetInnerText().ToLower(), x => x.Element("td").GetInnerText());
            var photoNr = properties["foto nr"];

            // persons
            var principal = new Person()
            {
                Role = EventRole.Deceased,
                GivenNames = properties.Get("voornaam"),
                FamilyName = properties.Get("achternaam"),
                BirthDate = Date.TryParse(properties.Get("geboortedatum")),
                DeathDate = Date.TryParse(properties.Get("overlijdensdatum")),
                BirthPlace = properties.Get("geboorteplaats"),
                DeathPlace = properties.Get("Overlijdensplaats"),
            };
            var others = container
                .Descendants("table").WithClass("names-table")
                .Elements("tbody")
                .Descendants("tr")
                .Select(row =>
                {
                    // init
                    var result = default(Person);
                    var cells = row
                        .Descendants("td")
                        .Select(x => x.GetInnerText())
                        .ToArray();

                    // same photo number?
                    if (photoNr == cells[4])
                    {
                        result = new Person()
                        {
                            Role = EventRole.Deceased,
                            GivenNames = cells[0],
                            FamilyName = cells[1],
                            BirthDate = Date.TryParse(cells[2]),
                            DeathDate = Date.TryParse(cells[3]),
                        };
                    }

                    // done
                    return result;
                })
                .Where(x => x != null)
                .ToArray();

            // done
            this.CemeteryName = breadcrumb.First();
            this.CemeteryPlace = string.Join(", ", breadcrumb.Skip(1));
            this.Persons = others.Prepend(principal).ToArray();
        }
    }
}