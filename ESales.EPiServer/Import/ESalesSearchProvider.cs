using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using Apptus.ESales.Connector;
using Apptus.ESales.EPiServer.DataAccess;
using Apptus.ESales.EPiServer.Extensions;
using Apptus.ESales.EPiServer.Import.Ads;
using Apptus.ESales.EPiServer.Import.Configuration;
using Apptus.ESales.EPiServer.Util;
using Autofac;
using Mediachase.Search;
using Apptus.ESales.EPiServer.Config;
using Mediachase.Search.Extensions;

namespace Apptus.ESales.EPiServer.Import
{
    ///<summary>
    /// Implementation of <c>Mediachase.Search.SearchProvider</c> for using Apptus eSales as a search engine.
    ///</summary>
    public class ESalesSearchProvider : SearchProvider
    {
        private IContainer _container;
        private IConfiguration _configuration;

        /// <summary>
        /// Initializes the provider.
        /// </summary>
        /// <param name="name">The friendly name of the provider.</param>
        /// <param name="config">A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The name of the provider is null.
        /// </exception>
        public override void Initialize( string name, NameValueCollection config )
        {
            try
            {
                if ( String.IsNullOrEmpty( name ) )
                {
                    name = "ESalesSearchProvider";
                }
                base.Initialize( name, config );

                Build();
                _configuration = _container.Resolve<IConfiguration>();
            }
            catch(Exception e)
            {
                throw e.Unwrap();
            }
        }

        private void Build()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<AdAttributeHelper>();
            builder.RegisterType<AppConfig>().As<IAppConfig>();
            builder.RegisterType<MetaDataMapper>().As<IMetaDataMapper>();
            builder.RegisterType<FileSystem>().As<IFileSystem>();
            builder.RegisterType<Configuration.Configuration>().As<IConfiguration>().SingleInstance();
            _container = builder.Build();
        }

        ///<summary>
        /// This method does nothing, the indexes are built separately in <see cref="ESalesCatalogIndexBuilder"/>
        ///</summary>
        public override void Index( string applicationName, string scope, ISearchDocument document )
        {
        }

        ///<summary>
        /// This method does nothing, the indexes are built separately in <see cref="ESalesCatalogIndexBuilder"/>
        ///</summary>
        public override int Remove( string applicationName, string scope, string key, string value )
        {
            return default( int );
        }

        ///<summary>
        /// This method does nothing, the indexes are built separately in <see cref="ESalesCatalogIndexBuilder"/>
        ///</summary>
        public override void RemoveAll( string applicationName, string scope )
        {
        }

        ///<summary>
        /// This method does nothing, the indexes are built separately in <see cref="ESalesCatalogIndexBuilder"/>
        ///</summary>
        public override void Commit( string applicationName )
        {
        }

        ///<summary>
        /// This method does nothing, the indexes are built separately in <see cref="ESalesCatalogIndexBuilder"/>
        ///</summary>
        public override void Close( string applicationName, string scope )
        {
        }

        ///<summary>
        /// This property does nothing, the indexes are built separately in <see cref="ESalesCatalogIndexBuilder"/>
        ///</summary>
        public override string QueryBuilderType
        {
            get { return default( string ); }
        }

        ///<summary>
        /// Queries eSales based on the criteria.
        ///</summary>
        ///<param name="applicationName">The application name for the query.</param>
        ///<param name="criteria">The search criteria.</param>
        ///<returns>Search results, including a product count, product listing and navigation panel.</returns>
        ///<exception cref="ProviderException">If the application name does not match the application name configured for eSales.</exception>
        public override ISearchResults Search( string applicationName, ISearchCriteria criteria )
        {
            criteria.Locale = criteria.Locale.ToESalesLocale();
            var pc = PerformQuery( criteria );
            return pc == null ? new ESalesSearchResult( criteria ) : new ESalesSearchResult( _configuration, pc, criteria );
        }

        private PanelContent PerformQuery( ISearchCriteria criteria )
        {
            IList<Filter> filters = new List<Filter> { FilterBuilder.Attribute( "locale_filter", criteria.Locale ) };
            var arguments = new ArgMap();

            AddFilterCriteria( criteria, filters );
            AddCatalogEntrySearchCriteria( criteria, filters, arguments );
            AddSortArgument( criteria, arguments );
            AddFacetAttributes( arguments );

            var filter = filters.Count == 1 ? filters[0] : FilterBuilder.And( filters.ToArray() );
            arguments.Add( "filter", filter );
            arguments.Add( "window_first", criteria.StartingRecord + 1 );
            arguments.Add( "window_last", criteria.RecordsToRetrieve + criteria.StartingRecord );

            var p = Util.ESales.Connector.Session( Util.Session.Key ).Panel( "/commerce-standard-search" );
            return p.RetrieveContent( arguments );
        }

        private void AddFacetAttributes( ArgMap arguments )
        {
            var builder = new StringBuilder();
            foreach ( var facetAttribute in _configuration.FacetAttributes )
            {
                if ( builder.Length > 0 )
                {
                    builder.Append( "," );
                }
                builder.Append( facetAttribute.Name + "__facet" );
            }
            arguments.Add( "filter_attributes", builder.ToString() );
        }

        private static void AddFilterCriteria( ISearchCriteria criteria, IList<Filter> filters )
        {
            for ( var i = 0; i < criteria.ActiveFilterFields.Length; ++i )
            {
                var val = criteria.ActiveFilterValues.ElementAt( i );
                var field = criteria.ActiveFilterFields.ElementAt( i );
                if ( ( typeof( PriceRangeValue ) != val.GetType() || ( (PriceRangeValue) val ).currency.Equals( criteria.Currency, StringComparison.OrdinalIgnoreCase ) ) )
                {
                    var attributeFilter = CreateFilter( field, val );
                    if ( attributeFilter != null )
                    {
                        filters.Add( attributeFilter );
                    }
                }
            }
        }

        private void AddCatalogEntrySearchCriteria( ISearchCriteria criteria, IList<Filter> filters, ArgMap arguments )
        {
            if ( criteria is CatalogEntrySearchCriteria )
            {
                var c = criteria as CatalogEntrySearchCriteria;
                AddOrFilter( filters, c.CatalogNames, "_catalog" );
                AddOrFilter( filters, c.CatalogNodes, "_node" );
                AddOrFilter( filters, c.Outlines, "_outline" );
                AddOrFilter( filters, c.SearchIndex, "_metaclass" );
                AddOrFilter( filters, c.ClassTypes, "_classtype" );
                const string year0 = "0.0";
                const string yearFarInFuture = "99999999999999999";
                filters.Add( FilterBuilder.Range( "startdate", year0, ConvertToTimestamp( c.StartDate ).ToString( CultureInfo.InvariantCulture ), false, true ) );
                filters.Add( FilterBuilder.Range( "enddate", ConvertToTimestamp( c.EndDate ).ToString( CultureInfo.InvariantCulture ), yearFarInFuture, true, false ) );
                var searchAttributes = SearchAttributes();
                if ( !( string.IsNullOrEmpty( c.SearchPhrase ) || string.IsNullOrEmpty( searchAttributes ) ) )
                {
                    arguments.Add( "search_phrase", c.SearchPhrase );
                    arguments.Add( "search_attributes", searchAttributes );
                }
            }
        }

        private static void AddSortArgument( ISearchCriteria criteria, ArgMap arguments )
        {
            if ( criteria.Sort == null ) return;

            var searchSortFields = criteria.Sort.GetSort();
            if ( searchSortFields == null || searchSortFields.Length == 0 ) return;

            var builder = new StringBuilder();
            foreach ( var field in searchSortFields )
            {
                if ( builder.Length > 0 )
                {
                    builder.Append( "," );
                }
                builder.Append( field.FieldName ).Append( " " ).Append( field.IsDescending ? "desc" : "asc" );
            }
            arguments.Add( "sort_by", builder.ToString() );
        }

        private string SearchAttributes()
        {
            var configuration = _container.Resolve<IConfiguration>();
            return string.Join(
                ",",
                configuration.ProductAttributes
                             .Where( a => a.SearchOptions != null )
                             .Select( a => a.Name ) );
        }

        private static void AddOrFilter( ICollection<Filter> filters, IEnumerable sc, string attribute )
        {
            if ( sc == null ) return;

            IList<Filter> f = ( from string cn in sc
                                where cn != null
                                select cn.EndsWith( "*" )
                                     ? FilterBuilder.Attribute( attribute, cn.Substring( 0, cn.Length - 1 ) )
                                     : FilterBuilder.Attribute( attribute, cn ) ).ToList();
            if ( f.Count > 0 )
            {
                filters.Add( f.Count == 1 ? f.ElementAt( 0 ) : FilterBuilder.Or( f.ToArray() ) );
            }
        }

        private static Filter CreateFilter( string field, ISearchFilterValue val )
        {
            if ( typeof( RangeValue ) == val.GetType() )
            {
                return CreateFilter( field, (RangeValue) val );
            }
            if ( typeof( PriceRangeValue ) == val.GetType() )
            {
                return CreateFilter( field, (PriceRangeValue) val );
            }
            if ( typeof( SimpleValue ) == val.GetType() )
            {
                return CreateFilter( field, (SimpleValue) val );
            }
            return null;
        }

        private static Filter CreateFilter( string field, RangeValue val )
        {
            return FilterBuilder.Range( field, val.lowerbound, val.upperbound, val.lowerboundincluded, val.upperboundincluded );
        }

        private static Filter CreateFilter( string field, PriceRangeValue val )
        {
            return FilterBuilder.Range( field, val.lowerbound, val.upperbound, val.lowerboundincluded, val.upperboundincluded );
        }

        private static Filter CreateFilter( string field, SimpleValue val )
        {
            return FilterBuilder.Attribute( field, val.value );
        }

        


        private static double ConvertToTimestamp( DateTime value )
        {
            var span = ( value - new DateTime( 1970, 1, 1, 0, 0, 0, 0 ).ToLocalTime() );
            return span.TotalSeconds;
        }
    }
}
