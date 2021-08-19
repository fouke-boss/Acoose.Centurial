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
        public void BS_Huwelijk()
        {
            // draai test
            var result = ScraperTest.ExecuteFromEmbeddedResource<Package.nl.WieWasWie>("https://www.wiewaswie.nl/nl/detail/30179586", "Acoose.Centurial.Package.Tests.nl.WieWasWie - BS Huwelijk.html");

            // valideren
            Assert.IsTrue(result.Source.Info.Length == 9);
            Assert.IsTrue(result.Source.Info.OfType<PersonInfo>().Count() == 5);

            // persoon ophalen
            var gerardus = result.Source.Info
                .OfType<PersonInfo>()
                .Where(x => x.FamilyName.Single() == "Boss")
                .Where(x => x.GivenNames.Single() == "Gerardus")
                .Single();
            var birth = gerardus.Events.SingleOrDefault(x => x.Type == "Birth");
            Assert.IsTrue(birth != null);
            Assert.IsTrue(birth.Date.Length == 1);
            Assert.IsTrue(birth.Date.Single().ToDateString() == "1877-08-11");
        }
        [TestMethod]
        public void BS_Geboorte()
        {
            // draai test
            var result = ScraperTest.ExecuteFromEmbeddedResource<Package.nl.WieWasWie>("https://www.wiewaswie.nl/nl/detail/55534975", "Acoose.Centurial.Package.Tests.nl.WieWasWie - BS Geboorte.html");

            // valideren
            Assert.IsTrue(result.Source.Info.Length == 5);
            Assert.IsTrue(result.Source.Info.OfType<PersonInfo>().Count() == 3);
        }
        [TestMethod]
        public void BS_Overlijden()
        {
            // draai test
            var result = ScraperTest.ExecuteFromEmbeddedResource<Package.nl.WieWasWie>("https://www.wiewaswie.nl/en/detail/49955499", "Acoose.Centurial.Package.Tests.nl.WieWasWie - BS Overlijden.html");

            // valideren
            Assert.IsTrue(result.Source.Info.Length == 5);
            Assert.IsTrue(result.Source.Info.OfType<PersonInfo>().Count() == 3);
        }
        [TestMethod]
        public void DTB_Dopen()
        {
            // draai test
            var result = ScraperTest.ExecuteFromEmbeddedResource<Package.nl.WieWasWie>("https://www.wiewaswie.nl/nl/detail/49909227", "Acoose.Centurial.Package.Tests.nl.WieWasWie - DTB Dopen.html");

            // valideren
            Assert.IsTrue(result.Source.Info.Length == 5);
            Assert.IsTrue(result.Source.Info.OfType<PersonInfo>().Count() == 3);
        }
    }
}