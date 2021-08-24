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
    public class AlleFriezen
    {
        [TestMethod]
        public void BS_Geboorte()
        {
            // draai test
            var result = ScraperTest.ExecuteFromEmbeddedResource<Package.nl.AlleFriezen>("https://allefriezen.nl/zoeken/deeds/4aac4e15-f247-bd66-09af-c03ab15d71d9", "Acoose.Centurial.Package.Tests.nl.AlleFriezen - BS Geboorte.html");

            // valideren
            Assert.IsTrue(result.Source.Info.Length == 5);
            Assert.IsTrue(result.Source.Info.OfType<PersonInfo>().Count() == 3);
        }
        [TestMethod]
        public void BS_Overlijden()
        {
            // draai test
            var result = ScraperTest.ExecuteFromEmbeddedResource<Package.nl.AlleFriezen>("https://allefriezen.nl/zoeken/deeds/bbe4ab39-9950-99ba-f0ca-477324d15383", "Acoose.Centurial.Package.Tests.nl.AlleFriezen - BS Overlijden.html");

            // valideren
            Assert.IsTrue(result.Source.Info.Length == 5);
            Assert.IsTrue(result.Source.Info.OfType<PersonInfo>().Count() == 3);
        }
        [TestMethod]
        public void BS_Overig()
        {
            // draai test
            var result = ScraperTest.ExecuteFromEmbeddedResource<Package.nl.AlleFriezen>("https://allefriezen.nl/zoeken/deeds/bc989c50-6833-49ad-8d75-8fb500450529", "Acoose.Centurial.Package.Tests.nl.AlleFriezen - Overig.html");

            // valideren
            Assert.IsTrue(result.Source.Info.Length == 1);
        }
    }
}