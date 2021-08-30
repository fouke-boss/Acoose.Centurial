using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acoose.Centurial.Package
{
    public class Property
    {
        internal Property(string key, HtmlNode html)
        {
            // init
            this.Key = key;
            this.Html = html;
        }

        public string Key
        {
            get;
        }
        public HtmlNode Html
        {
            get;
        }

        public override string ToString()
        {
            return $"{this.Key}={this.Html.GetInnerText()}";
        }
    }
}