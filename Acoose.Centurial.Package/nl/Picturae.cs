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
    public abstract class Picturae : Scraper.Default
    {
        protected Picturae(string websiteTitle, string websiteURL)
        {
            // init
            this.WebsiteTitle = websiteTitle;
            this.WebsiteURL = websiteURL;
        }

        public string WebsiteTitle
        {
            get;
        }
        public string WebsiteURL
        {
            get;
        }
        public Record Data
        {
            get;
            private set;
        }

        private void Parse(Context context)
        {
            // init
            var container = context.Body()
                .Descendants("div").WithId("picturae-genealogy-container")
                .Single();

            // fields
            var fields = this.GetSourceFields(container);

            // record
            var record = this.ParseRecord(fields);

            // persons
            record.Persons = container
                .Descendants("deed").WithAttribute("data-persons", "persons.person")
                .Descendants("person-data")
                .Select(p => p.Parents("li").WithClass("ng-scope").FirstOrDefault())
                .Where(x => x != null)
                .Select(p =>
                {
                    // init
                    var result = new Person()
                    {
                        Role = Utility.TryParseEventRole(p.Attribute("data-ng-if")) ?? EventRole.Attendee,
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
                    if (p.Descendants("i").WithAnyClass("pic-icon-male", "pic-icon-male-1", "pic-icon-boy").Any())
                    {
                        result.Gender = Gender.Male;
                    }
                    else if (p.Descendants("i").WithAnyClass("pic-icon-female", "pic-icon-female-1", "pic-icon-girl").Any())
                    {
                        result.Gender = Gender.Female;
                    }

                    // done
                    return result;
                })
                .Where(x => x != null)
                .ToArray();

            // improve
            this.Customize(record, fields);

            // done
            this.Data = record;
        }
        private Record ParseRecord(Dictionary<string, string> fields)
        {
            // init
            var record = new Record()
            {
                RecordDate = Date.TryParse(fields.Get("deed.metadata.datum")),
                RecordPlace = fields.Get("register.metadata.gemeente"),
                EventPlace = fields.Get("deed.metadata.plaats") ?? fields.Get("register.metadata.gemeente"),
                Number = fields.Get("deed.metadata.nummer"),
                Page = fields.Get("deed.metadata.pagina"),
                Label = fields.Get("register.metadata.naam")
            };

            // init
            var bron = fields.Get("bron").ToLower();
            var registratie = fields.Get("soort registratie").ToLower();

            // event and record type
            record.RecordType = RecordType.TryParse(bron) ?? RecordType.TryParse(registratie);
            record.EventType = Utility.TryParseEventType(registratie) ?? Utility.TryParseEventType(bron);

            // collection
            record.CollectionNumber = fields.Get("register.metadata.archiefnummer");
            record.SeriesNumber = fields.Get("register.metadata.inventarisnummer");

            // done
            return record;
        }
        protected abstract void Customize(Record record, Dictionary<string, string> fields);
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

        public override IEnumerable<Activity> GetActivities(Context context)
        {
            // init
            this.Parse(context);

            // screen shot
            yield return new Activity.ScreenCaptureActivity(context);
        }

        public override Genealogy.Extensibility.Data.Source GetSource(Context context, Activity[] activities)
        {
            var result = base.GetSource(context, activities);
            return result;
        }

        protected override IEnumerable<Repository> GetProvenance(Context context)
        {
            // laag 1: website
            yield return new Website()
            {
                Title = this.WebsiteTitle,
                Url = this.WebsiteURL,
                IsVirtualArchive = true,
                Items = new OnlineItem[]
                {
                    new OnlineItem()
                    {
                        Url = context.Url,
                        Accessed = Date.Today,
                        Item = new DatabaseEntry()
                        {
                           EntryFor = this.Data.GenerateItemOfInterest()
                        }
                    }
                }
            };

            // laag 2: record
            yield return this.Data.GenerateRepository();
        }
        protected override IEnumerable<Info> GetInfo(Context context)
        {
            return this.Data.GenerateInfos();
        }
        protected override IEnumerable<Genealogy.Extensibility.Data.File> GetFiles(Context context, Activity[] activities)
        {
            return base.GetFiles(context, activities);
        }
    }
}