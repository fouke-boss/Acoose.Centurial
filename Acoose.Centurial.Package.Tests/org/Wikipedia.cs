using Acoose.Genealogy.Extensibility.Data.References;
using Acoose.Genealogy.Extensibility.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acoose.Centurial.Package.Tests.org
{
    [TestClass]
    public class Wikipedia
    {
        [TestMethod]
        public void Fritz_Walter()
        {
            // init ([Bearbeiten] in header)
            var result = Utility.ExecuteTest<Package.org.Wikipedia>("https://de.wikipedia.org/wiki/Fritz_Walter");

            // assert
            Assert.AreEqual("Fritz Walter", result.Source.Provenance.OfType<Website>().Single().Items.Select(x => x.Item).OfType<WikiEntry>().Single().Subject);
        }
    }
}