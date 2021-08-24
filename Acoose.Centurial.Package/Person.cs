using Acoose.Genealogy.Extensibility.Data;
using Acoose.Genealogy.Extensibility.Data.References;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acoose.Centurial.Package
{
    public class Person
    {
        public int Id
        {
            get; set;
        }
        public EventRole Role
        {
            get; set;
        }

        public string Name
        {
            get
            {
                return string.Join(" ", new string[] { this.GivenNames, this.FamilyName }.Where(x => !string.IsNullOrWhiteSpace(x)));
            }
            set
            {
                // parse name
                Acoose.Genealogy.Extensibility.ParsingUtility.ParseName(value, out string lastName, out string givenNames, out string particles);

                // done
                this.FamilyName = string.Join(" ", new string[] { particles, lastName }.Where(x => !string.IsNullOrWhiteSpace(x)));
                this.GivenNames = givenNames;
            }
        }
        public string FamilyName
        {
            get; set;
        }
        public string GivenNames
        {
            get; set;
        }

        public Date BirthDate
        {
            get; set;
        }
        public string BirthPlace
        {
            get; set;
        }

        public Date DeathDate
        {
            get; set;
        }
        public string DeathPlace
        {
            get; set;
        }

        public Gender? Gender
        {
            get; set;
        }

        public Age Age
        {
            get; set;
        }
        public string Occupation
        {
            get; set;
        }

        internal PersonInfo ToInfo(Date recordDate)
        {
            // create
            var result = new PersonInfo()
            {
                Id = this.Id.ToString(),
                FamilyName = this.FamilyName.ToArrayIfAny(),
                GivenNames = this.GivenNames.ToArrayIfAny(),
                Gender = this.Gender.ToArrayIfAny(),
                Occupation = this.Occupation.ToArrayIfAny()
                    .Select(x => new Status<string>() { Date = recordDate, Value = x })
                    .ToArray(),
                Age = this.Age.ToArrayIfAny()
                    .Select(x => new Status<Age>() { Date = recordDate, Value = x })
                    .ToArray()
            };

            // events
            result.ImportEvent("Birth", this.BirthDate, this.BirthPlace);
            result.ImportEvent("Death", this.DeathDate, this.DeathPlace);

            // done
            return result;
        }

        public override string ToString()
        {
            return $"{this.Name} ({this.Role})";
        }
    }
}