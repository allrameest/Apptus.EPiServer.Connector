using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Core.Managers;

namespace Apptus.ESales.EPiServer.DataAccess
{
    internal class CatalogSystemMapper : ICatalogSystemMapper
    {
        private readonly ICatalogSystem _catalogSystem;

        public CatalogSystemMapper()
        {
            _catalogSystem = CatalogContext.Current;
        }

        public CatalogDto.CatalogDataTable GetCatalogs()
        {
            return _catalogSystem.GetCatalogDto().Catalog;
        }

        public int StartFindItemsForIndexing( Guid searchSetId, int catalogId, bool isIncremental, DateTime? earliestModifiedDate, DateTime? latestModifiedDate )
        {
            return _catalogSystem.StartFindItemsForIndexing( searchSetId, catalogId, isIncremental, earliestModifiedDate, latestModifiedDate );
        }

        public CatalogEntryDto.CatalogEntryDataTable ContinueFindItemsForIndexing( Guid searchSetId, int startIndex, int count )
        {
            return _catalogSystem.ContinueFindItemsForIndexing( searchSetId, startIndex, count ).CatalogEntry;
        }

        public CatalogRelationDto.CatalogEntryRelationDataTable GetCatalogRelations( int catalogId )
        {
            return
                _catalogSystem.GetCatalogRelationDto( catalogId, 0, 0, "", new CatalogRelationResponseGroup( CatalogRelationResponseGroup.ResponseGroup.CatalogEntry ) )
                              .CatalogEntryRelation;
        }
        
        public CatalogRelationDto.NodeEntryRelationDataTable GetCatalogRelations( int catalogId, int catalogNodeId )
        {
            return _catalogSystem.GetCatalogRelationDto( catalogId, catalogNodeId, 0, "", new CatalogRelationResponseGroup( CatalogRelationResponseGroup.ResponseGroup.NodeEntry ) )
                          .NodeEntryRelation;
        }

        public CatalogRelationDto.CatalogNodeRelationDataTable GetCatalogNodeRelations( int catalogId )
        {
            return _catalogSystem.GetCatalogRelationDto( catalogId, 0, 0, "", new CatalogRelationResponseGroup( CatalogRelationResponseGroup.ResponseGroup.CatalogNode ) )
                          .CatalogNodeRelation;
        }

        public CatalogEntryDto.CatalogEntryRow GetCatalogEntry( int catalogEntryId )
        {
            return _catalogSystem.GetCatalogEntryDto( catalogEntryId ).CatalogEntry.FirstOrDefault();
        }

        public CatalogEntryDto.CatalogEntryRow GetCatalogEntry( string catalogEntryCode )
        {
            return _catalogSystem.GetCatalogEntryDto( catalogEntryCode ).CatalogEntry.FirstOrDefault();
        }

        public CatalogNode GetCatalogNode( string code )
        {
            return _catalogSystem.GetCatalogNode( code );
        }

        public CatalogNode GetCatalogNode( int id )
        {
            return _catalogSystem.GetCatalogNode( id );
        }

        public IEnumerable<int> GetDeletedEntryIds( DateTime since, out int totalNumberOfRecords )
        {
            totalNumberOfRecords = 0;
            var appLog = LogManager.GetAppLog( "catalog", DataRowState.Deleted.ToString(), "entry", DateTime.MinValue, 0, int.MaxValue, ref totalNumberOfRecords );
            return totalNumberOfRecords <= 0
                       ? Enumerable.Empty<int>()
                       : appLog.ApplicationLog.Where( l => l.Created >= since ).Select( l => int.Parse( l.ObjectKey ) );
        }
    }
}