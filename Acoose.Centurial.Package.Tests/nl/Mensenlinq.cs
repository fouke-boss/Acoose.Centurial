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
    public class Mensenlinq
    {
        [TestMethod]
        public void MartijnJankoBoss()
        {
            // draai test
            var result = ScraperTest.ExecuteFromEmbeddedResource<Package.nl.Mensenlinq>("https://www.mensenlinq.nl/overlijdensberichten/martijn-janko-boss-3179153", "Acoose.Centurial.Package.Tests.nl.Mensenlinq1.html");

            // valideren
            Assert.IsTrue(result.Source.Info.Length == 1);
            Assert.IsTrue(result.Source.Info.Single() is PersonInfo);

            // persoon ophalen
            var persoon = result.Source.Info.Single() as PersonInfo;
            Assert.IsTrue(persoon.Birth != null);
            Assert.IsTrue(persoon.Birth.Date.Length == 1);
            Assert.IsTrue(persoon.Birth.Date.Single().ToDateString() == "1991-03-05");
            Assert.IsTrue(persoon.Birth.Place.Single() == "Wirdum");
            Assert.IsTrue(persoon.Death != null);
            Assert.IsTrue(persoon.Death.Date.Length == 1);
            Assert.IsTrue(persoon.Death.Date.Single().ToDateString() == "2011-01-13");
        }
        [TestMethod]
        public void HennieCoenenFijen()
        {
            // draai test
            var result = ScraperTest.ExecuteFromEmbeddedResource<Package.nl.Mensenlinq>("https://mensenlinq.nl/overlijdensberichten/hennie-coenen-fijen-8726190/", "Acoose.Centurial.Package.Tests.nl.Mensenlinq2.html");

            // valideren
            Assert.IsTrue(result.Source.Info.Length == 1);
            Assert.IsTrue(result.Source.Info.Single() is PersonInfo);

            // persoon ophalen
            var persoon = result.Source.Info.Single() as PersonInfo;
            Assert.IsTrue(persoon.Birth != null);
            Assert.IsTrue(persoon.Birth.Date.Length == 1);
            Assert.IsTrue(persoon.Birth.Date.Single().ToDateString() == "1923-08-01");
            Assert.IsTrue(persoon.Birth.Place.Single() == "Amsterdam");
            Assert.IsTrue(persoon.Death != null);
            Assert.IsTrue(persoon.Death.Date.Length == 1);
            Assert.IsTrue(persoon.Death.Date.Single().ToDateString() == "2020-02-21");
        }
    }
}