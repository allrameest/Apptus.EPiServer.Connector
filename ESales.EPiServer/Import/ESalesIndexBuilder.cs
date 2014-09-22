using System.Collections.Generic;
using Apptus.ESales.EPiServer.Config;
using Apptus.ESales.EPiServer.DataAccess;

namespace Apptus.ESales.EPiServer.Import
{
    internal class ESalesIndexBuilder
    {
        private readonly bool _incremental;
        private readonly IIndexSystemMapper _indexSystem;
        private readonly IAppConfig _appConfig;
        private readonly IEnumerable<IIndexBuilder> _builders;
        private readonly IEnumerable<IIndexImporter> _importers;
        private readonly IEnumerable<IIndexDeleter> _deleters;

        public ESalesIndexBuilder( bool incremental, IIndexSystemMapper indexSystem, IAppConfig appConfig, IEnumerable<IIndexBuilder> builders,
                                   IEnumerable<IIndexImporter> importers, IEnumerable<IIndexDeleter> deleters )
        {
            _incremental = incremental;
            _indexSystem = indexSystem;
            _appConfig = appConfig;
            _builders = builders;
            _importers = importers;
            _deleters = deleters;
        }

        public void Run()
        {
            ReportStart();
            BuildFiles();
            ImportFiles();
            DeleteFiles();
            ReportCompletion();
        }

        private void BuildFiles()
        {
            foreach ( var indexBuilder in _builders )
            {
                indexBuilder.Build( _incremental );
            }
        }

        private void ImportFiles()
        {
            foreach ( var importer in _importers )
            {
                importer.Import( _incremental );
            }
        }

        private void DeleteFiles()
        {
            foreach ( var deleter in _deleters )
            {
                deleter.Delete( _incremental );
            }
        }

        private void ReportStart()
        {
            _indexSystem.Log( "eSalesCatalogIndexBuilder started.", 0 );
            _indexSystem.Log( "Variants export is {0}.", 0, _appConfig.EnableVariants ? "enabled" : "disabled" );
            _indexSystem.Log( "Ads export is {0}.", 0, _appConfig.EnableAds ? "enabled and exported from " + _appConfig.AdsSource : "disabled" );
        }

        private void ReportCompletion()
        {
            _indexSystem.Log( "eSalesCatalogIndexBuilder finished.", 100 );
        }
    }
}
