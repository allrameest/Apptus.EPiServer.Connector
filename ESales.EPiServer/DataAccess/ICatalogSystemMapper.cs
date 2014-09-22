using System;
using System.Collections;
using System.Collections.Generic;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Catalog.Objects;

namespace Apptus.ESales.EPiServer.DataAccess
{
    internal interface ICatalogSystemMapper
    {
        int StartFindItemsForIndexing( Guid searchSetId, int catalogId, bool isIncremental, DateTime? earliestModifiedDate, DateTime? latestModifiedDate );
        CatalogDto.CatalogDataTable GetCatalogs();
        CatalogEntryDto.CatalogEntryDataTable ContinueFindItemsForIndexing( Guid searchSetId, int startIndex, int count );
        CatalogRelationDto.CatalogEntryRelationDataTable GetCatalogRelations( int catalogId );
        CatalogRelationDto.NodeEntryRelationDataTable GetCatalogRelations( int catalogId, int catalogNodeId );
        CatalogRelationDto.CatalogNodeRelationDataTable GetCatalogNodeRelations( int catalogId );
        CatalogEntryDto.CatalogEntryRow GetCatalogEntry( int catalogEntryId );
        CatalogEntryDto.CatalogEntryRow GetCatalogEntry( string catalogEntryCode );
        CatalogNode GetCatalogNode( string code );
        CatalogNode GetCatalogNode( int id );
        IEnumerable<int> GetDeletedEntryIds( DateTime since, out int totalNumberOfRecords );
    }
}