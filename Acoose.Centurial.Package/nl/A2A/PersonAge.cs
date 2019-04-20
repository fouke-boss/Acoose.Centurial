using Acoose.Genealogy.Extensibility.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Acoose.Centurial.Package.nl.A2A
{
    public class PersonAge
    {
        private static readonly Regex YEAR_PATTERN = new Regex(@"(\d+) jaa?r", RegexOptions.IgnoreCase);
        private static readonly Regex MONTH_PATTERN = new Regex(@"(\d+) maand", RegexOptions.IgnoreCase);
        private static readonly Regex WEEK_PATTERN = new Regex(@"(\d+) wee?k", RegexOptions.IgnoreCase);
        private static readonly Regex DAY_PATTERN = new Regex(@"(\d+) dag", RegexOptions.IgnoreCase);

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
            Age result = null;

            // parts
            var parts = new int?[]
            {
                this.GetPart(this.PersonAgeYears, YEAR_PATTERN),
                this.GetPart(this.PersonAgeMonths, MONTH_PATTERN),
                this.GetPart(this.PersonAgeWeeks, WEEK_PATTERN),
                this.GetPart(this.PersonAgeDays, DAY_PATTERN),
            };

            // any part present?
            if (parts.Any(x => x.HasValue))
            {
                result = new Age()
                {
                    Years = parts[0],
                    Months = parts[1],
                    Weeks = parts[2],
                    Days = parts[3]
                };
            }

            // literal?
            if (result == null)
            {
                // literal?
                if (int.TryParse(this.PersonAgeLiteral, out int n))
                {
                    result = new Age()
                    {
                        Years = n
                    };
                }
            }

            // done
            return result;
        }
        private int? GetPart(string value, Regex pattern)
        {
            // try to parse
            if (int.TryParse(value, out int n))
            {
                // direct
                return n;
            }

            // match?
            var match = pattern.Match(this.PersonAgeLiteral);
            if (match.Success)
            {
                return int.Parse(match.Groups[1].Value);
            }
            else
            {
                return null;
            }
        }
    }
}