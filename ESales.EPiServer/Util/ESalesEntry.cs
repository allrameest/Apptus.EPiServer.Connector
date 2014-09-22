using System;
using System.Collections.Generic;
using System.Linq;
using Mediachase.Commerce.Catalog.Objects;

namespace Apptus.ESales.EPiServer.Util
{
    public class ESalesEntry
    {
        public ESalesEntry() {}

        [Obsolete]
        public ESalesEntry(Entry entry, double proportion, string ticket, IEnumerable<string> outline) :
            this(entry, proportion, ticket, outline, new Dictionary<string, string>())
        {
        }

        public ESalesEntry(Entry entry, double proportion, string ticket, IEnumerable<string> outline, Dictionary<string, string> attributes)
        {
            Entry = entry;
            Proportion = proportion;
            Ticket = ticket;
            Outline = outline.ToArray();
            Attributes = attributes;
        }

        public Entry Entry { get; private set; }
        public double Proportion { get; private set; }
        public string Ticket { get; private set; }
        public string[] Outline{ get; private set; }
        public Dictionary<string, string> Attributes { get; private set; }
    }
}