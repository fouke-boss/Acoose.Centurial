using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Extensibility = Acoose.Genealogy.Extensibility;
using Data = Acoose.Genealogy.Extensibility.Data;
using References = Acoose.Genealogy.Extensibility.Data.References;
using Acoose.Genealogy.Extensibility.Data;

namespace Acoose.Centurial.Package.nl.A2A
{
    [XmlRoot("A2A", Namespace = "http://Mindbus.nl/A2A")]
    public class Akte
    {
        public static Akte ParseXml(string xml)
        {
            using (var tr = new StringReader(xml))
            {
                using (var xr = XmlReader.Create(tr))
                {
                    return LoadXml(xr);
                }
            }
        }
        public static Akte LoadXml(string path)
        {
            using (var fs = System.IO.File.OpenRead(path))
            {
                return LoadXml(fs);
            }
        }
        public static Akte LoadXml(Stream stream)
        {
            using (var xr = XmlReader.Create(stream))
            {
                return LoadXml(xr);
            }
        }
        public static Akte LoadXml(XmlReader reader)
        {
            // init
            var xs = new XmlSerializer(typeof(Akte));
            return (Akte)xs.Deserialize(reader);
        }

        [XmlAttribute]
        public string Version
        {
            get;
            set;
        }

        [XmlElement("Person")]
        public Person[] Persons
        {
            get;
            set;
        }
        [XmlElement("Event")]
        public Event[] Events
        {
            get;
            set;
        }
        [XmlElement("RelationEP")]
        public RelationEP[] RelationsEP
        {
            get;
            set;
        }
        [XmlElement("RelationPP")]
        public RelationPP[] RelationsPP
        {
            get;
            set;
        }
        [XmlElement]
        public Source Source
        {
            get;
            set;
        }

        public References.Representation ToReference(string itemOfInterest)
        {
            // minimal requirements
            if (this.Source?.SourceReference == null)
            {
                return null;
            }

            // init
            var source = this.Source;
            var @ref = source.SourceReference;

            // get record
            var record = this.GetRecord(@ref, itemOfInterest);
            if (record == null)
            {
                return null;
            }

            // wrap
            var archivedItems = new References.ArchivedItem[]
            {
                new References.ArchivedItem()
                {
                    Identifier = this.Source.SourceReference.RegistryNumber,
                    Item = record
                }
            };

            // collection?
            if (!string.IsNullOrWhiteSpace(@ref.Archive) || !string.IsNullOrWhiteSpace(@ref.Collection))
            {
                archivedItems = new References.ArchivedItem[]
                {
                    new References.ArchivedItem()
                    {
                        Identifier = @ref.Archive,
                        Item = new References.Collection()
                        {
                            Name = @ref.Collection,
                            Items = archivedItems
                        }
                    }
                };
            }

            // archive
            return new References.PublicArchive()
            {
                Name = @ref.InstitutionName,
                Place = @ref.Place,
                Items = archivedItems
            };
        }
        private References.Record GetRecord(SourceReference @ref, string itemOfInterest)
        {
            // init
            var formats = new References.RecordScriptFormat[]
            {
                new References.RecordScriptFormat()
                {
                    Label = @ref.Book,
                    Page = @ref.Folio,
                    Number = @ref.DocumentNumber,
                    Date = this.Source.SourceDate?.ToData(),
                    ItemOfInterest = itemOfInterest
                }
            };

            // title
            var title = (this.GetTitle(this.Source.SourceType) is string t ? new References.GenericTitle() { Literal = false, Value = t } : null);

            // source type?
            if (this.Source.SourceType.StartsWith("BS "))
            {
                return new References.VitalRecord()
                {
                    Jurisdiction = this.Source.SourcePlace?.ToString(),
                    Title = title,
                    Items = formats
                };
            }
            else if (this.Source.SourceType.StartsWith("DTB "))
            {
                // church record
                return new References.ChurchRecord()
                {
                    Church = "Kerk",
                    Place = this.Events.NullCoalesce().FirstOrDefault()?.EventPlace?.ToString(),
                    Title = title,
                    Items = formats
                };
            }
            else
            {
                // vital record
                return new References.LocalGovernmentRecord()
                {
                    Jurisdiction = this.Source.SourcePlace?.ToString(),
                    Title = title,
                    Items = formats
                };
            }
        }
        private string GetTitle(string sourceType)
        {
            // init
            var type = sourceType.ToLower();

            // known type?
            switch (type)
            {
                case "bs geboorte":
                case "bs huwelijk":
                case "bs overlijden":
                    return "Burgerlijke stand";
                case "dtb begraven":
                    return "Begraafregister";
                case "dtb dopen":
                    return "Doopboeken";
                case "dtb trouwen":
                    return "Trouwboeken";
                default:
                    if (type.StartsWith("other:"))
                    {
                        return sourceType.Substring(6).Trim();
                    }
                    else
                    {
                        return sourceType;
                    }
            }
        }

        public Info[] GetInfo(out string itemOfInterest)
        {
            // init
            itemOfInterest = string.Empty;

            // find event date
            var eventDate = this.Events.NullCoalesce().FirstOrDefault()?.EventDate?.ToData();

            // persons
            var persons = this.Persons.NullCoalesce()
                .Select((x, i) => new
                {
                    Id = x.Id,
                    Person = x.ToData(i + 1, eventDate)
                })
                .ToArray();

            // relationships between persons
            var relationships = new RelationshipCollection();

            // person 2 person relationships
            foreach (var pp in this.RelationsPP.NullCoalesce().Where(x => x.PersonKeyRef.NullCoalesce().Count() == 2))
            {
                // find
                var person1 = persons.Single(x => x.Id == pp.PersonKeyRef[0]).Person;
                var person2 = persons.Single(x => x.Id == pp.PersonKeyRef[1]).Person;

                // relation
                relationships.Create(person1, person2, pp.RelationType, eventDate);
            }

            // events
            foreach (var e in this.Events.NullCoalesce())
            {
                // init
                var eventPlace = e.EventPlace?.ToString();

                // find actors in this event
                var dict = this.RelationsEP.NullCoalesce()
                    .Where(x => x.EventKeyRef == e.Id)
                    .GroupBy(x => x.RelationType)
                    .ToDictionary(r => r.Key, g => g.Select(r => persons.SingleOrDefault(x => x.Id == r.PersonKeyRef)?.Person).ToArray());
                var actors = new RoleDictionary(dict);

                // relationships
                var subject = this.GetSubject(e.EventType)
                    .Where(x => actors.ContainsRole(x))
                    .FirstOrDefault();
                if (string.IsNullOrEmpty(subject))
                {
                    // vader, moeder, relatie en ... subject
                    if (actors.ContainsRole("Vader") || actors.ContainsRole("Moeder"))
                    {
                        // init
                        var all = actors.Roles
                            .Where(x => x != "Vader")
                            .Where(x => x != "Moeder")
                            .Where(x => x != "Relatie")
                            .ToArray();
                        if (all.Length == 1)
                        {
                            subject = all.Single();
                        }
                    }
                }

                // event met enkel persoon
                relationships.Create(actors[subject], actors["Moeder"], null, ParentChild.Person2IsBiologicalParentOfPerson1);
                relationships.Create(actors[subject], actors["Vader"], null, ParentChild.Person2IsBiologicalParentOfPerson1);
                relationships.Create(actors[subject], actors["Relatie"], true, null);

                // bruid en bruidegom
                var partnerships = relationships.Create(actors["Bruid"], actors["Bruidegom"], true, null);
                relationships.Create(actors["Vader van de bruid"], actors["Bruid"], null, ParentChild.Person1IsBiologicalParentOfPerson2);
                relationships.Create(actors["Moeder van de bruid"], actors["Bruid"], null, ParentChild.Person1IsBiologicalParentOfPerson2);
                relationships.Create(actors["Vader van de bruidegom"], actors["Bruidegom"], null, ParentChild.Person1IsBiologicalParentOfPerson2);
                relationships.Create(actors["Moeder van de bruidegom"], actors["Bruidegom"], null, ParentChild.Person1IsBiologicalParentOfPerson2);

                // specific dates
                switch (e.EventType.Split(':').Last())
                {
                    case "Doop":
                        actors[subject].ImportEvent("Baptism", e);
                        break;
                    case "Geboorte":
                        actors[subject].ImportEvent("Birth", e);
                        break;
                    case "Huwelijk":
                    case "Trouwen":
                        partnerships.ImportEvent("Marriage", e);
                        break;
                    case "Ondertrouw":
                        partnerships.ImportEvent("Ondertrouw", e);
                        break;
                    case "Echtscheiding":
                        partnerships.ImportEvent("Divorce", e);
                        break;
                    case "Overlijden":
                        actors[subject].ImportEvent("Death", e);
                        break;
                    case "Registratie":
                        if (!string.IsNullOrEmpty(eventPlace))
                        {
                            actors[subject].ForEach(x => x.Residence = x.Residence.Ensure(new Status<string>() { Date = eventDate, Value = eventPlace }));
                        }
                        break;
                }

                // gender
                actors["Bruidegom"].ForEach(x => x.Gender = x.Gender.Ensure(Gender.Male));
                actors["Vader"].ForEach(x => x.Gender = x.Gender.Ensure(Gender.Male));
                actors["Vader van de bruid"].ForEach(x => x.Gender = x.Gender.Ensure(Gender.Male));
                actors["Vader van de bruidegom"].ForEach(x => x.Gender = x.Gender.Ensure(Gender.Male));
                actors["Bruid"].ForEach(x => x.Gender = x.Gender.Ensure(Gender.Female));
                actors["Moeder"].ForEach(x => x.Gender = x.Gender.Ensure(Gender.Female));
                actors["Moeder van de bruid"].ForEach(x => x.Gender = x.Gender.Ensure(Gender.Female));
                actors["Moeder van de bruidegom"].ForEach(x => x.Gender = x.Gender.Ensure(Gender.Female));

                // item of interest
                if (string.IsNullOrWhiteSpace(itemOfInterest))
                {
                    // init
                    PersonInfo[] interests = null;

                    // marriage or single person?
                    if (partnerships.Length > 0)
                    {
                        // marriage
                        interests = new PersonInfo[] { actors["Bruidegom"].First(), actors["Bruid"].First() };
                    }
                    else
                    {
                        // single person
                        interests = actors[subject];
                    }

                    // create names
                    var names = interests
                        .Select(x => string.Format("{0} {1}", x.GivenNames.FirstOrDefault(), x.FamilyName.FirstOrDefault()).Trim())
                        .ToArray();

                    // length?
                    switch (names.Length)
                    {
                        case 0:
                        case 1:
                            itemOfInterest = names.FirstOrDefault();
                            break;
                        case 2:
                            itemOfInterest = string.Join(" en ", names);
                            break;
                        default:
                            itemOfInterest = string.Join(" en ", new string[]
                            {
                                string.Join(", ", names.Take(names.Length - 1)),
                                names.Last()
                            });
                            break;
                    }
                }
            }

            // done
            return persons
                .Select(x => x.Person)
                .Cast<Info>()
                .Concat(relationships.ToArray())
                .ToArray();
        }
        private IEnumerable<string> GetSubject(string eventType)
        {
            switch (eventType.Split(':').Last())
            {
                case "Doop":
                    yield return "Dopeling";
                    yield return "Kind";
                    break;
                case "Geboorte":
                    yield return "Kind";
                    break;
                case "Overlijden":
                    yield return "Overledene";
                    break;
                case "Registratie":
                    yield return "Geregistreerde";
                    break;
            }
        }

        private class RoleDictionary
        {
            private readonly Dictionary<string, PersonInfo[]> _Items;

            public RoleDictionary(Dictionary<string, PersonInfo[]> dictionary)
            {
                // init
                this._Items = dictionary;
            }

            public int Count
            {
                get
                {
                    return this._Items.Count;
                }
            }
            public bool ContainsRole(string role)
            {
                return this._Items.ContainsKey(role);
            }
            public string[] Roles
            {
                get
                {
                    return this._Items
                        .Select(x => x.Key)
                        .ToArray();
                }
            }
            public PersonInfo[] this[string role]
            {
                get
                {
                    return (!string.IsNullOrEmpty(role) && this._Items.ContainsKey(role) ? this._Items[role] : new PersonInfo[] { });
                }
            }
        }
    }
}