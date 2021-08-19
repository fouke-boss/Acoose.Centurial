﻿using Acoose.Genealogy.Extensibility.Data;
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
        public void BS_Overlijden()
        {
            // draai test
            var result = ScraperTest.ExecuteFromEmbeddedResource<Package.nl.AlleFriezen>("https://allefriezen.nl/zoeken/deeds/bbe4ab39-9950-99ba-f0ca-477324d15383", "Acoose.Centurial.Package.Tests.nl.AlleFriezen - BS Overlijden.html");

            // valideren
            Assert.IsTrue(result.Source.Info.Length == 5);
            Assert.IsTrue(result.Source.Info.OfType<PersonInfo>().Count() == 3);
        }
    }
}