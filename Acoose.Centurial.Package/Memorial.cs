using Acoose.Genealogy.Extensibility.Data;
using Acoose.Genealogy.Extensibility.Data.References;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acoose.Centurial.Package
{
    public class Memorial
    {
        public string URL
        {
            get; set;
        }

        public string WebsiteTitle
        {
            get; set;
        }
        public string WebsiteURL
        {
            get; set;
        }

        public string CemeteryName
        {
            get; set;
        }
        public string CemeteryPlace
        {
            get; set;
        }
        public string CemeteryAccess
        {
            get; set;
        }

        public string PhotographedBy
        {
            get; set;
        }
        public Date PhotographedAt
        {
            get; set;
        }

        public Person[] Persons
        {
            get; set;
        }
        public string[] Images
        {
            get; set;
        }

        public IEnumerable<Info> GenerateInfos()
        {
            // init
            var burialPlace = string.Join(", ", new string[] { this.CemeteryName, this.CemeteryPlace }.Where(x => !string.IsNullOrWhiteSpace(x)));

            // done
            return this.Persons
                .Select((p, i) =>
                {
                    // id
                    p.Id = i + 1;

                    // done
                    var result = p.ToInfo(p.DeathDate);

                    // set burial place
                    result.ImportEvent("Burial", null, burialPlace);

                    // done
                    return result;
                })
                .ToArray();
        }
        public IEnumerable<Repository> GenerateProvenance()
        {
            // init
            var web = (this.Images.NullCoalesce().Any() ? (Acoose.Genealogy.Extensibility.Data.References.Source) new DigitalImage() : new DatabaseEntry() { EntryFor = this.Persons.First().Name });

            // layer 1: website
            yield return new Website()
            {
                Title = this.WebsiteTitle,
                Url = this.WebsiteURL,
                IsVirtualArchive = true,
                Items = new OnlineItem[]
               {
                    new OnlineItem()
                    {
                        Url = this.URL,
                        Accessed = Date.Today,
                        Item = web
                    }
               }
            };

            // layer 2: photograph
            if (!string.IsNullOrEmpty(this.PhotographedBy))
            {
                // parse
                Acoose.Genealogy.Extensibility.ParsingUtility.ParseName(this.PhotographedBy, out var familyName, out var givenNames, out var particles);

                // done
                yield return new UnknownRepository()
                {
                    Items = new Genealogy.Extensibility.Data.References.Source[]
                    {
                        new Photograph()
                        {
                            Creator = new PersonalName()
                            {
                                FamilyName = familyName,
                                GivenNames = givenNames,
                                Particles = particles
                            },
                            Date = this.PhotographedAt
                        }
                    }
                };
            }

            // layer 3: cemetery
            yield return new UnknownRepository()
            {
                Items = new Genealogy.Extensibility.Data.References.Source[]
                {
                    new Cemetery()
                    {
                        CemeteryName = this.CemeteryName,
                        Place = this.CemeteryPlace,
                        AccessData = this.CemeteryAccess,
                        Items = new CemeteryItem[]
                        {
                            new Gravestone()
                            {
                                Person = string.Join(" & ", this.Persons.Select(x => x.Name))
                            }
                        }
                    }
                }
            };
        }
    }
}