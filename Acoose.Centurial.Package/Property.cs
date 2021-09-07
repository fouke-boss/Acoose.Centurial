using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acoose.Centurial.Package
{
    public class Property<T>
    {
        internal Property(string key, T value)
        {
            // init
            this.Key = key;
            this.Value = value;
        }

        public string Key
        {
            get;
        }
        public T Value
        {
            get;
        }

        public bool IsMatch(string key)
        {
            return string.Compare(key, this.Key, true) == 0;
        }

        public override string ToString()
        {
            return $"{this.Key}={this.Value}";
        }
    }
}