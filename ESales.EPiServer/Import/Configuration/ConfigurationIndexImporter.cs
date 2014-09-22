using System.IO;
using Apptus.ESales.EPiServer.DataAccess;

namespace Apptus.ESales.EPiServer.Import.Configuration
{
    internal class ConfigurationIndexImporter : IIndexImporter
    {
        private readonly IIndexSystemMapper _indexSystem;
        private readonly FileHelper _fileHelper;
        private readonly IConnectorMapper _connector;
        private readonly IFileSystem _fileSystem;

        public ConfigurationIndexImporter( IIndexSystemMapper indexSystem, FileHelper fileHelper, IConnectorMapper connector, IFileSystem fileSystem )
        {
            _indexSystem = indexSystem;
            _fileHelper = fileHelper;
            _connector = connector;
            _fileSystem = fileSystem;
        }

        public void Import( bool incremental )
        {
            if ( !incremental )
            {
                _indexSystem.Log( "Importing configuration." );
                using ( var stream = _fileSystem.Open( _fileHelper.ConfigurationFile, FileMode.Open, FileAccess.Read ) )
                {
                    _connector.ImportConfiguration( stream );
                }
                _indexSystem.Log( "Done importing configuration." );
            }
        }
    }
}