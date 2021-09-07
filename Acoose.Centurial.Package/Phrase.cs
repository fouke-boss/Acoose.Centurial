using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acoose.Centurial.Package
{
    public class Phrase : IEnumerable<Phrase.Part>
    {
        private List<Part> _Items;

        public static Phrase Parse(string value)
        {
            return new Phrase(ParseParts(value));
        }
        private static IEnumerable<Part> ParseParts(string value)
        {
            // null?
            if (!string.IsNullOrWhiteSpace(value))
            {
                // init
                var index = 0;
                while (index < value.Length)
                {
                    // init
                    var start = index;

                    // find opening
                    while (index < value.Length && !"([{".Contains(value[index]))
                    {
                        index++;
                    }

                    // yield
                    if (index > start)
                    {
                        yield return new Part(value.Substring(start, index - start), null);
                    }

                    // init
                    start = index;

                    // find closing
                    while (index < value.Length && !")]}".Contains(value[index]))
                    {
                        index++;
                    }

                    // yield
                    if (index > start)
                    {
                        yield return new Part(value.Substring(start + 1, index - start - 1), value[start]);
                    }

                    // done
                    index++;
                }
            }
        }

        private Phrase(IEnumerable<Part> parts)
        {
            // init
            this._Items = parts.ToList();
        }

        public string IgnoreBrackets()
        {
            return this.Format(this.Where(x => x.OpeningBracket == null));
        }
        public string BracketsOnly()
        {
            return this.Format(this.Where(x => x.OpeningBracket.HasValue));
        }

        public string PreferWithBrackets()
        {
            // init
            var preferred = this
                .Where(x => x.OpeningBracket.HasValue);

            // done
            return (preferred.Any() ? this.Format(preferred) : this.Format(this));
        }
        public string PreferFirstWithinBrackets()
        {
            // init
            var preferred = this
                .Where(x => x.OpeningBracket.HasValue);

            // done
            return (preferred.Any() ? this.Format(preferred.Take(1)) : this.Format(this));
        }
        public string PreferWithoutBrackets()
        {
            // init
            var preferred = this
                .Where(x => x.OpeningBracket == null);

            // done
            return (preferred.Any() ? this.Format(preferred) : this.Format(this));
        }
        private string Format(IEnumerable<Part> parts)
        {
            return string.Join("", parts.Select(x => x.Text.TrimEnd()));
        }

        public IEnumerator<Part> GetEnumerator()
        {
            return this._Items.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public class Part
        {
            internal Part(string text, char? bracket)
            {
                // init
                this.Text = text;
                this.OpeningBracket = bracket;
            }

            public string Text
            {
                get; private set;
            }
            public char? OpeningBracket
            {
                get; private set;
            }
            public char? ClosingBracket
            {
                get
                {
                    // null?
                    if (this.OpeningBracket == '[')
                    {
                        return ']';
                    }
                    else if (this.OpeningBracket == '(')
                    {
                        return ')';
                    }
                    else if (this.OpeningBracket == '{')
                    {
                        return '}';
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            public override string ToString()
            {
                // init
                return $"{this.OpeningBracket}{this.Text}{this.ClosingBracket}";
            }
        }
    }
}