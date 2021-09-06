using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acoose.Centurial.Package.com
{
    partial class Ancestry
    {
        private class PropertyName
        {
            public static readonly PropertyName Age = new PropertyName("age", "alter");
            public static readonly PropertyName BaptismDate = new PropertyName("baptism date", "taufdatum");
            public static readonly PropertyName BaptismPlace = new PropertyName("baptism place", "taufort");
            public static readonly PropertyName BirthDate = new PropertyName("birth date", "geburtsdatum");
            public static readonly PropertyName BirthPlace = new PropertyName("birth place", "geburtsort");
            public static readonly PropertyName BurialDate = new PropertyName("burial date", "bestattungsdatum");
            public static readonly PropertyName BurialPlace = new PropertyName("burial place", "bestattungsort");
            public static readonly PropertyName CertificateNumber = new PropertyName("certificate number", "urkunde nummer");
            public static readonly PropertyName Child = new PropertyName("child", "kind");
            public static readonly PropertyName Church = new PropertyName("church", "kirche");
            public static readonly PropertyName CivilRegistrationOffice = new PropertyName("civil registration office", "standesamt");
            public static readonly PropertyName DeathAge = new PropertyName("death age", "sterbealter");
            public static readonly PropertyName DeathDate = new PropertyName("death date", "sterbedatum");
            public static readonly PropertyName DeathPlace = new PropertyName("death place", "Sterbeort");
            public static readonly PropertyName Father = new PropertyName("father", "vater");
            public static readonly PropertyName Gender = new PropertyName("gender", "geschlecht");
            public static readonly PropertyName MaidenName = new PropertyName("maiden name", "mädchenname");
            public static readonly PropertyName MarriageAge = new PropertyName("marriage age", "alter zur zeit der heirat");
            public static readonly PropertyName MarriageDate = new PropertyName("marriage date", "heiratsdatum");
            public static readonly PropertyName MarriagePlace = new PropertyName("marriage place", "heiratsort");
            public static readonly PropertyName Mother = new PropertyName("mother", "mutter");
            public static readonly PropertyName Name = new PropertyName("name", "name");
            public static readonly PropertyName PageNumber = new PropertyName("page number", "seitennummer");
            public static readonly PropertyName ParishAsItAppears = new PropertyName("parish as it appears", "kirchgemeinde wie angezeigt");
            public static readonly PropertyName SourceCitation = new PropertyName("source citation", "quellenangabe");
            public static readonly PropertyName SourceInformationInSearch = new PropertyName("source information", "quelleninformationen");
            public static readonly PropertyName SourceInformationInDiscoveryUI = new PropertyName("source information", "angaben zur quelle");
            public static readonly PropertyName Spouse = new PropertyName("spouse", "ehepartner");

            private PropertyName(string english, string deutsch)
            {
                // init
                this.English = english;
                this.Deutsch = deutsch;
            }

            public string English
            {
                get;
            }
            public string Deutsch
            {
                get;
            }

            public string this[Language language]
            {
                get
                {
                    switch (language)
                    {
                        case Language.Deutsch:
                            return this.Deutsch;
                        default:
                            return this.English;
                    }
                }
            }
        }
    }
}