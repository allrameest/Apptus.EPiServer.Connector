using System.Collections.Generic;
using System.Linq;
using Apptus.ESales.Connector;
using Apptus.ESales.EPiServer.Config;
using Apptus.ESales.EPiServer.DataAccess;

namespace Apptus.ESales.EPiServer.Util
{
    public static class ESales
    {
        private static readonly IAppConfig AppConfig = new AppConfig();
        private static readonly ConnectorHelper ConnectorHelper = new ConnectorHelper( AppConfig );

        public static Connector.Connector Connector
        {
            get { return ConnectorHelper.GetConnector(); }
        }

        public static string ManagerURL
        {
            get { return AppConfig.ManagerUrl; }
        }

        public static PanelContent GetPanelContent( string path, IEnumerable<KeyValuePair<string, object>> args )
        {
            return GetPanelContent( path, BuildArguments( args ) );
        }

        public static PanelContent GetPanelContent( string path, IEnumerable<KeyValuePair<string, string>> args )
        {
            return GetPanelContent( path, args as ArgMap ?? BuildArguments( args ) );
        }

        public static PanelContent GetPanelContent( string path, ArgMap args )
        {
            var panel = Connector.Session( Session.Key ).Panel( path );
            return panel.RetrieveContent( args );
        }

        private static ArgMap BuildArguments( IEnumerable<KeyValuePair<string, string>> args )
        {
            var a = new ArgMap();
            foreach ( var kvp in args.Where( kvp => kvp.Value != null ) )
            {
                a.SafeAdd( kvp.Key, kvp.Value );
            }
            return a;
        }

        private static ArgMap BuildArguments( IEnumerable<KeyValuePair<string, object>> args )
        {
            var a = new ArgMap();
            foreach ( var kvp in args.Where( kvp => kvp.Value != null ) )
            {
                a.SafeAdd( kvp.Key, kvp.Value );
            }
            return a;
        }

        public static string CssClickClass( string ticket )
        {
            return CssClass.Get().ForClickNotification( ticket );
        }

        public static void NotifyPayment( IEnumerable<string> productKeys )
        {
            var order = new Order();
            foreach ( var productKey in productKeys )
            {
                order.AddProduct( productKey );
            }
            NotifyPayment( order );
        }

        public static void NotifyPayment( Order order )
        {
            Connector.Session( Session.Key ).NotifyPayment( order );
        }

        public static void NotifyUser( string userName )
        {
            if ( string.IsNullOrEmpty( userName ) ) return;
            Connector.Session( Session.Key ).NotifyProperty( "customer_key", userName );
        }   
        
        public static void NotifyMarket( string market )
        {
            if ( string.IsNullOrEmpty( market ) ) return;
            Connector.Session( Session.Key ).NotifyProperty( "market", market );
        }

        public static void EndSession()
        {
            Connector.Session( Session.Key ).End();
        }
    }
}
