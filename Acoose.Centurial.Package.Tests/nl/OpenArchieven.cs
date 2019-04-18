using System;
using Acoose.Genealogy.Extensibility;
using Acoose.Genealogy.Extensibility.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Acoose.Centurial.Package.Tests.nl
{
    [TestClass]
    public class OpenArchieven
    {
        [TestMethod]
        public void BsGeboorte()
        {
            var result = ScraperTest.ExecuteFromWeb<Package.nl.OpenArchieven>("https://www.openarch.nl/rhl:0c407a29-76a9-06aa-6b82-f8b29d8064ea");
        }
        [TestMethod]
        public void BsHuwelijk()
        {
            var result = ScraperTest.ExecuteFromWeb<Package.nl.OpenArchieven>("https://www.openarch.nl/rhl:8b171211-c5d9-1775-ff70-7162ba387cb1");
        }
        [TestMethod]
        public void DtbDopen()
        {
            var result = ScraperTest.ExecuteFromWeb<Package.nl.OpenArchieven>("https://www.openarch.nl/gld:99736541-F428-4E04-ABEE-33EF1BA9C819");
        }
        [TestMethod]
        public void DtbTrouwen()
        {
            var result = ScraperTest.ExecuteFromWeb<Package.nl.OpenArchieven>("https://www.openarch.nl/gld:32D8B842-1D91-4D84-9CAF-C08A8D79179E");
        }
        [TestMethod]
        public void Bevolkingsregistratie()
        {
            // https://www.openarch.nl/svp:5148C8E9-7F6F-6E85-E053-CA00A8C0A581
            var result = ScraperTest.ExecuteFromWeb<Package.nl.OpenArchieven>("https://www.openarch.nl/brd:2bad5cd5-0873-9939-366f-616d4e4d4972");
        }
        [TestMethod]
        public void Ondertrouw()
        {
            var result = ScraperTest.ExecuteFromWeb<Package.nl.OpenArchieven>("https://www.openarch.nl/saa:3e264352-3071-c7e9-5b13-4ac7733a377a");
        }
    }
}