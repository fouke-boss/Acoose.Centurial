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
    [Scraper("https://www.wiewaswie.nl/*/detail/*")]
    public partial class WieWasWie : RecordScraper
    {
        public WieWasWie()
            : base("WieWasWie")
        {
        }

        protected override void Scan(Context context)
        {
            // init
            var container = context.Html
                .Descendants("div")
                .Where(x => x.GetAttributeValue("class", "").Contains("sourcedetail-themepage"))
                .Single();

            // persons
            this.Persons = container
                .Descendants("div")
                .Where(x => x.GetAttributeValue("class", "") == "person")
                .Select(div =>
                {
                    // init
                    var result = new Package.Person();

                    // process descirption list
                    div.GetDescriptionLists((dt, dd) =>
                    {
                        // init
                        var dataDictionary = dt.GetAttributeValue("data-dictionary", null);
                        if (string.IsNullOrWhiteSpace(dataDictionary))
                        {
                            // name
                            result.Name = dd.GetInnerText();
                            result.Role = Utility.TryParseEventRole(dt.GetInnerText()) ?? EventRole.Attendee;
                        }
                        else
                        {
                            switch (dataDictionary)
                            {
                                case "SourceDetail.BirthDate":
                                    result.BirthDate = Date.TryParse(dd.GetInnerText());
                                    break;
                                case "SourceDetail.BirthPlace":
                                    result.BirthPlace = dd.GetInnerText();
                                    break;
                                case "SourceDetail.Age":
                                    result.Age = Utility.TryParseAge(dd.GetInnerText());
                                    break;
                                case "SourceDetail.Gender":
                                    result.Gender = Utility.TryParseGender(dd.GetInnerText());
                                    break;
                                case "SourceDetail.PlaceOfResidence":
                                    result.Residence = dd.GetInnerText();
                                    break;
                                case "SourceDetail.Profession":
                                    result.Occupation = dd.GetInnerText();
                                    break;
                            }
                        }
                    });

                    // done
                    return result;
                })
                .ToArray();

            // event
            container
                .Descendants("div").WithClass("gebeurtenis")
                .FirstOrDefault()
                .GetDescriptionLists((dt, dd) =>
                {
                    // init
                    var dataDictionary = dt.GetAttributeValue("data-dictionary", null);
                    switch (dataDictionary)
                    {
                        case "SourceDetail.Event":
                            var eventType = dd.GetInnerText()?.ToLower();
                            switch (eventType)
                            {
                                case "bidprentje":
                                    this.EventType = Package.EventType.Death;
                                    break;
                                default:
                                    this.EventType = Utility.TryParseEventType(dd.GetInnerText());
                                    break;
                            }
                            break;
                        case "SourceDetail.EventDate":
                            this.EventDate = Date.TryParse(dd.GetInnerText());
                            break;
                        case "SourceDetail.EventPlace":
                            this.EventPlace = dd.GetInnerText();
                            break;
                        case "SourceDetail.Religion":
                            this.Organization = dd.GetInnerText();
                            break;
                    }
                });

            // record
            container
                .Descendants("div").WithClass("akte")
                .Single()
                .GetDescriptionLists((dt, dd) =>
                {
                    switch (dt.Attribute("data-dictionary")?.ToLower())
                    {
                        case "sourcedetail.documenttype":
                            // init
                            var docType = dd.GetInnerText();

                            // parse
                            this.RecordType = RecordType.TryParse(docType);
                            break;
                        case "sourcedetail.heritageinstitutionname":
                            this.ArchiveName = dd.GetInnerText();
                            break;
                        case "sourcedetail.heritageinstitutionplace":
                            this.ArchivePlace = dd.GetInnerText();
                            break;
                        case "sourcedetail.archive":
                            this.SeriesNumber = dd.GetInnerText();
                            break;
                        case "sourcedetail.registrationdate":
                            this.RecordDate = Date.TryParse(dd.GetInnerText());
                            break;
                        case "sourcedetail.certificateplace":
                            this.RecordPlace = dd.GetInnerText();
                            break;
                        case "sourcedetail.page":
                            switch (dt.GetInnerText().ToLower())
                            {
                                case "page":
                                case "pagina":
                                    this.Page = dd.GetInnerText();
                                    break;
                                default:
                                    this.Number = dd.GetInnerText();
                                    break;
                            }
                            break;
                        case "sourcedetail.book":
                            this.Label = dd.GetInnerText();
                            break;
                    }

                });
        }
    }
}