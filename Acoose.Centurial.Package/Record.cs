using Acoose.Genealogy.Extensibility.Data;
using Acoose.Genealogy.Extensibility.Data.References;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acoose.Centurial.Package
{
    public class Record
    {
        private static readonly EventRole[] PRINCIPAL_ROLES = new EventRole[] { EventRole.Child, EventRole.Deceased, EventRole.Principal };
        private static readonly EventRole[] PRINCIPAL_PARENT_ROLES = new EventRole[] { EventRole.Father, EventRole.Mother };
        private static readonly EventRole[] PRINCIPAL_PARTNER_ROLES = new EventRole[] { EventRole.Partner };
        private static readonly EventRole[] BRIDE_ROLES = new EventRole[] { EventRole.Bride };
        private static readonly EventRole[] BRIDE_PARENT_ROLES = new EventRole[] { EventRole.FatherOfBride, EventRole.MotherOfBride };
        private static readonly EventRole[] GROOM_ROLES = new EventRole[] { EventRole.Groom };
        private static readonly EventRole[] GROOM_PARENT_ROLES = new EventRole[] { EventRole.FatherOfGroom, EventRole.MotherOfGroom };

        public RecordType RecordType
        {
            get; set;
        }
        public Date RecordDate
        {
            get; set;
        }
        public string RecordPlace
        {
            get; set;
        }

        public string ArchiveName
        {
            get; set;
        }
        public string ArchivePlace
        {
            get; set;
        }
        public string CollectionName
        {
            get; set;
        }
        public string CollectionNumber
        {
            get; set;
        }
        public string Title
        {
            get; set;
        }
        public string SeriesNumber
        {
            get; set;
        }

        /// <summary>
        /// Organization or church name.
        /// </summary>
        public string Organization
        {
            get; set;
        }
        public string Label
        {
            get; set;
        }

        public string Page
        {
            get; set;
        }
        public string Number
        {
            get; set;
        }

        public EventType? EventType
        {
            get; set;
        }
        public Date EventDate
        {
            get; set;
        }
        public string EventPlace
        {
            get; set;
        }

        public Person[] Persons
        {
            get; set;
        }
        public Person Principal
        {
            get
            {
                return this.Persons.SingleOrDefault(x => PRINCIPAL_ROLES.Contains(x.Role));
            }
        }
        public IEnumerable<Person> Principals
        {
            get
            {
                return this.Persons
                    .Where(x => PRINCIPAL_ROLES.Contains(x.Role) || x.Role == EventRole.Bride || x.Role == EventRole.Groom);
            }
        }

        public string[] Images
        {
            get;set;
        }

        public IEnumerable<Info> GenerateInfos()
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
                    return Gender.Male;
                case EventRole.Mother:
                case EventRole.MotherOfBride:
                case EventRole.MotherOfGroom:
                case EventRole.Bride:
                    return Gender.Female;
                default:
                    return null;
            }
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
        public string GenerateItemOfInterest()
        {
            return string.Join(" & ", this.Principals.Select(x => x.Name));
        }

        public Repository GenerateRepository()
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
    }
}