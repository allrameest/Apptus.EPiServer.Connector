using System;
using System.Collections.Generic;
using System.Linq;
using Apptus.ESales.EPiServer.Config;
using Apptus.ESales.EPiServer.DataAccess;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Search;

namespace Apptus.ESales.EPiServer.Import.Products
{
    internal class ProductIndexBuilder : IIndexBuilder
    {
        private bool _incremental;
        private readonly IAppConfig _appConfig;
        private readonly ICatalogSystemMapper _catalogSystem;
        private readonly IIndexSystemMapper _indexSystem;
        private readonly IKeyLookup _keyLookup;
        private readonly EntryConverter _entryConverter;
        private readonly IOperationsWriter _writer;
        private readonly IProductConverter _converterPlugin;
        private readonly IReadOnlyCollection<IProductsAppender> _appenderPlugins;
        private Progress _progress;

        public ProductIndexBuilder(
            IAppConfig appConfig, ICatalogSystemMapper catalogSystem, IIndexSystemMapper indexSystem, IKeyLookup keyLookup,
            EntryConverter entryConverter, IOperationsWriter writer, IEnumerable<IProductsAppender> appenderPlugins, IProductConverter converterPlugin = null)
        {
            _appConfig = appConfig;
            _catalogSystem = catalogSystem;
            _indexSystem = indexSystem;
            _keyLookup = keyLookup;
            _entryConverter = entryConverter;
            _writer = writer;
            _converterPlugin = converterPlugin;
            _appenderPlugins = appenderPlugins.ToArray();
        }

        public void Build( bool incremental )
        {
            using ( _writer )
            {
                _incremental = incremental;
                var catalogs = _catalogSystem.GetCatalogs();
                _progress = new Progress( catalogs.Count );
                var languages = AddEntries( catalogs ).Distinct();
                DeleteEntries( languages.ToArray() );
                UseAppenderPlugin( incremental );
                ReportCompletion();
                _indexSystem.SaveBuild( Status.Completed );
            }
        }

        private void UseAppenderPlugin(bool incremental)
        {
            var entitiesToAppend = _appenderPlugins.SelectMany(a => a.Append(incremental));
            foreach (var entity in entitiesToAppend)
            {
                _writer.Add(entity);
            }
        }

        private IEnumerable<string> AddEntries( IEnumerable<CatalogDto.CatalogRow> catalogs )
        {
            foreach ( var catalog in catalogs )
            {
                var languages = GetCatalogLanguages( catalog ).ToArray();
                WriteDocuments( catalog, languages );
                foreach ( var language in languages )
                {
                    yield return language;
                }
            }
        }

        private void WriteDocuments( CatalogDto.CatalogRow catalog, string[] languages )
        {
            var variantHelper = new ESalesVariantHelper( GetRelations( catalog.CatalogId ) );
            var catalogEntryProvider = CatalogEntryProviderFactory.Create( _incremental, catalog.CatalogId, variantHelper, _catalogSystem, _indexSystem );
            _progress.TotalNbrOfEntries = catalogEntryProvider.Count;
            var firstCatalogEntryId = int.MaxValue;
            var lastCatalogEntryId = int.MinValue;
            _indexSystem.Log( "Begin indexing catalog \"{0}\" in eSales...", _progress.GetCurrentProgressPercent(), catalog.Name );

            foreach ( var entry in catalogEntryProvider.GetCatalogEntries() )
            {
                SetFirstAndLastEntry( entry.CatalogEntryId, ref firstCatalogEntryId, ref lastCatalogEntryId );
                IndexEntry( entry, languages, catalog, variantHelper );
            }

            _progress.IncreaseCatalogCount();
            _indexSystem.Log( "Done indexing catalog \"{0}\".", _progress.GetCurrentProgressPercent(), catalog.Name );
            _indexSystem.SetBuildProperties( firstCatalogEntryId, lastCatalogEntryId, catalog.Name );
        }

        private static void SetFirstAndLastEntry( int id, ref int firstCatalogEntryId, ref int lastCatalogEntryId )
        {
            if ( id < firstCatalogEntryId )
            {
                firstCatalogEntryId = id;
            }
            if ( id > lastCatalogEntryId )
            {
                lastCatalogEntryId = id;
            }
        }

        private void IndexEntry( CatalogEntryDto.CatalogEntryRow entry, string[] languages, CatalogDto.CatalogRow catalog,
                                 ESalesVariantHelper variantHelper )
        {
            if ( variantHelper.IsVariant( entry.CatalogEntryId ) )
            {
                Add( entry, languages, catalog, variantHelper );
            }
            else
            {
                UpdateProduct( entry, languages, catalog, variantHelper );
            }

            ReportAddProgress();
        }

        private void UpdateProduct( CatalogEntryDto.CatalogEntryRow entry, string[] languages, CatalogDto.CatalogRow catalog,
                                    ESalesVariantHelper variantHelper )
        {
            if ( _incremental )
            {
                // Variants might have changed from products -> variants, so delete as products just in case.
                var variantEntries = variantHelper.GetVariants( entry.CatalogEntryId ).Select( v => _catalogSystem.GetCatalogEntry( v ) );
                RemoveProducts( new[] { entry }.Concat( variantEntries ), languages );
            }
            Add( entry, languages, catalog, variantHelper );
        }

        private void Add( CatalogEntryDto.CatalogEntryRow entry, IEnumerable<string> languages, CatalogDto.CatalogRow catalog,
                          ESalesVariantHelper variantHelper )
        {
            foreach ( var convertedEntry in _entryConverter.Convert( entry, languages, catalog, variantHelper ) )
            {
                var entity = convertedEntry;
                if ( _converterPlugin != null )
                {
                    entity = _converterPlugin.Convert( convertedEntry );
                }
                _writer.Add( entity );
            }
        }

        private void RemoveProducts( IEnumerable<CatalogEntryDto.CatalogEntryRow> entries, IEnumerable<string> languages )
        {
            foreach ( var key in entries.SelectMany( e => languages.Select( l => _keyLookup.Value( e, l ) ) ) )
            {
                _writer.Remove( new Product( key ) );
            }
        }

        private IEnumerable<CatalogRelationDto.CatalogEntryRelationRow> GetRelations( int catalogId )
        {
            if ( _appConfig.EnableVariants )
            {
                var catalogRelationTable = _catalogSystem.GetCatalogRelations( catalogId );
                return catalogRelationTable.Rows
                                           .Cast<CatalogRelationDto.CatalogEntryRelationRow>()
                                           .Where( r => r.RelationTypeId.Equals( "ProductVariation", StringComparison.InvariantCultureIgnoreCase ) );
            }
            return Enumerable.Empty<CatalogRelationDto.CatalogEntryRelationRow>();
        }

        private static IEnumerable<string> GetCatalogLanguages( CatalogDto.CatalogRow catalog )
        {
            return new[] { catalog.DefaultLanguage }.Concat( catalog.GetCatalogLanguageRows().Select( l => l.LanguageCode ) ).Distinct();
        }

        private void DeleteEntries( string[] languages )
        {
            if ( _incremental )
            {
                foreach ( var entity in GetDeletedEntities( _indexSystem.LastBuildDate, languages ) )
                {
                    _writer.Remove( entity );
                }
            }
            else
            {
                _writer.Clear( "product" );
            }
        }

        private IEnumerable<IEntity> GetDeletedEntities( DateTime lastBuild, string[] languages )
        {
            var count = 0;
            int totalRecords;
            lastBuild = lastBuild.ToUniversalTime();

            foreach ( var id in _catalogSystem.GetDeletedEntryIds( lastBuild, out totalRecords ) )
            {
                foreach ( var document in CreateDeleteEntities( id, languages ) )
                {
                    yield return document;
                }

                ReportRemoveProgress( ++count, totalRecords );
            }
        }

        private IEnumerable<IEntity> CreateDeleteEntities( int id, IEnumerable<string> languages )
        {
            foreach ( var key in languages.Select( l => _keyLookup.Value( id, l ) ) )
            {
                yield return new Product( key );
                yield return new Variant( key );
            }
        }


        

        private void ReportAddProgress()
        {
            _progress.IncreaseEntryCount();
            if ( _progress.ProcessedNbrOfEntries > 0 && _progress.ProcessedNbrOfEntries % 100 == 0 )
            {
                _indexSystem.Log( "Entries completed: {0}/{1}.", _progress.GetCurrentProgressPercent(), _progress.ProcessedNbrOfEntries,
                                  _progress.TotalNbrOfEntries );
            }
        }

        private void ReportRemoveProgress( int count, int totalRecords )
        {
            if ( count % 20 == 0 )
            {
                var percentage = (double) count / totalRecords * 100.0;
                _indexSystem.Log( string.Format( "Removing old entry from index ({0}/{1}) ...", count, totalRecords ), percentage );
            }

            if ( count == totalRecords )
            {
                _indexSystem.Log( string.Format( "CatalogIndexBuilder Removed {0} records.", count ), 100d );
            }
        }

        private void ReportCompletion()
        {
            _indexSystem.Log( "eSalesCatalogIndexBuilder processed a total of {0} catalog entries in {1} catalog{2}.", 100,
                              _progress.GrandTotalProcessedNbrOfEntries, _progress.ProcessedNbrOfCatalogs, _progress.ProcessedNbrOfCatalogs > 1 ? "s" : "" );
        }


        private class Progress
        {
            private readonly int _totalNbrOfCatalogs;

            internal Progress( int totalNbrOfCatalogs )
            {
                _totalNbrOfCatalogs = totalNbrOfCatalogs;
                ProcessedNbrOfCatalogs = 0;
                ProcessedNbrOfEntries = 0;
                GrandTotalProcessedNbrOfEntries = 0;
            }

            internal int TotalNbrOfEntries { get; set; }
            internal int ProcessedNbrOfEntries { get; private set; }
            internal int GrandTotalProcessedNbrOfEntries { get; private set; }
            internal int ProcessedNbrOfCatalogs { get; private set; }

            internal double GetCurrentProgressPercent()
            {
                var catalogPart = ToPercent( ProcessedNbrOfCatalogs, _totalNbrOfCatalogs );

                if ( TotalNbrOfEntries == 0 )
                {
                    return catalogPart;
                }

                var entryPart = ToPercent( ProcessedNbrOfEntries, TotalNbrOfEntries );

                return ( catalogPart + ( entryPart / _totalNbrOfCatalogs ) );
            }

            internal void IncreaseEntryCount()
            {
                ProcessedNbrOfEntries++;
                GrandTotalProcessedNbrOfEntries++;
            }

            internal void IncreaseCatalogCount()
            {
                ProcessedNbrOfCatalogs++;
                ProcessedNbrOfEntries = 0;
            }

            private static double ToPercent( int part, int whole )
            {
                if ( whole <= 0 )
                {
                    return 100;
                }
                return ( (double) part ) / whole * 100;
            }
        }
    }
}
