using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Acoose.Centurial.Package.nl.A2A
{
    public class Scan
    {
        [XmlElement]
        public string OrderSequenceNumber
        {
            get;
            set;
        }
        [XmlElement]
        public string Uri
        {
            get;
            set;
        }
        [XmlElement]
        public string UriViewer
        {
            get;
            set;
        }
        [XmlElement]
        public string UriPreview
        {
            get;
            set;
        }
    }
}