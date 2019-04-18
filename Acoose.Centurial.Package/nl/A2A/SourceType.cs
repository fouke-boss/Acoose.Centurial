using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Acoose.Centurial.Package.nl.A2A
{
    public enum SourceType
    {
        [XmlEnum("DTB Dopen")]
        DtbDopen,
        [XmlEnum("DTB Trouwen")]
        DtbTrouwen,
        [XmlEnum("DTB Begraven")]
        DtbBegraven,
        [XmlEnum("BS Geboorte")]
        BsGeboorte,
        [XmlEnum("BS Huwelijk")]
        BsHuwelijk,
        [XmlEnum("BS Overlijden")]
        BsOverlijden,
        [XmlEnum("Bevolkingsregister")]
        Bevolkingsregister,
        [XmlEnum("Notariële archieven")]
        NotariëleArchieven,
        [XmlEnum("VOC Opvarenden")]
        VocOpvarenden,
        [XmlEnum("Kadaster")]
        Kadaster,
        [XmlEnum("Memories van Successie")]
        MemoriesVanSuccessie
    }
}