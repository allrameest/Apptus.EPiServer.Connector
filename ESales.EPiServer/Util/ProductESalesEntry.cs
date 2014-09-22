using System;
using System.Collections.Generic;
using Mediachase.Commerce.Catalog.Objects;

namespace Apptus.ESales.EPiServer.Util
{
    public class ProductESalesEntry : ESalesEntry
    {
        public ProductESalesEntry(Entry entry, double proportion, string ticket, IEnumerable<string> outline, IEnumerable<ESalesEntry> variants, Dictionary<string, string> attributes)
            : base(entry, proportion, ticket, outline, attributes)
        {
            Variants = variants;
        }

        [Obsolete]
        public ProductESalesEntry(Entry entry, double proportion, string ticket, IEnumerable<string> outline, IEnumerable<ESalesEntry> variants) : base(entry, proportion, ticket, outline)
        {
            Variants = variants;
        }

        public IEnumerable<ESalesEntry> Variants { get; private set; }
    }
}