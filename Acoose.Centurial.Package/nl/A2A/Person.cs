using Acoose.Genealogy.Extensibility.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Acoose.Centurial.Package.nl.A2A
{
    public class Person
    {
        [XmlAttribute("pid")]
        public string Id
        {
            get;
            set;
        }
        [XmlElement]
        public PersonName PersonName
        {
            get;
            set;
        }
        [XmlElement]
        public string Gender
        {
            get;
            set;
        }
        [XmlElement]
        public DetailPlace Residence
        {
            get;
            set;
        }
        [XmlElement]
        public PersonReligion Religion
        {
            get;
            set;
        }
        [XmlElement]
        public DetailPlace Origin
        {
            get;
            set;
        }
        [XmlElement]
        public PersonAge Age
        {
            get;
            set;
        }
        [XmlElement]
        public TransDate BirthDate
        {
            get;
            set;
        }
        [XmlElement]
        public DetailPlace BirthPlace
        {
            get;
            set;
        }
        [XmlElement]
        public string Profession
        {
            get;
            set;
        }
        [XmlElement]
        public string MaritalStatus
        {
            get;
            set;
        }

        internal PersonInfo ToData(int id, Date eventDate)
        {
            // init
            var result = new PersonInfo() { Id = id.ToString() };

            // name
            result.FamilyName = this.PersonName?.FamilyName.ToArrayIfAny();
            result.GivenNames = this.PersonName?.GivenNames.ToArrayIfAny();
            result.Nickname = this.PersonName?.Nickname.ToArrayIfAny();
            result.Gender = this.ConvertGender().ToArray();

            // birth
            result.Birth = new InfoEvent()
            {
                Date = this.BirthDate?.ToData().ToArrayIfAny(),
                Place = this.BirthPlace?.ToString().ToArrayIfAny()
            };

            // statuses
            result.Age = this.Age?.ToData().ToArrayIfAny()
                .Select(x => new Status<Age>() { Date = eventDate, Value = x })
                .ToArray();
            result.Residence = this.Residence?.ToString().ToArrayIfAny()
                .Select(x => new Status<string>() { Date = eventDate, Value = x })
                .ToArray();
            result.Occupation = this.Profession.ToArrayIfAny()
                .Where(x => !(x.Contains("beroep") && (x.Contains("zonder") || x.Contains("geen"))))
                .Select(x => new Status<string>() { Date = eventDate, Value = x })
                .ToArray();
            result.Religion = this.Religion?.PersonReligionLiteral.ToArrayIfAny()
                .Select(x => new Status<string>() { Date = eventDate, Value = x })
                .ToArray();

            // done
            return result;
        }
        private IEnumerable<Gender> ConvertGender()
        {
            switch (this.Gender)
            {
                case "Man":
                    yield return Genealogy.Extensibility.Data.Gender.Male;
                    break;
                case "Vrouw":
                    yield return Genealogy.Extensibility.Data.Gender.Female;
                    break;
                case "Onbekend":
                case null:
                case "":
                    break;
                default:
                    yield return Genealogy.Extensibility.Data.Gender.Intersex;
                    break;
            }
        }
    }
}