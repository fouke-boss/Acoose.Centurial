using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acoose.Centurial.Package
{
    public class PropertyBag : IEnumerable<Property>
    {
        private readonly List<Property> _Items;

        /// <summary>
        /// Expects table, tbody, th and td tags.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static PropertyBag LoadFromTable(IEnumerable<HtmlNode> tables, Func<HtmlNode, string> keySelector)
        {
            // init
            var rows = tables
                .Select(table => table.Elements("tbody").SingleOrDefault() ?? table)
                .SelectMany(x => x.Elements("tr"))
                .Select(tr => 
                {
                    // init
                    var result = default(Property);

                    // tags
                    var th = tr.Elements("th").ToArray();
                    var td = tr.Elements("td").ToArray();

                    // type?
                    if (th.Length == 1 && td.Length == 1)
                    {
                        // key = th, value = td
                        result = new Property(keySelector(th.Single()), td.Single());
                    }
                    else if (th.Length == 0 && td.Length == 2)
                    {
                        // key = td[0], value = td[1]
                        result = new Property(keySelector(td.ElementAt(0)), td.ElementAt(1));
                    }

                    // done
                    return result;
                })
                .Where(x => x != null)
                .ToList();

            // done
            return new PropertyBag(rows);
        }

        private PropertyBag(List<Property> properties)
        {
            // init
            this._Items = properties;
        }
        public PropertyBag(IEnumerable<Property> properties)
        {
            // init
            this._Items = properties.ToList();
        }

        public IEnumerator<Property> GetEnumerator()
        {
            return this._Items.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public HtmlNode this[string key]
        {
            get
            {
                // init
                var match = this._Items
                    .Where(x => string.Compare(x.Key, key, true) == 0)
                    .SingleOrDefault();

                // done
                return match?.Html;
            }
        }
    }
}