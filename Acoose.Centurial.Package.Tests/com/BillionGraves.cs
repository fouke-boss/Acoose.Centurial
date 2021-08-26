using Acoose.Genealogy.Extensibility.Data;
using Acoose.Genealogy.Extensibility.Data.References;
using Acoose.Genealogy.Extensibility.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acoose.Centurial.Package.Tests.com
{
    [TestClass]
    public class BillionGraves
    {
        [TestMethod]
        public void Peter_Hendericks()
        {
            // execute
            var result = ScraperTest.ExecuteFromEmbeddedResource<Package.com.BillionGraves>("https://billiongraves.nl/grave/Peter-Hendericks/43666597", "Acoose.Centurial.Package.Tests.com.BillionGraves.Peter_Hendericks.html");

            // provenance
            result.FindProvenance<UnknownRepository>(1)
                .AssertChild<Photograph>()
                .AssertCondition(x => x.Creator is PersonalName p && p.FamilyName == "bethj1973" && string.IsNullOrEmpty(p.GivenNames))
                .AssertCondition(x => x.Date.Equals(Date.TryParse("28-11-2020")));
            result.FindProvenance<UnknownRepository>(2)
                .AssertChild<Cemetery>()
                .AssertCondition(x => x.CemeteryName == "Exeter Cemetery")
                .AssertCondition(x => x.Place == "Exeter, Fillmore, Nebraska, United States")
                .AssertCondition(x => x.AccessData == "Unnamed Rd");

            // persons
            var person1 = result.FindPerson("Peter Hendericks")
                .AssertDate("Birth", "1853")
                .AssertDate("Death", "1903");
        }
    }
}