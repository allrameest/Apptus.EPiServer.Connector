using System;
using System.Collections.Generic;
using Mediachase.Commerce.Catalog.Dto;

namespace Apptus.ESales.EPiServer.DataAccess
{
    public interface IModifiedCatalogEntryLoader
    {
        IEnumerable<CatalogEntryDto.CatalogEntryRow> Load(int catalogId, DateTime earliestModifiedDate, DateTime latestModifiedDate);
    }
}