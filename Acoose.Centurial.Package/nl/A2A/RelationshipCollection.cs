using Acoose.Genealogy.Extensibility.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acoose.Centurial.Package.nl.A2A
{
    internal class RelationshipCollection
    {
        private List<RelationshipInfo> _Items = new List<RelationshipInfo>();

        public RelationshipInfo[] Create(PersonInfo[] role1, PersonInfo[] role2, bool? isPartnership, ParentChildDirection? isParentChild)
        {
            return role1
                .SelectMany(p1 => role2.Select(p2 => this.Create(p1, p2, isPartnership, isParentChild)))
                .ToArray();
        }
        public RelationshipInfo Create(PersonInfo person1, PersonInfo person2, bool? isPartnership, ParentChildDirection? isParentChild)
        {
            // init
            var id = new string[] { person1.Id, person2.Id }
                .OrderBy(x => x)
                .ToArray();

            // match
            var match = this._Items
                .SingleOrDefault(x => x.Person1Id == id[0] && x.Person2Id == id[1]);
            if (match == null)
            {
                // create
                match = new RelationshipInfo()
                {
                    Person1Id = id[0],
                    Person2Id = id[1],
                };

                // add
                this._Items.Add(match);
            }

            // partnership
            if (isPartnership.HasValue)
            {
                // init
                match.IsPartnership = match.IsPartnership.Ensure(isPartnership.Value);
            }

            // parent/child
            if (isParentChild.HasValue)
            {
                // init
                var isReversed = (id[0] != person1.Id);
                var value = isParentChild.Value;
                if (isReversed)
                {
                    switch (value)
                    {
                        case ParentChildDirection.Person1IsParentOfPerson2:
                            value = ParentChildDirection.Person2IsParentOfPerson1;
                            break;
                        case ParentChildDirection.Person2IsParentOfPerson1:
                            value = ParentChildDirection.Person1IsParentOfPerson2;
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                }

                // done
                match.IsParentChild = match.IsParentChild.Ensure(value);
            }

            // done
            return match;
        }
        public RelationshipInfo Create(PersonInfo person1, PersonInfo person2, string relationType, Date eventDate)
        {
            // gender
            switch (relationType)
            {
                case "Echtgenoot":
                case "Weduwnaar":
                case "Zoon":
                    person2.Gender = person2.Gender.Ensure(Gender.Male);
                    break;
                case "Echtgenote":
                case "Weduwe":
                case "Dochter":
                    person2.Gender = person2.Gender.Ensure(Gender.Female);
                    break;
            }

            // relationship
            switch (relationType)
            {
                case "Relatie":
                case "Partner":
                case "Echtgenoot":
                case "Echtgenote":
                case "Weduwe":
                case "Weduwnaar":
                case "Gescheidene":
                case "Vorige partner":
                    // done
                    var result = this.Create(person1, person2, true, null);

                    // deceased
                    if (relationType == "Weduwe" || relationType == "Weduwnaar")
                    {
                        person1.VitalStatus = person1.VitalStatus.Ensure(new Status<VitalStatus>() { Date = eventDate, Value = VitalStatus.Deceased });
                    }

                    // done
                    return result;
                case "Kind":
                case "Dochter":
                case "Zoon":
                    return this.Create(person1, person2, null, ParentChildDirection.Person1IsParentOfPerson2);
                default:
                    return null;
            }
        }

        public RelationshipInfo[] ToArray()
        {
            return this._Items.ToArray();
        }
    }
}