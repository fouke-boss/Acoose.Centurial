using Acoose.Genealogy.Extensibility.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Acoose.Centurial.Package.nl.A2A
{
    public class Event
    {
        [XmlAttribute("eid")]
        public string Id
        {
            get;
            set;
        }
        [XmlElement]
        public string EventType
        {
            get;
            set;
        }
        [XmlElement]
        public TransDate EventDate
        {
            get;
            set;
        }
        [XmlElement]
        public DetailPlace EventPlace
        {
            get;
            set;
        }

        internal Info[] GetInfo(Akte akte, long idFactory)
        {
            // persons
            var persons = akte.RelationsEP.NullCoalesce()
                .Select(r => new
                {
                    Role = r.RelationType,
                    Person = akte.Persons.NullCoalesce().SingleOrDefault(x => x.Id == r.PersonKeyRef)
                })
                .ToArray();




            return null;
        }
    }
}