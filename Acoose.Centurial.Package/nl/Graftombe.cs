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
    public class Graftombe : Scraper.Default
    {
        public Memorial Data
        {
            get;
            private set;
        }

        public override IEnumerable<Activity> GetActivities(Context context)
        {
            // scan
            this.Data = this.Scan(context);

            // done
            var activities = this.Data.Images
                .NullCoalesce()
                .Select(x => new Activity.DownloadFileActivity(x))
                .Cast<Activity>()
                .ToList();
            if (activities.Count == 0)
            {
                activities.AddRange(base.GetActivities(context));
            }

            // done
            return activities;
        }
        private Memorial Scan(Context context)
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
            var photoNr = properties["foto nr"];
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

            // memorial
            return new Memorial()
            {
                URL = context.Url,
                WebsiteTitle = "Graftombe.nl",
                WebsiteURL = context.GetWebsiteUrl(),
                CemeteryName = breadcrumb.First(),
                CemeteryPlace = string.Join(", ", breadcrumb.Skip(1)),
                Persons = others.Prepend(principal).ToArray(),
            };
        }

        protected override IEnumerable<Repository> GetProvenance(Context context)
        {
            return this.Data.GenerateProvenance();
        }
        protected override IEnumerable<Info> GetInfo(Context context)
        {
            return this.Data.GenerateInfos();
        }
    }
}