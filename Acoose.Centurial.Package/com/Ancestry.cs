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
        private const string SOURCE_INFORMATION_PREFIX = "Ancestry.com";
        private const string SOURCE_INFORMATION_SUFFIX = "[database on-line]";
        private static readonly string[] QUERY_PARAMETERS = new string[] { "db", "dbid", "h", "indiv" };
        private static readonly string[] MALE_PREFIXES = new string[] { "mr" };
        private static readonly string[] FEMALE_PREFIXES = new string[] { "mrs", "miss", "ms" };
        private string[] _SourceCitation;

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
            this._SourceCitation = (citationPanel
                .Descendants("div")
                .Where(n =>
                {
                    // init
                    var h4 = n.Element("h4")?.GetInnerText();

                    // done
                    return PropertyName.SourceCitation.Any(x => string.Compare(x, h4, true) == 0);
                })
                .SelectMany(x => x.Descendants("p"))
                .SingleOrDefault().GetInnerText()?
                .Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                .NullCoalesce()
                .Select(x => x.TrimAll())
                .ToArray();

            // split?
            if (this._SourceCitation.Length >= 3 && this._SourceCitation.Take(3).All(p => !p.Contains(":")))
            {
                this.ArchiveName = this._SourceCitation[0];
                this.ArchivePlace = this._SourceCitation[1];
                this.Label = this._SourceCitation[2];
            }

            // online
            this.URL = this.CleanURL(context.Url);
            this.OnlineCollectionName = this.GetOnlineCollectionName(citationPanel);

            // person
            var person = this.GetPrincipal(recordData);
            var others = new string[][] { PropertyName.Father, PropertyName.Mother, PropertyName.Spouse, PropertyName.Child }
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
                            Role = Utility.TryParseEventRole(property.First()).Value
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

        private Person GetPrincipal(PropertyBag<HtmlNode> recordData)
        {
            // init
            var result = new Person();

            // name
            var name = recordData[PropertyName.Name].ToPhrase().PreferFirstWithinBrackets().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
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
            result.Age = Utility.TryParseAge(recordData.Contains(PropertyName.Age).FirstOrDefault().GetInnerText());
            result.Gender = result.Gender ?? Utility.TryParseGender(recordData[PropertyName.Gender].ToPhrase().PreferWithoutBrackets());
            result.Occupation = recordData[PropertyName.Occupation].GetInnerText();

            // maiden name
            if (recordData[PropertyName.MaidenName].GetInnerText() is string maidenName && !string.IsNullOrWhiteSpace(maidenName))
            {
                result.FamilyName = maidenName;
            }

            // done
            return result;
        }
        private void AddEvent(Package.EventType eventType, PropertyBag<HtmlNode> recordData, string[] date, string[] place)
        {
            // init
            var dateValue = recordData[date].GetInnerText();
            var placeValue = recordData[place].ToPhrase().PreferWithoutBrackets();

            // any?
            if (!string.IsNullOrWhiteSpace(dateValue) || !string.IsNullOrWhiteSpace(placeValue))
            {
                // create
                var result = this.GetEvent(eventType);
                result.Date = Date.TryParse(dateValue);
                result.Place = placeValue.NullIfWhitespace();
            }
        }
        private void FixRecordType(PropertyBag<HtmlNode> recordData)
        {
            // init
            var properties = PropertyBag<string>.Load(
                this._SourceCitation
                    .NullCoalesce()
                    .Select(x => x.Split(':'))
                    .Where(x => x.Skip(1).Any()),
                x => x.First().TrimAll(), // key
                x => string.Join(":", x.Skip(1)).TrimAll() // value
            );

            // record type
            if (recordData.ContainsKey(PropertyName.ResidenceDate))
            {
                this.RecordType = RecordType.Census;
            }
            else
            {
                // type
                this.RecordType = RecordType.TryParse(this.Label) ?? RecordType.TryParse(this.OnlineCollectionName) ?? RecordType.TryParse(string.Join("|", this._SourceCitation));
            }


            // init
            if (this.RecordType is RecordType<Census>)
            {
                // census id
                this.CensusID = properties[PropertyName.NaraSeriesTitle] ??
                    recordData[PropertyName.Database].GetInnerText() ??
                    this.OnlineCollectionName;

                // record place
                this.RecordPlace = this.RecordPlace ?? properties[PropertyName.CensusPlace] ?? recordData[PropertyName.ResidencePlace].GetInnerText();
                if (this.RecordPlace == null)
                {
                    // county and state
                    this.RecordPlace = new string[]
                    {
                        recordData[PropertyName.County].ToPhrase().PreferWithoutBrackets(),
                        recordData[PropertyName.State].ToPhrase().PreferWithoutBrackets()
                    }.Join(", ");
                }

                // record date
                this.RecordDate = Date.TryParse(recordData[PropertyName.ResidenceDate].GetInnerText()) ??
                    Date.TryParse(properties[PropertyName.Year]);

                // from census name
                if (this.RecordDate == null)
                {
                    // from census id
                    var years = (this.CensusID ?? "")
                        .Split(new char[] { ' ', ',', '-' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => int.TryParse(x, out var year) ? year : -1)
                        .Where(x => x != -1)
                        .ToArray();
                    if (years.Count() == 1)
                    {
                        this.RecordDate = Date.Exact(Calendar.Gregorian, years.Single());
                    }
                }

                // miscellaneous
                this.CivilDivision = recordData[PropertyName.CivilDivision].GetInnerText();
                this.HouseholdId = recordData[PropertyName.FamilyNumber].GetInnerText();
            }
            else
            {
                // determine main event
                var mainEvent = this.Events
                    .OrderByDescending(x => x.Type)
                    .FirstOrDefault();
                if (mainEvent != null && !string.IsNullOrEmpty(mainEvent.Place))
                {
                    // set
                    this.RecordDate = mainEvent.Date;
                    this.RecordPlace = mainEvent.Place;

                    // details
                    this.Number = recordData[PropertyName.CertificateNumber].GetInnerText();
                }
            }

            // label
            var label = new string[][] { PropertyName.CivilRegistrationOffice }
                .Select(x => recordData[x])
                .Select(x => x.GetInnerText())
                .Prepend(this.Label)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim());
            this.Label = string.Join(", ", label).NullIfWhitespace();

            // organization
            var organization = new string[][] { PropertyName.Church, PropertyName.ParishAsItAppears }
                .Select(x => recordData[x].GetInnerText())
                .Prepend(this.Organization)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim());
            this.Organization = string.Join(", ", organization).NullIfWhitespace();

            // page
            this.Page = Utility.TryParsePageNumber(recordData[PropertyName.PageNumber].GetInnerText() ?? properties[PropertyName.PageNumber]);
        }
        private string GetOnlineCollectionName(HtmlNode citationPanel)
        {
            // init
            var result = default(string);
            var names = new string[][] { PropertyName.SourceInformationInSearch, PropertyName.SourceInformationInDiscoveryUI }
                .SelectMany(x => x)
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
                var end = info.IndexOf(SOURCE_INFORMATION_SUFFIX);
                if (info.StartsWith(SOURCE_INFORMATION_PREFIX) && end > 0)
                {
                    // find . after prefix
                    var start = info.IndexOf('.', SOURCE_INFORMATION_PREFIX.Length);

                    // done
                    result = info.Substring(start, end - start).TrimAll();
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

            // census
            if (this.RecordType is RecordType<Census>)
            {
                // set residence
                results
                    .OfType<PersonInfo>()
                    .ForEach(person =>
                    {
                        person.Residence = person.Residence.Ensure(new Status<string>() { Date = this.RecordDate, Value = this.RecordPlace });
                    });
            }

            // done
            return results;
        }
    }
}