using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acoose.Centurial.Package
{
    public class PropertyBag<T> : IEnumerable<Property<T>>
    {
        private readonly List<Property<T>> _Items;

        /// <summary>
        /// Expects table, tbody, th and td tags.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static PropertyBag<HtmlNode> LoadFromTable(IEnumerable<HtmlNode> candidates, Func<HtmlNode, string> keySelector)
        {
            // init
            var rows = candidates
                .Select(table => table.Elements("tbody").SingleOrDefault() ?? table)
                .SelectMany(x => x.Elements("tr"))
                .Select(tr => 
                {
                    // init
                    var result = default(Property<HtmlNode>);

                    // tags
                    var th = tr.Elements("th").ToArray();
                    var td = tr.Elements("td").ToArray();

                    // type?
                    if (th.Length == 1 && td.Length == 1)
                    {
                        // key = th, value = td
                        result = new Property<HtmlNode>(keySelector(th.Single()), td.Single());
                    }
                    else if (th.Length == 0 && td.Length == 2)
                    {
                        // key = td[0], value = td[1]
                        result = new Property<HtmlNode>(keySelector(td.ElementAt(0)), td.ElementAt(1));
                    }

                    // done
                    return result;
                })
                .Where(x => x != null)
                .ToList();

            // done
            return new PropertyBag<HtmlNode>(rows);
        }
        public static PropertyBag<T> Load<TSource>(IEnumerable<TSource> candidates, Func<TSource, string> keySelector, Func<TSource, T> valueSelector)
        {
            // init
            var properties = candidates
                .Select(c => new Property<T>(keySelector(c), valueSelector(c)));

            // done
            return new PropertyBag<T>(properties);
        }

        private PropertyBag(List<Property<T>> properties)
        {
            // init
            this._Items = properties;
        }
        public PropertyBag(IEnumerable<Property<T>> properties)
        {
            // init
            this._Items = properties.ToList();
        }

        public IEnumerator<Property<T>> GetEnumerator()
        {
            return this._Items.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public T this[params string[] keys]
        {
            get
            {
                // done
                return this.Exact(keys)
                    .FirstOrDefault();
            }
        }
        public T FirstOrDefault(params string[] keys)
        {
            // init
            return keys
                .Select(x => this[x])
                .Where(x => x != null)
                .FirstOrDefault();
        }
        public IEnumerable<T> Exact(string[] keys)
        {
            return keys
                .SelectMany(key => this._Items.Where(x => x.IsMatch(key)))
                .Select(x => x.Value);
        }
        public IEnumerable<T> Contains(string[] keys)
        {
            return keys
                .SelectMany(key => this._Items.Where(i => i.Key.Split(new char[] { '-', ' ' }).Any(x => string.Compare(key, x, true) == 0)))
                .Select(x => x.Value);
        }
        public bool ContainsKey(string[] keys)
        {
            return this._Items
                .Any(i => keys.Any(k => i.IsMatch(k)));
        }
    }
}