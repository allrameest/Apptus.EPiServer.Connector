using System.Collections.Generic;

namespace Apptus.ESales.EPiServer.Import.Configuration
{
    internal interface IConfiguration
    {
        IEnumerable<ConfigurationAttribute> ProductAttributes { get; }
        IEnumerable<ConfigurationAttribute> AdAttributes { get; }
        IEnumerable<FacetAttribute> FacetAttributes { get; }
    }
}
