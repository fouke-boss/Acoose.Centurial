using Acoose.Genealogy.Extensibility.Data;
using Acoose.Genealogy.Extensibility.Data.References;
using Acoose.Genealogy.Extensibility.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Acoose.Centurial.Package.nl
{
    [Scraper("https://www.genealogieonline.nl/*")]
    public class GenealogieOnline : Scraper.Default
    {
        protected override IEnumerable<Repository> GetProvenance(Context context)
        {
            // author
            var author = ParseAuthor(context.GetMetaTag("author"));

            // collection url
            var collectionUrl = new Uri(context.Url).AbsolutePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

            // collection name
            var collectionName = context.Html.SelectSingleNode("//div[contains(@class, 'panel-body')]/a")?.InnerText;
            if (string.IsNullOrWhiteSpace(collectionName))
            {
                collectionName = context.Html.SelectSingleNode("//p[starts-with(., 'De publicatie') and contains(., 'is samengesteld door')]//b")?.InnerText;
            }

            // page title
            var pageTitle = context.GetPageTitle().Split('»').FirstOrDefault()?.Trim();

            // layer 1: website > collection > 
            yield return new Website()
            {
                Title = "Genealogie Online",
                IsVirtualArchive = true,
                Url = "https://www.genealogieonline.nl/",
                Items = new OnlineItem[]
                {
                    new OnlineItem()
                    {
                        Url = string.Format("https://www.genealogieonline.nl/{0}/", collectionUrl),
                        Item = new OnlineCollection()
                        {
                            Creator = new Name[]
                            {
                                author
                            },
                            Title = collectionName,
                            Items = new OnlineItem[]
                            {
                                new OnlineItem()
                                {
                                    Accessed = Date.Today,
                                    Url = context.Url,
                                    Item = new WebPage()
                                    {
                                        Title = new GenericTitle(){ Value = pageTitle, Literal = true }
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }
        private static PersonalName ParseAuthor(string author)
        {
            // parse name
            Acoose.Genealogy.Extensibility.ParsingUtility.ParseName(author, out string familyName, out string givenNames, out string particles);

            // done
            return new PersonalName()
            {
                FamilyName = familyName,
                GivenNames = givenNames,
                Particles = particles
            };
        }
    }
}