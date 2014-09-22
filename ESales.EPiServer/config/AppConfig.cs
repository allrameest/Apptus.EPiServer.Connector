using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.IO;
using System.Linq;
using System.Net;
using Apptus.ESales.EPiServer.Import;
using Mediachase.Search;

namespace Apptus.ESales.EPiServer.Config
{
    public class AppConfig : IAppConfig
    {
        public AppConfig()
        {
            LoadConfiguration();
        }

        public string ApplicationId { get; private set; }
        public string ApplicationPath { get; private set; }
        public string AppDataPath { get; private set; }
        public bool KeepDataFiles { get; private set; }
        public string ConfigurationXmlFile { get; private set; }
        public string ClusterUrl { get; private set; }
        public bool Saas { get; private set; }
        public string SaasUser { get; private set; }
        public string SaasPassword { get; private set; }
        public string ManagerUrl { get; private set; }
        public bool Enable { get; private set; }
        public string Scope { get; private set; }
        public bool EnableVariants { get; private set; }
        public string AdsSource { get; private set; }
        public IEnumerable<string> FilterAttributes { get; private set; }
        public IEnumerable<string> SuggestAttributes { get; private set; }
        public bool EnableAds { get; private set; }
        public bool UseCodeAsKey { get; private set; }
        public string FilterConfigurationFile { get; private set; }
        public bool Debug { get; private set; }
        
        private void LoadConfiguration()
        {
            var settings = SearchConfiguration.Instance.SearchProviders.Providers
                                              .Cast<ProviderSettings>()
                                              .FirstOrDefault( ps => ps.Type != null && ps.Type.StartsWith( typeof( ESalesSearchProvider ).FullName ) );
            if ( settings != null )
            {
                ReadParameters( settings.Parameters );
            }
            else
            {
                throw new ProviderException( "Could not find provider setting for " + typeof( ESalesSearchProvider ).FullName );
            }
        }

        private void ReadParameters( NameValueCollection settings )
        {
            var parser = new SettingsParser( settings );
            ApplicationId = parser.RequiredString( "applicationName" );
            Scope = parser.RequiredString( "scope" );
            ApplicationPath = parser.RequiredString( "appPath", Directory.Exists );
            
            ManagerUrl = parser.OptionalString( "managerURL", string.Format( "http://{0}:35350", Dns.GetHostName() ) );
            
            Saas = parser.OptionalBool( "saas", false );
            string clusterUrl;
            if ( Saas )
            {
                SaasUser = parser.OptionalString( "saasUser", null );
                SaasPassword = parser.OptionalString( "saasPassword", null );
                clusterUrl = parser.OptionalString( "clusterURL", null );
            }
            else
            {
                clusterUrl = parser.RequiredString( "clusterURL" );
            }
            ClusterUrl = AppConfigCleaner.CleanupClusterUrl( clusterUrl, SaasUser, SaasPassword );

            AppDataPath = parser.OptionalString( "appData", ApplicationPath + "\\App_Data\\ESales" );
            Directory.CreateDirectory( AppDataPath );

            Enable = parser.OptionalBool( "enable", true );
            KeepDataFiles = parser.OptionalBool( "keepDataFiles", false );
            ConfigurationXmlFile = parser.OptionalString( "xmlConfigurationFile", null );
            FilterAttributes = parser.OptionalEnumerable( "filterAttributes", ";" ).Select( a => a.ToLowerInvariant() ).Distinct();
            SuggestAttributes = parser.OptionalEnumerable( "suggestAttributes", ";" ).Select( a => a.ToLowerInvariant() ).Distinct();
            EnableVariants = parser.OptionalBool( "enableVariants", false );
            EnableAds = parser.OptionalBool( "enableAds", false );
            UseCodeAsKey = parser.RequiredBool( "useCodeAsKey" );
            FilterConfigurationFile = parser.OptionalString( "filterConfigurationFile", ApplicationPath + "\\Configs\\Mediachase.Search.Filters.config" );
            Debug = parser.OptionalBool( "debug", false );

            if ( EnableAds )
            {
                AdsSource = parser.OptionalString( "adsSource", "commerce", v => !string.IsNullOrWhiteSpace( v ) && ( v == "commerce" || File.Exists( v ) ) );
            }
        }
    }
}
