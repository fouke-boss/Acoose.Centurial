using Acoose.Genealogy.Extensibility.Data;
using Acoose.Genealogy.Extensibility.Data.References;
using Acoose.Genealogy.Extensibility.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acoose.Centurial.Package.net
{
    [Scraper("https://www.centurial.net/")]
    public class Centurial : Scraper.Default
    {
        public string TestId
        {
            get;
            private set;
        }

        public override IEnumerable<Activity> GetActivities(Context context)
        {
            // init
            this.TestId = new Uri(context.Url).Query.Split('=').LastOrDefault();

            // error (for testing purposes)
            if (this.TestId == "1")
            {
                throw new NotFiniteNumberException("TestError1");
            }

            // done
            return base.GetActivities(context);
        }
        public override Genealogy.Extensibility.Data.Source GetSource(Context context, Activity[] activities)
        {
            // errors (for testing purposes)
            if (this.TestId == "2")
            {
                throw new ArgumentException("TestError2");
            }
            else if(this.TestId == "3")
            {
                var files = System.IO.Directory.GetFiles(@"C:\");
            }

            // dome
            return base.GetSource(context, activities);
        }
        protected override IEnumerable<Repository> GetProvenance(Context context)
        {
            // init
            var result = base.GetProvenance(context).OfType<Website>().Single();
            result.Creator = new Name[] { new InstitutionalName() { Name = "Acoose.NET" } };

            // done
            yield return result;
        }
    }
}