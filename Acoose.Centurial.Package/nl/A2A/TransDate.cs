using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Acoose.Centurial.Package.nl.A2A
{
    public class TransDate
    {
        [XmlAttribute]
        public string Calendar
        {
            get;
            set;
        }
        [XmlAttribute]
        public DateTime IndexDateTime
        {
            get;
            set;
        }

        [XmlElement]
        public string LiteralDate
        {
            get;
            set;
        }
        [XmlElement]
        public string Year
        {
            get;
            set;
        }
        [XmlElement]
        public string Month
        {
            get;
            set;
        }
        [XmlElement]
        public string Day
        {
            get;
            set;
        }
        [XmlElement]
        public string Hour
        {
            get;
            set;
        }
        [XmlElement]
        public string Minute
        {
            get;
            set;
        }

        public Acoose.Genealogy.Extensibility.Data.Date ToData()
        {
            // init
            var parts = new string[] { this.Day, this.Month, this.Year }
                .Select(x => int.TryParse(x, out int v) ? v : (int?)null)
                .SkipWhile(x => !x.HasValue)
                .Reverse()
                .ToArray();

            // done
            if (parts.Length > 0)
            {
                return Acoose.Genealogy.Extensibility.Data.Date.Exact(Acoose.Genealogy.Extensibility.Data.Calendar.Gregorian, parts);
            }
            else
            {
                // none
                return null;
            }
        }
    }
}