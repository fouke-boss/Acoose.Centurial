using Acoose.Genealogy.Extensibility;
using Acoose.Genealogy.Extensibility.Data;
using Acoose.Genealogy.Extensibility.Data.References;
using Acoose.Genealogy.Extensibility.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Acoose.Centurial.Package.org
{
    [Scraper("http://*.familysearch.org/ark:/*")]
    public class FamilySearch : Genealogy.Extensibility.Web.Scraper.Default
    {
        private static readonly Regex FILM_CITATION_PATTERN = new Regex("\"(.+)\".+\\(.+\\), (.+ image \\d+ of \\d+);(.+)\\.");
        private static readonly Regex[] RECORD_CITATION_PATTERNS = new Regex[]
        {
            new Regex(@"\(http.*\),\s*(.+);\s*citing\s*(.+)"),
            new Regex(@"\(http.*\),\s*(.+);\s*(.+)")
        };

        public override IEnumerable<Activity> GetActivities(Context context)
        {
            // init
            var main = context.Html.SelectSingleNode("//main");

            // are we dealing with a film viewer or a record?
            if (main.SelectSingleNode(".//record-details") != null)
            {
                // record
                this.PageType = PageTypes.Record;
            }
            else if (main.SelectSingleNode(".//fs-film-viewer") != null)
            {
                // film viewer
                this.PageType = PageTypes.FilmViewer;
            }
            else
            {
                // not supported
                this.PageType = PageTypes.NotSupported;
            }

            // mode
            switch (this.PageType)
            {
                case PageTypes.FilmViewer:
                    // download image
                    var imagePath = context.Html.SelectNodes("//a[contains(@class, 'saveButton')]")
                        .NullCoalesce()
                        .FirstOrDefault()?
                        .GetAttributeValue("href", "");
                    var imageUrl = string.Format("https://www.familysearch.org{0}", imagePath);

                    // done
                    return new Activity[] { new Activity.DownloadFileActivity(imageUrl) };
                default:
                    // get screenshot
                    return base.GetActivities(context);
            }
        }

        public PageTypes PageType
        {
            get;
            private set;
        }

        protected override IEnumerable<Genealogy.Extensibility.Data.File> GetFiles(Context context, Activity[] activities)
        {
            // page type?
            switch (this.PageType)
            {
                case PageTypes.FilmViewer:
                    // init
                    var activity = activities.OfType<Activity.DownloadFileActivity>().Single();
                    var extension = (activity.MimeType ?? "").Split('/').Last();
                    var url = new Uri(activity.Url).Query.Split(';').First().Split('=').Last().UrlDecode();

                    // done
                    return new Genealogy.Extensibility.Data.File[]
                    {
                        new Genealogy.Extensibility.Data.File()
                        {
                            OriginalName = string.Format("record-image_{0}.{1}", System.IO.Path.GetFileName(url), extension),
                            Raw = activity.Raw
                        }
                    };
                default:
                    return base.GetFiles(context, activities);
            }
        }
        protected override IEnumerable<Repository> GetProvenance(Context context)
        {
            // page type?
            switch (this.PageType)
            {
                case PageTypes.FilmViewer:
                    return this.GetProvenanceForFilmViewer(context);
                case PageTypes.Record:
                    return this.GetProvenanceForRecord(context);
                case PageTypes.NotSupported:
                    return base.GetProvenance(context);
                default:
                    throw new NotSupportedException();
            }
        }
        protected IEnumerable<Repository> GetProvenanceForRecord(Context context)
        {
            // get citation and catalog
            var collectionName = this.GetMetaTag(context, "historicalCollection");
            var note = this.GetMetaTag(context, "note").HtmlDecode();

            // parse citation
            var match = RECORD_CITATION_PATTERNS
                .Select(x => x.Match(note))
                .FirstOrDefault(x => x.Success);
            var entry = match?.Groups[1].Value.Trim();
            var citing = match?.Groups[2].Value.Trim(' ', '.');

            // layer 1: digital image
            yield return new Website()
            {
                Title = "FamilySearch",
                Url = "https://www.familysearch.org/",
                IsVirtualArchive = true,
                Items = new OnlineItem[]
                {
                    new OnlineItem()
                    {
                        Item = new OnlineCollection()
                        {
                            Title = collectionName,
                            Items = new OnlineItem[]
                            {
                                new OnlineItem()
                                {
                                    Accessed = Date.Today,
                                    Url = context.Url,
                                    Item = new DatabaseEntry()
                                    {
                                        EntryFor = entry
                                    }
                                }
                            }
                        }
                    }
                }
            };

            // layer 2: unspecified
            yield return new UnknownRepository()
            {
                Items = new Genealogy.Extensibility.Data.References.Source[]
                {
                    new Unspecified()
                    {
                        CreditLine = citing
                    }
                }
            };
        }
        protected IEnumerable<Repository> GetProvenanceForFilmViewer(Context context)
        {
            // get citation and catalog
            var citation = context.Html.SelectNodes("//p[@id='image-citation']")
                .NullCoalesce()
                .FirstOrDefault()?.InnerText;
            var catalogs = context.Html.SelectNodes("//tr[@data-ng-repeat='filmNote in imageInfo.filmNotes']/td")
                .NullCoalesce()
                .Select(x => (x.InnerText ?? "").Trim(' ', ';', ':', '.'))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();
            var catalog = string.Join("; ", catalogs);

            // parse citation
            var match = FILM_CITATION_PATTERN.Match(citation);
            var collection = match.Groups[1].Value.Trim(' ', ',');
            var path = match.Groups[2].Value.HtmlDecode().Trim();
            var citing = match.Groups[3].Value.Trim();

            // layer 1: digital image
            yield return new Website()
            {
                Title = "FamilySearch",
                Url = "https://www.familysearch.org/",
                IsVirtualArchive = true,
                Items = new OnlineItem[]
                {
                    new OnlineItem()
                    {
                        Item = new OnlineCollection()
                        {
                            Title = collection,
                            Items = new OnlineItem[]
                            {
                                new OnlineItem()
                                {
                                    Accessed = Date.Today,
                                    Path = path,
                                    Url = context.Url,
                                    Item = new DigitalImage()
                                    {
                                        CreditLine = citing
                                    }
                                }
                            }
                        }
                    }
                }
            };

            // layer 2: unspecified source, or can we do a bit more?
            var pathParts = path.Split('>').Select(x => x.Trim()).ToArray();
            if (pathParts.Length == 3 && catalog.ToLower().Contains("burgerlijke stand"))
            {
                // layer 2: vital record
                yield return new UnknownRepository()
                {
                    Items = new Genealogy.Extensibility.Data.References.Source[]
                    {
                        new VitalRecord()
                        {
                            Jurisdiction = pathParts.First(),
                            Title = new GenericTitle(){Value = "Burgerlijke stand", Literal=false },
                            Items = new RecordScriptFormat[]
                            {
                                new RecordScriptFormat()
                            }
                        }
                    }
                };
            }
            else
            {
                // layer 2: unspecified
                yield return new UnknownRepository()
                {
                    Items = new Genealogy.Extensibility.Data.References.Source[]
                    {
                        new Unspecified()
                        {
                            CreditLine = catalog
                        }
                    }
                };
            }
        }

        private string GetMetaTag(Context context, string itemprop)
        {
            return context.MetaTags
                .First(x => x.GetAttributeValue("itemprop", "") == itemprop)
                .GetAttributeValue("content", "");
        }

        public enum PageTypes
        {
            Record,
            FilmViewer,
            NotSupported
        }
    }
}