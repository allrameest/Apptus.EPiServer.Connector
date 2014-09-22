using Mediachase.Commerce.Catalog.Dto;

namespace Apptus.ESales.EPiServer.DataAccess
{
    internal interface IKeyLookup
    {
        string Value( int id, string language );
        string Value( CatalogEntryDto.CatalogEntryRow entry, string language );
    }
}