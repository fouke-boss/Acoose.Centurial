using Acoose.Genealogy.Extensibility.Data;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acoose.Centurial.Package
{
    public class Event
    {
        public EventType Type
        {
            get; set;
        }
        public Date Date
        {
            get; set;
        }
        public string Place
        {
            get; set;
        }

        public Person[] Persons
        {
            get; set;
        }


    }
}