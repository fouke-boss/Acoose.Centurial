using Acoose.Genealogy.Extensibility.Data.References;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acoose.Centurial.Package
{
    public abstract class RecordType
    {
        public static readonly RecordType BurgerlijkeStand = new RecordType<VitalRecord>(x => x.Title = x.Title ?? "Burgerlijke stand".ToGenericTitle(false));
        public static readonly RecordType DoopTrouwBegraaf = new RecordType<ChurchRecord>(x => x.Church = x.Church ?? "Kerk");
        public static readonly RecordType Bevolkingsregister = new RecordType<VitalRecord>(x => x.Title = x.Title ?? "Bevolkingsregister".ToGenericTitle(false));
        public static readonly RecordType Census = new RecordType<Census>(null);
        public static readonly RecordType Standesämter = new RecordType<VitalRecord>(x => x.Title = x.Title ?? "Standesämter".ToGenericTitle(false));
        public static readonly RecordType ChurchParish = new RecordType<ChurchRecord>(x => x.Church = x.Church ?? "Church Parish");
        public static readonly RecordType Kirchenbuch = new RecordType<ChurchRecord>(x => x.Church = x.Church ?? "Kirche");
        public static readonly RecordType Cemetery = new RecordType<CemeteryRecord>(null);

        public static RecordType TryParse(string value)
        {
            // null
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            // init
            value = value.ToLower();

            // try to parse
            if (value.StartsWith("bs ") || value.Contains("burgerlijke stand"))
            {
                return RecordType.BurgerlijkeStand;
            }
            if (value.Contains("standesamt") || value.Contains("standesämt") || value.Contains("personenstandsregister"))
            {
                return RecordType.Standesämter;
            }
            else if (value.Contains("bevolkingsregister"))
            {
                return RecordType.Bevolkingsregister;
            }
            else if (value.Contains("volkstelling") || value.Contains("census") || value.Contains("volkszählung"))
            {
                return RecordType.Census;
            }
            else if (value.StartsWith("dtb ") ||
                (value.Contains("doop") && value.Contains("trouw") && value.Contains("begra"))
            )
            {
                return RecordType.DoopTrouwBegraaf;
            }
            else if (value.Contains("church") ||
                value.Contains("parish") ||
                value.Contains("presbyt") ||
                value.Contains("lutheran") ||
                value.Contains("catholic"))
            {
                return RecordType.ChurchParish;
            }
            else if (value.Contains("kirch"))
            {
                return RecordType.Kirchenbuch;
            }
            else if (value.Contains("begraafplaats") || value.Contains("cemetery"))
            {
                return RecordType.Cemetery;
            }
            else
            {
                return null;
            }
        }

        private protected RecordType()
        {
        }

        public abstract Source Generate(RecordScraper record);
    }
    public sealed class RecordType<T> : RecordType
        where T : Source, new()
    {
        private readonly Action<T> _Action;

        internal RecordType(Action<T> action)
        {
            // init
            this._Action = action;
        }

        public override Source Generate(RecordScraper record)
        {
            // init
            var result = new T();

            // type
            switch (result)
            {
                case VitalRecord v:
                    v.Jurisdiction = record.RecordPlace;
                    v.Title = record.Title.ToGenericTitle(true);
                    v.Items = record.GenerateRecordScriptFormat();
                    v.Creator = record.Organization;
                    break;
                case ChurchRecord c1:
                    c1.Church = record.Organization;
                    c1.Place = record.EventPlace ?? record.RecordPlace;
                    c1.Title = record.Title.ToGenericTitle(true);
                    c1.Items = record.GenerateRecordScriptFormat();
                    break;
                case Census c2:
                    c2.Jurisdiction = record.RecordPlace;
                    c2.CensusId = record.CensusID;
                    c2.Title = record.Title;
                    c2.Items = record.GenerateCensusScriptFormt();
                    break;
                case CemeteryRecord c3:
                    c3.Cemetery = record.Organization;
                    c3.Place = record.ArchivePlace;
                    c3.Title = record.Title.ToGenericTitle(true);
                    c3.Items = record.GenerateRecordScriptFormat();
                    break;
                default:
                    throw new NotImplementedException();
            }

            // action
            this._Action?.Invoke(result);

            // is the reference complete (are all requried fields completed)?
            var isComplete = false;
            switch (result)
            {
                case VitalRecord v:
                    isComplete = (!string.IsNullOrWhiteSpace(v.Jurisdiction) && v.Title != null);
                    break;
                case ChurchRecord c1:
                    isComplete = (!string.IsNullOrWhiteSpace(c1.Church) && !string.IsNullOrWhiteSpace(c1.Place));
                    break;
                case Census c2:
                    isComplete = (!string.IsNullOrWhiteSpace(c2.Jurisdiction) && !string.IsNullOrWhiteSpace(c2.CensusId));
                    break;
                case CemeteryRecord c3:
                    isComplete = (!string.IsNullOrWhiteSpace(c3.Cemetery) && !string.IsNullOrWhiteSpace(c3.Place));
                    break;
                default:
                    throw new NotImplementedException();
            }

            // done
            return (isComplete ? result : default);
        }
    }
}