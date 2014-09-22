using System.Collections.Generic;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Pricing;

namespace Apptus.ESales.EPiServer.DataAccess
{
    internal interface IPriceServiceMapper
    {
        IEnumerable<IPriceValue> GetCatalogEntryPrices(CatalogKey catalogKey);
    }
}