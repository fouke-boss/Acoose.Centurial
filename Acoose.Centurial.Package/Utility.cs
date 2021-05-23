using Acoose.Genealogy.Extensibility.Data;
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
            return (values ?? (new T[] { }));
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
    }
}