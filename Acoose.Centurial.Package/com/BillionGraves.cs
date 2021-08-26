using Acoose.Genealogy.Extensibility.Data;
using Acoose.Genealogy.Extensibility.Data.References;
using Acoose.Genealogy.Extensibility.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acoose.Centurial.Package.com
{
    [Scraper("http://billiongraves.*/grave/*")]
    public class BillionGraves : Scraper.Default
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
            var vitalInfo = context.Body()
                .Descendants("div").WithId("VitalInformation")
                .Single();
            var dataItems = vitalInfo
                .Descendants("div").WithClass("Information")
                .Elements("div").WithClass("bg-list-item")
                .ToArray();
            var personData = dataItems
                .WithAny(n => n.Descendants("img").WithAttribute("src").Where(x => x.Attribute("src").EndsWith("person.svg")))
                .Single();
            var cemeteryData = dataItems.WithAttribute("itemprop", "deathPlace")
                .Single();
            var cemeteryAddress = cemeteryData
                .Descendants("div").WithAttribute("itemprop", "address")
                .Descendants().WithAttribute("itemprop")
                .ToArray();
            var photographData = dataItems
                .WithAny(n => n.Descendants("amp-img").Where(x => x.Attribute("alt").Contains("oto")))
                .Descendants("div").WithAny(n => n.Elements("h2").WithClass("bg-list-title"))
                .Single();
            var images = vitalInfo
                .Descendants("amp-carousel").WithId("record-image-carousel")
                .Descendants("div").WithClass("i-amphtml-slide-item")
                .Elements("div").WithClass("record-image-wrapper")
                .Elements("amp-img")
                .Select(x => x.Attribute("src"))
                .ToArray();

            // deceased
            var deceased = new Person()
            {
                Name = personData.Descendants("h2").WithClass("bg-list-title").Single().GetInnerText(),
                BirthDate = Date.TryParse(personData.Descendants("time").WithAttribute("itemprop", "birthDate").Single().Attribute("datetime")),
                DeathDate = Date.TryParse(personData.Descendants("time").WithAttribute("itemprop", "deathDate").Single().Attribute("datetime")),
                Role = EventRole.Deceased
            };

            // memorial
            return new Memorial()
            {
                URL = context.Url,
                WebsiteTitle = "BillionGraves",
                WebsiteURL = context.GetWebsiteUrl(),
                CemeteryName = cemeteryData
                    .Descendants("h2").WithAttribute("itemprop", "name")
                    .Single().GetInnerText(),
                CemeteryPlace = string.Join(", ", cemeteryAddress.Where(x => x.Attribute("itemprop") != "streetAddress").Select(x => x.GetInnerText()).Where(x => !string.IsNullOrWhiteSpace(x))),
                CemeteryAccess = string.Join(", ", cemeteryAddress.Where(x => x.Attribute("itemprop") == "streetAddress").Select(x => x.GetInnerText()).Where(x => !string.IsNullOrWhiteSpace(x))),
                PhotographedBy = photographData.Elements("h2").Single().GetInnerText(),
                PhotographedAt = Date.TryParse(photographData.Elements("div").Single().GetInnerText()),
                Persons = new Person[] { deceased },
                //Images = images
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