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
    public class GenealogieOnline
    {
        [TestMethod]
        public void CollectionPage()
        {
            this.Test("https://www.genealogieonline.nl/genealogie-coret/", "Genealogie Coret", "Genealogie Coret");
        }
        [TestMethod]
        public void PersonPage()
        {
            this.Test("https://www.genealogieonline.nl/genealogie-coret/I002022.php", "Genealogie Coret", "Deloris Martha Ellis");
        }

        private void Test(string url, string collectionTitle, string pageTitle)
        {
            // init
            var result = ScraperTest.ExecuteFromWeb<Package.nl.GenealogieOnline>(url);
            var website = result.Source.Provenance
                .OfType<Website>()
                .Single();
            var collection = website.Items
                .Select(x => x.Item)
                .OfType<OnlineCollection>()
                .Single();
            var webpage = collection.Items
                .Select(x => x.Item)
                .OfType<WebPage>()
                .Single();

            // assert
            Assert.AreEqual(collectionTitle, collection.Title);
            Assert.AreEqual(pageTitle, webpage.Title?.Value);
        }
    }
}