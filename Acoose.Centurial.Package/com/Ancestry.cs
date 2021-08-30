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
                if (parts.Length >= 3)
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
            var person = new Person()
            {
                Name = recordData["name"].GetInnerText(),
                Gender = Utility.TryParseGender(recordData["gender"].GetChildText()),
                BirthDate = Date.TryParse(recordData["birth date"].GetChildText()),
                DeathPlace = recordData["death place"].GetChildText(),
                Age = Utility.TryParseAge(recordData["age"].GetInnerText())
            };
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
        private static readonly string[] QUERY_PARAMETERS = new string[] { "dbid", "h", "indiv" };
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

        private enum Property
        {
            Name,
            Gender,
            Age,
            BirthDate,
            Father,
            Mother
        }
    }
}