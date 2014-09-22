using System.IO;
using Apptus.ESales.EPiServer.Config;
using Apptus.ESales.EPiServer.DataAccess;

namespace Apptus.ESales.EPiServer.Import.Ads
{
    internal class AdsIndexImporter : IIndexImporter
    {
        private readonly IAppConfig _appConfig;
        private readonly IIndexSystemMapper _indexSystem;
        private readonly FileHelper _fileHelper;
        private readonly IConnectorMapper _connector;
        private readonly IFileSystem _fileSystem;

        public AdsIndexImporter( IAppConfig appConfig, IIndexSystemMapper indexSystem, FileHelper fileHelper, IConnectorMapper connector, IFileSystem fileSystem )
        {
            _appConfig = appConfig;
            _indexSystem = indexSystem;
            _fileHelper = fileHelper;
            _connector = connector;
            _fileSystem = fileSystem;
        }

        public void Import( bool incremental )
        {
            if ( !incremental && _appConfig.EnableAds )
            {
                _indexSystem.Log( "Importing ads." );
                using ( var stream = _fileSystem.Open( _fileHelper.AdsFile, FileMode.Open, FileAccess.Read ) )
                {
                    _connector.ImportAds( stream );
                }
                _indexSystem.Log( "Done importing ads." );
            }
        }
    }
}