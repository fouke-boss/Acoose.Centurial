using Acoose.Genealogy.Extensibility.Data;
using Acoose.Genealogy.Extensibility.Data.References;
using Acoose.Genealogy.Extensibility.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acoose.Centurial.Package.nl
{
    [Scraper("https://www.online-begraafplaatsen.nl/graf/*")]
    [Scraper("https://www.online-begraafplaatsen.nl/zerken.asp")]
    public class OnlineBegraafplaatsen : MemorialScraper
    {
        public OnlineBegraafplaatsen()
            : base("Online Begraafplaatsen")
        {
        }

        protected override void Scan(Context context)
        {
            // init
            var zerk = context.Body()
                .Descendants("div").WithClass("zrk")
                .Single();
            var cemetery = zerk
                .Descendants("div").WithClass("tabrow")
                .Select(row =>
                {
                    // init
                    var cells = row.Descendants("div").WithClass("tabcell")
                        .Select(x => x.GetInnerText())
                        .ToArray();

                    // any?
                    return (cells.Length == 2 && cells[0] == "Begraafplaats" ? cells[1] : default);
                })
                .Where(x => x != null)
                .Single().Split(',')
                .Select(x => x.Trim())
                .ToArray();
            var persons = zerk
                .Descendants("table")
                .Descendants("tr")
                .Skip(1) // headers
                .Select(row =>
                {
                    // init
                    var cells = row
                    .Elements("td")
                    .Select(x => x.GetInnerText())
                    .ToArray();

                    // done
                    return new Person()
                    {
                        Name = cells[0],
                        BirthDate = Date.TryParse(cells[1]),
                        DeathDate = Date.TryParse(cells[2]),
                        Age = Utility.TryParseAge(cells[3])
                    };
                })
                .ToArray();

            // done
            this.CemeteryName = cemetery[0];
            this.CemeteryAccess = cemetery[1];
            this.CemeteryPlace = cemetery[2];
            this.Persons = persons;
            this.Images = zerk
                .Elements("img").WithClass("zrk_img")
                .Select(x => $"https://www.online-begraafplaatsen.nl/{x.Attribute("src").TrimStart('/')}")
                .ToArray();
        }
    }
}