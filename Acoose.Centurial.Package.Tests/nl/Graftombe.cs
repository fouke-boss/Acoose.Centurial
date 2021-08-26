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

            //// valideren
            //Assert.IsTrue(result.Source.Info.Length == 9);
            //Assert.IsTrue(result.Source.Info.OfType<PersonInfo>().Count() == 5);

            //// persoon ophalen
            //var gerardus = result.Source.Info
            //    .OfType<PersonInfo>()
            //    .Where(x => x.FamilyName.Single() == "Boss")
            //    .Where(x => x.GivenNames.Single() == "Gerardus")
            //    .Single();
            //var birth = gerardus.Events.SingleOrDefault(x => x.Type == "Birth");
            //Assert.IsTrue(birth != null);
            //Assert.IsTrue(birth.Date.Length == 1);
            //Assert.IsTrue(birth.Date.Single().ToDateString() == "1877-08-11");
        }
    }
}