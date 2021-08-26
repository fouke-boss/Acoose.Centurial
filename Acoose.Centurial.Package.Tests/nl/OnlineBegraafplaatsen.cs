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