using System.Linq;
using Apptus.ESales.Connector;
using Apptus.ESales.EPiServer.Config;
using Apptus.ESales.EPiServer.Import.Configuration;
using Apptus.ESales.EPiServer.Util;
using Mediachase.Search;
using Mediachase.Search.Extensions;

namespace Apptus.ESales.EPiServer.Import
{
    ///<summary>
    /// An eSales search result.
    ///</summary>
    internal class ESalesSearchResult : SearchResults
    {
        private ISearchFacetGroup[] _facetGroups;

        ///<summary>
        /// An empty search result.
        ///</summary>
        ///<param name="criteria">The search criteria.</param>
        public ESalesSearchResult( ISearchCriteria criteria )
            : base( EmptySearchDocuments(), criteria )
        {
        }

        ///<summary>
        /// A search result.
        ///</summary>
        ///<param name="configuration">Configuration, used for facet parsing.</param>
        ///<param name="panelContent">The panel content from eSales.</param>
        ///<param name="criteria">The search criteria.</param>
        public ESalesSearchResult( IConfiguration configuration, PanelContent panelContent, ISearchCriteria criteria )
            : base( CreateSearchDocuments( panelContent ), criteria )
        {
            PopulateFacets( panelContent, configuration, criteria.Locale, criteria.Currency );
        }

        ///<summary>
        /// eSales facets.
        ///</summary>
        public override ISearchFacetGroup[] FacetGroups { get { return _facetGroups; } }

        private void PopulateFacets(PanelContent panelContent, IConfiguration configuration, string locale, string currency)
        {
            if (panelContent == null || !panelContent.HasSubpanel("facets")) return;
            _facetGroups = ValueConverter.GetFacetGroups(configuration, panelContent.Subpanel("facets"), locale, currency, null);
        }

        private static ISearchDocuments EmptySearchDocuments()
        {
            return new SearchDocuments { TotalCount = 0 };
        }

        private static ISearchDocuments CreateSearchDocuments( PanelContent pc )
        {
            if ( pc == null || !pc.HasSubpanel( "search-results" ) ) return EmptySearchDocuments();
            pc = pc.Subpanel( "search-results" );
            if ( !pc.HasSubpanel( "search-hits" ) ) return EmptySearchDocuments();
            pc = pc.Subpanel( "search-hits" );
            if ( !pc.HasResult ) return EmptySearchDocuments();

            var documents = new SearchDocuments();
            foreach ( var product in pc.ResultAsProducts() )
            {
                var productDocument = new SearchDocument();
                foreach ( var field in product.Select( a => new SearchField( a.Name, a.Value ) ) )
                {
                    productDocument.Add( field );
                }
                documents.Add( productDocument );

                foreach ( var variant in product.VariantList )
                {
                    var variantDocument = new SearchDocument();
                    foreach ( var field in variant.Select( a => new SearchField( a.Name, a.Value ) ) )
                    {
                        variantDocument.Add( field );
                    }
                    documents.Add( variantDocument );
                }
            }

            documents.TotalCount = documents.Count; //Bug in SearchResults, SearchDocuments.TotalCount is never set implicitly.
            return documents;
        }
    }
}