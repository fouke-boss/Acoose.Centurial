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
    public class OnlineBegraafplaatsen : Scraper.Default
    {
        public Memorial Data
        {
            get;
            private set;
        }

        public override IEnumerable<Activity> GetActivities(Context context)
        {
            // scan
            this.Data = this.Scan(context);

            // done
            var activities = this.Data.Images
                .NullCoalesce()
                .Select(x => new Activity.DownloadFileActivity(x))
                .Cast<Activity>()
                .ToList();
            if (activities.Count == 0)
            {
                activities.AddRange(base.GetActivities(context));
            }

            // done
            return activities;
        }
        private Memorial Scan(Context context)
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

            // memorial
            return new Memorial()
            {
                URL = context.Url,
                WebsiteTitle = "Online Begraafplaatsen",
                WebsiteURL = context.GetWebsiteUrl(),
                CemeteryName = cemetery[0],
                CemeteryAccess = cemetery[1],
                CemeteryPlace = cemetery[2],
                Persons = persons,
                Images = zerk
                    .Elements("img").WithClass("zrk_img")
                    .Select(x => $"https://www.online-begraafplaatsen.nl/{x.Attribute("src").TrimStart('/')}")
                    .ToArray()
            };
        }

        protected override IEnumerable<Repository> GetProvenance(Context context)
        {
            return this.Data.GenerateProvenance();
        }
        protected override IEnumerable<Info> GetInfo(Context context)
        {
            return this.Data.GenerateInfos();
        }

        private string TrimImagePath(string path)
        {
            // empty?
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            // init
            var parts = path.Split('?').First().Split('/', '\\')
                .ToList();

            // remove photo folder
            if (parts.Count > 4 && parts[4] == "photos")
            {
                if (parts[3].StartsWith("photo"))
                {
                    parts.RemoveAt(3);
                }
                else
                {
                    throw new NotSupportedException($"Unsupported image path '{path}'.");
                }
            }

            // done
            return string.Join("/", parts);
        }
    }
}