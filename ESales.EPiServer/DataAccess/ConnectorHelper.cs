using Apptus.ESales.Connector;
using Apptus.ESales.EPiServer.Config;

namespace Apptus.ESales.EPiServer.DataAccess
{
    internal class ConnectorHelper
    {
        private readonly IAppConfig _appConfig;

        public ConnectorHelper( IAppConfig appConfig )
        {
            _appConfig = appConfig;
        }

        public Connector.Connector GetConnector()
        {
            if ( !_appConfig.Saas )
            {
                return OnPremConnector.GetOrCreate( _appConfig.ClusterUrl );
            }
            return CloudConnector.GetOrCreate( _appConfig.ClusterUrl );
        }
    }
}
