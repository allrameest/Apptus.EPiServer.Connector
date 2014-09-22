using Apptus.ESales.EPiServer.DataAccess;

namespace Apptus.ESales.EPiServer.Import.Configuration
{
    internal class ConfigurationIndexBuilder : IIndexBuilder
    {
        private readonly ConfigurationWriter _configurationWriter;
        private readonly IIndexSystemMapper _indexSystem;

        public ConfigurationIndexBuilder( ConfigurationWriter configurationWriter, IIndexSystemMapper indexSystem )
        {
            _configurationWriter = configurationWriter;
            _indexSystem = indexSystem;
        }

        public void Build( bool incremental )
        {
            if ( !incremental )
            {
                _indexSystem.Log( "Extracting configuration." );
                _configurationWriter.WriteConfiguration();
                _indexSystem.Log( "Done extracting configuration." );
            }
        }
    }
}