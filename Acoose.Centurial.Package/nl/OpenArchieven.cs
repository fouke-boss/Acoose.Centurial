using Acoose.Genealogy.Extensibility.Data;
using Acoose.Genealogy.Extensibility.Data.References;
using Acoose.Genealogy.Extensibility.Web;
using Acoose.Centurial.Package.nl.A2A;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acoose.Centurial.Package.nl
{
    [Scraper("https://www.openarch.nl/*")]
    public class OpenArchieven : Scraper.Default
    {
        public Akte Akte
        {
            get;
            private set;
        }

        public override IEnumerable<Activity> GetActivities(Context context)
        {
            // load a2a
            var a2a = context.Html.OwnerDocument.GetElementbyId("a2a")?.InnerText;
            if (!string.IsNullOrWhiteSpace(a2a))
            {
                this.Akte = A2A.Akte.ParseXml(a2a);
            }

            // done
            return base.GetActivities(context);
        }

        protected override IEnumerable<Repository> GetProvenance(Context context)
        {
            // akte?
            var akte = this.Akte?.ToReference();
            if (akte is Repository archive)
            {
                // layer 1: database entry
                yield return context.GetWebsite(() => new DatabaseEntry()
                {
                    EntryFor = context.GetPageTitle()
                });

                // layer 2: record in archive
                yield return archive;
            }
            else
            {
                // nope
                yield return base.GetProvenance(context).Single();
            }
        }
        protected override IEnumerable<Info> GetInfo(Context context)
        {
            // init
            var results = this.Akte?.GetInfo();
            if (results == null)
            {
                results = base.GetInfo(context).ToArray();
            }

            // done
            return results;
        }
    }
}