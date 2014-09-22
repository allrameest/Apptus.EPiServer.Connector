using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Apptus.ESales.EPiServer.Config;
using Apptus.ESales.EPiServer.DataAccess;
using Apptus.ESales.EPiServer.Resources;

namespace Apptus.ESales.EPiServer.Import.Configuration
{
    internal class ConfigurationWriter
    {
        private readonly FileHelper _fileHelper;
        private readonly IFileSystem _fileSystem;
        private readonly IConfiguration _configuration;
        private readonly IAppConfig _appConfig;
        private readonly AttributeConverter _attributeConverter;
        private IDictionary<string, FacetAttribute> _facetDict;

        public ConfigurationWriter( FileHelper fileHelper, IFileSystem fileSystem, IConfiguration configuration, IAppConfig appConfig, AttributeConverter attributeConverter )
        {
            _fileHelper = fileHelper;
            _fileSystem = fileSystem;
            _configuration = configuration;
            _appConfig = appConfig;
            _attributeConverter = attributeConverter;
        }

        public void WriteConfiguration()
        {
            //if we have custom configuration, write and exit 
            if ( !string.IsNullOrWhiteSpace( _appConfig.ConfigurationXmlFile ) )
            {
                TransferPrepared();
                return;
            }

            using ( var outFile = _fileSystem.Open( _fileHelper.ConfigurationFile, FileMode.CreateNew, FileAccess.Write ) )
            using ( var eSalesConfigurationWriter = new StreamWriter( outFile, Encoding.UTF8 ) )
            {
                var eSalesConfiguration = new configuration
                    {
                        search_refinements = Loader.Load<search_refinements>(),
                        formats = Loader.Load<formats>().format,
                        tokenizations = Loader.Load<tokenizations>().tokenization,
                        normalizations = Loader.Load<normalizations>().normalization
                    };
                _facetDict = _configuration.FacetAttributes.ToDictionary( fa => fa.Name );
                eSalesConfiguration.product_attributes = GetProductAttributes().ToArray();
                eSalesConfiguration.ad_attributes = GetAdAttributes().ToArray();
                new XmlSerializer( eSalesConfiguration.GetType() ).Serialize( eSalesConfigurationWriter, eSalesConfiguration );
                eSalesConfigurationWriter.Flush();
            }
        }

        private IEnumerable<attribute> GetAdAttributes()
        {
            if ( _appConfig.EnableAds )
            {
                if ( _appConfig.AdsSource == "commerce" )
                {
                    return _configuration.AdAttributes.Select( a => _attributeConverter.Convert( a ) );
                }
                var adsSerializer = new XmlSerializer( typeof( configuration ) );
                using ( var reader = XmlReader.Create( _appConfig.AdsSource ) )
                {
                    var adsConfig = (configuration) adsSerializer.Deserialize( reader );
                    return adsConfig.ad_attributes;
                }
            }
            return Enumerable.Empty<attribute>();
        }

        private IEnumerable<attribute> GetProductAttributes()
        {
            foreach ( var attribute in _configuration.ProductAttributes )
            {
                yield return _attributeConverter.Convert( attribute );
                var facetAttribute = FacetAttribute( attribute );
                if ( facetAttribute != null )
                {
                    yield return facetAttribute;
                }
            }
        }

        private void TransferPrepared()
        {
            _fileSystem.Copy( _appConfig.ConfigurationXmlFile, _fileHelper.ConfigurationFile );
        }

        private attribute FacetAttribute( ConfigurationAttribute a )
        {
            if ( !_facetDict.ContainsKey( a.Name ) ) return null;
            var fa = new ConfigurationAttribute(
                a.Name + "__facet",
                type.@string,
                Present.No,
                filterOptions: new FilterOptions( Format.PipeSeparated, Tokenization.None ),
                sortOptions: new SortOptions( Normalization.CaseInsensitive ) );
            return _attributeConverter.Convert( fa );
        }
    }
}
