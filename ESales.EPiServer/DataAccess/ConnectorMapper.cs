using System.IO;

namespace Apptus.ESales.EPiServer.DataAccess
{
    internal class ConnectorMapper : IConnectorMapper
    {
        private readonly ConnectorHelper _connectorHelper;

        public ConnectorMapper( ConnectorHelper connectorHelper )
        {
            _connectorHelper = connectorHelper;
        }

        public void ImportAds( Stream stream, string id )
        {
            _connectorHelper.GetConnector().ImportAds( stream, id );
        }

        public void ImportProducts( Stream stream, string id )
        {
            _connectorHelper.GetConnector().ImportProducts( stream, id );
        }

        public void ImportConfiguration( Stream stream, string id )
        {
            _connectorHelper.GetConnector().ImportConfiguration( stream, id );
        }
    }
}