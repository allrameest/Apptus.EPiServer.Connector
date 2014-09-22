using System.Collections.Generic;
using Mediachase.Commerce.Catalog.Dto;

namespace Apptus.ESales.EPiServer.DataAccess
{
    internal interface IEntryAdditionalData
    {
        IEnumerable<CatalogEntryDto.CatalogItemSeoRow> GetCatalogItemSeoRows( CatalogEntryDto.CatalogEntryRow entry );
        IEnumerable<CatalogEntryDto.CatalogItemAssetRow> GetCatalogItemAssetRows( CatalogEntryDto.CatalogEntryRow entry );
    }
}
