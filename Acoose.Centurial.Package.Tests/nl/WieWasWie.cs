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
    public class WieWasWie
    {
        [TestMethod]
        public void Details1()
        {
            // draai test
            var result = ScraperTest.ExecuteFromEmbeddedResource<Package.nl.WieWasWie>("https://www.wiewaswie.nl/nl/detail/28375272", "Acoose.Centurial.Package.Tests.nl.WieWasWie - details1.html");

            //// valideren
            //Assert.IsTrue(result.Source.Info.Length == 1);
            //Assert.IsTrue(result.Source.Info.Single() is PersonInfo);

            //// persoon ophalen
            //var persoon = result.Source.Info.Single() as PersonInfo;
            //var birth = persoon.Events.SingleOrDefault(x => x.Type == "Birth");
            //var death = persoon.Events.SingleOrDefault(x => x.Type == "Death");
            //Assert.IsTrue(birth != null);
            //Assert.IsTrue(birth.Date.Length == 1);
            //Assert.IsTrue(birth.Date.Single().ToDateString() == "1991-03-05");
            //Assert.IsTrue(death != null);
            //Assert.IsTrue(death.Date.Length == 1);
            //Assert.IsTrue(death.Date.Single().ToDateString() == "2011-01-13");
        }
        [TestMethod]
        public void Details2()
        {
            // draai test
            var result = ScraperTest.ExecuteFromEmbeddedResource<Package.nl.WieWasWie>("https://www.wiewaswie.nl/nl/detail/28375272", "Acoose.Centurial.Package.Tests.nl.WieWasWie - details2.html");
        }
        [TestMethod]
        public void Details3()
        {
            // draai test
            var result = ScraperTest.ExecuteFromEmbeddedResource<Package.nl.WieWasWie>("https://www.wiewaswie.nl/nl/detail/28375272", "Acoose.Centurial.Package.Tests.nl.WieWasWie - details3.html");
        }
    }
}