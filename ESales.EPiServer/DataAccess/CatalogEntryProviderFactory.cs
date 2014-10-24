using Apptus.ESales.EPiServer.Import.Products;

namespace Apptus.ESales.EPiServer.DataAccess
{
    internal class CatalogEntryProviderFactory
    {
        private readonly ICatalogSystemMapper _catalogSystemMapper;
        private readonly IIndexSystemMapper _indexSystemMapper;

        public CatalogEntryProviderFactory(ICatalogSystemMapper catalogSystemMapper, IIndexSystemMapper indexSystemMapper)
        {
            _catalogSystemMapper = catalogSystemMapper;
            _indexSystemMapper = indexSystemMapper;
        }

        public ICatalogEntryProvider Create(bool incremental, int catalogId, ESalesVariantHelper eSalesVariantHelper)
        {
            if (incremental)
            {
                return new IncrementalCatalogEntryProvider(catalogId, eSalesVariantHelper, _catalogSystemMapper, _indexSystemMapper);
            }
            return new FullCatalogEntryProvider(catalogId, _catalogSystemMapper);
        }
    }
}