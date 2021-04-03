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
                // yes, create
                var result = new EventInfo()
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
        public static T[] Ensure<T>(this IEnumerable<T> items, T value)
        {
            // init
            var results = items.NullCoalesce()
                .ToList();

            // add
            if (!results.Contains(value))
            {
                results.Add(value);
            }

            // done
            return results.ToArray();
        }
    }
}