using System.Collections.Generic;
using System.Linq;
using Apptus.ESales.EPiServer.Import.Products;
using Apptus.ESales.EPiServer.Util;
using Mediachase.Commerce.Catalog.Dto;

namespace Apptus.ESales.EPiServer.DataAccess
{
    internal class IncrementalCatalogEntryProvider : ICatalogEntryProvider
    {
        private readonly ESalesVariantHelper _eSalesVariantHelper;
        private readonly ICatalogSystemMapper _catalogSystemMapper;
        private readonly CatalogEntryDto.CatalogEntryRow[] _allEntries;

        internal IncrementalCatalogEntryProvider(
            int catalogId, ESalesVariantHelper eSalesVariantHelper, ICatalogSystemMapper catalogSystemMapper,
            IIndexSystemMapper indexSystemMapper, IEnumerable<IModifiedCatalogEntryLoader> modifiedCatalogEntryLoaders)
        {
            _eSalesVariantHelper = eSalesVariantHelper;
            _catalogSystemMapper = catalogSystemMapper;

            var earliestModifiedDate = indexSystemMapper.LastBuildDate.ToUniversalTime();
            var latestModifiedDate = indexSystemMapper.CurrentBuildDate;

            var catalogEntryRows = modifiedCatalogEntryLoaders
                .SelectMany(loader => loader.Load(catalogId, earliestModifiedDate, latestModifiedDate))
                .Distinct(entry => entry.CatalogEntryId);

            var originalEntries = catalogEntryRows.ToDictionary(e => e.CatalogEntryId, e => e);
            _allEntries = AppendMissingVariants(originalEntries).ToArray();
        }

        public int Count
        {
            get { return _allEntries.Length; }
        }

        public IEnumerable<CatalogEntryDto.CatalogEntryRow> GetCatalogEntries()
        {
            return _allEntries.Select( e => e );
        }

        private IEnumerable<CatalogEntryDto.CatalogEntryRow> AppendMissingVariants( Dictionary<int, CatalogEntryDto.CatalogEntryRow> originalEntries )
        {
            foreach ( var entry in originalEntries.Values )
            {
                yield return entry;
                if ( entry.IsActive )
                {
                    foreach ( var variant in 
                        _eSalesVariantHelper.GetVariants( entry.CatalogEntryId )
                                            .Where( variantId => !originalEntries.ContainsKey( variantId ) )
                                            .Select( variantId => _catalogSystemMapper.GetCatalogEntry( variantId ) )
                                            .Where( variant => variant.IsActive ) )
                    {
                        yield return variant;
                    }
                }
            }
        }
    }
}