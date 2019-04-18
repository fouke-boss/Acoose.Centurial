using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Acoose.Centurial.Package.nl.A2A
{
    public class PersonReligion
    {
        [XmlElement]
        public string PersonReligionLiteral
        {
            get;
            set;
        }
    }
}
