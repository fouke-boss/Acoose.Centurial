using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Acoose.Centurial.Package.nl.A2A
{
    public class SourceReference
    {
        [XmlElement]
        public string Place
        {
            get;
            set;
        }
        [XmlElement]
        public string InstitutionName
        {
            get;
            set;
        }
        [XmlElement]
        public string Archive
        {
            get;
            set;
        }
        [XmlElement]
        public string Collection
        {
            get;
            set;
        }
        [XmlElement]
        public string Section
        {
            get;
            set;
        }
        [XmlElement]
        public string Book
        {
            get;
            set;
        }
        [XmlElement]
        public string Folio
        {
            get;
            set;
        }
        [XmlElement]
        public string Rolodeck
        {
            get;
            set;
        }
        [XmlElement]
        public string Stack
        {
            get;
            set;
        }
        [XmlElement]
        public string RegistryNumber
        {
            get;
            set;
        }
        [XmlElement]
        public string DocumentNumber
        {
            get;
            set;
        }
    }
}