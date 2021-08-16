using Acoose.Genealogy.Extensibility.Data;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acoose.Centurial.Package
{
    public static class Utility
    {
        public static IEnumerable<T> NullCoalesce<T>(this IEnumerable<T> values)
        {
            return values ?? Enumerable.Empty<T>();
        }
        public static string[] ToArrayIfAny(this string value)
        {
            return (string.IsNullOrWhiteSpace(value) ? new string[] { } : new string[] { value });
        }
        public static T[] ToArrayIfAny<T>(this T value)
            where T : class
        {
            return (value == null ? new T[] { } : new T[] { value });
        }
        public static T[] ToArrayIfAny<T>(this T? value)
            where T : struct
        {
            return (value == null ? null : new T[] { value.Value });
        }
        public static void ImportEvent<T>(this T info, string eventType, Date date, string place)
            where T : InfoWithEvents
        {
            // init
            var dates = date.ToArrayIfAny();
            var places = place.ToArrayIfAny();

            // any?
            if (dates.Any() || places.Any())
            {
                // init
                var result = default(EventInfo);

                // recurrence
                switch (eventType)
                {
                    case "Birth":
                    case "Death":
                    case "Cremation":
                        result = info.Events.NullCoalesce()
                            .Where(e => e.Type == eventType)
                            .SingleOrDefault();
                        break;
                    default:
                        result = info.Events.NullCoalesce()
                            .Where(e => e.Type == eventType)
                            .Where(e => (dates.Any() ? e.Date.Any(x => dates.Contains(x)) : true))
                            .Where(e => (places.Any() ? e.Place.Any(x => places.Contains(x)) : true))
                            .SingleOrDefault();
                        break;
                }

                // create?
                if (result == null)
                {
                    // create
                    result = new EventInfo()
                    {
                        Type = eventType,
                        Date = dates,
                        Place = places
                    };

                    // add
                    info.Events = info.Events.NullCoalesce()
                        .Append(result)
                        .ToArray();
                }

                // add
                result.Date.Ensure(dates);
                result.Place.Ensure(places);
            }
        }
        internal static void ImportEvent<T>(this T[] info, string eventType, nl.A2A.Event @event)
            where T : InfoWithEvents
        {
            // init
            foreach (var data in info)
            {
                // import
                data.ImportEvent(eventType, @event.EventDate?.ToData(), @event.EventPlace?.ToString());
            }
        }
        public static T[] Ensure<T>(this IEnumerable<T> items, IEnumerable<T> values)
        {
            // init
            var results = items.NullCoalesce()
                .ToList();

            // add
            foreach (var value in values)
            {
                if (!results.Contains(value))
                {
                    results.Add(value);
                }
            }

            // done
            return results.ToArray();
        }
        public static T[] Ensure<T>(this IEnumerable<T> items, T value)
        {
            return items.Ensure(new T[] { value });
        }

        public static string GetInnerText(this HtmlNode node)
        {
            return node.InnerText.Trim(' ', '\n', '\r', '\t');
        }
        public static void GetDescriptionLists(this HtmlNode node, Action<HtmlNode, HtmlNode> processor)
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
        public static EventRole TryParseEventRole(string role)
        {
            switch (role?.ToLower())
            {
                case "bride":
                case "bruid":
                    return EventRole.Bride;
                case "groom":
                case "bruidegom":
                    return EventRole.Groom;
                case "moeder van de bruidegom":
                    return EventRole.MotherOfGroom;
                case "vader van de bruidegom":
                    return EventRole.FatherOfGroom;
                case "moeder van de bruid":
                    return EventRole.MotherOfBride;
                case "vader van de bruid":
                    return EventRole.FatherOfBride;
                case "kind":
                case "child":
                    return EventRole.Child;
                case "vader":
                case "father":
                    return EventRole.Father;
                case "moeder":
                case "mother":
                    return EventRole.Mother;
                case "overledene":
                case "deceased":
                    return EventRole.Deceased;
                default:
                    throw new NotSupportedException();
            }
        }
        public static EventType TryParseEventType(string @event)
        {
            switch (@event?.ToLower())
            {
                case "huwelijk":
                    return EventType.CivilMarriage;
                case "geboorte":
                    return EventType.Birth;
                case "doop":
                    return EventType.Baptism;
                case "overlijden":
                    return EventType.Death;
                default:
                    throw new NotSupportedException();
            }
        }
        public static Gender? TryParseGender(string gender)
        {
            switch (gender?.ToLower())
            {
                case "m":
                case "man":
                case "male":
                    return Gender.Male;
                case "f":
                case "v":
                case "vrouw":
                case "female":
                    return Gender.Female;
                default:
                    return null;
            }
        }
    }
}