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
    [Scraper("http://*.findagrave.com/memorial/*")]
    public class FindAGrave : MemorialScraper
    {
        public FindAGrave()
            : base("Find a Grave")
        {
        }

        protected override void Scan(Context context)
        {
            // init
            var bio = context.Body()
                .Descendants("div").WithAttribute("itemtype", "https://schema.org/Person")
                .Descendants("div").WithClass("section-bio-cover")
                .Single();
            var rows = bio
                .Descendants("table").WithClass("mem-events")
                .Descendants("tr");
            var birth = rows
                .WithAny(n => n.Descendants().WithId("birthLabel"))
                .Descendants("td")
                .Single();
            var death = rows
                .WithAny(n => n.Descendants().WithId("deathLabel"))
                .Descendants("td")
                .SingleOrDefault();
            var cemetery = rows
                .WithAttribute("itemtype", "https://schema.org/Cemetery")
                .Descendants("td")
                .Single();
            var cemeteryPlace = cemetery
                .Descendants().WithAttribute("itemtype", "http://schema.org/PostalAddress")
                .Elements("span")
                .Select(x => x.GetInnerText())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();
            var photograph = context.Body()
                .Descendants("div").WithClass("section-transfer")
                .Descendants("ul").WithId("source")
                .Elements("li");
            var photographedAt = photograph
                .WithAny(n => n.Elements("input").WithId("addedDate"))
                .SingleOrDefault().GetInnerText()?.Split(':')?.Skip(1)?.FirstOrDefault();

            // photograph
            this.PhotographedBy = photograph
                .WithAny(n => n.Elements("input").WithId("createdBy"))
                .Elements("a")
                .SingleOrDefault().GetInnerText();
            this.PhotographedAt = Date.TryParse(photographedAt);

            // done
            this.CemeteryName = cemetery
                .Descendants("a")
                .Descendants().WithAttribute("itemprop", "name")
                .Single().GetInnerText();
            this.CemeteryPlace = string.Join(", ", cemeteryPlace);

            // persons
            this.Persons = new Person[] {
                new Person()
                {
                    Name = bio.Descendants("h1").WithId("bio-name").Single().GetInnerText(),
                    BirthDate = Date.TryParse(birth.Elements().WithAttribute("itemprop", "birthDate").SingleOrDefault()?.GetInnerText()),
                    BirthPlace = birth.Elements().WithAttribute("itemprop", "birthPlace").SingleOrDefault()?.GetInnerText(),
                    DeathDate = Date.TryParse(death.Elements().WithAttribute("itemprop", "deathDate").SingleOrDefault()?.GetInnerText()?.Split('(')?.First()),
                    DeathPlace = birth.Elements().WithAttribute("itemprop", "deathPlace").SingleOrDefault()?.GetInnerText(),
                    Role = EventRole.Deceased
                }
            };

            // images
            this.Images = context.Body()
                .Descendants("div").WithClass("section-photos")
                .Descendants("div").WithAttribute("itemtype", "https://schema.org/ImageGallery")
                .Descendants("img").WithAttribute("itemprop", "image")
                .Select(x => x.Attribute("data-src"))
                .Select(x => this.TrimImagePath(x))
                .Distinct()
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();
        }
        private string TrimImagePath(string path)
        {
            // empty?
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            // init
            var parts = path.Split('?').First().Split('/', '\\')
                .ToList();

            // remove photo folder
            if (parts.Count > 4 && parts[4] == "photos")
            {
                if (parts[3].StartsWith("photo"))
                {
                    parts.RemoveAt(3);
                }
                else
                {
                    throw new NotSupportedException($"Unsupported image path '{path}'.");
                }
            }

            // done
            return string.Join("/", parts);
        }
    }
}