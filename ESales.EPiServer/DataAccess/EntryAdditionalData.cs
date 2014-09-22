using System.Collections.Generic;
using System.Linq;
using Mediachase.Commerce.Catalog.Dto;

namespace Apptus.ESales.EPiServer.DataAccess
{
    internal class EntryAdditionalData : IEntryAdditionalData
    {
        public IEnumerable<CatalogEntryDto.CatalogItemSeoRow> GetCatalogItemSeoRows( CatalogEntryDto.CatalogEntryRow entry )
        {
            return entry.GetCatalogItemSeoRows() ?? Enumerable.Empty<CatalogEntryDto.CatalogItemSeoRow>();
        }

        public IEnumerable<CatalogEntryDto.CatalogItemAssetRow> GetCatalogItemAssetRows( CatalogEntryDto.CatalogEntryRow entry )
        {
            return entry.GetCatalogItemAssetRows() ?? Enumerable.Empty<CatalogEntryDto.CatalogItemAssetRow>();
        }
    }
}