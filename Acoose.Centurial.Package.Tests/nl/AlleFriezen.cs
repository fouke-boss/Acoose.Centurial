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
            result.FindProvenance<Website>(0)
                .AssertCondition(x => x.Title == "AlleFriezen")
                .AssertCondition(x => x.IsVirtualArchive == true)
                .AssertChild<OnlineItem>()
                .AssertChild<DatabaseEntry>()
                .AssertCondition(x => x.EntryFor == "Egberta Maria Biersma");
            result.FindProvenance<PublicArchive>(1)
                .AssertCondition(x => x.Name == "Tresoar")
                .AssertChild<ArchivedItem>()
                .AssertChild<Collection>()
                .AssertChild<ArchivedItem>()
                .AssertChild<VitalRecord>()
                .AssertCondition(x => x.Title.Value == "Burgerlijke stand")
                .AssertCondition(x => x.Jurisdiction == "Aengwirden")
                .AssertChild<RecordScriptFormat>()
                .AssertCondition(x => x.Number == "0018")
                .AssertCondition(x => x.Label == "Geboorteregister 1823")
                .AssertCondition(x => x.Date.Equals(Date.TryParse("1823-03-29")))
                .AssertCondition(x => x.ItemOfInterest == "Egberta Maria Biersma");

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
            result.FindRelationship(person2, person1)
                .AssertParentChild(ParentChild.Person1IsBiologicalParentOfPerson2);
            result.FindRelationship(person3, person1)
                .AssertParentChild(ParentChild.Person1IsBiologicalParentOfPerson2);
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
            result.FindRelationship(person2, person1)
                .AssertParentChild(ParentChild.Person1IsBiologicalParentOfPerson2);
            result.FindRelationship(person3, person1)
                .AssertParentChild(ParentChild.Person1IsBiologicalParentOfPerson2);

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