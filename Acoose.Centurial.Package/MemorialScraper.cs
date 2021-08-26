using Acoose.Genealogy.Extensibility.Data;
using Acoose.Genealogy.Extensibility.Data.References;
using Acoose.Genealogy.Extensibility.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acoose.Centurial.Package
{
    public abstract class MemorialScraper : Scraper.Default
    {
        protected MemorialScraper(string websiteTitle)
        {
            // init
            this.WebsiteTitle = websiteTitle;
        }

        public string URL
        {
            get; private set;
        }

        public string WebsiteTitle
        {
            get;
        }
        public string WebsiteURL
        {
            get; private set;
        }

        public string CemeteryName
        {
            get; protected set;
        }
        public string CemeteryPlace
        {
            get; protected set;
        }
        public string CemeteryAccess
        {
            get; protected set;
        }

        public string PhotographedBy
        {
            get; protected set;
        }
        public Date PhotographedAt
        {
            get; protected set;
        }

        public Person[] Persons
        {
            get; protected set;
        }
        public string[] Images
        {
            get; protected set;
        }

        public override IEnumerable<Activity> GetActivities(Context context)
        {
            // init
            this.URL = context.Url;
            this.WebsiteURL = context.GetWebsiteUrl();

            // scan
            this.Scan(context);

            // done
            var activities = this.Images
                .NullCoalesce()
                .Select(x => new Activity.DownloadFileActivity(x))
                .Cast<Activity>()
                .ToList();
            if (activities.Count == 0)
            {
                // screen capture
                activities.AddRange(base.GetActivities(context));
            }

            // done
            return activities;
        }
        protected abstract void Scan(Context context);

        protected override IEnumerable<Repository> GetProvenance(Context context)
        {
            // init
            var web = (this.Images.NullCoalesce().Any() ? (Acoose.Genealogy.Extensibility.Data.References.Source)new DigitalImage() : new DatabaseEntry() { EntryFor = this.Persons.First().Name });

            // layer 1: website
            yield return new Website()
            {
                Title = this.WebsiteTitle,
                Url = this.WebsiteURL,
                IsVirtualArchive = true,
                Items = new OnlineItem[]
               {
                    new OnlineItem()
                    {
                        Url = this.URL,
                        Accessed = Date.Today,
                        Item = web
                    }
               }
            };

            // layer 2: photograph
            if (!string.IsNullOrEmpty(this.PhotographedBy))
            {
                // parse
                Acoose.Genealogy.Extensibility.ParsingUtility.ParseName(this.PhotographedBy, out var familyName, out var givenNames, out var particles);

                // done
                yield return new UnknownRepository()
                {
                    Items = new Genealogy.Extensibility.Data.References.Source[]
                    {
                        new Photograph()
                        {
                            Creator = new PersonalName()
                            {
                                FamilyName = familyName,
                                GivenNames = givenNames,
                                Particles = particles
                            },
                            Date = this.PhotographedAt
                        }
                    }
                };
            }

            // layer 3: cemetery
            yield return new UnknownRepository()
            {
                Items = new Genealogy.Extensibility.Data.References.Source[]
                {
                    new Cemetery()
                    {
                        CemeteryName = this.CemeteryName,
                        Place = this.CemeteryPlace,
                        AccessData = this.CemeteryAccess,
                        Items = new CemeteryItem[]
                        {
                            new Gravestone()
                            {
                                Person = string.Join(" & ", this.Persons.Select(x => x.Name))
                            }
                        }
                    }
                }
            };
        }
        protected override IEnumerable<Info> GetInfo(Context context)
        {
            // init
            var burialPlace = string.Join(", ", new string[] { this.CemeteryName, this.CemeteryPlace }.Where(x => !string.IsNullOrWhiteSpace(x)));

            // done
            return this.Persons
                .Select((p, i) =>
                {
                    // id
                    p.Id = i + 1;

                    // done
                    var result = p.ToInfo(p.DeathDate);

                    // set burial place
                    result.ImportEvent("Burial", null, burialPlace);

                    // done
                    return result;
                })
                .ToArray();
        }
    }
}