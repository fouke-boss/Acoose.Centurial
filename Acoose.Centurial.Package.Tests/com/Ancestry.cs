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
        public void Page1()
        {
            // execute
            var result = Utility.ExecuteTest<Package.com.Ancestry>("https://search.ancestry.com/cgi-bin/sse.dll?hsrc=aDj4&_phstart=successSource&usePUBJs=true&indiv=1&dbid=2957&gsln=vierus&gsfn_x=1&gsln_x=1&cp=0&new=1&rank=1&uidh=t29&redir=false&msT=1&gss=angs-d&pcat=34&fh=32&h=46049932&recoff=&ml_rpos=33&queryId=f86604d1d417a0684ae63f9ca848fb3c");

            // provenance
            result.FindProvenance<UnknownRepository>(1)
                .AssertChild<Photograph>()
                .AssertCondition(x => x.Creator is PersonalName p && p.FamilyName == "S." && p.GivenNames == "Susan M.")
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
        public void DiscoveryUI_George_Johnson()
        {
            // execute
            var result = Utility.ExecuteTest<Package.com.Ancestry>("https://www.ancestry.com/discoveryui-content/view/36407081:61596");
        }
    }
}