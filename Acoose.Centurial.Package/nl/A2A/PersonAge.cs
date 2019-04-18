using Acoose.Genealogy.Extensibility.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Acoose.Centurial.Package.nl.A2A
{
    public class PersonAge
    {
        [XmlElement]
        public string PersonAgeLiteral
        {
            get;
            set;
        }
        [XmlElement]
        public string PersonAgeYears
        {
            get;
            set;
        }
        [XmlElement]
        public string PersonAgeMonths
        {
            get;
            set;
        }
        [XmlElement]
        public string PersonAgeWeeks
        {
            get;
            set;
        }
        [XmlElement]
        public string PersonAgeDays
        {
            get;
            set;
        }
        [XmlElement]
        public string PersonAgeHours
        {
            get;
            set;
        }
        [XmlElement]
        public string PersonAgeMinutes
        {
            get;
            set;
        }

        public Age ToData()
        {
            // init
            var parts = new int?[]
            {
                (int.TryParse(this.PersonAgeYears, out int y) ? y : (int?)null),
                (int.TryParse(this.PersonAgeMonths, out int m) ? m : (int?)null),
                (int.TryParse(this.PersonAgeWeeks, out int w) ? w : (int?)null),
                (int.TryParse(this.PersonAgeDays, out int d) ? d : (int?)null)
            };

            // null?
            if (parts.All(x => !x.HasValue))
            {
                return null;
            }

            // done
            return new Age()
            {
                Years = parts[0],
                Months = parts[1],
                Weeks = parts[2],
                Days = parts[3]
            };
        }
    }
}