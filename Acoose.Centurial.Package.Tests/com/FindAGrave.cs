using Acoose.Genealogy.Extensibility.Data;
using Acoose.Genealogy.Extensibility.Data.References;
using Acoose.Genealogy.Extensibility.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acoose.Centurial.Package.Tests.com
{
    [TestClass]
    public class FindAGrave
    {
        [TestMethod]
        public void Charles_Suffolk()
        {
            // execute
            var result = ScraperTest.ExecuteFromEmbeddedResource<Package.com.FindAGrave>("https://nl.findagrave.com/memorial/229778573/charles-suffolk", "Acoose.Centurial.Package.Tests.com.FindAGrave.Charles_Suffolk.html");

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
        [TestMethod]
        public void Michael_Faraday()
        {
            // execute
            var result = ScraperTest.ExecuteFromEmbeddedResource<Package.com.FindAGrave>("https://nl.findagrave.com/memorial/20883/michael-faraday", "Acoose.Centurial.Package.Tests.com.FindAGrave.Michael_Faraday.html");
        }

    }
}