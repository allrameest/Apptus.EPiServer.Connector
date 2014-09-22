using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Apptus.ESales.EPiServer.Config;
using Apptus.ESales.EPiServer.DataAccess;
using Apptus.ESales.EPiServer.Import.Ads;
using Mediachase.Commerce.Marketing.Dto;
using Mediachase.MetaDataPlus.Configurator;

namespace Apptus.ESales.EPiServer.Import.Configuration
{
    internal class Configuration : IConfiguration
    {
        private readonly IAppConfig _appConfig;
        private readonly AdAttributeHelper _adAttributeHelper;
        private readonly IMetaDataMapper _metaData;
        private readonly IFileSystem _fileSystem;
        private readonly Dictionary<string, FacetAttribute> _facetAttributesIndex;

        ///<summary>
        /// Creates a <c>Configuration</c> instance.
        ///</summary>
        public Configuration( IAppConfig appConfig, AdAttributeHelper adAttributeHelper, IMetaDataMapper metaData, IFileSystem fileSystem )
        {
            _appConfig = appConfig;
            _adAttributeHelper = adAttributeHelper;
            _metaData = metaData;
            _fileSystem = fileSystem;

            _facetAttributesIndex = GetFacetAttributes().ToDictionary( fa => fa.Name );
            ProductAttributes = GetProductAttributes();
            AdAttributes = _appConfig.AdsSource == "commerce" ? GetAdAttributeConfig() : Enumerable.Empty<ConfigurationAttribute>();
        }

        public IEnumerable<ConfigurationAttribute> ProductAttributes { get; private set; }

        public IEnumerable<ConfigurationAttribute> AdAttributes { get; private set; }

        public IEnumerable<FacetAttribute> FacetAttributes { get { return _facetAttributesIndex.Values; } }

        private IEnumerable<ConfigurationAttribute> GetStaticAttributes()
        {
            yield return new ConfigurationAttribute(
                "name",
                type.@string,
                Present.Yes,
                new SearchOptions( null, Format.PipeSeparated, false, IsSuggestable( "name" ) ),
                sortOptions: new SortOptions( Normalization.CaseInsensitive ),
                filterOptions: _appConfig.FilterAttributes.Contains( "name" ) ? new FilterOptions( Format.PipeSeparated, Tokenization.None ) : null );
            yield return new ConfigurationAttribute(
                "_metaclass",
                type.@string,
                Present.Yes,
                filterOptions: new FilterOptions( Format.PipeSeparated, Tokenization.None ) );
            yield return new ConfigurationAttribute(
                "_lang",
                type.@string,
                Present.Yes,
                filterOptions: new FilterOptions( Format.PipeSeparated, Tokenization.CaseInsensitive ) );
            yield return new ConfigurationAttribute(
                "_node",
                type.@string,
                Present.Yes,
                filterOptions: new FilterOptions( Format.PipeSeparated, Tokenization.None ) );
            yield return new ConfigurationAttribute(
                "_outline",
                type.@string,
                Present.Yes,
                filterOptions: new FilterOptions( Format.PipeSeparatedPaths, Tokenization.None ) );
            yield return new ConfigurationAttribute(
                "code",
                type.@string,
                Present.Yes,
                new SearchOptions( null, Format.PipeSeparated, false, _appConfig.SuggestAttributes.Contains( "code" ) ),
                new FilterOptions( Format.PipeSeparated, Tokenization.None ) );
            yield return new ConfigurationAttribute(
                "_content",
                type.@string,
                Present.Yes,
                filterOptions: new FilterOptions( Format.PipeSeparated, Tokenization.None ) );
            yield return new ConfigurationAttribute(
                "listprice",
                type.@double,
                Present.Yes,
                filterOptions: new FilterOptions( Format.PipeSeparated, Tokenization.None ),
                sortOptions: new SortOptions( Normalization.CaseInsensitive ) );
            yield return new ConfigurationAttribute(
                "_id",
                type.@string,
                Present.Yes,
                filterOptions: new FilterOptions( Format.PipeSeparated, Tokenization.None ) );
            yield return new ConfigurationAttribute(
                "product_key",
                type.@string,
                Present.Yes,
                filterOptions: new FilterOptions( Format.PipeSeparated, Tokenization.None ) );
            yield return new ConfigurationAttribute(
                "_catalog",
                type.@string,
                Present.Yes,
                filterOptions: new FilterOptions( Format.PipeSeparated, Tokenization.None ) );
            yield return new ConfigurationAttribute(
                "_classtype",
                type.@string,
                Present.Yes,
                filterOptions: new FilterOptions( Format.PipeSeparated, Tokenization.None ) );
            yield return new ConfigurationAttribute(
                "startdate",
                type.@double,
                Present.Yes,
                filterOptions: new FilterOptions( Format.PipeSeparated, Tokenization.None ) );
            yield return new ConfigurationAttribute(
                "enddate",
                type.@double,
                Present.Yes,
                filterOptions: new FilterOptions( Format.PipeSeparated, Tokenization.None ) );
            yield return new ConfigurationAttribute(
                "_sortorder",
                type.@double,
                Present.Yes,
                sortOptions: new SortOptions( Normalization.CaseInsensitive ) );
            yield return new ConfigurationAttribute(
                "ad_key_included",
                type.@string,
                Present.Yes,
                filterOptions: new FilterOptions( Format.PipeSeparated, Tokenization.None ) );
            yield return new ConfigurationAttribute(
                "ad_key_excluded",
                type.@string,
                Present.Yes,
                filterOptions: new FilterOptions( Format.PipeSeparated, Tokenization.None ) );
            yield return new ConfigurationAttribute(
                "locale_filter",
                type.@string,
                Present.Yes,
                filterOptions: new FilterOptions( Format.PipeSeparated, Tokenization.None ) );
            yield return new ConfigurationAttribute(
                "product_url",
                type.@string,
                Present.Yes );
            if ( _appConfig.EnableVariants )
            {
                yield return new ConfigurationAttribute(
                    "variant_key",
                    type.@string,
                    Present.Yes,
                    filterOptions: new FilterOptions( Format.None, Tokenization.None ) );
            }
        }

        private IEnumerable<ConfigurationAttribute> GetAdAttributeConfig()
        {
            return _adAttributeHelper.ConvertToAttributes( new PromotionDto.PromotionLanguageDataTable(), new PromotionDto.PromotionDataTable(),
                                                           new CampaignDto.CampaignDataTable() );
        }

        private IEnumerable<ConfigurationAttribute> GetProductAttributes()
        {
            var attributes = GetStaticAttributes().ToDictionary( a => a.Name.ToLower() );

            var metaClasses = _metaData.GetAll();

            foreach ( var metaField in metaClasses.SelectMany( metaClass => metaClass ) )
            {
                CreateAttribute( metaField, attributes );
            }

            foreach ( var facetAttribute in FacetAttributes )
            {
                if ( attributes.ContainsKey( facetAttribute.Name.ToLower() ) )
                {
                    continue;
                }

                var name = facetAttribute.Name.ToLowerInvariant();
                attributes.Add(
                    name,
                    new ConfigurationAttribute(
                        name,
                        facetAttribute is RangeFacetAttribute ? type.@double : type.@string,
                        Present.No,
                        filterOptions: _appConfig.FilterAttributes.Contains( name ) ? new FilterOptions( Format.PipeSeparated, Tokenization.None ) : null,
                        sortOptions: new SortOptions( Normalization.CaseInsensitive ) ) );
            }

            return attributes.Values;
        }

        private void CreateAttribute( MetaFieldEx mf, IDictionary<string, ConfigurationAttribute> attributes )
        {
            var attrName = mf.Name.ToLower();
            var navigable = IsNavigable( mf.Name );
            if ( mf.AllowSearch || navigable )
            {
                ConfigurationAttribute existing;
                ConfigurationAttribute a;
                if ( attributes.TryGetValue( attrName, out existing ) )
                {
                    attributes.Remove( attrName );

                    a = new ConfigurationAttribute(
                        existing.Name,
                        existing.Type,
                        existing.Present,
                        mf.AllowSearch
                            ? new SearchOptions(
                                  existing.SearchOptions != null ? existing.SearchOptions.Locale : null,
                                  existing.SearchOptions != null ? existing.SearchOptions.Format : Format.PipeSeparated,
                                  existing.SearchOptions != null && existing.SearchOptions.MatchSuffix,
                                  IsSuggestable( attrName ) )
                            : existing.SearchOptions,
                        existing.FilterOptions != null
                            ? new FilterOptions( existing.FilterOptions.Format, Tokenization.CaseInsensitive )
                            : null,
                        existing.SortOptions );
                }
                else
                {
                    var format = GetFormat( mf );
                    a = new ConfigurationAttribute(
                        attrName,
                        GetType( mf ),
                        mf.Presentable ? Present.Yes : Present.No,
                        mf.AllowSearch ? new SearchOptions( null, format, false, IsSuggestable( attrName ) ) : null,
                        _appConfig.FilterAttributes.Contains( attrName ) ? new FilterOptions( format, GetTokenization( mf ) ) : null,
                        new SortOptions( Normalization.CaseInsensitive ) );
                }
                attributes.Add( attrName, a );
            }
        }

        private static Format GetFormat( MetaFieldEx mf )
        {
            return mf.DataType == MetaDataType.LongHtmlString && mf.Tokenized ? Format.Html : Format.PipeSeparated;
        }

        private static Tokenization GetTokenization( MetaFieldEx mf )
        {
            if ( mf.Tokenized )
            {
                switch ( mf.DataType )
                {
                    case MetaDataType.LongHtmlString:
                    case MetaDataType.LongString:
                    case MetaDataType.NText:
                    case MetaDataType.NVarChar:
                    case MetaDataType.ShortString:
                    case MetaDataType.Text:
                        return Tokenization.Words;
                }
            }
            return Tokenization.CaseInsensitive;
        }

        private static type GetType( MetaFieldEx mf )
        {
            switch ( mf.DataType )
            {
                case MetaDataType.Bit:
                case MetaDataType.Binary:
                case MetaDataType.Decimal:
                case MetaDataType.Float:
                case MetaDataType.Int:
                case MetaDataType.Integer:
                case MetaDataType.Money:
                case MetaDataType.Numeric:
                case MetaDataType.Real:
                case MetaDataType.SmallInt:
                case MetaDataType.SmallMoney:
                case MetaDataType.TinyInt:
                    return type.@double;
                case MetaDataType.BigInt:
                    return type.@long;
            }
            return type.@string;
        }

        private bool IsSuggestable( string attribute )
        {
            return !_appConfig.SuggestAttributes.Any() || _appConfig.SuggestAttributes.Contains( attribute );
        }

        private bool IsNavigable( string attribute )
        {
            return _facetAttributesIndex.ContainsKey( attribute.ToLower() );
        }

        private IEnumerable<FacetAttribute> GetFacetAttributes()
        {
            ESalesSearchConfig searchConfig;
            using ( var filterConfiguration = _fileSystem.Open( _appConfig.FilterConfigurationFile, FileMode.Open, FileAccess.Read ) )
            {
                searchConfig = (ESalesSearchConfig) new XmlSerializer( typeof( ESalesSearchConfig ) ).Deserialize( filterConfiguration );
            }

            var dict = new Dictionary<string, FacetAttribute>();
            // Memorize filter structure
            foreach ( ESalesFilter filter in searchConfig.SearchFilters.Filter )
            {
                var facets = new List<IFacet>();
                string name = filter.Field;

                var priceFAs = new Dictionary<string, List<IFacet>>();

                var attrDescriptions = new Dictionary<string, string>();
                string defaultLocale = filter.Descriptions.DefaultLocale;
                foreach ( ESalesDescription desc in filter.Descriptions.Description )
                {
                    attrDescriptions[desc.Locale] = desc.Value;
                }

                if ( filter.Values.SimpleValue.Count > 0 )
                {
                    foreach ( ESalesSimpleValue v in filter.Values.SimpleValue )
                    {
                        var descriptions = new Dictionary<string, string>();
                        foreach ( ESalesDescription desc in v.Descriptions.Description )
                        {
                            descriptions[desc.Locale] = desc.Value;
                        }
                        facets.Add( new FacetAttribute.StringFacet( v.Key, v.Value, descriptions, v.Descriptions.DefaultLocale ) );
                    }
                    if ( facets.Count > 0 )
                    {
                        dict[name] = new StringFacetAttribute( name, facets, attrDescriptions, defaultLocale );
                    }
                }
                else if ( filter.Values.RangeValue.Count > 0 )
                {
                    foreach ( ESalesRangeValue v in filter.Values.RangeValue )
                    {
                        var descriptions = new Dictionary<string, string>();
                        foreach ( ESalesDescription desc in v.Descriptions.Description )
                        {
                            descriptions[desc.Locale] = desc.Value;
                        }
                        facets.Add( new FacetAttribute.RangeFacet( v.LowerBound, v.UpperBound, true, false, v.Key, descriptions, v.Descriptions.DefaultLocale ) );
                    }
                    if ( facets.Count > 0 )
                    {
                        dict[name] = new RangeFacetAttribute( name, facets, attrDescriptions, defaultLocale );
                    }
                }
                else if ( filter.Values.PriceRangeValue.Count > 0 )
                {
                    foreach ( ESalesPriceRangeValue v in filter.Values.PriceRangeValue )
                    {
                        string attrName = string.Format( "{0}{1}", name.ToLower(), v.Currency.ToLower() );
                        if ( !( priceFAs.ContainsKey( attrName ) ) )
                        {
                            priceFAs[attrName] = new List<IFacet>();
                        }
                        var descriptions = new Dictionary<string, string>();
                        foreach ( ESalesDescription desc in v.Descriptions.Description )
                        {
                            descriptions[desc.Locale] = desc.Value;
                        }
                        priceFAs[attrName].Add( new FacetAttribute.RangeFacet( v.LowerBound, v.UpperBound, v.LowerBoundIncluded, v.UpperBoundIncluded, v.Key.ToLower(),
                                                                               descriptions, v.Descriptions.DefaultLocale ) );
                    }
                    foreach ( string fa in priceFAs.Keys )
                    {
                        if ( priceFAs.Count > 0 )
                        {
                            dict[fa] = new RangeFacetAttribute( fa, priceFAs[fa], attrDescriptions, defaultLocale );
                        }
                    }
                }

            }
            return dict.Values;
        }
    }
}
