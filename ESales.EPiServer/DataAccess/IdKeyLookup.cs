using System.Globalization;
using Apptus.ESales.EPiServer.Util;
using Mediachase.Commerce.Catalog.Dto;

namespace Apptus.ESales.EPiServer.DataAccess
{
    internal class IdKeyLookup : IKeyLookup
    {
        public string Value( int id, string language )
        {
            return AttributeHelper.CreateKey( id.ToString( CultureInfo.InvariantCulture ), language );
        }

        public string Value( CatalogEntryDto.CatalogEntryRow entry, string language )
        {
            return AttributeHelper.CreateKey( entry.CatalogEntryId.ToString( CultureInfo.InvariantCulture ), language );
        }
    }
}