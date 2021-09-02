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
    public class WieWasWie
    {
        [TestMethod]
        public void BS_Huwelijk()
        {
            // draai test
            var result = Utility.ExecuteTest<Package.nl.WieWasWie>("https://www.wiewaswie.nl/nl/detail/30179586");

            // validate
            this.ValidateBS_Huwelijk(result);
        }
        [TestMethod]
        public void BS_Huwelijk_EN()
        {
            // draai test
            var result = Utility.ExecuteTest<Package.nl.WieWasWie>("https://www.wiewaswie.nl/en/detail/30179586");

            // validate
            this.ValidateBS_Huwelijk(result);
        }
        private void ValidateBS_Huwelijk(ScraperTest result)
        {
            // provenance
            result.AssertWebsite(0, "WieWasWie", "https://www.wiewaswie.nl", true)
                .AssertDatabaseEntry("Gerardus Boss & Anna Catharina Peters");
            result
                .AssertPublicArchive(1, "Regionaal Historisch Centrum Limburg", "Maastricht")
                .AssertArchivedItem("12.019")
                .AssertVitalRecord("Brunssum", "Burgerlijke stand")
                .AssertRecordScriptFormat(null, null, "9", "Gerardus Boss & Anna Catharina Peters", "25-11-1903");

            // persons
            var person1 = result.FindPerson("Gerardus Boss")
                .AssertPlace("Birth", "Brunssum")
                .AssertDate("Birth", "11-08-1877")
                .AssertAge(26, "25-11-1903")
                .AssertGender(Gender.Male);
            var person2 = result.FindPerson("Anna Catharina Peters")
                .AssertPlace("Birth", "Schinveld")
                .AssertDate("Birth", "09-11-1879")
                .AssertAge(24, "25-11-1903")
                .AssertGender(Gender.Female);
            var person3 = result.FindPerson("Maria Margaretha Boss")
                .AssertGender(Gender.Female);
            var person4 = result.FindPerson("Jan Leonard Peters")
                .AssertGender(Gender.Male);
            var person5 = result.FindPerson("Maria Catharina Hendrix")
                .AssertGender(Gender.Female);

            // relationships
            result.FindPartnership(person1, person2)
                .AssertDate("CivilMarriage", "25-11-1903")
                .AssertPlace("CivilMarriage", "Brunssum");
            result.FindParentChild(person3, person1);
            result.FindParentChild(person4, person2);
            result.FindParentChild(person5, person2);
        }

        [TestMethod]
        public void BS_Geboorte()
        {
            // draai test
            var result = Utility.ExecuteTest<Package.nl.WieWasWie>("https://www.wiewaswie.nl/nl/detail/55534975");

            // valideren
            Assert.IsTrue(result.Source.Info.Length == 5);
            Assert.IsTrue(result.Source.Info.OfType<PersonInfo>().Count() == 3);
        }
        [TestMethod]
        public void BS_Overlijden_1()
        {
            // draai test
            var result = Utility.ExecuteTest<Package.nl.WieWasWie>("https://www.wiewaswie.nl/en/detail/49955499");

            // valideren
            Assert.IsTrue(result.Source.Info.Length == 5);
            Assert.IsTrue(result.Source.Info.OfType<PersonInfo>().Count() == 3);
        }
        [TestMethod]
        public void BS_Overlijden_2()
        {
            // draai test
            var result = Utility.ExecuteTest<Package.nl.WieWasWie>("https://www.wiewaswie.nl/nl/detail/35909050");

            // provenance
            result.AssertWebsite(0, "WieWasWie", "https://www.wiewaswie.nl", true)
                .AssertDatabaseEntry("Maria Katharina Ritzen");
            result
                .AssertPublicArchive(1, "Regionaal Historisch Centrum Limburg", "Maastricht")
                .AssertArchivedItem("12.131")
                .AssertVitalRecord("Hoensbroek", "Burgerlijke stand")
                .AssertRecordScriptFormat(null, null, "64", "Maria Katharina Ritzen", "1959-10-05");

            // persons
            var person1 = result.FindPerson("Maria Katharina Ritzen")
                .AssertPlace("Birth", "Kohlnheid (Duitsland)")
                .AssertDate("Death", "1959-10-03")
                .AssertPlace("Death", "Hoensbroek")
                .AssertAge(70, "1959-10-05")
                .AssertGender(Gender.Female);
            var person2 = result.FindPerson("Peter Matthias Ritzen")
                .AssertGender(Gender.Male);
            var person3 = result.FindPerson("Maria Agnes Hubertina Janssen")
                .AssertGender(Gender.Female);
            var person4 = result.FindPerson("Christiaan Felix Quaedackers");

            // relationships
            result.FindParentChild(person2, person1);
            result.FindParentChild(person3, person1);
            result.FindPartnership(person4, person1);
        }
        [TestMethod]
        public void DTB_Dopen()
        {
            // draai test
            var result = Utility.ExecuteTest<Package.nl.WieWasWie>("https://www.wiewaswie.nl/nl/detail/49909227");

            // provenance
            result.AssertWebsite(0, "WieWasWie", "https://www.wiewaswie.nl", true)
                .AssertDatabaseEntry("Mechteld van Scherpenseel");
            result
                .AssertPublicArchive(1, "Het Utrechts Archief", "Utrecht")
                .AssertArchivedItem("DTB_RHC_ZO-Utrecht_65")
                .AssertChurchRecord("Nederduits-gereformeerd (later Nederlands-hervormd)", "Doorn")
                .AssertRecordScriptFormat("Doorn NH dopen 1674-1811", "166", null, "Mechteld van Scherpenseel", "09-02-1800");

            // persons
            var person1 = result.FindPerson("Mechteld van Scherpenseel")
                .AssertBaptism("09-02-1800", "Doorn")
                .AssertGender(Gender.Female);
            var person2 = result.FindPerson("Rijk van Scherpenseel")
                .AssertGender(Gender.Male);
            var person3 = result.FindPerson("Adriaantje Schuurman")
                .AssertGender(Gender.Female);

            // relationships
            result.FindParentChild(person2, person1);
            result.FindParentChild(person3, person1);
        }
        [TestMethod]
        public void Bevolkingsregister()
        {
            // draai test
            var result = Utility.ExecuteTest<Package.nl.WieWasWie>("https://www.wiewaswie.nl/nl/detail/34193965");

            // valideren
            result.FindPerson("Theodorus Johannes van der Vliet")
                .AssertBirth("13-08-1892", "Zuid Scharwoude");
            result.FindPerson("Neeltje Bruyn")
                .AssertBirth("03-04-1901", "Noord Scharwoude");
            result.FindPerson("Alida Margaretha van der Vliet");
            result.FindPerson("Emmerentiana van der Vliet");
            result.FindPerson("Clasina Elisabeth van der Vliet");
            result.FindPerson("Klaas van der Vliet");
            result.FindPerson("Guurtruida van der Vliet");
            result.FindPerson("Johannes Franciscus van der Vliet")
                .AssertBirth("15-01-1936", "Noord Scharwoude");
            result.FindPerson("Henderikus van der Vliet");
        }
        [TestMethod]
        public void VOC()
        {
            // draai test
            var result = Utility.ExecuteTest<Package.nl.WieWasWie>("https://www.wiewaswie.nl/nl/detail/44919");

            // public archive
            result
                .FindProvenance<UnknownRepository>(1)
                .AssertUnspecified("Delft, Delft, 35, Hendrik van Vliet; Nationaal Archief, Den Haag");

            // persons
            var person1 = result.FindPerson("Hendrik van Vliet");
            var person2 = result.FindPerson("Geertje Engels")
                .AssertGender(Gender.Female);

            // relationships
            result.FindPartnership(person1, person2);
        }
        [TestMethod]
        public void Overig()
        {
            // draai test
            var result = Utility.ExecuteTest<Package.nl.WieWasWie>("https://www.wiewaswie.nl/nl/detail/61743783");

            // valideren
            Assert.IsTrue(result.Source.Info.Length == 5);
            Assert.IsTrue(result.Source.Info.OfType<PersonInfo>().Count() == 5);
        }
    }
}