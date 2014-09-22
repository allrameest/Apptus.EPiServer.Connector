using System;
using System.Collections.Generic;
using System.Linq;
using Apptus.ESales.EPiServer.Import.Products;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Managers;

namespace Apptus.ESales.EPiServer.DataAccess
{
    internal class IncrementalCatalogEntryProvider : ICatalogEntryProvider
    {
        private readonly ESalesVariantHelper _eSalesVariantHelper;
        private readonly ICatalogSystemMapper _catalogSystemMapper;
        private readonly CatalogEntryDto.CatalogEntryRow[] _allEntries;

        internal IncrementalCatalogEntryProvider( int catalogId, ESalesVariantHelper eSalesVariantHelper, ICatalogSystemMapper catalogSystemMapper,
                                                  IIndexSystemMapper indexSystemMapper )
        {
            _eSalesVariantHelper = eSalesVariantHelper;
            _catalogSystemMapper = catalogSystemMapper;
            
            var searchSetId = Guid.NewGuid();
            _catalogSystemMapper.StartFindItemsForIndexing( searchSetId, catalogId, true, indexSystemMapper.LastBuildDate.ToUniversalTime(),
                                                            indexSystemMapper.CurrentBuildDate );
            var entryTable = _catalogSystemMapper.ContinueFindItemsForIndexing( searchSetId, 1, int.MaxValue - 1 );
            var originalEntries = entryTable.Rows.Cast<CatalogEntryDto.CatalogEntryRow>().ToDictionary( e => e.CatalogEntryId, e => e );
            _allEntries = AppendMissingVariants( originalEntries ).ToArray();
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