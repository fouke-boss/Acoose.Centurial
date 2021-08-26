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
            var result = Utility.ExecuteTest<Package.com.FindAGrave>("https://nl.findagrave.com/memorial/229778573/charles-suffolk");

            // provenance
            result.FindProvenance<UnknownRepository>(1)
                .AssertChild<Photograph>()
                .AssertCondition(x => x.Creator is PersonalName p && p.FamilyName == "S." && p.GivenNames == "Susan M.")
                .AssertCondition(x => x.Date.Equals(Date.TryParse("21-07-2021")));
            result.FindProvenance<UnknownRepository>(2)
                .AssertChild<Cemetery>()
                .AssertCondition(x => x.CemeteryName == "Lavenham Cemetery")
                .AssertCondition(x => x.Place == "Lavenham, Babergh District, Suffolk, England");

            // persons
            var person1 = result.FindPerson("Charles Suffolk")
                .AssertDate("Birth", "28-06-1918")
                .AssertDate("Death", "24-02-1992");
        }
        [TestMethod]
        public void Michael_Faraday()
        {
            // execute
            var result = Utility.ExecuteTest<Package.com.FindAGrave>("https://nl.findagrave.com/memorial/20883/michael-faraday");
        }
    }
}