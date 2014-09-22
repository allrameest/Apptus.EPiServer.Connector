using System.Collections.Generic;

namespace Apptus.ESales.EPiServer.Config
{
    public interface IAppConfig
    {
        string ApplicationId { get; }
        string ApplicationPath { get; }
        string AppDataPath { get; }
        bool KeepDataFiles { get; }
        string ConfigurationXmlFile { get; }
        string ClusterUrl { get; }
        bool Saas { get; }
        string SaasUser { get; }
        string SaasPassword { get; }
        string ManagerUrl { get; }
        bool Enable { get; }
        string Scope { get; }
        bool EnableVariants { get; }
        string AdsSource { get; }
        IEnumerable<string> FilterAttributes { get; }
        IEnumerable<string> SuggestAttributes { get; }
        bool EnableAds { get; }
        bool UseCodeAsKey { get; }
        string FilterConfigurationFile { get; }
        bool Debug { get; }
    }
}