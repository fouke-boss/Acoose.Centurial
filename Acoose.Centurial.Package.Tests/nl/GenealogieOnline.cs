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
    public class GenealogieOnline
    {
        [TestMethod]
        public void CollectionPage()
        {
            var result = ScraperTest.ExecuteFromWeb<Package.nl.GenealogieOnline>("https://www.genealogieonline.nl/genealogie-coret/");
        }
        [TestMethod]
        public void PersonPage()
        {
            var result = ScraperTest.ExecuteFromWeb<Package.nl.GenealogieOnline>("https://www.genealogieonline.nl/genealogie-coret/I002022.php");
        }
    }
}
