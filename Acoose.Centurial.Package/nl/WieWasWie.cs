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
    public partial class WieWasWie : Scraper.Default
    {
        public Record Data
        {
            get;
            private set;
        }

        public override IEnumerable<Activity> GetActivities(Context context)
        {
            // scan
            this.Scan(context);

            // screen shot
            yield return new Activity.ScreenCaptureActivity(context);
        }
        private void Scan(Context context)
        {
            // init
            var container = context.Html
                .Descendants("div")
                .Where(x => x.GetAttributeValue("class", "").Contains("sourcedetail-themepage"))
                .Single();
            var data = new Record();

            // persons
            data.Persons = container
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
                .Descendants("div")
                .Where(x => x.GetAttributeValue("class", "") == "gebeurtenis")
                .SingleOrDefault()
                .GetDescriptionLists((dt, dd) =>
                {
                    // init
                    var dataDictionary = dt.GetAttributeValue("data-dictionary", null);
                    switch (dataDictionary)
                    {
                        case "SourceDetail.Event":
                            data.EventType = Utility.TryParseEventType(dd.GetInnerText());
                            break;
                        case "SourceDetail.EventDate":
                            data.EventDate = Date.TryParse(dd.GetInnerText());
                            break;
                        case "SourceDetail.EventPlace":
                            data.EventPlace = dd.GetInnerText();
                            break;
                        case "SourceDetail.Religion":
                            data.Organization = dd.GetInnerText();
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
                            data.RecordType = RecordType.TryParse(docType);
                            break;
                        case "sourcedetail.heritageinstitutionname":
                            data.ArchiveName = dd.GetInnerText();
                            break;
                        case "sourcedetail.heritageinstitutionplace":
                            data.ArchivePlace = dd.GetInnerText();
                            break;
                        case "sourcedetail.registrationdate":
                            data.RecordDate = Date.TryParse(dd.GetInnerText());
                            break;
                        case "sourcedetail.certificateplace":
                            data.RecordPlace = dd.GetInnerText();
                            break;
                        case "sourcedetail.page":
                            data.Number = dd.GetInnerText();
                            break;
                    }

                });

            // done
            this.Data = data;
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
                Url = "https://www.wiewaswie.nl/",
                Title = "WieWasWie",
                Items = new OnlineItem[]
                {
                    new OnlineItem()
                    {
                        Url = context.Url,
                        Accessed = Date.Today,
                        Item = new DatabaseEntry()
                        {
                           EntryFor = this.Data.GenerateItemOfInterest(),
                        }
                    }
                }
            };

            // laag 2: onbekende bron
            yield return this.Data.GenerateRepository();
        }
        protected override IEnumerable<Info> GetInfo(Context context)
        {
            // done
            return this.Data.GenerateInfos();
        }
        protected override IEnumerable<Genealogy.Extensibility.Data.File> GetFiles(Context context, Activity[] activities)
        {
            return base.GetFiles(context, activities);
        }
    }
}