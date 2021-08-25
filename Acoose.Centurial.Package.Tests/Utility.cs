using Acoose.Genealogy.Extensibility.Data;
using Acoose.Genealogy.Extensibility.Data.References;
using Acoose.Genealogy.Extensibility.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acoose.Centurial.Package.Tests
{
    public static class Utility
    {
        public static T FindProvenance<T>(this ScraperTest test, int index)
            where T : Repository
        {
            // init
            var match = test.Source.Provenance.NullCoalesce()
                .Skip(index)
                .FirstOrDefault();

            // fail?
            if (match is T result)
            {
                return result;
            }
            else
            {
                Assert.Fail($"Not found: '{typeof(T).Name}'.");
                throw new NotSupportedException();
            }
        }
        public static T FindChild<T>(this IContainer container)
            where T : Representation
        {
            // init
            var items = container.Items
                .NullCoalesce()
                .ToList();

            // fail?
            if (items.Count == 1 && items.Single() is T result)
            {
                return result;
            }
            else
            {
                throw new NotSupportedException();
            }
        }
        public static T FindChild<T>(this IWrapper wrapper)
            where T : Representation
        {
            // init
            var item = wrapper.Item;

            // fail?
            if (item is T result)
            {
                return result;
            }
            else
            {
                throw new NotSupportedException();
            }
        }
        public static PersonInfo FindPerson(this ScraperTest test, string name)
        {
            // init
            var result = test.Source.Info
                .OfType<PersonInfo>()
                .SingleOrDefault(x => string.Compare(x.Name(), name, true) == 0);

            // fail?
            if (result == null)
            {
                Assert.Fail($"Person '{name}' not present.");
            }

            // done
            return result;
        }
        public static RelationshipInfo FindRelationship(this ScraperTest test, PersonInfo person1, PersonInfo person2)
        {
            // init
            var result = test.Source.Info
                .OfType<RelationshipInfo>()
                .SingleOrDefault(x => x.Person1Id == person1.Id && x.Person2Id == person2.Id);

            // fail?
            if (result == null)
            {
                Assert.Fail($"Relationship between '{person1.Name()}' and '{person2.Name()}' not present.");
            }

            // done
            return result;
        }

        public static string Name(this PersonInfo person)
        {
            // init
            var familyName = string.Join("/", person.FamilyName.NullCoalesce());
            var givenNames = string.Join("/", person.GivenNames.NullCoalesce());

            // done
            return string.Join(" ", new string[] { givenNames, familyName }.Where(x => !string.IsNullOrWhiteSpace(x)));
        }

        public static T AssertDate<T>(this T info, string eventType, string date)
            where T : InfoWithEvents
        {
            // init
            var match = info.Events
                .SingleOrDefault(x => x.Type == eventType);
            if (match == null)
            {
                Assert.Fail($"No event of type '{eventType}' present.");
            }

            // validate date
            var value = Date.TryParse(date);
            if (value == null)
            {
                throw new NotSupportedException(date);
            }

            // check
            if (!match.Date.Contains(value))
            {
                Assert.Fail($"Date '{date}' was for event '{eventType}' was not found.");
            }

            // done
            return info;
        }
        public static T AssertPlace<T>(this T info, string eventType, string place)
            where T : InfoWithEvents
        {
            // init
            var match = info.Events
                .SingleOrDefault(x => x.Type == eventType);
            if (match == null)
            {
                Assert.Fail($"No event of type '{eventType}' present.");
            }

            // check
            if (!match.Place.Contains(place))
            {
                Assert.Fail($"Place '{place}' was for event '{eventType}' was not found.");
            }

            // done
            return info;
        }
        public static PersonInfo AssertGender(this PersonInfo info, Gender gender)
        {
            // check
            if (!info.Gender.Contains(gender))
            {
                Assert.Fail($"Gender '{gender}' for person '{info.Name()}' was not found.");
            }

            // done
            return info;
        }
        public static RelationshipInfo AssertParentChild(this RelationshipInfo info, ParentChild parentChild)
        {
            // check
            if (!info.IsParentChild.NullCoalesce().Contains(parentChild))
            {
                Assert.Fail($"ParentChild '{parentChild}' not found.");
            }

            // done
            return info;
        }

        public static T AssertCondition<T>(this T representation, Func<T, bool> condition)
            where T : Representation
        {
            // init
            var result = false;

            // condition
            try
            {
                result = condition(representation);
            }
            catch 
            {
                result = false;
            }

            // condition
            if (!result)
            {
                Assert.Fail(representation.GetType().Name);
            }

            // done
            return representation;
        }
    }
}