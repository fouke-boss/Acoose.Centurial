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
        private static readonly Regex CITATION_PATTERN = new Regex("\"(.+)\".+\\(.+\\), (.+ image \\d+ of \\d+);(.+)\\.");

        public override IEnumerable<Activity> GetActivities(Context context)
        {
            // init
            var imagePath = context.Html.SelectNodes("//a[contains(@class, 'saveButton')]")
                .NullCoalesce()
                .FirstOrDefault()?
                .GetAttributeValue("href", "");
            var imageUrl = string.Format("https://www.familysearch.org{0}", imagePath);

            // done
            yield return new Activity.DownloadFileActivity(imageUrl);
        }

        protected override IEnumerable<Genealogy.Extensibility.Data.File> GetFiles(Context context, Activity[] activities)
        {
            // init
            var activity = activities.OfType<Activity.DownloadFileActivity>().Single();
            var extension = activity.MimeType.Split('/').Last();
            var url = new Uri(activity.Url).Query.Split(';').First().Split('=').Last().UrlDecode();

            // done
            yield return new Genealogy.Extensibility.Data.File()
            {
                OriginalName = string.Format("record-image_{0}.{1}", System.IO.Path.GetFileName(url), extension),
                Raw = activity.Raw
            };
        }
        protected override IEnumerable<Repository> GetProvenance(Context context)
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
            var match = CITATION_PATTERN.Match(citation);
            var collection = match.Groups[1].Value.Trim();
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
                yield return new None()
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
                yield return new None()
                {
                    Items = new Genealogy.Extensibility.Data.References.Source[]
                    {
                        new Unspecified()
                        {
                            CreditLine = string.Join("; ", catalogs)
                        }
                    }
                };
            }
        }
    }
}