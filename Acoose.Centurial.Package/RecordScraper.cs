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
    public abstract class RecordScraper : Scraper.Default
    {
        private static readonly EventRole[] PRINCIPAL_ROLES = new EventRole[] { EventRole.Child, EventRole.Deceased, EventRole.Principal };
        private static readonly EventRole[] PRINCIPAL_PARENT_ROLES = new EventRole[] { EventRole.Father, EventRole.Mother };
        private static readonly EventRole[] PRINCIPAL_PARTNER_ROLES = new EventRole[] { EventRole.Partner, EventRole.Husband, EventRole.Wife };
        private static readonly EventRole[] BRIDE_ROLES = new EventRole[] { EventRole.Bride };
        private static readonly EventRole[] BRIDE_PARENT_ROLES = new EventRole[] { EventRole.FatherOfBride, EventRole.MotherOfBride };
        private static readonly EventRole[] GROOM_ROLES = new EventRole[] { EventRole.Groom };
        private static readonly EventRole[] GROOM_PARENT_ROLES = new EventRole[] { EventRole.FatherOfGroom, EventRole.MotherOfGroom };

        protected RecordScraper(string websiteTitle)
        {
            // init
            this.WebsiteTitle = websiteTitle;
        }

        public string URL
        {
            get; protected set;
        }

        public string WebsiteTitle
        {
            get;
        }
        public string WebsiteURL
        {
            get; private set;
        }
        public string OnlineCollectionName
        {
            get; protected set;
        }

        public RecordType RecordType
        {
            get; protected set;
        }
        public Date RecordDate
        {
            get; protected set;
        }
        public string RecordPlace
        {
            get; protected set;
        }

        public EventType? EventType
        {
            get; protected set;
        }
        public Date EventDate
        {
            get; protected set;
        }
        public string EventPlace
        {
            get; protected set;
        }

        public string ArchiveName
        {
            get; protected set;
        }
        public string ArchivePlace
        {
            get; protected set;
        }
        public string CollectionName
        {
            get; protected set;
        }
        public string CollectionNumber
        {
            get; protected set;
        }
        public string Title
        {
            get; protected set;
        }
        public string SeriesNumber
        {
            get; protected set;
        }

        /// <summary>
        /// Organization or church name.
        /// </summary>
        public string Organization
        {
            get; protected set;
        }
        public string Label
        {
            get; protected set;
        }

        public string Page
        {
            get; protected set;
        }
        public string Number
        {
            get; protected set;
        }

        public Person[] Persons
        {
            get; protected set;
        }
        public Person Principal
        {
            get
            {
                return this.Persons
                    .NullCoalesce()
                    .SingleOrDefault(x => PRINCIPAL_ROLES.Contains(x.Role));
            }
        }
        public IEnumerable<Person> Principals
        {
            get
            {
                return this.Persons
                    .NullCoalesce()
                    .Where(x => PRINCIPAL_ROLES.Contains(x.Role) || x.Role == EventRole.Bride || x.Role == EventRole.Groom);
            }
        }
        public string GenerateItemOfInterest()
        {
            return string.Join(" & ", this.Principals.Select(x => x.Name));
        }

        public string[] Images
        {
            get; set;
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
            // layer 1: website
            yield return this.GenerateLayer1();

            // layer 2: record
            yield return this.GenerateLayer2();
        }
        private Repository GenerateLayer1()
        {
            // init
            var item = new OnlineItem()
            {
                Url = this.URL,
                Accessed = Date.Today,
                Item = new DatabaseEntry()
                {
                    EntryFor = this.GenerateItemOfInterest()
                }
            };

            // online collection?
            if (!string.IsNullOrWhiteSpace(this.OnlineCollectionName))
            {
                item = new OnlineItem()
                {
                    Item = new OnlineCollection()
                    {
                        Title = this.OnlineCollectionName,
                        Items = new OnlineItem[]
                        {
                            item
                        }
                    }
                };
            }

            // done
            return new Website()
            {
                Title = this.WebsiteTitle,
                Url = this.WebsiteURL,
                IsVirtualArchive = true,
                Items = new OnlineItem[]
                {
                    item
                }
            };
        }
        private Repository GenerateLayer2()
        {
            // init
            Acoose.Genealogy.Extensibility.Data.References.Source source;

            // source
            if (this.RecordType == null)
            {
                // init
                var parts = new string[]
                {
                    this.Organization,
                    this.RecordPlace,
                    this.Title,
                    this.Label,
                };

                // unknown
                source = new Unknown()
                {
                    CreditLine = string.Join("; ", parts.Where(x => !string.IsNullOrEmpty(x)))
                };
            }
            else
            {
                // per record type
                source = this.RecordType.Generate(this);
            }

            // archived item
            var archivedItems = new ArchivedItem[]
            {
                new ArchivedItem()
                {
                    Identifier = this.SeriesNumber,
                    Item = source
                }
            };

            // collection?
            if (!string.IsNullOrEmpty(this.CollectionName) || !string.IsNullOrEmpty(this.CollectionNumber))
            {
                // wrap
                archivedItems = new ArchivedItem[]
                {
                    new ArchivedItem()
                    {
                        Identifier = this.CollectionNumber,
                        Item = new Collection()
                        {
                            Name = this.CollectionName,
                            Items = archivedItems
                        }
                    }
                };
            }

            // archive
            return new PublicArchive()
            {
                Name = this.ArchiveName,
                Place = this.ArchivePlace,
                Items = archivedItems
            };
        }
        internal RecordScriptFormat[] GenerateRecordScriptFormat()
        {
            return new RecordScriptFormat[]
            {
                new RecordScriptFormat()
                {
                    Date = this.RecordDate,
                    Label = this.Label,
                    Number = this.Number,
                    Page = this.Page,
                    ItemOfInterest = this.GenerateItemOfInterest()
                }
            };
        }
        internal CensusScriptFormat[] GenerateCensusScriptFormt()
        {
            return new CensusScriptFormat[]
            {
                new CensusScriptFormat()
                {
                    Date = this.RecordDate,
                    Page = this.Page,
                    ItemOfInterest = this.GenerateItemOfInterest()
                }
            };
        }

        protected override IEnumerable<Info> GetInfo(Context context)
        {
            // persons
            var persons = this.Persons
                .Select((p, i) =>
                {
                    // id
                    p.Id = i + 1;

                    // gender
                    p.Gender = p.Gender ?? this.GetGender(p.Role);

                    // done
                    return p.ToInfo(this.RecordDate);
                })
                .ToList();

            // partnership
            var partnership = this.GenerateRelationship(BRIDE_ROLES, GROOM_ROLES, null, true)
                .ToArray();

            // relationships
            var relationships = new IEnumerable<RelationshipInfo>[]
            {
                partnership,
                this.GenerateRelationship(PRINCIPAL_PARENT_ROLES, PRINCIPAL_ROLES, ParentChild.Person1IsBiologicalParentOfPerson2, null),
                this.GenerateRelationship(PRINCIPAL_PARTNER_ROLES, PRINCIPAL_ROLES, null, true),
                this.GenerateRelationship(BRIDE_PARENT_ROLES, BRIDE_ROLES, ParentChild.Person1IsBiologicalParentOfPerson2, null),
                this.GenerateRelationship(GROOM_PARENT_ROLES, GROOM_ROLES, ParentChild.Person1IsBiologicalParentOfPerson2, null),
            }
            .SelectMany(x => x)
            .ToList();

            // done
            var results = persons
                .Cast<Info>()
                .Concat(relationships)
                .ToArray();

            // event
            if (this.EventType is EventType eventType)
            {
                // target
                var target = (partnership.Length > 0 ? (InfoWithEvents)partnership.Single() : persons.Single(x => x.Id == this.Principal.Id.ToString()));

                // marriage?
                var text = eventType.ToString();
                if (eventType == Package.EventType.Marriage)
                {
                    text = (this.RecordType is RecordType<ChurchRecord> ? "ChurchMarriage" : "CivilMarriage");
                }

                // import event
                target.ImportEvent(
                    text,
                    this.EventDate ?? this.RecordDate,
                    this.EventPlace ?? this.RecordPlace,
                    EnsureMode.AddIfNonePresent
                );
            }

            // done
            return results;
        }
        private IEnumerable<RelationshipInfo> GenerateRelationship(EventRole[] roles1, EventRole[] roles2, ParentChild? parentChild, bool? partnership)
        {
            foreach (var role1 in roles1)
            {
                foreach (var role2 in roles2)
                {
                    // init
                    var persons1 = this.Persons
                        .Where(x => x.Role == role1)
                        .ToList();
                    var persons2 = this.Persons
                        .Where(x => x.Role == role2)
                        .ToList();

                    // done
                    if (persons1.Count == 1 && persons2.Count == 1)
                    {
                        // init
                        var result = new RelationshipInfo()
                        {
                            Person1Id = persons1.Single().Id.ToString(),
                            Person2Id = persons2.Single().Id.ToString(),
                            IsParentChild = parentChild.ToArrayIfAny(),
                            IsPartnership = partnership.ToArrayIfAny()
                        };

                        // done
                        yield return result;
                    }
                }
            }
        }
        private Gender? GetGender(EventRole role)
        {
            switch (role)
            {
                case EventRole.Father:
                case EventRole.FatherOfBride:
                case EventRole.FatherOfGroom:
                case EventRole.Groom:
                case EventRole.Husband:
                    return Gender.Male;
                case EventRole.Mother:
                case EventRole.MotherOfBride:
                case EventRole.MotherOfGroom:
                case EventRole.Bride:
                case EventRole.Wife:
                    return Gender.Female;
                default:
                    return null;
            }
        }
    }
}