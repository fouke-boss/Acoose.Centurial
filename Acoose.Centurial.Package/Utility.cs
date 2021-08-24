﻿using Acoose.Genealogy.Extensibility.Data;
using Acoose.Genealogy.Extensibility.Web;
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
        private static char[] TRIM_CHARS = " \n\r\t:,;.()[]{}".ToArray();

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
            // null?
            if (node == null)
            {
                return null;
            }

            // init
            var texts = node
                .Descendants()
                .OfType<HtmlTextNode>()
                .Select(x => x.InnerText);

            // done
            return string.Join("", texts).TrimAll();
        }
        public static string TrimAll(this string text)
        {
            return text?.Trim(TRIM_CHARS);
        }
        public static void GetDescriptionLists(this HtmlNode node, Action<HtmlNode, HtmlNode> processor)
        {
            // find description lists
            foreach (var dl in node.DescendantsAndSelf().Where(x => x.NodeType == HtmlNodeType.Element && x.Name == "dl"))
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

        public static HtmlNode Body(this Context context)
        {
            return context.Html
                .Element("html")
                .Elements("body")
                .Single();
        }
        public static string Attribute(this HtmlNode node, string attribute)
        {
            return node.GetAttributeValue(attribute, null).NullIfWhitespace();
        }
        public static IEnumerable<HtmlNode> Descendants(this IEnumerable<HtmlNode> nodes)
        {
            return nodes.SelectMany(x => x.Descendants());
        }
        public static IEnumerable<HtmlNode> Descendants(this IEnumerable<HtmlNode> nodes, string name)
        {
            return nodes.SelectMany(x => x.Descendants(name));
        }

        public static IEnumerable<HtmlNode> WithAttribute(this IEnumerable<HtmlNode> nodes, string attribute)
        {
            return nodes.Where(x => x.Attribute(attribute) != null);
        }
        public static IEnumerable<HtmlNode> WithAttribute(this IEnumerable<HtmlNode> nodes, string attribute, string value)
        {
            return nodes.Where(x => x.Attribute(attribute) == value);
        }
        public static IEnumerable<HtmlNode> WithId(this IEnumerable<HtmlNode> nodes, string id)
        {
            return nodes.Where(x => x.Attribute("id") == id);
        }
        public static IEnumerable<HtmlNode> WithClass(this IEnumerable<HtmlNode> nodes, string @class)
        {
            return nodes
                .Where(n =>
                {
                    // init
                    var classes = (n.Attribute("class") ?? "").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    // done
                    return classes.Contains(@class);
                });
        }
        public static IEnumerable<HtmlNode> WithAnyClass(this IEnumerable<HtmlNode> nodes, params string[] @class)
        {
            return nodes
                .Where(n =>
                {
                    // init
                    var classes = (n.Attribute("class") ?? "").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    // done
                    return classes.Any(c => @class.Contains(c));
                });
        }

        public static EventRole? TryParseEventRole(string role)
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
                case "mothergroom":
                    return EventRole.MotherOfGroom;
                case "vader van de bruidegom":
                case "fathergroom":
                    return EventRole.FatherOfGroom;
                case "moeder van de bruid":
                case "motherbride":
                    return EventRole.MotherOfBride;
                case "vader van de bruid":
                case "fatherbride":
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
                    return null;
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
        public static Age TryParseAge(string age)
        {
            // init
            var result = default(Age);

            // null?
            if (!string.IsNullOrWhiteSpace(age))
            {
                // init
                var parts = age.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 1 && int.TryParse(parts[0], out var years1))
                {
                    // 26
                    result = new Age() { Years = years1 };
                }
                else if (parts.Length == 2 && parts[1] == "jaar" && int.TryParse(parts[0], out var years2))
                {
                    // 26 jaar
                    result = new Age() { Years = years2 };
                }
                else if (parts.Length == 2 && parts[1] == "weken" && int.TryParse(parts[0], out var weeks1))
                {
                    // 26 weken
                    result = new Age() { Weeks = weeks1 };
                }
            }

            // done
            return result;
        }
        public static T Get<T>(this Dictionary<string, T> dictionary, string key)
        {
            return (dictionary.TryGetValue(key, out var value) ? value : default(T));
        }

        public static Genealogy.Extensibility.Data.References.GenericTitle ToGenericTitle(this string title, bool isLiteral)
        {
            return (string.IsNullOrEmpty(title) ? null : new Genealogy.Extensibility.Data.References.GenericTitle() { Value = title, Literal = isLiteral });
        }
    }
}