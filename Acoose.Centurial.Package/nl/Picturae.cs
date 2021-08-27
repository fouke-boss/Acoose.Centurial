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
    public abstract class Picturae : RecordScraper
    {
        protected Picturae(string websiteTitle, string websiteURL)
            : base(websiteTitle)
        {
        }

        protected override void Scan(Context context)
        {
            // init
            var container = context.Body()
                .Descendants("div").WithId("picturae-genealogy-container")
                .Single();

            // fields
            var fields = this.GetSourceFields(container);
            var bron = fields.Get("bron").ToLower();
            var registratie = fields.Get("soort registratie").ToLower();

            // record
            this.RecordDate = Date.TryParse(fields.Get("deed.metadata.datum"));
            this.RecordPlace = fields.Get("register.metadata.gemeente");
            this.EventPlace = fields.Get("deed.metadata.plaats") ?? fields.Get("register.metadata.gemeente");
            this.Number = fields.Get("deed.metadata.nummer");
            this.Page = fields.Get("deed.metadata.pagina");
            this.Label = fields.Get("register.metadata.naam");

            // event and record type
            this.RecordType = RecordType.TryParse(bron) ?? RecordType.TryParse(registratie);
            this.EventType = Utility.TryParseEventType(registratie) ?? Utility.TryParseEventType(bron);

            // collection
            this.CollectionNumber = fields.Get("register.metadata.archiefnummer");
            this.SeriesNumber = fields.Get("register.metadata.inventarisnummer");

            // persons
            this.Persons = container
                .Descendants("deed").WithAttribute("data-persons", "persons.person")
                .Descendants("person-data")
                .Select(p =>
                {
                    // init
                    var result = new Person()
                    {
                        Role = Utility.TryParseEventRole(p.Attribute("data-person")) ?? EventRole.Attendee,
                        Name = p
                            .Descendants("a").WithClass("person")
                            .First() // the 2nd and further will be partners
                            .GetInnerText(),
                        BirthDate = Date.TryParse(this.GetProperty(p, "person.metadata.datum_geboorte")),
                        BirthPlace = this.GetProperty(p, "person.metadata.plaats_geboorte"),
                        DeathDate = Date.TryParse(this.GetProperty(p, "person.metadata.datum_overlijden")),
                        DeathPlace = this.GetProperty(p, "person.metadata.plaats_overlijden"),
                        Age = Utility.TryParseAge(this.GetProperty(p, "person.metadata.leeftijd")),
                        Occupation = this.GetProperty(p, "person.metadata.beroep")?.Trim('(', ')')?.TrimAll()
                    };

                    // gender
                    if (p.ParentNode.Descendants("i").WithAnyClass("pic-icon-male", "pic-icon-male-1", "pic-icon-boy").Any())
                    {
                        result.Gender = Gender.Male;
                    }
                    else if (p.ParentNode.Descendants("i").WithAnyClass("pic-icon-female", "pic-icon-female-1", "pic-icon-girl").Any())
                    {
                        result.Gender = Gender.Female;
                    }

                    // done
                    return result;
                })
                .Where(x => x != null)
                .ToArray();

            // images (disable,d because these images are served from memorix.nl, which is a different host and therefore not accessible
            //this.Images = container
            //    .Descendants("aside").WithClass("record-actions")
            //    .Descendants("a")
            //    .Select(x => x.Attribute("href"))
            //    .Where(x => !string.IsNullOrEmpty(x) && x != "#")
            //    .ToArray();

            // customize
            this.Customize(fields);
        }
        protected abstract void Customize(Dictionary<string, string> fields);
        private string GetProperty(HtmlNode node, string name)
        {
            // init
            var span = node.Descendants("span")
                .Where(x => x.Attribute("ng-if") == name || x.Attribute("data-ng-if") == name)
                .SingleOrDefault();

            // null?
            if (span == null)
            {
                return null;
            }

            // done
            return this.GetInnerText(span);
        }
        private Dictionary<string, string> GetSourceFields(HtmlNode container)
        {
            // init
            var nodes1 = container
                .Descendants("ul").WithClass("registration-info-list")
                .Descendants("li");
            var nodes2 = container
                .Descendants("aside").WithClass("record-scource")
                .Descendants().WithAttribute("data-ng-if");

            // loop
            return nodes1
                .Concat(nodes2)
                .Select(node => new
                {
                    Key = node.Attribute("ng-if") ?? node.Attribute("data-ng-if") ?? node.Element("strong").GetInnerText()?.ToLower(),
                    Value = this.GetInnerText(node)
                })
                .GroupBy(x => x.Key)
                .ToDictionary(
                    e => e.Key,
                    e => string.Join(" ", e.Select(x => x.Value))
                );
        }
        private string GetInnerText(HtmlNode node)
        {
            // init
            var parts = node.ChildNodes
                .OfType<HtmlTextNode>()
                .Select(x => x.InnerText);

            // done
            return string.Join("", parts).TrimAll();
        }
    }
}