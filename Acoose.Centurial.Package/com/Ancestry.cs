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

namespace Acoose.Centurial.Package.com
{
    [Scraper("https://search.ancestry.com/cgi-bin/sse.dll")]
    [Scraper("https://search.ancestry.de/cgi-bin/sse.dll")]
    [Scraper("https://search.ancestry.co.uk/cgi-bin/sse.dll")]
    [Scraper("https://search.ancestrylibrary.com/cgi-bin/sse.dll")]
    [Scraper("https://search.ancestry.ca/cgi-bin/sse.dll")]
    [Scraper("https://search.ancestrylibrary.com.au/cgi-bin/sse.dll")]
    [Scraper("https://www.ancestry.*/discoveryui-content/view/*")]
    public partial class Ancestry : RecordScraper
    {
        private const string SOURCE_INFORMATION_PREFIX = "Ancestry.com.";
        private const string SOURCE_INFORMATION_SUFFIX = "[database on-line]";
        private static readonly string[] QUERY_PARAMETERS = new string[] { "db", "dbid", "h", "indiv" };
        private static readonly string[] MALE_PREFIXES = new string[] { "mr" };
        private static readonly string[] FEMALE_PREFIXES = new string[] { "mrs", "miss", "ms" };

        private Dictionary<Language, Dictionary<Property, string>> _PropertyNames = new Dictionary<Language, Dictionary<Property, string>>()
        {
            {
                Language.English,
                new Dictionary<Property, string>()
                {
                    { Property.Name, "name" },
                    { Property.Gender, "gender" }
                }
            }
        };

        public Ancestry()
            : base("Ancestry")
        {
            // init
            this.Events = new List<Event>();
        }

        protected override void Scan(Context context)
        {
            // init
            var language = Language.English;
            var page = context.Body()
                .Elements().WithId("mainContent")
                .Elements("div").WithClass("page")
                .Single();
            var recordData = page
                .Descendants("div").WithId("recordData")
                .Elements("table")
                .ToPropertyBag(x => x.GetInnerText());
            var citationPanel = page
                .Descendants("section").WithId("sourceCitation")
                .Single();

            // source citation
            var sourceCitation = citationPanel
                .Descendants("div")
                .Where(x => x.Element("h4")?.GetInnerText() == "Source Citation")
                .SelectMany(x => x.Descendants("p"))
                .SingleOrDefault();
            if (sourceCitation is HtmlNode)
            {
                // split
                var parts = sourceCitation.GetInnerText().Split(';')
                    .Select(x => x.TrimAll())
                    .ToArray();
                if (parts.Length >= 3 && parts.Take(3).All(p => !p.Contains(":")))
                {
                    this.ArchiveName = parts[0];
                    this.ArchivePlace = parts[1];
                    this.Label = parts[2];
                }
            }

            // online
            this.URL = this.CleanURL(context.Url);
            this.OnlineCollectionName = this.GetOnlineCollectionName(citationPanel);

            // person
            var person = this.GetPrincipal(recordData);
            var others = new string[] { "father", "mother", "spouse" }
                .Select(property =>
                {
                    // init
                    var name = recordData[property].GetInnerText();
                    var result = default(Person);

                    // any?
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        // init
                        result = new Person()
                        {
                            Name = name,
                            Role = Utility.TryParseEventRole(property).Value
                        };
                    }

                    // done
                    return result;
                })
                .Where(x => x != null)
                .ToArray();
            this.Persons = others
                .Prepend(person)
                .ToArray();

            // events
            this.AddEvent(Package.EventType.Birth, recordData["birth date"], recordData["birth place"]);
            this.AddEvent(Package.EventType.Baptism, recordData["baptism date"], recordData["baptism place"]);
            this.AddEvent(Package.EventType.Marriage, recordData["marriage date"], recordData["marriage place"]);
            this.AddEvent(Package.EventType.Death, recordData["death date"], recordData["death place"]);
            this.AddEvent(Package.EventType.Burial, recordData["burial date"], recordData["burial place"]);

            // record
            this.FixRecordType(recordData);
        }
        private Person GetPrincipal(PropertyBag recordData)
        {
            // init
            var result = new Person();

            // name
            var name = recordData["name"].ToPhrase().PreferWithBrackets().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var index = 0;
            if (MALE_PREFIXES.Any(x => string.Compare(x, name.FirstOrDefault(), true) == 0))
            {
                // male
                result.Gender = Gender.Male;

                // skip
                index = 1;
            }
            else if (FEMALE_PREFIXES.Any(x => string.Compare(x, name.FirstOrDefault(), true) == 0))
            {
                // female
                result.Gender = Gender.Female;

                // skip
                index = 1;
            }

            // properties
            result.Name = string.Join(" ", name.Skip(index));
            result.Age = Utility.TryParseAge(recordData.First("age", "marriage age").GetInnerText());
            result.Gender = result.Gender ?? Utility.TryParseGender(recordData["gender"].ToPhrase().PreferWithoutBrackets());

            // done
            return result;
        }
        private void AddEvent(Package.EventType eventType, HtmlNode date, HtmlNode place)
        {
            // init
            var dateValue = date.GetInnerText();
            var placeValue = place.ToPhrase().PreferWithoutBrackets();

            // any?
            if (!string.IsNullOrWhiteSpace(dateValue) || !string.IsNullOrWhiteSpace(placeValue))
            {
                // create
                var result = this.GetEvent(eventType);
                result.Date = Date.TryParse(dateValue);
                result.Place = placeValue.NullIfWhitespace();
            }
        }
        private void FixRecordType(PropertyBag recordData)
        {
            // init
            var mainEvent = this.Events
                .OrderByDescending(x => x.Type)
                .FirstOrDefault();
            if (mainEvent != null && !string.IsNullOrEmpty(mainEvent.Place))
            {
                // set
                this.RecordDate = mainEvent.Date;
                this.RecordPlace = mainEvent.Place;

                // type
                this.RecordType = RecordType.TryParse(this.Label);

                // details
                this.Number = recordData["certificate number"].GetInnerText();
            }

            // label
            var label = new HtmlNode[] { recordData["civil registration office"] }
                .Select(x => x.GetInnerText())
                .Prepend(this.Label)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim());
            this.Label = string.Join(", ", label).NullIfWhitespace();

            // organization
            var organization = new string[] { "church" }
                .Select(x => recordData[x].GetInnerText())
                .Prepend(this.Organization)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim());
            this.Organization = string.Join(", ", organization).NullIfWhitespace();
        }
        private string GetOnlineCollectionName(HtmlNode citationPanel)
        {
            // init
            var result = default(string);

            // source information
            var sourceInformation = citationPanel
                .Descendants("div").WithClass("sourceText")
                .Where(x => x.ParentNode.Element("h4")?.GetInnerText() == "Source Information")
                .SelectMany(x => x.Descendants("p"))
                .FirstOrDefault();
            if (sourceInformation is HtmlNode)
            {
                // init
                var info = sourceInformation.GetInnerText();
                var index = info.IndexOf(SOURCE_INFORMATION_SUFFIX);
                if (info.StartsWith(SOURCE_INFORMATION_PREFIX) && index > 0)
                {
                    result = info.Substring(SOURCE_INFORMATION_PREFIX.Length, index - SOURCE_INFORMATION_PREFIX.Length).TrimAll();
                }
            }

            // done
            return result;
        }

        private string CleanURL(string url)
        {
            // init
            var parts = url.Split('?');
            if (parts.Length != 2)
            {
                return url;
            }

            // split
            var query = parts[1]
                .Split('&')
                .Select(x => x.Split('='))
                .Where(x => x.Length == 2)
                .Where(x => QUERY_PARAMETERS.Contains(x[0]))
                .OrderBy(x => Array.IndexOf(QUERY_PARAMETERS, x[0]))
                .Select(x => string.Join("=", x));

            // done
            return $"{parts[0]}?{string.Join("&", query)}";
        }

        private List<Event> Events
        {
            get;
        }
        private Event GetEvent(EventType type)
        {
            // init
            var result = this.Events
                .NullCoalesce()
                .SingleOrDefault(x => x.Type == type);
            if (result == null)
            {
                // init
                result = new Event()
                {
                    Type = type
                };

                // add
                this.Events.Add(result);
            }

            // done
            return result;
        }

        protected override IEnumerable<Info> GetInfo(Context context)
        {
            // init
            var results = base.GetInfo(context).ToArray();

            // events
            this.Events
                .NullCoalesce()
                .ForEach(e =>
                {
                // init
                var target = default(InfoWithEvents);
                    var eventType = e.Type.ToString();

                // type?
                if (e.Type == Package.EventType.Marriage)
                    {
                    // relationship
                    target = results
                    .OfType<RelationshipInfo>()
                    .Single(r => r.IsPartnership.NullCoalesce().Any(x => x));

                    // type of marriage
                    eventType = (this.RecordType is RecordType<ChurchRecord> ? "ChurchMarriage" : "CivilMarriage");
                    }
                    else
                    {
                    // principal
                    target = results
                    .OfType<PersonInfo>()
                    .Single(p => p.Id == this.Principal.Id.ToString());
                    }

                // import
                target.ImportEvent(
                eventType,
                e.Date,
                e.Place,
                EnsureMode.AddIfNonePresent
            );
                });

            // done
            return results;
        }

        private enum Property
        {
            Name,
            Gender,
            Age,
            BirthDate,
            Father,
            Mother
        }
        private class Event
        {
            public EventType Type
            {
                get; set;
            }
            public Date Date
            {
                get; set;
            }
            public string Place
            {
                get; set;
            }
        }
    }
}