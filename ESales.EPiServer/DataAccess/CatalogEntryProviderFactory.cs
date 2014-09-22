using Apptus.ESales.EPiServer.Import.Products;

namespace Apptus.ESales.EPiServer.DataAccess
{
    internal static class CatalogEntryProviderFactory
    {
        public static ICatalogEntryProvider Create( bool incremental, int catalogId, ESalesVariantHelper eSalesVariantHelper, ICatalogSystemMapper catalogSystemMapper,
                                             IIndexSystemMapper indexSystemMapper )

        {
            if ( incremental )
            {
                return new IncrementalCatalogEntryProvider( catalogId, eSalesVariantHelper, catalogSystemMapper, indexSystemMapper );
            }
            return new FullCatalogEntryProvider( catalogId, catalogSystemMapper );
        }
    }
}