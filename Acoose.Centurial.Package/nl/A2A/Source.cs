using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Acoose.Centurial.Package.nl.A2A
{
    public class Source
    {
        [XmlElement]
        public TransDate SourceDate
        {
            get;
            set;
        }
        [XmlElement]
        public DetailPlace SourcePlace
        {
            get;
            set;
        }
        [XmlElement]
        public string SourceType
        {
            get;
            set;
        }
        [XmlElement]
        public SourceReference SourceReference
        {
            get;
            set;
        }
    }
}