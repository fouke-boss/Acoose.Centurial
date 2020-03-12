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
    [Scraper("https://mensenlinq.nl/overlijdensberichten/*")]
    public class Mensenlinq : Scraper.Default
    {
        private static readonly Regex DEATH_DATE_REGEX = new Regex(@"(\d{2}-\d{2}-\d{4})");

        public string Naam
        {
            get; private set;
        }
        public string[] Kranten
        {
            get; private set;
        }

        public override IEnumerable<Activity> GetActivities(Context context)
        {
            // hyperlinks van de advertenties opzoeken
            var obituaryDetails = context.Html
                .Descendants("div")
                .Where(x => x.GetAttributeValue("class", "") == "obituary-details")
                .ToArray();
            var figures = obituaryDetails
                .SelectMany(x => x.Descendants("div"))
                .Where(x => x.GetAttributeValue("class", "") == "other")
                .SelectMany(x => x.Descendants("figure"))
                .ToArray();
            if (figures.Length == 0)
            {
                // single figure
                figures = obituaryDetails
                    .SelectMany(x => x.Descendants("div"))
                    .Where(x => x.GetAttributeValue("class", "") == "main")
                    .SelectMany(x => x.Descendants("figure"))
                    .ToArray();

                // De namen van de kranten waar de advertentie uit komt, achter elkaar plaatsen, "dubbelen" verwijderen, gescheiden door komma's
                this.Kranten = obituaryDetails
                    .SelectMany(x => x.Descendants("ul"))
                    .Where(x => x.GetAttributeValue("class", "") == "papers")
                    .SelectMany(x => x.Descendants("li"))
                    .Select(x => x.InnerText)
                    .Distinct()
                    .ToArray();
            }
            else
            {
                // De namen van de kranten waar de advertenties uit komen, achter elkaar plaatsen, "dubbelen" verwijderen, gescheiden door komma's
                this.Kranten = figures
                    .Select(x => x.GetAttributeValue("data-title", ""))
                    .Distinct()
                    .ToArray();
            }

            // klaar
            return figures
                .SelectMany(x => x.Descendants("img"))
                .Select(x => x.GetAttributeValue("src", ""))
                .Select(x => new Activity.DownloadFileActivity(x));
        }

        protected override IEnumerable<Repository> GetProvenance(Context context)
        {
            // titel zoeken
            var title = context.Html
                .Descendants("div")
                .Where(x => x.GetAttributeValue("class", "") == "obituary-details")
                .SelectMany(x => x.Descendants("h1"))
                .FirstOrDefault()?.InnerText;
            if (title.StartsWith("Overlijdensbericht "))
            {
                title = title.Substring(19);
            }

            // laag 1: website
            yield return new Website()
            {
                Url = context.GetWebsiteUrl(),
                Title = context.GetWebsiteTitle(),
                Items = new OnlineItem[]
                {
                    new OnlineItem()
                    {
                        Url = context.Url,
                        Accessed = Date.Today,
                        Item = new DatabaseEntry()
                        {
                           EntryFor = title,
                        }
                    }
                }
            };

            // laag 2: onbekende bron
            yield return new UnknownRepository()
            {
                Items = new Genealogy.Extensibility.Data.References.Source[]
                {
                    new Unknown()
                    {
                        CreditLine = string.Join(", ", this.Kranten)
                    }
                }
            };
        }
        protected override IEnumerable<Info> GetInfo(Context context)
        {
            // init
            var script = context.Html
                .Descendants("script")
                .Where(x => x.GetAttributeValue("type", "") == "application/ld+json")
                .Select(x => Newtonsoft.Json.JsonConvert.DeserializeObject(x.InnerText))
                .Cast<JObject>()
                .SelectMany(x => x.Properties())
                .ToList();
            var title = context.GetMetaTag("og:title");

            // values
            var name = script.FirstOrDefault(x => x.Name == "name")?.Value?.ToString()?.Trim();
            var birthDate = script.FirstOrDefault(x => x.Name == "birthDate")?.Value?.ToString()?.Trim();
            var birthPlace = script.FirstOrDefault(x => x.Name == "birthPlace")?.Value?.ToString()?.Trim();
            var deathDate = DEATH_DATE_REGEX.Match(title).Groups.Cast<Group>().FirstOrDefault()?.Value?.Trim();

            // naam parsen
            Acoose.Genealogy.Extensibility.ParsingUtility.ParseName(name, out string lastName, out string givenNames, out string particles);
            var familyName = string.Join(" ", new string[] { particles, lastName }.Where(x => !string.IsNullOrWhiteSpace(x)));

            // geboren
            var birth = new InfoEvent();
            if (Date.TryParse(birthDate) is Date bdate)
            {
                birth.Date = new Date[] { bdate };
            }
            if (!string.IsNullOrWhiteSpace(birthPlace))
            {
                birth.Place = new string[] { birthPlace };
            }
            // overlijden
            var death = new InfoEvent();
            if (Date.TryParse(deathDate) is Date ddate)
            {
                death.Date = new Date[] { ddate };
            }

            // done
            yield return new PersonInfo()
            {
                FamilyName = new string[] { familyName },
                GivenNames = new string[] { givenNames },
                Birth = birth,
                Death = death,
            };
        }
        protected override IEnumerable<Genealogy.Extensibility.Data.File> GetFiles(Context context, Activity[] activities)
        {
            return base.GetFiles(context, activities);
        }
    }
}