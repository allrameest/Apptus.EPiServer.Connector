using System.Collections.Generic;
using Mediachase.Commerce.Catalog.Dto;

namespace Apptus.ESales.EPiServer.Import.Products
{
    internal interface ICatalogEntryProvider
    {
        int Count { get; }
        IEnumerable<CatalogEntryDto.CatalogEntryRow> GetCatalogEntries();
    }
}