using System;
using System.Collections.Generic;
using System.Linq;
using Mediachase.Commerce.Catalog.Dto;

namespace Apptus.ESales.EPiServer.DataAccess
{
    internal class DefaultModifiedCatalogEntryLoader : IModifiedCatalogEntryLoader
    {
        private readonly ICatalogSystemMapper _catalogSystemMapper;

        public DefaultModifiedCatalogEntryLoader(ICatalogSystemMapper catalogSystemMapper)
        {
            _catalogSystemMapper = catalogSystemMapper;
        }

        public IEnumerable<CatalogEntryDto.CatalogEntryRow> Load(int catalogId, DateTime earliestModifiedDate, DateTime latestModifiedDate)
        {
            var searchSetId = Guid.NewGuid();
            _catalogSystemMapper.StartFindItemsForIndexing(searchSetId, catalogId, true, earliestModifiedDate, latestModifiedDate);
            var entryTable = _catalogSystemMapper.ContinueFindItemsForIndexing(searchSetId, 1, int.MaxValue - 1);
            return entryTable.Rows.Cast<CatalogEntryDto.CatalogEntryRow>();
        }
    }
}