using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Acoose.Centurial.Package.nl.A2A
{
    public enum EventType
    {
        [XmlEnum]
        Begraven,
        [XmlEnum]
        Doop,
        [XmlEnum]
        Echtscheiding,
        [XmlEnum]
        Geboorte,
        [XmlEnum]
        Huwelijk,
        [XmlEnum("Memorie van successie")]
        MemorieVanSuccessie,
        [XmlEnum("Notariële akte")]
        NotariëleAkte,
        [XmlEnum]
        Ondertrouw,
        [XmlEnum]
        Overlijden,
        [XmlEnum]
        Registratie,
        [XmlEnum]
        Trouwen
    }
}