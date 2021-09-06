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
    public class StadsarchiefAmsterdam
    {
        [TestMethod]
        public void DTP_Doop()
        {
            // test
            var result = Utility.ExecuteTest<Package.nl.StadsarchiefAmsterdam>("https://archief.amsterdam/indexen/deeds/49e78ba5-da38-4d20-9763-c3730c6a15b2?person=961f6af7-7f94-53f7-e053-b784100aa83b");

            // provenance
            result
                .AssertWebsite(0, "Stadsarchief Amsterdam", "https://archief.amsterdam/", true)
                .AssertDatabaseEntry("Henrika Gesina");
            result
                .AssertPublicArchive(1, "Stadsarchief Amsterdam", "Amsterdam")
                .AssertArchivedItem("5001")
                .AssertCollection(null)
                .AssertArchivedItem("85")
                .AssertChurchRecord("Noorderkerk", "Amsterdam")
                .AssertRecordScriptFormat("DTB Dopen", "45(folio 23)/12", null, "Henrika Gesina", "1789-05-31");

            // persons
            var person1 = result.FindPerson("Henrika Gesina")
                .AssertEvent("Baptism", "1789-05-31", "Amsterdam");
            var person2 = result.FindPerson("Hendrik Jan Arendshorst");
            var person3 = result.FindPerson("Aleida Bernhardina Morgenstern");
            var person4 = result.FindPerson("Diderikus Johannis Morgenstern");

            // relationships
            result.FindParentChild(person2, person1);
            result.FindParentChild(person3, person1);
        }
        [TestMethod]
        public void DTP_Begraven()
        {
            // test
            var result = Utility.ExecuteTest<Package.nl.StadsarchiefAmsterdam>("https://archief.amsterdam/indexen/deeds/ac03544a-218b-4508-be41-0aa32eba1e22?person=99e87e17-1aeb-2bb2-e053-b784100a6a2e");

            // provenance
            result
                .AssertWebsite(0, "Stadsarchief Amsterdam", "https://archief.amsterdam/", true)
                .AssertDatabaseEntry("Haermen Jansen");
            result
                .AssertPublicArchive(1, "Stadsarchief Amsterdam", "Amsterdam")
                .AssertArchivedItem("5001")
                .AssertCollection(null)
                .AssertArchivedItem("1194")
                .AssertCemeteryRecord("St. Anthonis Kerkhof", "Amsterdam")
                .AssertRecordScriptFormat("DTB Begraven", "20 en p.21", null, "Haermen Jansen", "1665-07-30");

            // persons
            var person1 = result.FindPerson("Haermen Jansen")
                .AssertEvent("Burial", "1665-07-30", "Amsterdam");
        }
    }
}