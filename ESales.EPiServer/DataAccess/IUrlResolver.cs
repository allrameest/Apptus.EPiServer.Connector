using Mediachase.Commerce.Catalog.Dto;

namespace Apptus.ESales.EPiServer.DataAccess
{
    internal interface IUrlResolver
    {
        string GetEntryUrl( CatalogEntryDto.CatalogEntryRow entry, string language );
    }
}
