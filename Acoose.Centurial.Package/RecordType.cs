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
        public static readonly RecordType Bevolkingsregister = new RecordType<Census>(x => x.CensusId = "Bevolkingsregister");

        public static RecordType TryParse(string value)
        {
            // init
            value = value.ToLower();

            // try to parse
            if (value.StartsWith("bs ") || value.Contains("burgerlijke stand"))
            {
                return RecordType.BurgerlijkeStand;
            }
            else if (value.Contains("bevolkingsregister"))
            {
                return RecordType.Bevolkingsregister;
            }
            else if (value.StartsWith("dtb ") ||
                (value.Contains("doop") && value.Contains("trouw") && value.Contains("begra"))
            )
            {
                return RecordType.DoopTrouwBegraaf;
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
                    break;
                case ChurchRecord c1:
                    c1.Church = record.Organization;
                    c1.Place = record.RecordPlace ?? record.EventPlace;
                    c1.Title = record.Title.ToGenericTitle(true);
                    c1.Items = record.GenerateRecordScriptFormat();
                    break;
                case Census c2:
                    c2.Jurisdiction = record.RecordPlace;
                    c2.Title = record.Title;
                    c2.Items = record.GenerateCensusScriptFormt();
                    break;
                default:
                    throw new NotImplementedException();
            }

            // action
            this._Action?.Invoke(result);

            // done
            return result;
        }
    }
}