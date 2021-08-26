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
        public void BS_Geboorte()
        {
            // test
            var result = Utility.ExecuteTest<Package.nl.RegionaalArchiefTilburg>("https://www.regionaalarchieftilburg.nl/zoek-een-persoon/deeds/48a02c0d-25d7-a080-2527-5b51dea12b2a?person=afe01680-5b54-8fb4-07f7-207e5589a166");

            // provenance
            result.FindProvenance<PublicArchive>(1)
                .AssertCondition(x => x.Name == "Regionaal Archief Tilburg")
                .AssertChild<ArchivedItem>()
                .AssertChild<Collection>()
                .AssertChild<ArchivedItem>()
                .AssertChild<VitalRecord>()
                .AssertCondition(x => x.Jurisdiction == "Baarle-Nassau")
                .AssertCondition(x => x.Title.Value == "Burgerlijke stand")
                .AssertChild<RecordScriptFormat>()
                .AssertCondition(x => x.Number == "23")
                .AssertCondition(x => x.Label == "Geboorteregister 1920")
                .AssertCondition(x => x.Date.Equals(Date.TryParse("27-04-1920")))
                .AssertCondition(x => x.ItemOfInterest == "Louisa Theodora Francisca van den Heuvel");
        }

        [TestMethod]
        public void DTP_Huwelijk()
        {
            // test
            var result = Utility.ExecuteTest<Package.nl.RegionaalArchiefTilburg>("https://www.regionaalarchieftilburg.nl/zoek-een-persoon/deeds/14014cda-7f60-f1fd-f07d-df296dc44569?person=b1add30f-554a-0cec-501f-e24d5c5f1e9c");

            // provenance
            result.FindProvenance<Website>(0)
                .AssertCondition(x => x.Title == "Regionaal Archief Tilburg")
                .AssertCondition(x => x.IsVirtualArchive == true)
                .AssertChild<OnlineItem>()
                .AssertChild<DatabaseEntry>()
                .AssertCondition(x => x.EntryFor == "Anthonij Hessels & Anna Maria Struijk");
            result.FindProvenance<PublicArchive>(1)
                .AssertCondition(x => x.Name == "Regionaal Archief Tilburg")
                .AssertChild<ArchivedItem>()
                .AssertChild<Collection>()
                .AssertChild<ArchivedItem>()
                .AssertChild<ChurchRecord>()
                .AssertCondition(x => x.Church == "nederduits-gereformeerde gemeente")
                .AssertCondition(x => x.Place == "Oosterhout")
                .AssertChild<RecordScriptFormat>()
                .AssertCondition(x => x.Page == "11")
                .AssertCondition(x => x.Label == "trouwboek 1780-1796")
                .AssertCondition(x => x.Date.Equals(Date.TryParse("1781-01-13")))
                .AssertCondition(x => x.ItemOfInterest == "Anthonij Hessels & Anna Maria Struijk");

            // persons
            var person1 = result.FindPerson("Anthonij Hessels")
                .AssertPlace("Birth", "Oosterhout")
                .AssertGender(Gender.Male);
            var person2 = result.FindPerson("Anna Maria Struijk")
                .AssertPlace("Birth", "Oosterhout")
                .AssertGender(Gender.Female);
        }
        [TestMethod]
        public void DTP_Doop()
        {
            // test
            var result = Utility.ExecuteTest<Package.nl.RegionaalArchiefTilburg>("https://www.regionaalarchieftilburg.nl/zoek-een-persoon/deeds/ecd59e5d-0584-08c1-cb06-175ffe990f57?person=d2a05bae-3248-29c0-4eb0-6673f4bfcb71");

            // provenance
            result.FindProvenance<PublicArchive>(1)
                .AssertCondition(x => x.Name == "Regionaal Archief Tilburg")
                .AssertChild<ArchivedItem>()
                .AssertChild<Collection>()
                .AssertChild<ArchivedItem>()
                .AssertChild<ChurchRecord>()
                .AssertCondition(x => x.Church == "Rooms-katholieke parochie ´t Goirke")
                .AssertCondition(x => x.Place == "Tilburg")
                .AssertChild<RecordScriptFormat>()
                .AssertCondition(x => x.Page == "65")
                .AssertCondition(x => x.Label == "Doopboek 1821-1825")
                .AssertCondition(x => x.Date.Equals(Date.TryParse("1 february 1825")))
                .AssertCondition(x => x.ItemOfInterest == "Maria van den Berg");
        }
    }
}