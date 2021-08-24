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
            var partnership = this.GenerateParentChild(EventRole.Bride, EventRole.Groom, null, true)
                .ToArray();

            // relationships
            var relationships = new IEnumerable<RelationshipInfo>[]
            {
                partnership,
                this.GenerateParentChild(EventRole.Father, EventRole.Child, ParentChild.Person1IsBiologicalParentOfPerson2, null),
                this.GenerateParentChild(EventRole.Mother, EventRole.Child, ParentChild.Person1IsBiologicalParentOfPerson2, null),
                this.GenerateParentChild(EventRole.Father, EventRole.Deceased, ParentChild.Person1IsBiologicalParentOfPerson2, null),
                this.GenerateParentChild(EventRole.Mother, EventRole.Deceased, ParentChild.Person1IsBiologicalParentOfPerson2, null),
                this.GenerateParentChild(EventRole.FatherOfBride, EventRole.Bride, ParentChild.Person1IsBiologicalParentOfPerson2, null),
                this.GenerateParentChild(EventRole.MotherOfBride, EventRole.Bride, ParentChild.Person1IsBiologicalParentOfPerson2, null),
                this.GenerateParentChild(EventRole.FatherOfGroom, EventRole.Groom, ParentChild.Person1IsBiologicalParentOfPerson2, null),
                this.GenerateParentChild(EventRole.MotherOfGroom, EventRole.Groom, ParentChild.Person1IsBiologicalParentOfPerson2, null)
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

                // import event
                target.ImportEvent(
                    this.EventType.ToString(),
                    this.EventDate ?? this.RecordDate,
                    this.EventPlace ?? this.RecordPlace,
                    EnsureMode.AddIfNonePresent
                );
            }

            // done
            return results;
        }
        private IEnumerable<RelationshipInfo> GenerateParentChild(EventRole parent, EventRole child, ParentChild? parentChild, bool? partners)
        {
            // init
            var parents = this.Persons
                .Where(x => x.Role == parent)
                .ToList();
            var children = this.Persons
                .Where(x => x.Role == child)
                .ToList();

            // done
            if (parents.Count == 1 && children.Count == 1)
            {
                // init
                var result = new RelationshipInfo()
                {
                    Person1Id = parents.Single().Id.ToString(),
                    Person2Id = children.Single().Id.ToString(),
                    IsParentChild = parentChild.ToArrayIfAny(),
                    IsPartnership = partners.ToArrayIfAny()
                };

                // done
                yield return result;
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