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
        public Info[] Info
        {
            get;
            private set;
        }
        public string ItemOfInterest
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
                // init
                this.Akte = A2A.Akte.ParseXml(a2a);

                // parse info
                this.Info = this.Akte.GetInfo(out string itemOfInterest);
                this.ItemOfInterest = itemOfInterest;
            }

            // prepare
            var results = base.GetActivities(context).ToList();

            // image present through proxy?
            if (this.Akte?.Source?.AvailableScans is Scan[] scans)
            {
                // image path?
                var imageScan = scans.FirstOrDefault(x => x.Uri.StartsWith("https://www.openarch.nl/proxy/"));

                // any?
                if (imageScan != null)
                {
                    // add download activity
                    results.Add(new Activity.DownloadFileActivity(imageScan.Uri));
                }
            }

            // done
            return results;
        }

        public override Genealogy.Extensibility.Data.Source GetSource(Context context, Activity[] activities)
        {
            // init
            var capture = activities
                .OfType<Activity.ScreenCaptureActivity>()
                .Single();
            var images = activities
                .OfType<Activity.DownloadFileActivity>()
                .Where(x => x.Raw.NullCoalesce().Count() > 0)
                .ToArray();

            // done
            return new Genealogy.Extensibility.Data.Source()
            {
                Provenance = this.GetProvenance(context, images).ToArray(),
                Files = this.GetFiles(context, capture, images).ToArray(),
                Info = this.GetInfo(context).ToArray()
            };
        }

        protected IEnumerable<Repository> GetProvenance(Context context, Activity.DownloadFileActivity[] images)
        {
            // akte?
            var akte = this.Akte?.ToReference(this.ItemOfInterest);
            if (akte is Repository archive)
            {
                // download activity OK?
                if (images.Length > 0)
                {
                    // digital image of a record

                    // layer 1: digital image
                    yield return context.GetWebsite(() => new DigitalImage());

                    // layer 2: record in archive
                    yield return archive;
                }
                else
                {
                    // database entry of a record

                    // layer 1: database entry
                    yield return context.GetWebsite(() => new DatabaseEntry()
                    {
                        EntryFor = context.GetPageTitle()
                    });

                    // layer 2: record in archive
                    yield return archive;
                }


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
            return this.Info ?? base.GetInfo(context).ToArray();
        }
        protected IEnumerable<Genealogy.Extensibility.Data.File> GetFiles(Context context, Activity.ScreenCaptureActivity capture, Activity.DownloadFileActivity[] images)
        {
            // download activity
            if (images.Count() == 0)
            {
                // use screen capture
                return new Genealogy.Extensibility.Data.File[]
                {
                    capture.ToData()
                };
            }
            else
            {
                // use downlaoded image scan
                return images
                    .Select(x => x.ToData())
                    .ToArray();
            }
        }
    }
}