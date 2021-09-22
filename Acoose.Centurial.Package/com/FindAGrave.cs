using Acoose.Genealogy.Extensibility.Data;
using Acoose.Genealogy.Extensibility.Data.References;
using Acoose.Genealogy.Extensibility.Web;
using HtmlAgilityPack;
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
            var data = PropertyBag<string>.Load(
                    rows.Descendants().WithAttribute("itemProp"), 
                    x => x.Attribute("itemProp"), x => x.ToPhrase().IgnoreBrackets()
                );

            // cemetery
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
            this.CemeteryName = cemetery
                .Descendants("a")
                .Descendants().WithAttribute("itemprop", "name")
                .SingleOrDefault().GetInnerText();
            this.CemeteryPlace = string.Join(", ", cemeteryPlace);


            // photograph
            var photograph = context.Body()
                .Descendants("div").WithClass("section-transfer")
                .Descendants("ul").WithId("source")
                .Elements("li");
            var photographedAt = photograph
                .WithAny(n => n.Elements("input").WithId("addedDate"))
                .SingleOrDefault().GetInnerText()?.Split(':')?.Skip(1)?.FirstOrDefault();
            this.PhotographedAt = Date.TryParse(photographedAt);
            this.PhotographedBy = new string[] { "createdBy", "maintainedBy" }
                .Select(id =>
                {
                    return photograph
                        .WithAny(n => n.Elements("input").WithId(id))
                        .Elements("a")
                        .SingleOrDefault().GetInnerText();
                })
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .FirstOrDefault();

            // person
            var person = new Person()
            {
                Name = bio.Descendants("h1").WithId("bio-name").Single().GetChildText(),
                BirthDate = Date.TryParse(data["birthDate"]),
                BirthPlace = data["birthPlace"],
                DeathDate = Date.TryParse(data["deathDate"]),
                DeathPlace = data["deathPlace"],
                Role = EventRole.Deceased
            };

            // name
            var nameNode = bio.Descendants("h1").WithId("bio-name").Single();
            if (nameNode.Elements("i").Any())
            {
                // <i> contains maiden name, all before <i> are the given names
                person.GivenNames = nameNode.ChildNodes
                    .Cast<HtmlNode>()
                    .TakeWhile(x => x is HtmlTextNode)
                    .Select(x => x.InnerText)
                    .Join("").TrimAll();

                // maiden name
                person.FamilyName = nameNode.Elements("i")
                    .Select(x => x.GetInnerText())
                    .Join("");
            }
            else
            {
                // parse full name
                person.Name = nameNode.GetInnerText();
            }

            // persons
            this.Persons = new Person[] {
                person
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