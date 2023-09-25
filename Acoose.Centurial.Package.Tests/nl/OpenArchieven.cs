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
            var result = ScraperTest.ExecuteFromWeb<Package.nl.OpenArchieven>("https://www.openarchieven.nl/rhl:0c407a29-76a9-06aa-6b82-f8b29d8064ea");
        }
        [TestMethod]
        public void BsHuwelijk()
        {
            var result = ScraperTest.ExecuteFromWeb<Package.nl.OpenArchieven>("https://www.openarchieven.nl/rhl:8b171211-c5d9-1775-ff70-7162ba387cb1");
        }
        [TestMethod]
        public void DtbDopen1()
        {
            var result = ScraperTest.ExecuteFromWeb<Package.nl.OpenArchieven>("https://www.openarchieven.nl/gld:99736541-F428-4E04-ABEE-33EF1BA9C819");
        }
        [TestMethod]
        public void DtbDopen2()
        {
            var result = ScraperTest.ExecuteFromWeb<Package.nl.OpenArchieven>("https://www.openarchieven.nl/elo:9f2781d8-a009-e740-859b-4d20f7a65040");
        }
        [TestMethod]
        public void DtbTrouwen()
        {
            var result = ScraperTest.ExecuteFromWeb<Package.nl.OpenArchieven>("https://www.openarchieven.nl/gld:32D8B842-1D91-4D84-9CAF-C08A8D79179E");
        }
        [TestMethod]
        public void Bevolkingsregistratie()
        {
            // https://www.openarchieven.nl/svp:5148C8E9-7F6F-6E85-E053-CA00A8C0A581
            var result = ScraperTest.ExecuteFromWeb<Package.nl.OpenArchieven>("https://www.openarchieven.nl/brd:2bad5cd5-0873-9939-366f-616d4e4d4972");
        }
        [TestMethod]
        public void Ondertrouw()
        {
            var result = ScraperTest.ExecuteFromWeb<Package.nl.OpenArchieven>("https://www.openarchieven.nl/saa:3e264352-3071-c7e9-5b13-4ac7733a377a");
        }

        [TestMethod]
        public void Ondertrouw2()
        {
            var result = ScraperTest.ExecuteFromWeb<Package.nl.OpenArchieven>("https://www.openarchieven.nl/saa:ca594e08-23ec-b51b-37cb-efccbe49b0cc");
        }

        [TestMethod]
        public void Bug1()
        {
            var result = ScraperTest.ExecuteFromWeb<Package.nl.OpenArchieven>("https://www.openarchieven.nl/gra:b74ef301-aecc-483e-af46-13e228f0de0d");
        }
    }
}