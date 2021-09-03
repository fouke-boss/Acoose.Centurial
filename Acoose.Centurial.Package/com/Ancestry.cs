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
    [Scraper("https://search.ancestry.co.uk/cgi-bin/sse.dll")]
    [Scraper("https://search.ancestry.de/cgi-bin/sse.dll")]
    [Scraper("https://search.ancestry.ca/cgi-bin/sse.dll")]
    [Scraper("https://search.ancestry.com.au/cgi-bin/sse.dll")]
    [Scraper("https://search.ancestrylibrary.com/cgi-bin/sse.dll")]
    [Scraper("https://search.ancestrylibrary.com.au/cgi-bin/sse.dll")]
    [Scraper("https://search.ancestrylibraryedition.co.uk/cgi-bin/sse.dll")]
    [Scraper("https://www.ancestry.com/discoveryui-content/view/*")]
    [Scraper("https://www.ancestry.co.uk/discoveryui-content/view/*")]
    [Scraper("https://www.ancestry.de/discoveryui-content/view/*")]
    [Scraper("https://www.ancestry.ca/discoveryui-content/view/*")]
    [Scraper("https://www.ancestry.com.au/discoveryui-content/view/*")]
    [Scraper("https://www.ancestrylibrary.com/discoveryui-content/view/*")]
    [Scraper("https://www.ancestrylibrary.com.au/discoveryui-content/view/*")]
    [Scraper("https://www.ancestrylibraryedition.co.uk/discoveryui-content/view/*")]
    public partial class Ancestry : RecordScraper
    {
        private const string SOURCE_INFORMATION_PREFIX = "Ancestry.com.";
        private const string SOURCE_INFORMATION_SUFFIX = "[database on-line]";
        private static readonly string[] QUERY_PARAMETERS = new string[] { "db", "dbid", "h", "indiv" };
        private static readonly string[] MALE_PREFIXES = new string[] { "mr" };
        private static readonly string[] FEMALE_PREFIXES = new string[] { "mrs", "miss", "ms" };

        public Ancestry()
            : base("Ancestry")
        {
            // init
            this.Events = new List<Event>();
        }

        protected override void Scan(Context context)
        {
            // init
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

            // language
            this.Language = this.GetLanguage(context);

            // source citation
            var sourceCitation = citationPanel
                .Descendants("div")
                .Where(x => string.Compare(x.Element("h4")?.GetInnerText(), PropertyName.SourceCitation[this.Language], true) == 0)
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
            var others = new PropertyName[] { PropertyName.Father, PropertyName.Mother, PropertyName.Spouse, PropertyName.Child }
                .Select(property =>
                {
                    // init
                    var name = recordData[property[this.Language]].GetInnerText();
                    var result = default(Person);

                    // any?
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        // init
                        result = new Person()
                        {
                            Name = name,
                            Role = Utility.TryParseEventRole(property.English).Value
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
            this.AddEvent(Package.EventType.Birth, recordData, PropertyName.BirthDate, PropertyName.BirthPlace);
            this.AddEvent(Package.EventType.Baptism, recordData, PropertyName.BaptismDate, PropertyName.BaptismPlace);
            this.AddEvent(Package.EventType.Marriage, recordData, PropertyName.MarriageDate, PropertyName.MarriagePlace);
            this.AddEvent(Package.EventType.Death, recordData, PropertyName.DeathDate, PropertyName.DeathPlace);
            this.AddEvent(Package.EventType.Burial, recordData, PropertyName.BurialDate, PropertyName.BurialPlace);

            // record
            this.FixRecordType(recordData);
        }

        public Language Language
        {
            get; private set;
        }
        private Language GetLanguage(Context context)
        {
            // init
            var country = context.GetWebsiteUrl()
                .Trim('/', '\\')
                .Split('.')
                .Last();

            // done
            switch (country)
            {
                case "de":
                    return Language.Deutsch;
                default:
                    return Language.English;
            }
        }

        private Person GetPrincipal(PropertyBag recordData)
        {
            // init
            var result = new Person();

            // name
            var name = recordData[PropertyName.Name[this.Language]].ToPhrase().PreferWithBrackets().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
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
            result.Age = Utility.TryParseAge(recordData.First(PropertyName.Age[this.Language], PropertyName.MarriageAge[this.Language], PropertyName.DeathAge[this.Language]).GetInnerText());
            result.Gender = result.Gender ?? Utility.TryParseGender(recordData[PropertyName.Gender[this.Language]].ToPhrase().PreferWithoutBrackets());

            // maiden name
            if (recordData[PropertyName.MaidenName[this.Language]].GetInnerText() is string maidenName && !string.IsNullOrWhiteSpace(maidenName))
            {
                result.FamilyName = maidenName;
            }

            // done
            return result;
        }
        private void AddEvent(Package.EventType eventType, PropertyBag recordData, PropertyName date, PropertyName place)
        {
            // init
            var dateValue = recordData[date[this.Language]].GetInnerText();
            var placeValue = recordData[place[this.Language]].ToPhrase().PreferWithoutBrackets();

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
                this.RecordType = RecordType.TryParse(this.Label) ?? RecordType.TryParse(this.OnlineCollectionName);

                // details
                this.Number = recordData[PropertyName.CertificateNumber[this.Language]].GetInnerText();
            }

            // label
            var label = new PropertyName[] { PropertyName.CivilRegistrationOffice }
                .Select(x => recordData[x[this.Language]])
                .Select(x => x.GetInnerText())
                .Prepend(this.Label)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim());
            this.Label = string.Join(", ", label).NullIfWhitespace();

            // organization
            var organization = new PropertyName[] { PropertyName.Church, PropertyName.ParishAsItAppears }
                .Select(x => recordData[x[this.Language]].GetInnerText())
                .Prepend(this.Organization)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim());
            this.Organization = string.Join(", ", organization).NullIfWhitespace();

            // page
            this.Page = Utility.TryParsePageNumber(recordData[PropertyName.PageNumber[this.Language]].GetInnerText());
        }
        private string GetOnlineCollectionName(HtmlNode citationPanel)
        {
            // init
            var result = default(string);
            var names = new PropertyName[] { PropertyName.SourceInformationInSearch, PropertyName.SourceInformationInDiscoveryUI }
                .Select(x => x[this.Language])
                .ToArray();

            // source information
            var sourceInformation = citationPanel
                .Descendants("div").WithClass("sourceText")
                .Where(n => names.Any(x => string.Compare(n.ParentNode.Element("h4")?.GetInnerText(), x, true) == 0))
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
                .Select(x => x.Join("="))
                .Join("&");

            // done
            return new string[] { parts[0], query }.Join("?");
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

        private class PropertyName
        {
            public static readonly PropertyName Age = new PropertyName("age", "alter");
            public static readonly PropertyName BaptismDate = new PropertyName("baptism date", "taufdatum");
            public static readonly PropertyName BaptismPlace = new PropertyName("baptism place", "taufort");
            public static readonly PropertyName BirthDate = new PropertyName("birth date", "geburtsdatum");
            public static readonly PropertyName BirthPlace = new PropertyName("birth place", "geburtsort");
            public static readonly PropertyName BurialDate = new PropertyName("burial date", "bestattungsdatum");
            public static readonly PropertyName BurialPlace = new PropertyName("burial place", "bestattungsort");
            public static readonly PropertyName CertificateNumber = new PropertyName("certificate number", "urkunde nummer");
            public static readonly PropertyName Child = new PropertyName("child", "kind");
            public static readonly PropertyName Church = new PropertyName("church", "kirche");
            public static readonly PropertyName CivilRegistrationOffice = new PropertyName("civil registration office", "standesamt");
            public static readonly PropertyName DeathAge = new PropertyName("death age", "sterbealter");
            public static readonly PropertyName DeathDate = new PropertyName("death date", "sterbedatum");
            public static readonly PropertyName DeathPlace = new PropertyName("death place", "Sterbeort");
            public static readonly PropertyName Father = new PropertyName("father", "vater");
            public static readonly PropertyName Gender = new PropertyName("gender", "geschlecht");
            public static readonly PropertyName MaidenName = new PropertyName("maiden name", "mädchenname");
            public static readonly PropertyName MarriageAge = new PropertyName("marriage age", "alter zur zeit der heirat");
            public static readonly PropertyName MarriageDate = new PropertyName("marriage date", "heiratsdatum");
            public static readonly PropertyName MarriagePlace = new PropertyName("marriage place", "heiratsort");
            public static readonly PropertyName Mother = new PropertyName("mother", "mutter");
            public static readonly PropertyName Name = new PropertyName("name", "name");
            public static readonly PropertyName PageNumber = new PropertyName("page number", "seitennummer");
            public static readonly PropertyName ParishAsItAppears = new PropertyName("parish as it appears", "kirchgemeinde wie angezeigt");
            public static readonly PropertyName SourceCitation = new PropertyName("source citation", "quellenangabe");
            public static readonly PropertyName SourceInformationInSearch = new PropertyName("source information", "quelleninformationen");
            public static readonly PropertyName SourceInformationInDiscoveryUI = new PropertyName("source information", "angaben zur quelle");
            public static readonly PropertyName Spouse = new PropertyName("spouse", "ehepartner");

            private PropertyName(string english, string deutsch)
            {
                // init
                this.English = english;
                this.Deutsch = deutsch;
            }

            public string English
            {
                get;
            }
            public string Deutsch
            {
                get;
            }

            public string this[Language language]
            {
                get
                {
                    switch (language)
                    {
                        case Language.Deutsch:
                            return this.Deutsch;
                        default:
                            return this.English;
                    }
                }
            }
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