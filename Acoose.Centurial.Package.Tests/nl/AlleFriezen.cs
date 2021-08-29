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
    public class AlleFriezen
    {
        [TestMethod]
        public void BS_Geboorte()
        {
            // draai test
            var result = Utility.ExecuteTest<Package.nl.AlleFriezen>("https://allefriezen.nl/zoeken/deeds/4aac4e15-f247-bd66-09af-c03ab15d71d9");

            // provenance
            result
                .AssertWebsite(0, "AlleFriezen", "https://allefriezen.nl", true)
                .AssertDatabaseEntry("Egberta Maria Biersma");
            result
                .AssertPublicArchive(1, "Tresoar", null)
                .AssertArchivedItem("30-02")
                .AssertChild<Collection>()
                .AssertArchivedItem("1004")
                .AssertVitalRecord("Aengwirden", "Burgerlijke stand")
                .AssertRecordScriptFormat("Geboorteregister 1823", null, "0018", "Egberta Maria Biersma", "1823-03-29");

            // persons
            var person1 = result.FindPerson("Egberta Maria Biersma")
                .AssertDate("Birth", "1823-03-28")
                .AssertPlace("Birth", "Aengwirden")
                .AssertGender(Gender.Female);
            var person2 = result.FindPerson("Roelof Biersma")
                .AssertGender(Gender.Male);
            var person3 = result.FindPerson("Geesje Wachters")
                .AssertGender(Gender.Female);

            // relationships
            result.FindParentChild(person2, person1);
            result.FindParentChild(person3, person1);
        }
        [TestMethod]
        public void BS_Overlijden()
        {
            // draai test
            var result = Utility.ExecuteTest<Package.nl.AlleFriezen>("https://allefriezen.nl/zoeken/deeds/bbe4ab39-9950-99ba-f0ca-477324d15383");

            // persons
            var person1 = result.FindPerson("Gerrit Pieters Biersma")
                .AssertDate("Death", "1829-04-18")
                .AssertPlace("Death", "Bolsward")
                .AssertGender(Gender.Male);
            var person2 = result.FindPerson("Pieter Pieters Biersma")
                .AssertGender(Gender.Male);
            var person3 = result.FindPerson("Grietje Jillerts Kuipers")
                .AssertGender(Gender.Female);

            // relationships
            result.FindParentChild(person2, person1);
            result.FindParentChild(person3, person1);

            // valideren
            Assert.IsTrue(result.Source.Info.Length == 5);
            Assert.IsTrue(result.Source.Info.OfType<PersonInfo>().Count() == 3);
        }
        [TestMethod]
        public void Overig()
        {
            // draai test
            var result = Utility.ExecuteTest<Package.nl.AlleFriezen>("https://allefriezen.nl/zoeken/deeds/bc989c50-6833-49ad-8d75-8fb500450529");

            // valideren
            Assert.IsTrue(result.Source.Info.Length == 2);
        }
    }
}