using Acoose.Genealogy.Extensibility.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acoose.Centurial.Package.nl.A2A
{
    internal static class Utility
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
        public static void Import<T>(this T[] info, Event @event, Func<T, InfoEvent> getter, Action<T, InfoEvent> setter)
            where T : Info
        {
            // init
            foreach (var data in info)
            {
                // init
                var result = getter(data) ?? new InfoEvent();

                // date
                if (@event.EventDate?.ToData() is Date date)
                {
                    // init
                    if (!result.Date.NullCoalesce().Contains(date))
                    {
                        result.Date = result.Date.Ensure(date);
                    }
                }

                // place
                if (@event.EventPlace?.ToString() is string place && !string.IsNullOrWhiteSpace(place))
                {
                    // init
                    if (!result.Place.NullCoalesce().Contains(place))
                    {
                        result.Place = result.Place.Ensure(place);
                    }
                }

                // done
                setter(data, result);
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