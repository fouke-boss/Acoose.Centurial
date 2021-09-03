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
    public class BHIC
    {
        [TestMethod]
        public void BS_Huwelijk()
        {
            // test
            var result = Utility.ExecuteTest<Package.nl.BHIC>("https://www.bhic.nl/memorix/genealogy/search/deeds/09634674-52e0-4449-6355-3613d1bbafed?person=f31815d2-462c-11e3-a747-d206bceb4d38");

            // provenance
            result
                .AssertWebsite(0, "Brabants Historisch Informatie Centrum", "https://www.bhic.nl/", true)
                .AssertDatabaseEntry("Johannes van Summeren & Maria Hendrica van de Loo");
            result
                .AssertPublicArchive(1, "Brabants Historisch Informatie Centrum", "'s-Hertogenbosch")
                .AssertArchivedItem("50")
                .AssertCollection(null)
                .AssertArchivedItem("1084")
                .AssertVitalRecord("Boxtel", "Burgerlijke stand")
                .AssertRecordScriptFormat("Huwelijksregister Boxtel 1812", null, "22", "Johannes van Summeren & Maria Hendrica van de Loo", "1812-02-02");

            // persons
            var person1 = result.FindPerson("Johannes van Summeren")
                .AssertPlace("Birth", "Boxtel");
            var person2 = result.FindPerson("Maria Hendrica van de Loo")
                .AssertPlace("Birth", "Boxtel");
            var person3 = result.FindPerson("Martinus van Summeren");
            var person4 = result.FindPerson("Johanna van Dinther");
            var person5 = result.FindPerson("Hendricus van de Loo");
            var person6 = result.FindPerson("Megchelina Schellen");

            // relationships
            result.FindPartnership(person1, person2)
                .AssertEvent("CivilMarriage", "1812-02-02", "Boxtel");
            result.FindParentChild(person3, person1);
            result.FindParentChild(person4, person1);
            result.FindParentChild(person5, person2);
            result.FindParentChild(person6, person2);
        }
        [TestMethod]
        public void DTP_Doop()
        {
            // test
            var result = Utility.ExecuteTest<Package.nl.BHIC>("https://www.bhic.nl/memorix/genealogy/search/deeds/00e7025f-3631-148e-35db-4d54c468c359?person=9e0f47eb-003f-6c8c-a9ba-cf56aeacb82a");

            // provenance
            result
                .AssertWebsite(0, "Brabants Historisch Informatie Centrum", "https://www.bhic.nl/", true)
                .AssertDatabaseEntry("Maria Joanna Colvenbach");
            result
                .AssertPublicArchive(1, "Brabants Historisch Informatie Centrum", "'s-Hertogenbosch")
                .AssertArchivedItem("1437")
                .AssertCollection(null)
                .AssertArchivedItem("36")
                .AssertChurchRecord("Rooms-Katholiek", "Velp")
                .AssertRecordScriptFormat("Velp RK doopboek 1795-1810", "6", null, "Maria Joanna Colvenbach", "1798-04-19");

            // persons
            var person1 = result.FindPerson("Maria Joanna Colvenbach")
                .AssertEvent("Baptism", "1798-04-19", "Velp");
            var person2 = result.FindPerson("Henrici Colvenbach");
            var person3 = result.FindPerson("Joannae Arts");

            // relationships
            result.FindParentChild(person2, person1);
            result.FindParentChild(person3, person1);
        }
    }
}