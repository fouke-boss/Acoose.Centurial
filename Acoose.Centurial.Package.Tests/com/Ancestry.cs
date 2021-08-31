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
    public class Ancestry
    {
        [TestMethod]
        public void EN_Search_Paul_Vierus()
        {
            // execute
            var result = Utility.ExecuteTest<Package.com.Ancestry>("https://search.ancestry.com/cgi-bin/sse.dll?dbid=2957&h=46049932&indiv=1");

            // provenance
            result.AssertWebsite(0, "Ancestry", "https://search.ancestry.com", true)
                .AssertOnlineCollection("Berlin, Germany, Marriages, 1874-1936")
                .AssertDatabaseEntry("Paul Friedrich Gustav Vierus");
            result.AssertPublicArchive(1, "Landesarchiv Berlin", "Berlin, Deutschland")
                .AssertArchivedItem(null)
                .AssertVitalRecord("Berlin, Berlin, Deutschland", "Standesämter")
                .AssertRecordScriptFormat("Personenstandsregister Heiratsregister, Steglitz", null, "21", "Paul Friedrich Gustav Vierus", "1896-03-14");

            // persons
            var person1 = result.FindPerson("Paul Friedrich Gustav Vierus")
                .AssertDate("Birth", "1871-03-27")
                .AssertGender(Gender.Male)
                .AssertAge(24, "1896-03-14");
            var person2 = result.FindPerson("Elisabeth Clara Marie Reddig");
            var person3 = result.FindPerson("Erdmann Friedrich Julius Vierus")
                .AssertGender(Gender.Male);
            var person4 = result.FindPerson("Johanna Charlotte Florentine Vierus")
                .AssertGender(Gender.Female);

            // relationships
            result.FindPartnership(person1, person2)
                .AssertEvent("CivilMarriage", "1896-03-14", "Berlin, Berlin, Deutschland");
            result.FindParentChild(person3, person1);
            result.FindParentChild(person4, person1);
        }
        [TestMethod]
        public void EN_Search_Auguste_Nettelmann()
        {
            // execute
            var result = Utility.ExecuteTest<Package.com.Ancestry>("https://search.ancestry.com/cgi-bin/sse.dll?db=BerlinGermanyDeaths&h=22258896&indiv=try");

            // provenance
            result.AssertWebsite(0, "Ancestry", "https://search.ancestry.com", true)
                .AssertOnlineCollection("Berlin, Germany, Deaths, 1874-1955")
                .AssertDatabaseEntry("Auguste Rebekka Vordenbäumen");
            result.AssertPublicArchive(1, "Landesarchiv Berlin", "Berlin, Deutschland")
                .AssertArchivedItem(null)
                .AssertVitalRecord("Berlin, Berlin, Deutschland", "Standesämter")
                .AssertRecordScriptFormat("Personenstandsregister Sterberegister, Charlottenburg II", null, "1018", "Auguste Rebekka Vordenbäumen", "1899-07-10");

            // persons
            var person1 = result.FindPerson("Auguste Rebekka Vordenbäumen")
                .AssertDate("Death", "1899-07-10")
                .AssertAge(29, "1899-07-10");
            var person2 = result.FindPerson("Friedrich Nettelmann");
            var person3 = result.FindPerson("Wilhelm Vordenbäumen")
                .AssertGender(Gender.Male);
            var person4 = result.FindPerson("Sophie Vordenbäumen")
                .AssertGender(Gender.Female);

            // relationships
            result.FindPartnership(person1, person2);
            result.FindParentChild(person3, person1);
            result.FindParentChild(person4, person1);
        }
        [TestMethod]
        public void EN_DiscoveryUI_Maria_Avent()
        {
            // execute
            var result = Utility.ExecuteTest<Package.com.Ancestry>("https://www.ancestry.com/discoveryui-content/view/6495613:1623");

            // provenance
            result.AssertWebsite(0, "Ancestry", "https://www.ancestry.com", true)
                .AssertOnlineCollection("London, England, Church of England Marriages and Banns, 1754-1936")
                .AssertDatabaseEntry("Maria Annie Avent");
            result.AssertPublicArchive(1, "London Metropolitan Archives", "London, England")
                .AssertArchivedItem(null)
                .AssertChurchRecord("Church Parish", "St Paul, Greenwich, Greenwich, England")
                .AssertRecordScriptFormat("London Church of England Parish Registers", null, null, "Maria Annie Avent", "1918-12-28");

            // persons
            var person1 = result.FindPerson("Maria Annie Avent")
                .AssertDate("Birth", "1893")
                .AssertAge(25, "1918-12-28")
                .AssertGender(Gender.Female);
            var person2 = result.FindPerson("Wilfred Edward Alden");
            var person3 = result.FindPerson("William Septimus Avent")
                .AssertGender(Gender.Male);

            // relationships
            result.FindPartnership(person1, person2)
                .AssertEvent("ChurchMarriage", "1918-12-28", "St Paul, Greenwich, Greenwich, England");
            result.FindParentChild(person3, person1);
        }
        [TestMethod]
        public void EN_Search_Charles_Johnson()
        {
            // execute
            var result = Utility.ExecuteTest<Package.com.Ancestry>("https://search.ancestry.com/cgi-bin/sse.dll?dbid=61048&h=1500464664&indiv=1");

            // provenance
            result.AssertWebsite(0, "Ancestry", "https://search.ancestry.com", true)
                .AssertOnlineCollection("U.S., Presbyterian Church Records, 1701-1970")
                .AssertDatabaseEntry("Charles W. Johnson");
            result.AssertPublicArchive(1, "Presbyterian Historical Society", "Philadelphia, Pennsylvania")
                .AssertArchivedItem(null)
                .AssertChurchRecord("First Presbyterian Church", "Perth Amboy, New Jersey, USA")
                .AssertRecordScriptFormat("U.S., Presbyterian Church Records, 1701-1907", null, null, "Charles W. Johnson", "1970");

            // persons
            var person1 = result.FindPerson("Charles W. Johnson")
                .AssertDate("Death", "1970-08-21")
                .AssertDate("Burial", "1970")
                .AssertPlace("Burial", "Perth Amboy, New Jersey, USA")
                .AssertGender(Gender.Male);
        }
        [TestMethod]
        public void DiscoveryUI_George_Johnson()
        {
            // execute
            var result = Utility.ExecuteTest<Package.com.Ancestry>("https://www.ancestry.com/discoveryui-content/view/36407081:61596");
        }
    }
}