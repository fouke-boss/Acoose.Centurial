using Acoose.Genealogy.Extensibility.Data;
using Acoose.Genealogy.Extensibility.Data.References;
using Acoose.Genealogy.Extensibility.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Acoose.Centurial.Package.Tests.com
{
    [TestClass]
    public class Ancestry
    {
        [TestMethod]
        public void Search_Paul_Vierus()
        {
            // execute
            this.Execute("https://search.ancestry.com/cgi-bin/sse.dll?dbid=2957&h=46049932&indiv=1",
                (extension, result) =>
                {
                    // provenance
                    result.AssertWebsite(0, "Ancestry", $"https://search.ancestry.{extension}", true)
                        .AssertOnlineCollection(extension == "de" ?
                            "Berlin, Deutschland, Heiratsregister, 1874-1936" :
                            "Berlin, Germany, Marriages, 1874-1936")
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
            );
        }

        [TestMethod]
        public void Search_Auguste_Nettelmann()
        {
            // execute
            this.Execute("https://search.ancestry.com/cgi-bin/sse.dll?db=BerlinGermanyDeaths&h=22258896&indiv=try",
                (extension, result) =>
                {
                    // provenance
                    result.AssertWebsite(0, "Ancestry", $"https://search.ancestry.{extension}", true)
                        .AssertOnlineCollection(extension == "de" ?
                            "Berlin, Deutschland, Sterberegister, 1874-1955" :
                            "Berlin, Germany, Deaths, 1874-1955")
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
            );
        }
        [TestMethod]
        public void DiscoveryUI_Maria_Avent()
        {
            // execute
            this.Execute("https://www.ancestry.com/discoveryui-content/view/6495613:1623",
                (extension, result) =>
                {
                    // provenance
                    result.AssertWebsite(0, "Ancestry", $"https://www.ancestry.{extension}", true)
                        .AssertOnlineCollection(extension == "de" ?
                            "London, England, Heiraten und Aufgebote zur Eheschließung der Church of England, 1754-1936" :
                            "London, England, Church of England Marriages and Banns, 1754-1936")
                        .AssertDatabaseEntry("Maria Annie Avent");
                    result.AssertPublicArchive(1, "London Metropolitan Archives", "London, England")
                        .AssertArchivedItem(null)
                        .AssertChurchRecord("Church Parish", "St Paul, Greenwich, Greenwich, England")
                        .AssertRecordScriptFormat("London Church of England Parish Registers", null, null, "Maria Annie Avent", "1918-12-28");

                    // persons
                    var person1 = result.FindPerson("Maria Annie Avent")
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
            );
        }
        [TestMethod]
        public void Search_Charles_Johnson()
        {
            // execute
            this.Execute("https://search.ancestry.com/cgi-bin/sse.dll?dbid=61048&h=1500464664&indiv=1",
                (extension, result) =>
                {
                    // provenance
                    result.AssertWebsite(0, "Ancestry", $"https://search.ancestry.{extension}", true)
                        .AssertOnlineCollection(extension == "de" ?
                            "USA, presbyterianische Register, 1701-1970" :
                            "U.S., Presbyterian Church Records, 1701-1970")
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
            );
        }
        [TestMethod]
        public void DiscoveryUI_Maria_Dreher()
        {
            // execute
            this.Execute("https://www.ancestry.com/discoveryui-content/view/1208697774:61060",
                (extension, result) =>
                {
                    // provenance
                    result.AssertWebsite(0, "Ancestry", $"https://www.ancestry.{extension}", true)
                        .AssertOnlineCollection(extension == "de" ?
                            "Baden und Hessen, Deutschland, evangelische Kirchenbücher, 1502-1985" :
                            "Baden and Hesse Germany, Lutheran Baptisms, Marriages, and Burials, 1502-1985");
                    result.FindProvenance<UnknownRepository>(1)
                        .AssertChild<Unspecified>()
                        .AssertCondition(x => x.CreditLine == "Vogelbach, 62/63, Maria Boss");

                    // persons
                    var person1 = result.FindPerson("Maria Boss")
                        .AssertGender(Gender.Female);
                    var person2 = result.FindPerson("Leo Dreher");
                    var person3 = result.FindPerson("Hugo Dreher");

                    // relationships
                    result.FindPartnership(person1, person2);
                    result.FindParentChild(person1, person3);
                }
            );
        }
        [TestMethod]
        public void DiscoveryUI_Hugo_Dreher()
        {
            // execute
            this.Execute("https://www.ancestry.com/discoveryui-content/view/8697774:61060",
                (extension, result) =>
                {
                    // provenance
                    result.AssertWebsite(0, "Ancestry", $"https://www.ancestry.{extension}", true)
                        .AssertOnlineCollection(extension == "de" ?
                            "Baden und Hessen, Deutschland, evangelische Kirchenbücher, 1502-1985" :
                            "Baden and Hesse Germany, Lutheran Baptisms, Marriages, and Burials, 1502-1985")
                        .AssertDatabaseEntry("Hugo Dreher");
                    result.FindProvenance<UnknownRepository>(1)
                        .AssertChurchRecord("Vogelbach", "Vogelbach, Baden, Preußen")
                        .AssertRecordScriptFormat(null, "62/63", null, "Hugo Dreher", "1930-04-12");

                    // persons
                    var person1 = result.FindPerson("Hugo Dreher")
                        .AssertDate("Birth", "1902-03-01")
                        .AssertAge(28, "1930-04-12")
                        .AssertGender(Gender.Male);
                    var person2 = result.FindPerson("Emilie Grether");
                    var person3 = result.FindPerson("Leo Dreher")
                        .AssertGender(Gender.Male);
                    var person4 = result.FindPerson("Maria Dreher")
                        .AssertGender(Gender.Female);

                    // relationships
                    result.FindPartnership(person1, person2)
                        .AssertEvent("ChurchMarriage", "1930-04-12", "Vogelbach, Baden, Preußen");
                    result.FindParentChild(person3, person1);
                    result.FindParentChild(person4, person1);
                }
            );
        }

        [TestMethod]
        public void Search_Pat_Linden_1860()
        {
            // execute
            this.Execute("https://search.ancestry.de/cgi-bin/sse.dll?dbid=3564&h=21582789&indiv=1",
                (extension, result) =>
                {
                    // provenance
                    result.AssertWebsite(0, "Ancestry", $"https://www.ancestry.{extension}", true)
                        .AssertOnlineCollection(extension == "de" ?
                            "New York, USA, kompilierter Index für Volkszählungen und Volkszählungsersätze, 1790-1890" :
                            "New York, U.S., Compiled Census and Census Substitutes Index, 1790-1890")
                        .AssertDatabaseEntry("Pat Linden");
                    result.FindProvenance<UnknownRepository>(1)
                        .AssertCensus("New York County, NY", "NY 1860 Federal Census Index")
                        .AssertCensusScriptFormat("12 W. Nyc District 1", "829", null, "Pat Linden", "1860");

                    // persons
                    var person1 = result.FindPerson("Pat Linden")
                        .AssertResidence("New York County, NY", "1860");
                }
            );
        }
        [TestMethod]
        public void DiscoveryUI_Richard_Linden_1883()
        {
            // execute
            this.Execute("https://www.ancestry.com/discoveryui-content/view/223553:8810",
                (extension, result) =>
                {
                    // provenance
                    result.AssertWebsite(0, "Ancestry", $"https://www.ancestry.{extension}", true)
                        .AssertOnlineCollection(extension == "de" ?
                            "Oklahoma und indianisches Territorium, USA, Volkszählungen und Listen der Indianer, 1851-1959" :
                            "Oklahoma and Indian Territory, U.S., Indian Censuses and Rolls, 1851-1959")
                        .AssertDatabaseEntry("Richard Linden");
                    result.FindProvenance<UnknownRepository>(1)
                        .AssertCensus("Cooweeskoowee", "Cherokee Census, 1883")
                        .AssertCensusScriptFormat(null, null, null, "Richard Linden", "1883");

                    // persons
                    var person1 = result.FindPerson("Richard Linden")
                        .AssertAge(8, "1883")
                        .AssertResidence("Cooweeskoowee", "1883");
                }
            );
        }
        [TestMethod]
        public void DiscoveryUI_John_Toell_1880()
        {
            // execute
            this.Execute("https://www.ancestry.com/discoveryui-content/view/48958297:6742",
                (extension, result) =>
                {
                    // provenance
                    result.AssertWebsite(0, "Ancestry", $"https://www.ancestry.{extension}", true)
                        .AssertOnlineCollection(extension == "de" ?
                            "US-Volkszählung 1880" :
                            "1880 United States Federal Census")
                        .AssertDatabaseEntry("John Toell");
                    result.FindProvenance<UnknownRepository>(1)
                        .AssertCensus("Five Creeks, Clay, Kansas", extension == "de" ? "US-Volkszählung 1880" : "1880 United States Federal Census")
                        .AssertCensusScriptFormat(null, "68A", null, "John Toell", "1880");

                    // persons
                    var person1 = result.FindPerson("John Toell")
                        .AssertAge(4, "1880")
                        .AssertGender(Gender.Male)
                        .AssertResidence("Five Creeks, Clay, Kansas", "1880");
                }
            );
        }
        [TestMethod]
        public void DiscoveryUI_John_Toelle_1930()
        {
            // execute
            this.Execute("https://www.ancestry.com/discoveryui-content/view/109940063:6224",
                (extension, result) =>
                {
                    // provenance
                    result.AssertWebsite(0, "Ancestry", $"https://www.ancestry.{extension}", true)
                        .AssertOnlineCollection(extension == "de" ?
                            "US-Volkszählung 1930" :
                            "1930 United States Federal Census")
                        .AssertDatabaseEntry("John C. Toelle");
                    result.FindProvenance<UnknownRepository>(1)
                        .AssertCensus("Hillsboro, Washington, Oregon", extension == "de" ? "US-Volkszählung 1930" : "1930 United States Federal Census")
                        .AssertCensusScriptFormat(null, "11A", "275", "John C. Toelle", "1930");

                    // persons
                    var person1 = result.FindPerson("John C. Toelle")
                        .AssertAge(53, "1930")
                        .AssertGender(Gender.Male)
                        .AssertOccupation("Carpenter", "1930")
                        .AssertResidence("Hillsboro, Washington, Oregon", "1930");
                }
            );
        }
        [TestMethod]
        public void DiscoveryUI_George_Johnson_1939()
        {
            // execute
            this.Execute("https://www.ancestry.com/discoveryui-content/view/36407081:61596",
                (extension, result) =>
                {
                    // provenance
                    result.AssertWebsite(0, "Ancestry", $"https://www.ancestry.{extension}", true)
                        .AssertOnlineCollection(extension == "de" ?
                            "1939, Register für England und Wales" :
                            "1939 England and Wales Register")
                        .AssertDatabaseEntry("George R. Johnson");
                    result.AssertPublicArchive(1, "The National Archives", "Kew, London, England")
                        .AssertArchivedItem(null)
                        .AssertCensus("Leicester, Leicestershire, England", extension == "de" ? "1939, Register für England und Wales" : "1939 England and Wales Register")
                        .AssertCensusScriptFormat(null, null, null, "George R. Johnson", "1939");

                    // persons
                    var person1 = result.FindPerson("George R. Johnson")
                        .AssertDate("Birth", "1911-10-20")
                        .AssertGender(Gender.Male)
                        .AssertOccupation("Departmental Manager-Wholesale Outfitter", "1939")
                        .AssertResidence("Leicester, Leicestershire, England", "1939");
                }
            );
        }

        private void Execute(string url, Action<string, ScraperTest> test, [CallerMemberName] string caseName = null)
        {
            // init
            foreach (var extension in new string[] { "de", "com" })
            {
                // init
                var fullUrl = url.Replace("ancestry.com", $"ancestry.{extension}");
                var resourceName = string.Format("Acoose.Centurial.Package.Tests.com.Ancestry.{0}_{1}.html", caseName, extension.ToUpper());

                // done
                var result = ScraperTest.ExecuteFromEmbeddedResource<Package.com.Ancestry>(fullUrl, resourceName);

                // execute
                test(extension, result);
            }
        }
    }
}