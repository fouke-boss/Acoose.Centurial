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
        public string ItemOfInterest
        {
            get; set;
        }

        public EventType EventType
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

            // relationships
            var relationships = new IEnumerable<RelationshipInfo>[]
            {
                this.GenerateParentChild(EventRole.Bride, EventRole.Groom, null, true),
                this.GenerateParentChild(EventRole.Father, EventRole.Child, ParentChild.Person1IsBiologicalParentOfPerson2, null),
                this.GenerateParentChild(EventRole.Mother, EventRole.Child, ParentChild.Person1IsBiologicalParentOfPerson2, null),
                this.GenerateParentChild(EventRole.Father, EventRole.Deceased, ParentChild.Person1IsBiologicalParentOfPerson2, null),
                this.GenerateParentChild(EventRole.Mother, EventRole.Deceased, ParentChild.Person1IsBiologicalParentOfPerson2, null),
                this.GenerateParentChild(EventRole.FatherOfBride, EventRole.Bride, ParentChild.Person1IsBiologicalParentOfPerson2, null),
                this.GenerateParentChild(EventRole.MotherOfBride, EventRole.Bride, ParentChild.Person1IsBiologicalParentOfPerson2, null),
                this.GenerateParentChild(EventRole.FatherOfGroom, EventRole.Groom, ParentChild.Person1IsBiologicalParentOfPerson2, null),
                this.GenerateParentChild(EventRole.MotherOfGroom, EventRole.Groom, ParentChild.Person1IsBiologicalParentOfPerson2, null)
            }
            .SelectMany(x => x);

            // done
            var results = persons
                .Cast<Info>()
                .Concat(relationships)
                .ToArray();

            // event
            switch (this.EventType)
            {
                case EventType.Baptism:
                case EventType.Birth:
                    this.ImportEventForPerson(results, EventRole.Child);
                    break;
                case EventType.CivilMarriage:
                    this.ImportEventForPartnership(results);
                    break;
                case EventType.Death:
                    this.ImportEventForPerson(results, EventRole.Deceased);
                    break;
                default:
                    throw new NotSupportedException();
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
                case EventRole.Child:
                    return null;
                default:
                    throw new NotSupportedException();
            }
        }
        private void ImportEventForPerson(IEnumerable<Info> info, EventRole role)
        {
            // init
            var principal = this.Persons.Single(x => x.Role == role);
            var match = info
                .OfType<PersonInfo>()
                .Single(x => x.Id == principal.Id.ToString());

            // add
            match.ImportEvent(this.EventType.ToString(), this.EventDate, this.EventPlace);
        }
        private void ImportEventForPartnership(IEnumerable<Info> info)
        {
            // init
            var match = info
                .OfType<RelationshipInfo>()
                .Single(r => r.IsPartnership.NullCoalesce().Any(x => x));

            // add
            match.ImportEvent(this.EventType.ToString(), this.EventDate, this.EventPlace);
        }

        public Repository GenerateRepository()
        {
            // record format
            var details = new RecordScriptFormat[]
            {
                new RecordScriptFormat()
                {
                    Date = this.RecordDate,
                    Label = this.Label,
                    Number = this.Number,
                    Page = this.Page,
                    ItemOfInterest = "xx"
                }
            };

            // init
            var source = default(Acoose.Genealogy.Extensibility.Data.References.Source);

            switch (this.RecordType)
            {
                case RecordType.Vital:
                    source = new VitalRecord()
                    {
                        Jurisdiction = this.RecordPlace,
                        Title = this.Title.ToGenericTitle(true),
                        Items = details
                    };
                    break;
                case RecordType.Church:
                    source = new ChurchRecord()
                    {
                        Church = this.Organization,
                        Place = this.RecordPlace,
                        Title = this.Title.ToGenericTitle(true),
                        Items = details
                    };
                    break;
                case RecordType.Census:
                    source = new Census()
                    {
                        Jurisdiction = this.RecordPlace,
                        Title = this.Title,
                        Items = new CensusScriptFormat[]
                        {
                            new CensusScriptFormat()
                            {
                                Date = this.RecordDate,
                                Page = this.Page,
                                ItemOfInterest = "xx"
                            }
                        }
                    };
                    break;
                default:
                    throw new NotSupportedException();
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