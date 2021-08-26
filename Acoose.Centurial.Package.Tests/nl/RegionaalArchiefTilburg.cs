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
    public class RegionaalArchiefTilburg
    {
        [TestMethod]
        public void DTP_Huwelijk()
        {
            // draai test
            var result = Utility.ExecuteTest<Package.nl.RegionaalArchiefTilburg>("https://www.regionaalarchieftilburg.nl/zoek-een-persoon/deeds/14014cda-7f60-f1fd-f07d-df296dc44569?person=b1add30f-554a-0cec-501f-e24d5c5f1e9c");

            // provenance
            result.FindProvenance<Website>(0)
                .AssertCondition(x => x.Title == "AlleFriezen")
                .AssertCondition(x => x.IsVirtualArchive == true)
                .AssertChild<OnlineItem>()
                .FindChild<DatabaseEntry>()
                .AssertCondition(x => x.EntryFor == "Egberta Maria Biersma");
            result.FindProvenance<PublicArchive>(1)
                .AssertCondition(x => x.Name == "Tresoar")
                .AssertChild<ArchivedItem>()
                .FindChild<Collection>()
                .AssertChild<ArchivedItem>()
                .FindChild<VitalRecord>()
                .AssertCondition(x => x.Title.Value == "Burgerlijke stand")
                .AssertCondition(x => x.Jurisdiction == "Aengwirden")
                .AssertChild<RecordScriptFormat>()
                .AssertCondition(x => x.Number == "0018")
                .AssertCondition(x => x.Label == "Geboorteregister 1823")
                .AssertCondition(x => x.Date.Equals(Date.TryParse("1823-03-29")))
                .AssertCondition(x => x.ItemOfInterest == "Egberta Maria Biersma");

            //// persons
            //var person1 = result.FindPerson("Egberta Maria Biersma")
            //    .AssertDate("Birth", "1823-03-28")
            //    .AssertPlace("Birth", "Aengwirden")
            //    .AssertGender(Gender.Female);
            //var person2 = result.FindPerson("Roelof Biersma")
            //    .AssertGender(Gender.Male);
            //var person3 = result.FindPerson("Geesje Wachters")
            //    .AssertGender(Gender.Female);

            //// relationships
            //result.FindRelationship(person2, person1)
            //    .AssertParentChild(ParentChild.Person1IsBiologicalParentOfPerson2);
            //result.FindRelationship(person3, person1)
            //    .AssertParentChild(ParentChild.Person1IsBiologicalParentOfPerson2);
        }
    }
}