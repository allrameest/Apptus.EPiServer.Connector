using System.IO;
using Apptus.ESales.EPiServer.Config;
using Apptus.ESales.EPiServer.DataAccess;

namespace Apptus.ESales.EPiServer.Import.Configuration
{
    internal class ConfigurationIndexDeleter : IIndexDeleter
    {
        private readonly IIndexSystemMapper _indexSystem;
        private readonly IAppConfig _configuration;
        private readonly FileHelper _fileHelper;

        public ConfigurationIndexDeleter( IIndexSystemMapper indexSystem, IAppConfig configuration, FileHelper fileHelper )
        {
            _indexSystem = indexSystem;
            _configuration = configuration;
            _fileHelper = fileHelper;
        }

        public void Delete( bool incremental )
        {
            if ( !incremental && !_configuration.KeepDataFiles )
            {
                File.Delete( _fileHelper.ConfigurationFile );
                _indexSystem.Log( "Deleted temporary configuration file." );
            }
        }
    }
}