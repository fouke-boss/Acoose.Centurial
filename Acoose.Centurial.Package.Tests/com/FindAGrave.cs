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
                .AssertCondition(x => x.Creator is PersonalName p && p.FamilyName == null && p.GivenNames == "Susan M.S.")
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
        public void Louise_Atkinson()
        {
            // execute
            var result = Utility.ExecuteTest<Package.com.FindAGrave>("https://nl.findagrave.com/memorial/160043493/louise-may-atkinson");

            // provenance
            result.FindProvenance<UnknownRepository>(1)
                .AssertChild<Photograph>()
                .AssertCondition(x => x.Creator is PersonalName p && p.FamilyName == "Shaffer" && p.GivenNames == "Jen")
                .AssertCondition(x => x.Date.Equals(Date.TryParse("2016-03-26")));
            result.FindProvenance<UnknownRepository>(2)
                .AssertChild<Cemetery>()
                .AssertCondition(x => x.CemeteryName == "Rose Hills Memorial Park")
                .AssertCondition(x => x.Place == "Whittier, Los Angeles County, California, VS");

            // persons
            var person1 = result.FindPerson("Louise May Mooney")
                .AssertBirth("1931-01-31", "Pennsylvania, USA")
                .AssertDeath("2011-11-19", "San Bernardino County, California, USA");
        }
        [TestMethod]
        public void Stephen_Evans()
        {
            // execute
            var result = Utility.ExecuteTest<Package.com.FindAGrave>("https://nl.findagrave.com/memorial/130290237/stephen-vincent-evans");

            // provenance
            result.AssertWebsite(0, "Find a Grave", "https://nl.findagrave.com", true)
                .AssertWebPage("Stephen Vincent Evans (1897-1977) - Find a...");

            // persons
            var person1 = result.FindPerson("Stephen Vincent Evans")
                .AssertBirth("1897-01-10", "Morrison County, Minnesota, USA")
                .AssertDeath("1977-04-30", "Santa Cruz, Santa Cruz County, California, USA");
        }
        [TestMethod]
        public void Lorna_ONeill()
        {
            // execute
            var result = Utility.ExecuteTest<Package.com.FindAGrave>("https://nl.findagrave.com/memorial/230657838/lorna-marie-o'neill");

            // provenance
            result.AssertWebsite(0, "Find a Grave", "https://nl.findagrave.com", true)
                .AssertWebPage("Lorna Marie Hazleton O'Neill (1923-1982) -...");

            // persons
            var person1 = result.FindPerson("Lorna Marie Hazleton")
                .AssertBirth("1923-05-13", "Turtle Creek, Allegheny County, Pennsylvania, USA")
                .AssertDeath("1982-10-26", "Atlanta, DeKalb County, Georgia, USA");
        }
        [TestMethod]
        public void Michael_Faraday()
        {
            // execute
            var result = Utility.ExecuteTest<Package.com.FindAGrave>("https://nl.findagrave.com/memorial/20883/michael-faraday");
        }
    }
}