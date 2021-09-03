using Acoose.Genealogy.Extensibility.Data;
using Acoose.Genealogy.Extensibility.Data.References;
using Acoose.Genealogy.Extensibility.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Acoose.Centurial.Package.Tests
{
    public static class Utility
    {
        public static ScraperTest ExecuteTest<T>(string url, [CallerMemberName] string caseName = null)
            where T : Scraper, new()
        {
            // init
            var urlCountry = typeof(T).Namespace.Split('.').Last();
            var resourceName = $"Acoose.Centurial.Package.Tests.{urlCountry}.{typeof(T).Name}.{caseName}.html";

            // execute
            var result = ScraperTest.ExecuteFromEmbeddedResource<T>(url, resourceName);

            // first layer of provenance should be a website (it's a scraper!), check the url
            var onlineItem = result
                .FindProvenance<Website>(0)
                .AssertChild<OnlineItem>();

            // online collections
            while (onlineItem.Item is OnlineCollection c)
            {
                onlineItem = c.AssertChild<OnlineItem>();
            }

            // check
            onlineItem
                .AssertCondition(x => Equals(x.Url, url))
                .AssertCondition(x => Equals(x.Accessed, Date.Today));

            // done
            return result;
        }

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

        public static T AssertChild<T>(this Representation parent)
            where T : Representation
        {
            // init
            var result = default(T[]);

            // find?
            if (parent is IContainer container)
            {
                result = container.Items
                    .NullCoalesce()
                    .OfType<T>()
                    .ToArray();
            }
            else if (parent is IWrapper wrapper && wrapper.Item is T item)
            {
                result = new T[] { item };
            }

            // fail?
            if (result.Length == 1)
            {
                return result.Single();
            }
            else
            {
                throw new NotSupportedException();
            }
        }
        //public static T AssertChild<T>(this IContainer container)
        //    where T : Representation
        //{
        //    // init
        //    var items = container.Items
        //        .NullCoalesce()
        //        .ToList();

        //    // fail?
        //    if (items.Count == 1 && items.Single() is T result)
        //    {
        //        return result;
        //    }
        //    else
        //    {
        //        throw new NotSupportedException();
        //    }
        //}
        //public static T AssertChild<T>(this IWrapper wrapper)
        //    where T : Representation
        //{
        //    // init
        //    var item = wrapper.Item;

        //    // fail?
        //    if (item is T result)
        //    {
        //        return result;
        //    }
        //    else
        //    {
        //        throw new NotSupportedException();
        //    }
        //}

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
                .Where(x =>
                {
                    // init
                    var ids = new string[] { x.Person1Id, x.Person2Id };

                    // done
                    return (ids.Contains(person1.Id) && ids.Contains(person2.Id));
                })
                .SingleOrDefault();

            // fail?
            if (result == null)
            {
                Assert.Fail($"Relationship between '{person1.Name()}' and '{person2.Name()}' not present.");
            }

            // done
            return result;
        }
        public static RelationshipInfo FindParentChild(this ScraperTest test, PersonInfo parent, PersonInfo child)
        {
            // init
            var result = test.FindRelationship(parent, child);

            // check
            var required = (result.Person1Id == parent.Id ? ParentChild.Person1IsBiologicalParentOfPerson2 : ParentChild.Person2IsBiologicalParentOfPerson1);

            // validate
            if (!result.IsParentChild.NullCoalesce().Contains(required))
            {
                Assert.Fail();
            }

            // done
            return result;
        }
        public static RelationshipInfo FindPartnership(this ScraperTest test, PersonInfo partner1, PersonInfo partner2)
        {
            // init
            var result = test.FindRelationship(partner1, partner2);

            // check
            if (!result.IsPartnership.NullCoalesce().Any())
            {
                Assert.Fail();
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

        public static T AssertEvent<T>(this T info, string eventType, string date, string place)
            where T : InfoWithEvents
        {
            // init
            var match = info.Events
                .SingleOrDefault(x => x.Type == eventType);
            if (match == null)
            {
                Assert.Fail($"No event of type '{eventType}' present.");
            }

            // date
            if (Date.TryParse(date) is Date value && !match.Date.Contains(value))
            {
                Assert.Fail($"Date '{date}' was for event '{eventType}' was not found.");
            }

            // place
            if (place != null && !match.Place.Contains(place))
            {
                Assert.Fail($"Place '{place}' was for event '{eventType}' was not found.");
            }

            // done
            return info;
        }
        public static T AssertBirth<T>(this T info, string date, string place)
            where T : InfoWithEvents
        {
            return info
                .AssertEvent<T>("Birth", date, place);
        }
        public static T AssertDeath<T>(this T info, string date, string place)
            where T : InfoWithEvents
        {
            return info
                .AssertEvent<T>("Death", date, place);
        }
        public static T AssertBaptism<T>(this T info, string date, string place)
            where T : InfoWithEvents
        {
            return info
                .AssertEvent<T>("Baptism", date, place);
        }

        public static T AssertDate<T>(this T info, string eventType, string date)
            where T : InfoWithEvents
        {
            // init
            var match = info.Events
                .NullCoalesce()
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
        public static PersonInfo AssertAge(this PersonInfo info, int age, string date)
        {
            return info.AssertAge(new Age() { Years = age }, date);
        }
        public static PersonInfo AssertAge(this PersonInfo info, Age age, string date)
        {
            // init
            var value = info.Age.NullCoalesce().SingleOrDefault();

            // check
            var valid = (value != null && Equals(value.Date, Date.TryParse(date)) && (age == null) == (value.Value == null));
            if (valid && age != null)
            {
                // check age
                valid = (value.Value is Age a &&
                    Equals(a.Years, age.Years) &&
                    Equals(a.Months, age.Months) &&
                    Equals(a.Weeks, age.Weeks) &&
                    Equals(a.Days, age.Days));
            }

            // done
            if (!valid)
            {
                Assert.Fail($"Age for person '{info.Name()}' was not valid.");
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
        public static PublicArchive AssertPublicArchive(this ScraperTest test, int index, string name, string place)
        {
            // init
            return test
                .FindProvenance<PublicArchive>(index)
                .AssertCondition(x => Equals(x.Name, name))
                .AssertCondition(x => Equals(x.Place, place));
        }
        public static OnlineItem AssertWebsite(this ScraperTest test, int index, string title, string url, bool isVirtualArchive)
        {
            // init
            return test
                .FindProvenance<Website>(index)
                .AssertCondition(x => Equals(x.Title, title))
                //.AssertCondition(x => Equals(x.Url, url))
                .AssertCondition(x => (x.IsVirtualArchive == isVirtualArchive))
                .AssertChild<OnlineItem>();
        }

        public static ArchivedItem AssertArchivedItem(this Representation container, string identifier)
        {
            // init
            return container
                .AssertChild<ArchivedItem>()
                .AssertCondition(x => Equals(x.Identifier, identifier));
        }

        public static Collection AssertCollection(this Representation wrapper, string name)
        {
            // init
            return wrapper
                .AssertChild<Collection>()
                .AssertCondition(x => Equals(x.Name, name));
        }
        public static OnlineItem AssertOnlineCollection(this Representation wrapper, string title)
        {
            // init
            return wrapper
                .AssertChild<OnlineCollection>()
                .AssertCondition(x => Equals(x.Title, title))
                .AssertChild<OnlineItem>();
        }

        public static VitalRecord AssertVitalRecord(this Representation wrapper, string jurisdiction, string title)
        {
            // init
            return wrapper
                .AssertChild<VitalRecord>()
                .AssertCondition(x => Equals(x.Jurisdiction, jurisdiction))
                .AssertCondition(x => Equals(x.Title?.Value, title));
        }
        public static ChurchRecord AssertChurchRecord(this Representation wrapper, string church, string place)
        {
            // init
            return wrapper
                .AssertChild<ChurchRecord>()
                .AssertCondition(x => Equals(x.Church, church))
                .AssertCondition(x => Equals(x.Place, place));
        }
        public static CemeteryRecord AssertCemeteryRecord(this Representation wrapper, string cemetery, string place)
        {
            // init
            return wrapper
                .AssertChild<CemeteryRecord>()
                .AssertCondition(x => Equals(x.Cemetery, cemetery))
                .AssertCondition(x => Equals(x.Place, place));
        }
        public static DatabaseEntry AssertDatabaseEntry(this Representation wrapper, string entryFor)
        {
            // init
            return wrapper
                .AssertChild<DatabaseEntry>()
                .AssertCondition(x => Equals(x.EntryFor, entryFor));
        }
        public static Unspecified AssertUnspecified(this Representation parent, string creditLine)
        {
            // init
            return parent
                .AssertChild<Unspecified>()
                .AssertCondition(x => Equals(x.CreditLine, creditLine));
        }

        public static RecordScriptFormat AssertRecordScriptFormat(this Representation container, string label, string page, string number, string itemOfInterest, string date)
        {
            // init
            return container
                .AssertChild<RecordScriptFormat>()
                .AssertCondition(x => Equals(x.Label, label))
                .AssertCondition(x => Equals(x.Page, page))
                .AssertCondition(x => Equals(x.Number, number))
                .AssertCondition(x => Equals(x.ItemOfInterest, itemOfInterest))
                .AssertCondition(x => Equals(x.Date, Date.TryParse(date)));
        }
    }
}