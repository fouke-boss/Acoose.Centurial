using Acoose.Genealogy.Extensibility.Data;
using Acoose.Genealogy.Extensibility.Data.References;
using Acoose.Genealogy.Extensibility.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acoose.Centurial.Package.org
{
    [Scraper("http://*.wikipedia.org/wiki/*")]
    public class Wikipedia : Scraper.Default
    {
        protected override IEnumerable<Repository> GetProvenance(Context context)
        {
            // init
            var subject = context.Html.SelectNodes("//h1[@id='firstHeading']")
                .FirstOrDefault()?.InnerText;

            // return provenance
            yield return context.GetWebsite(() => new WikiEntry()
            {
                Subject = HtmlAgilityPack.HtmlEntity.DeEntitize(subject)
            });
        }
    }
}