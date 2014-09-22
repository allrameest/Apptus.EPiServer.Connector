using System.IO;

namespace Apptus.ESales.EPiServer.DataAccess
{
    internal interface IConnectorMapper
    {
        void ImportAds( Stream stream, string id = null );
        void ImportProducts( Stream stream, string id = null );
        void ImportConfiguration( Stream stream, string id = null );
    }
}