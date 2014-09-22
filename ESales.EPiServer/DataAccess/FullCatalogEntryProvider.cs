using System;
using System.Collections.Generic;
using Apptus.ESales.EPiServer.Import.Products;
using Mediachase.Commerce.Catalog.Dto;

namespace Apptus.ESales.EPiServer.DataAccess
{
    internal class FullCatalogEntryProvider : ICatalogEntryProvider
    {
        private readonly ICatalogSystemMapper _catalogSystemMapper;
        private readonly Guid _searchSetId;

        internal FullCatalogEntryProvider( int catalogId, ICatalogSystemMapper catalogSystemMapper )
        {
            _catalogSystemMapper = catalogSystemMapper;
            _searchSetId = Guid.NewGuid();
            Count = _catalogSystemMapper.StartFindItemsForIndexing( _searchSetId, catalogId, false, null, null );
        }

        public int Count { get; private set; }

        public IEnumerable<CatalogEntryDto.CatalogEntryRow> GetCatalogEntries()
        {
            // The chunk size has to be so small because there is a bug in Mediachase.Commerce.Catalog.Dto.CatalogEntryDto.GetPriceValues()
            // where the entry is located in a list of prices. The list and the chunk are the same size, so if it's too large then a
            // lot of comparisons are made. A fix would be to change that list to a dictionary. This has been reported to EPiServer.
            const int batchSize = 100;
            for ( var i = 1; i <= Count; i += batchSize )
            {
                foreach ( var entry in _catalogSystemMapper.ContinueFindItemsForIndexing( _searchSetId, i, batchSize ) )
                {
                    yield return entry;
                }
            }
        }
    }
}