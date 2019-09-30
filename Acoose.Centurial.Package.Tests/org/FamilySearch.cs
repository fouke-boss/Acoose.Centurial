using Acoose.Genealogy.Extensibility.Data.References;
using Acoose.Genealogy.Extensibility.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acoose.Centurial.Package.Tests.org
{
    [TestClass]
    public class FamilySearch
    {
        [TestMethod]
        public void Image1()
        {
            this.Image("https://www.familysearch.org/ark:/61903/3:1:3QS7-89NP-WX3X?i=11&wc=S3FW-DPX%3A343580001%2C1066486202&cc=2026214",
                "FamilySearch.Image1.txt",
                "Netherlands, Limburg Province, Civil Registration, 1792-1963",
                "Heer > Huwelijken 1937 > image 12 of 95",
                "Regionaal Historisch Centrum Limburg, Maastricht (Limburg Regional History Center, Maastricht)"
            );
        }
        [TestMethod]
        public void Image2()
        {
            this.Image("https://www.familysearch.org/ark:/61903/3:1:S3HT-XXDT-DK?i=6&wc=3PM9-DP8%3A1063290601&cc=1469062",
                "FamilySearch.Image2.txt",
                "Massachusetts Marriages, 1841-1915",
                "004329366 > image 7 of 1214",
                "State Archives, Boston"
            );
        }
        private void Image(string url, string resourceName, string collectionTitle, string path, string citing)
        {
            // init
            var result = ScraperTest.ExecuteFromEmbeddedResource<Package.org.FamilySearch>(url, resourceName);
            var website = result.Source.Provenance
                .OfType<Website>()
                .Single();
            var collection = website.Items
                .Select(x => x.Item)
                .OfType<OnlineCollection>()
                .Single();
            var digitalImage = collection.Items
                .Select(x => x.Item)
                .OfType<DigitalImage>()
                .Single();

            // assert
            Assert.AreEqual(collectionTitle, collection.Title);
            Assert.AreEqual(collection.Items.Single().Path, path);
            Assert.AreEqual(citing, digitalImage.CreditLine);
        }


        [TestMethod]
        public void Record1()
        {
            this.Record("https://www.familysearch.org/ark:/61903/1:1:XTHF-8MX",
                "FamilySearch.Record1.txt",
                "Virginia Births and Christenings, 1584-1917",
                "Mary Abbott in entry for James Abbott, 25 Mar 1876",
                "Accomack, Virginia, reference 93; FHL microfilm 30,136"
            );
        }
        [TestMethod]
        public void Record2()
        {
            this.Record("https://www.familysearch.org/ark:/61903/1:1:KLG4-61P",
                "FamilySearch.Record2.txt",
                "United States Public Records, 1970-2009",
                "Neil Boss, Residence, Hanson, Massachusetts, United States",
                "a third party aggregator of publicly available information"
            );
        }
        private void Record(string url, string resourceName, string collectionTitle, string entryFor, string citing)
        {
            // init
            var result = ScraperTest.ExecuteFromEmbeddedResource<Package.org.FamilySearch>(url, resourceName);
            var website = result.Source.Provenance
                .OfType<Website>()
                .Single();
            var collection = website.Items
                .Select(x => x.Item)
                .OfType<OnlineCollection>()
                .Single();
            var databaseEntry = collection.Items
                .Select(x => x.Item)
                .OfType<DatabaseEntry>()
                .Single();
            var unspecified = result.Source.Provenance
                .OfType<UnknownRepository>()
                .Single().Items
                .OfType<Unspecified>()
                .Single();

            // assert
            Assert.AreEqual(collectionTitle, collection.Title);
            Assert.AreEqual(entryFor, databaseEntry.EntryFor);
            Assert.AreEqual(citing, unspecified.CreditLine);
        }
    }
}