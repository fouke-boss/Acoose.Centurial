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
    [Scraper("https://www.wiewaswie.nl/*")]
    public partial class WieWasWie : Scraper.Default
    {
        private static readonly Regex DEATH_DATE_REGEX = new Regex(@"(\d{2}-\d{2}-\d{4})");

        public override IEnumerable<Activity> GetActivities(Context context)
        {
            //return Enumerable.Empty<Activity>();
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
                Url = context.GetWebsiteUrl(),
                Title = context.GetWebsiteTitle(),
                Items = new OnlineItem[]
                {
                    new OnlineItem()
                    {
                        Url = context.Url,
                        Accessed = Date.Today,
                        Item = new DatabaseEntry()
                        {
                           EntryFor = "xxx",
                        }
                    }
                }
            };

            // laag 2: onbekende bron
            yield return new UnknownRepository()
            {
                Items = new Genealogy.Extensibility.Data.References.Source[]
                {
                    new Unknown()
                    {
                        //CreditLine = string.Join(", ", this.Kranten)
                    }
                }
            };
        }
        protected override IEnumerable<Info> GetInfo(Context context)
        {
            // titel zoeken
            var container = context.Html
                .Descendants("div")
                .Where(x => x.GetAttributeValue("class", "").Contains("sourcedetail-themepage"))
                .Single();
            var @event = container
                .Descendants("div")
                .Where(x => x.GetAttributeValue("class", "") == "gebeurtenis")
                .Select(x => new WieWasWie.Event(x))
                .SingleOrDefault();
            @event.Persons = container
                .Descendants("div")
                .Where(x => x.GetAttributeValue("class", "") == "person")
                .Select(x => new WieWasWie.Person(x))
                .ToArray();

            // done
            return @event.Generate();
        }
        protected override IEnumerable<Genealogy.Extensibility.Data.File> GetFiles(Context context, Activity[] activities)
        {
            return base.GetFiles(context, activities);
        }
    }
}