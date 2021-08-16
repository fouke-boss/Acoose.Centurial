using Acoose.Genealogy.Extensibility.Data;
using Acoose.Genealogy.Extensibility.Data.References;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acoose.Centurial.Package.nl
{
    partial class WieWasWie
    {
        private class Person
        {
            public Person(HtmlNode node)
            {
                // process description list
                node.GetDescriptionLists((dt, dd) =>
                {
                    // init
                    var dataDictionary = dt.GetAttributeValue("data-dictionary", null);
                    if (string.IsNullOrWhiteSpace(dataDictionary))
                    {
                        // name
                        this.Name = dd.GetInnerText();
                        this.Role = Utility.TryParseEventRole(dt.GetInnerText());
                    }
                    else
                    {
                        switch (dataDictionary)
                        {
                            case "SourceDetail.BirthDate":
                                this.BirthDate = Date.TryParse(dd.GetInnerText());
                                break;
                            case "SourceDetail.BirthPlace":
                                this.BirthPlace = dd.GetInnerText();
                                break;
                            case "SourceDetail.Age":
                                this.Age = int.TryParse(dd.GetInnerText(), out var age) ? age : default;
                                break;
                            case "SourceDetail.Gender":
                                this.Gender = Utility.TryParseGender(dd.GetInnerText());
                                break;
                            default:
                                throw new NotSupportedException(dataDictionary);
                        }
                    }
                });
            }

            public int Id
            {
                get; set;
            }
            public EventRole Role
            {
                get; private set;
            }
            public string Name
            {
                get; private set;
            }
            public Date BirthDate
            {
                get; private set;
            }
            public string BirthPlace
            {
                get; private set;
            }
            public int? Age
            {
                get; private set;
            }
            public Gender? Gender
            {
                get; set;
            }

            internal PersonInfo ToInfo()
            {
                // parse name
                Acoose.Genealogy.Extensibility.ParsingUtility.ParseName(this.Name, out string lastName, out string givenNames, out string particles);
                var familyName = string.Join(" ", new string[] { particles, lastName }.Where(x => !string.IsNullOrWhiteSpace(x)));

                // create
                var result = new PersonInfo()
                {
                    Id = this.Id.ToString(),
                    FamilyName = familyName.ToArrayIfAny(),
                    GivenNames = givenNames.ToArrayIfAny(),
                    Gender = this.Gender.ToArrayIfAny()
                };

                // events
                result.ImportEvent("Birth", this.BirthDate, this.BirthPlace);

                // done
                return result;
            }
        }
    }
}