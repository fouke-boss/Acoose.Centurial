using Acoose.Genealogy.Extensibility.Data;
using Acoose.Genealogy.Extensibility.Data.References;
using Acoose.Genealogy.Extensibility.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acoose.Centurial.Package.Tests
{
    [TestClass]
    public class UtilityTest
    {
        [TestMethod]
        public void Phrase1()
        {
            // init
            var result = Phrase.Parse("Fouke Boss");

            // done
            Assert.IsTrue(result.Count() == 1);
            Assert.IsTrue(result.ElementAt(0).Text == "Fouke Boss");
            Assert.IsTrue(result.ElementAt(0).OpeningBracket == null);
        }
        [TestMethod]
        public void Phrase2()
        {
            // init
            var result = Phrase.Parse("(Fouke Boss)");

            // done
            Assert.IsTrue(result.Count() == 1);
            Assert.IsTrue(result.ElementAt(0).Text == "Fouke Boss");
            Assert.IsTrue(result.ElementAt(0).OpeningBracket == '(');
        }
        [TestMethod]
        public void Phrase3()
        {
            // init
            var result = Phrase.Parse("Fouke Boss (Male)").ToArray();

            // done
            Assert.IsTrue(result.Count() == 2);
            Assert.IsTrue(result.ElementAt(0).Text == "Fouke Boss ");
            Assert.IsTrue(result.ElementAt(0).OpeningBracket == null);
            Assert.IsTrue(result.ElementAt(1).Text == "Male");
            Assert.IsTrue(result.ElementAt(1).OpeningBracket == '(');
        }
        [TestMethod]
        public void Phrase4()
        {
            // init
            var result = Phrase.Parse("Fouke (Male) Boss");

            // done
            Assert.IsTrue(result.Count() == 3);
            Assert.IsTrue(result.ElementAt(0).Text == "Fouke ");
            Assert.IsTrue(result.ElementAt(0).OpeningBracket == null);
            Assert.IsTrue(result.ElementAt(1).Text == "Male");
            Assert.IsTrue(result.ElementAt(1).OpeningBracket == '(');
            Assert.IsTrue(result.ElementAt(2).Text == " Boss");
            Assert.IsTrue(result.ElementAt(2).OpeningBracket == null);
            Assert.IsTrue(result.IgnoreBrackets() == "Fouke Boss");
        }
    }
}