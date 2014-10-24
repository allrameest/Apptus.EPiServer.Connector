using System.Collections.Generic;
using Apptus.ESales.EPiServer.Import.Products;

namespace Apptus.ESales.EPiServer.DataAccess
{
    internal class CatalogEntryProviderFactory
    {
        private readonly ICatalogSystemMapper _catalogSystemMapper;
        private readonly IIndexSystemMapper _indexSystemMapper;
        private readonly IEnumerable<IModifiedCatalogEntryLoader> _modifiedCatalogEntryLoaders;

        public CatalogEntryProviderFactory(ICatalogSystemMapper catalogSystemMapper, IIndexSystemMapper indexSystemMapper, IEnumerable<IModifiedCatalogEntryLoader> modifiedCatalogEntryLoaders)
        {
            _catalogSystemMapper = catalogSystemMapper;
            _indexSystemMapper = indexSystemMapper;
            _modifiedCatalogEntryLoaders = modifiedCatalogEntryLoaders;
        }

        public ICatalogEntryProvider Create(bool incremental, int catalogId, ESalesVariantHelper eSalesVariantHelper)
        {
            if (incremental)
            {
                return new IncrementalCatalogEntryProvider(catalogId, eSalesVariantHelper, _catalogSystemMapper, _indexSystemMapper, _modifiedCatalogEntryLoaders);
            }
            return new FullCatalogEntryProvider(catalogId, _catalogSystemMapper);
        }
    }
}