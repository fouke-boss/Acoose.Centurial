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
    public class AlleGroningers
    {
        [TestMethod]
        public void BS_Geboorte()
        {
            // draai test
            var result = ScraperTest.ExecuteFromEmbeddedResource<Package.nl.AlleGroningers>("https://www.allegroningers.nl/zoeken-op-naam/deeds/0756ba18-cee8-8db7-6017-848bbd248691", "Acoose.Centurial.Package.Tests.nl.AlleGroningers - BS Geboorte.html");

            // valideren
            Assert.IsTrue(result.Source.Info.Length == 5);
            Assert.IsTrue(result.Source.Info.OfType<PersonInfo>().Count() == 3);
        }
        [TestMethod]
        public void BS_Huwelijk()
        {
            // draai test
            var result = ScraperTest.ExecuteFromEmbeddedResource<Package.nl.AlleGroningers>("https://www.allegroningers.nl/zoeken-op-naam/deeds/47a3f2bc-215f-430a-140f-ba43ef2fa12d", "Acoose.Centurial.Package.Tests.nl.AlleGroningers - BS Huwelijk.html");

            // valideren
            Assert.IsTrue(result.Source.Info.Length == 11);
            Assert.IsTrue(result.Source.Info.OfType<PersonInfo>().Count() == 6);
        }
        [TestMethod]
        public void BS_Overlijden()
        {
            // draai test
            var result = ScraperTest.ExecuteFromEmbeddedResource<Package.nl.AlleGroningers>("https://www.allegroningers.nl/zoeken-op-naam/deeds/27788d0d-57dc-d1ff-a9fd-14dbe17ef93b?person=5afd6369-954b-b934-5420-47b666850cb9", "Acoose.Centurial.Package.Tests.nl.AlleGroningers - BS Overlijden.html");

            // valideren
            Assert.IsTrue(result.Source.Info.Length == 5);
            Assert.IsTrue(result.Source.Info.OfType<PersonInfo>().Count() == 3);
        }

        [TestMethod]
        public void DTP_Doop()
        {
            // draai test
            var result = ScraperTest.ExecuteFromEmbeddedResource<Package.nl.AlleGroningers>("https://www.allegroningers.nl/zoeken-op-naam/deeds/5f51221d-bfab-3c0d-457c-e7c87d073490?person=656573b0-b631-c31c-4330-669fbf905afd", "Acoose.Centurial.Package.Tests.nl.AlleGroningers - DTP Doop.html");

            // valideren
            Assert.IsTrue(result.Source.Info.Length == 5);
            Assert.IsTrue(result.Source.Info.OfType<PersonInfo>().Count() == 3);
        }
    }
}