using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Acoose.Centurial.Package.nl.A2A
{
    public class PersonName
    {
        [XmlElement]
        public string PersonNameLiteral
        {
            get;
            set;
        }
        [XmlElement]
        public string PersonNameTitle
        {
            get;
            set;
        }
        [XmlElement]
        public string PersonNameTitleOfNobility
        {
            get;
            set;
        }
        [XmlElement]
        public string PersonNameFirstName
        {
            get;
            set;
        }
        [XmlElement]
        public string PersonNameNickName
        {
            get;
            set;
        }
        [XmlElement]
        public string PersonNameAlias
        {
            get;
            set;
        }
        [XmlElement]
        public string PersonNamePatronym
        {
            get;
            set;
        }
        [XmlElement]
        public string PersonNamePrefixLastName
        {
            get;
            set;
        }
        [XmlElement]
        public string PersonNameLastName
        {
            get;
            set;
        }
        [XmlElement]
        public string PersonNameFamilyName
        {
            get;
            set;
        }
        [XmlElement]
        public string PersonNameInitials
        {
            get;
            set;
        }

        internal string FamilyName
        {
            get
            {
                // init
                var parts = new string[]
                {
                    this.PersonNamePrefixLastName,
                    this.PersonNamePatronym,
                    this.PersonNameLastName
                };

                // done
                return string.Join(" ", parts.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()));
            }
        }
        internal string Nickname
        {
            get
            {
                return this.PersonNameNickName;
            }
        }
        internal string GivenNames
        {
            get
            {
                return (string.IsNullOrWhiteSpace(this.PersonNameFirstName) ? this.PersonNameInitials : this.PersonNameFirstName);
            }
        }
    }
}