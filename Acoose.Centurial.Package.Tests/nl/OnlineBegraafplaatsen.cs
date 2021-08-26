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
    public class OnlineBegraafplaatsen
    {
        [TestMethod]
        public void Quadvlieg_Boss()
        {
            // execute
            var result = ScraperTest.ExecuteFromEmbeddedResource<Package.nl.OnlineBegraafplaatsen>("https://www.online-begraafplaatsen.nl/graf/1630904/2601706/M-E-Boss-1893-1976", "Acoose.Centurial.Package.Tests.nl.OnlineBegraafplaatsen.Quadvlieg_Boss.html");

            // provenance
            result.FindProvenance<UnknownRepository>(1)
                .AssertChild<Cemetery>()
                .AssertCondition(x => x.CemeteryName == "Begraafplaats")
                .AssertCondition(x => x.Place == "Brunssum")
                .AssertCondition(x => x.AccessData == "Merkelbeekerstraat");

            // persons
            var person1 = result.FindPerson("N.J. Quadvlieg")
                .AssertDate("Birth", "09-11-1889")
                .AssertDate("Death", "29-01-1949");
            var persons = result.FindPerson("M.E. Boss")
                .AssertDate("Birth", "05-08-1893")
                .AssertDate("Death", "12-01-1976");
        }
    }
}