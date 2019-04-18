using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Acoose.Centurial.Package.nl.A2A
{
    public class DetailPlace
    {
        [XmlElement]
        public string Country
        {
            get;
            set;
        }
        [XmlElement]
        public string Province
        {
            get;
            set;
        }
        [XmlElement]
        public string State
        {
            get;
            set;
        }
        [XmlElement]
        public string County
        {
            get;
            set;
        }
        [XmlElement]
        public string Place
        {
            get;
            set;
        }
        [XmlElement]
        public string Municipality
        {
            get;
            set;
        }
        [XmlElement]
        public string PartMunicipality
        {
            get;
            set;
        }
        [XmlElement]
        public string Block
        {
            get;
            set;
        }
        [XmlElement]
        public string Quarter
        {
            get;
            set;
        }
        [XmlElement]
        public string Street
        {
            get;
            set;
        }
        [XmlElement]
        public string DescriptiveLocationIndicator
        {
            get;
            set;
        }
        [XmlElement]
        public string HouseName
        {
            get;
            set;
        }
        [XmlElement]
        public string HouseNumber
        {
            get;
            set;
        }
        [XmlElement]
        public string HouseNumberAddition
        {
            get;
            set;
        }

        [XmlElement]
        public string Longitude
        {
            get;
            set;
        }
        [XmlElement]
        public string Latitude
        {
            get;
            set;
        }

        public override string ToString()
        {
            // init
            var address = string.Format("{0} {1}{2}", this.Street, this.HouseNumber, this.HouseNumberAddition);
            var parts = new string[]
            {
                this.Country,
                this.Province,
                this.State,
                this.State,
                this.Place,
                this.Municipality,
                this.PartMunicipality,
                this.Block,
                this.Quarter,
                this.HouseName,
                address
            }
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToArray();

            // done
            return string.Join(", ", parts);
        }
    }
}