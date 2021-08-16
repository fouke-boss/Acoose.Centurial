using Acoose.Genealogy.Extensibility.Data;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acoose.Centurial.Package.nl
{
    partial class WieWasWie
    {
        private class Event
        {
            public Event(HtmlNode node)
            {
                // process description lists
                node.GetDescriptionLists((dt, dd) =>
                {
                    // init
                    var dataDictionary = dt.GetAttributeValue("data-dictionary", null);
                    switch (dataDictionary)
                    {
                        case "SourceDetail.Event":
                            this.Type = Utility.TryParseEventType(dd.GetInnerText());
                            break;
                        case "SourceDetail.EventDate":
                            this.Date = Date.TryParse(dd.GetInnerText());
                            break;
                        case "SourceDetail.EventPlace":
                            this.Place = dd.GetInnerText();
                            break;
                    }
                });
            }
            private static void ProcessDescriptionLists(HtmlNode node, Action<HtmlNode, HtmlNode> processor)
            {
                // find description lists
                foreach (var dl in node.DescendantsAndSelf("dl"))
                {
                    // init
                    var index = 0;
                    var nodes = dl.ChildNodes
                        .Where(x => x.NodeType == HtmlNodeType.Element)
                        .Where(x => x.Name == "dt" || x.Name == "dd")
                        .ToArray();

                    // loop
                    while (index < nodes.Length)
                    {
                        // find <dt>
                        while (index < nodes.Length && nodes[index].Name != "dt")
                        {
                            index++;
                        }

                        // copy
                        var dt = nodes[index];
                        index++;

                        // find <dd>
                        var dds = new List<HtmlNode>();
                        while (index < nodes.Length && nodes[index].Name == "dd")
                        {
                            // add
                            dds.Add(nodes[index]);

                            // increment
                            index++;
                        }

                        // execute
                        if (dt != null)
                        {
                            processor(dt, dds.FirstOrDefault());
                        }
                    }
                }
            }

            public EventType Type
            {
                get; private set;
            }
            public Date Date
            {
                get; private set;
            }
            public string Place
            {
                get; private set;
            }

            public Person[] Persons
            {
                get; set;
            }

            public IEnumerable<Info> Generate()
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
                        return p.ToInfo();
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
                switch (this.Type)
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
                match.ImportEvent(this.Type.ToString(), this.Date, this.Place);
            }
            private void ImportEventForPartnership(IEnumerable<Info> info)
            {
                // init
                var match = info
                    .OfType<RelationshipInfo>()
                    .Single(r => r.IsPartnership.NullCoalesce().Any(x => x));

                // add
                match.ImportEvent(this.Type.ToString(), this.Date, this.Place);
            }
        }
    }
}