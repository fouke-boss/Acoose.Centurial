using Acoose.Genealogy.Extensibility.Data;
using Acoose.Genealogy.Extensibility.Data.References;
using Acoose.Genealogy.Extensibility.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acoose.Centurial.Package.Tests.nl
{
    [TestClass]
    public class Graftombe
    {
        [TestMethod]
        public void Martin_Peters()
        {
            // execute
            var result = ScraperTest.ExecuteFromEmbeddedResource<Package.nl.Graftombe>("https://www.graftombe.nl/names/info/1283307/peters", "Acoose.Centurial.Package.Tests.nl.Graftombe.Martin_Peters.html");

            // provenance
            result.FindProvenance<UnknownRepository>(1)
                .AssertChild<Cemetery>()
                .AssertCondition(x => x.CemeteryName == "Hoensbroek Randweg")
                .AssertCondition(x => x.Place == "Heerlen, Limburg");

            // persons
            var person1 = result.FindPerson("Martin Peters")
                .AssertDate("Birth", "18-02-1924")
                .AssertDate("Death", "30-08-2008");
            var persons = result.FindPerson("Ellie Michiels")
                .AssertDate("Birth", "07-02-1925")
                .AssertDate("Death", "02-08-2004");
        }
    }
}