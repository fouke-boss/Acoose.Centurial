﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Acoose.Genealogy.Extensibility.Data;
using Acoose.Genealogy.Extensibility.Data.References;
using Acoose.Genealogy.Extensibility.Web;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

namespace Acoose.Centurial.Package.nl
{
    [Scraper("https://www.bhic.nl/memorix/genealogy/search/deeds/*")]
    public partial class BHIC : Picturae
    {
        public BHIC()
            : base("Brabants Historisch Informatie Centrum", "https://www.bhic.nl/")
        {
        }

        protected override void Customize(HtmlNode container, Dictionary<string, string> fields)
        {
            // archief
            this.ArchiveName = "Brabants Historisch Informatie Centrum";
            this.ArchivePlace = "'s-Hertogenbosch";
        }
    }
}