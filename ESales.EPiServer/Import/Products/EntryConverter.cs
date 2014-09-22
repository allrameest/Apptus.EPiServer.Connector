using System;
using System.Collections.Generic;
using System.Linq;
using Apptus.ESales.EPiServer.DataAccess;
using Apptus.ESales.EPiServer.Import.Ads;
using Apptus.ESales.EPiServer.Import.Configuration;
using Apptus.ESales.EPiServer.Import.Formatting;
using Apptus.ESales.EPiServer.Util;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Pricing;
using Mediachase.MetaDataPlus;
using Mediachase.MetaDataPlus.Configurator;

namespace Apptus.ESales.EPiServer.Import.Products
{
    internal class EntryConverter
    {
        private readonly ICatalogSystemMapper _catalogSystem;
        private readonly IMetaDataMapper _metaData;
        private readonly IPriceServiceMapper _priceService;
        private readonly IKeyLookup _keyLookup;
        private readonly PromotionEntryCodeProvider _promotionProvider;
        private readonly IConfiguration _configuration;
        private readonly IUrlResolver _urlResolver;
        private readonly Formatter _formatter;
        private readonly HashSet<string> _standardAttributes;
        private readonly Dictionary<string, ConfigurationAttribute> _confProductAttributes;
        private readonly Dictionary<string, FacetAttribute> _confFacetAttributes;

        public EntryConverter( ICatalogSystemMapper catalogSystem, IMetaDataMapper metaData, IPriceServiceMapper priceService, IKeyLookup keyLookup,
                               PromotionEntryCodeProvider promotionProvider, IConfiguration configuration, IUrlResolver urlResolver, Formatter formatter )
        {
            _catalogSystem = catalogSystem;
            _metaData = metaData;
            _priceService = priceService;
            _keyLookup = keyLookup;
            _promotionProvider = promotionProvider;
            _configuration = configuration;
            _urlResolver = urlResolver;
            _formatter = formatter;
            _standardAttributes = new HashSet<string>( new[] { "locale" } );
            _confProductAttributes = _configuration.ProductAttributes.ToDictionary( pa => pa.Name );
            _confFacetAttributes = _configuration.FacetAttributes.ToDictionary( fa => fa.Name );
        }

        public IEnumerable<IEntity> Convert( CatalogEntryDto.CatalogEntryRow entry, IEnumerable<string> languages, CatalogDto.CatalogRow catalog,
                                             ESalesVariantHelper variantHelper )
        {
            var entities = GetEntities( entry, languages, catalog.Name, variantHelper );
            return entities;
        }

        private IEnumerable<IEntity> GetEntities( CatalogEntryDto.CatalogEntryRow entry, IEnumerable<string> languages, string catalogName,
                                          ESalesVariantHelper variantHelper )
        {
            foreach ( var language in languages )
            {
                var locale = language.ToESalesLocale();
                var attributes = new List<Attribute>
                    {
                        NewAttribute( "id", entry.CatalogEntryId + "_" + language ),
                        NewAttribute( "_id", entry.CatalogEntryId ),
                        NewAttribute( "code", entry.Code ),
                        NewAttribute( "name", entry.Name ),
                        NewAttribute( "_lang", language ),
                        NewAttribute( "locale", locale ),
                        NewAttribute( "locale_filter", locale ),
                        NewAttribute( "startdate", entry.StartDate ),
                        NewAttribute( "enddate", entry.EndDate ),
                        NewAttribute( "_classtype", entry.ClassTypeId.ToLowerInvariant() ),
                        NewAttribute( "_catalog", catalogName ),
                        NewAttribute( "product_url", _urlResolver.GetEntryUrl( entry, language ) )
                    };
                attributes.AddRange( GetPrices( entry ) );
                attributes.AddRange( GetNodeEntryRelations( catalogName, new CatalogEntryRowMapper(entry) ) );
                attributes.AddRange( GetMetaData( entry, language ) );
                AddAds( entry, attributes );
                AddFacets( attributes );
                
                var filteredAttributes = FilterConfiguredAttributes( attributes );

                yield return GetEntity( entry, variantHelper, language, filteredAttributes );
            }
        }

        private IEnumerable<Attribute> FilterConfiguredAttributes( IEnumerable<Attribute> attributes )
        {
            return attributes.Where( a => _confProductAttributes.ContainsKey( FromFacetName( a.Name ) ) || _standardAttributes.Contains( a.Name ) );
        }

        private void AddAds( CatalogEntryDto.CatalogEntryRow entry, List<Attribute> attributes )
        {
            AddIncludedAds( entry, attributes );
            AddExcludedAds( entry, attributes );
        }

        private void AddIncludedAds( CatalogEntryDto.CatalogEntryRow entry, List<Attribute> attributes )
        {
            AddAdAttribute( _promotionProvider.GetIncluded( entry.Code ), "ad_key_included", attributes );
        }

        private void AddExcludedAds( CatalogEntryDto.CatalogEntryRow entry, List<Attribute> attributes )
        {
            AddAdAttribute( _promotionProvider.GetExcluded( entry.Code ), "ad_key_excluded", attributes );
        }

        private void AddAdAttribute( IEnumerable<string> adKeys, string attributeName, List<Attribute> attributes )
        {
            var keys = adKeys as string[] ?? adKeys.ToArray();
            if ( keys.Any() )
            {
                attributes.Add( NewAttribute( attributeName, keys ) );
            }
        }

        private void AddFacets( List<Attribute> attributes )
        {
            var facetAttributes = GetFacets( attributes ).ToList();
            attributes.AddRange( facetAttributes );
        }

        private IEnumerable<Attribute> GetFacets( IEnumerable<Attribute> attributes )
        {
            foreach ( var attribute in attributes )
            {
                FacetAttribute facetAttribute;
                if ( _confFacetAttributes.TryGetValue( attribute.Name, out facetAttribute ) )
                {
                    var facetValues = attribute.Values.Select( v => facetAttribute.GetFacetFromDataValue( v ) ).Where( f => f != null ).Select( f => f.Value ).ToArray();
                    if ( facetValues.Any() )
                    {
                        yield return NewAttribute( ToFacetName( attribute.Name ), string.Join( "|", facetValues ) );
                    }
                }
            }
        }

        private IEntity GetEntity( CatalogEntryDto.CatalogEntryRow entry, ESalesVariantHelper variantHelper, string language,
                                          IEnumerable<Attribute> attributes )
        {
            var keyValue = _keyLookup.Value( entry, language );
            if ( variantHelper.IsVariant( entry.CatalogEntryId ) )
            {
                var productKey = _keyLookup.Value( _catalogSystem.GetCatalogEntry( variantHelper.GetParentProduct( entry.CatalogEntryId ) ), language );
                return new Variant( keyValue, productKey, attributes );
            }
            return new Product( keyValue, attributes );
        }

        private IEnumerable<Attribute> GetMetaData( CatalogEntryDto.CatalogEntryRow entryRow, string language )
        {
            if ( entryRow.MetaClassId == 0 )
            {
                yield break;
            }
            
            var metaClass = _metaData.LoadMetaClassCached( entryRow.MetaClassId, language );
            if ( metaClass == null )
            {
                yield break;
            }

            yield return NewAttribute( "_metaclass", metaClass.Name );

            var metaFieldValues = _metaData.GetMetaFieldValues( entryRow );
            if ( metaFieldValues == null )
            {
                yield break;
            }

            if ( metaFieldValues["DisplayName"] == null || metaFieldValues["DisplayName"].ToString() == "" )
            {
                metaFieldValues["DisplayName"] = entryRow.Name;
            }

            foreach ( var field in 
                metaClass
                    .Where( f => metaFieldValues.Contains( f.Name ) )
                    .Select( f => new { MetaField = f, Value = GetMetaFieldValues( f, metaFieldValues[f.Name] ).Where( v => v != null ) } )
                    .Where( fv => fv.Value.Any() )
                    .Select( fv => NewAttribute( fv.MetaField.Name.ToLowerInvariant(), fv.Value ) ) )
            {
                yield return field;
            }
        }

        private static IEnumerable<object> GetMetaFieldValues( MetaFieldEx field, object value )
        {
            if ( value == null )
            {
                return Enumerable.Empty<object>();
            }

            var metaDictionaryItem = value as MetaDictionaryItem;
            if ( metaDictionaryItem != null )
            {
                return new[] { metaDictionaryItem.Value };
            }
            
            if ( field.DataType != MetaDataType.DictionaryMultiValue && field.DataType != MetaDataType.EnumMultiValue )
            {
                return new[] { value };
            }

            var metaDictionaryItems = value as MetaDictionaryItem[];
            if ( metaDictionaryItems != null )
            {
                return metaDictionaryItems.Select( i => i.Value );
            }

            var metaStringDictionary = value as MetaStringDictionary;
            if ( metaStringDictionary != null )
            {
                return metaStringDictionary.Keys.Cast<string>();
            }
            return Enumerable.Empty<object>();
        }

        protected internal IEnumerable<Attribute> GetNodeEntryRelations( string catalogName, ICatalogEntryRowMapper entryRow )
        {
            var nodeEntryRelations = from relationRow in entryRow.GetNodeEntryRelationRows().Where( r => r.CatalogId == entryRow.CatalogId )
                                     let node = _catalogSystem.GetCatalogNode( relationRow.CatalogNodeId )
                                     where node != null
                                     select new { Row = relationRow, Node = node };

            var isInStock = entryRow.InventoryRow != null && entryRow.InventoryRow.InStockQuantity > 0;
            var loadedSortOrder = false;
            var allNodes = new HashSet<string>();
            var allOutlines = new List<string>();
            foreach ( var nodeEntry in nodeEntryRelations )
            {
                if ( !loadedSortOrder )
                {
                    yield return NewAttribute("_sortorder", isInStock ? nodeEntry.Row.SortOrder : 0);
                    loadedSortOrder = true;
                }
                List<string> outlines;
                HashSet<string> nodes;
                ExtractNodesAndOutlines( entryRow.CatalogId, catalogName, nodeEntry.Node, out outlines, out nodes );
                allNodes.UnionWith( nodes );
                allOutlines.AddRange( outlines );
            }

            yield return NewAttribute( "_node", allNodes );
            yield return NewAttribute( "_outline", allOutlines );
        }

        private HashSet<CatalogNode> GetParentNodes( int catalogId, CatalogNode node )
        {
            var parentNodeIds = new HashSet<int> { node.ParentNodeId };
          
            var nodeRelationTable = _catalogSystem.GetCatalogNodeRelations( catalogId );
            foreach ( CatalogRelationDto.CatalogNodeRelationRow relationRow in nodeRelationTable.Rows )
            {
                if (relationRow.ChildNodeId.Equals( node.CatalogNodeId ))
                {
                    parentNodeIds.Add( relationRow.ParentNodeId );
                }
            }

            var parentNodes = new HashSet<CatalogNode>();
            foreach ( var id in parentNodeIds )
            {
                if ( id == 0 ) { continue; }
                var parentNode = _catalogSystem.GetCatalogNode( id );
                if ( parentNode == null ) { continue; }
                parentNodes.Add( parentNode );
            }

            return parentNodes;
        }

        private void ExtractNodesAndOutlines( int catalogId, string catalogName, CatalogNode rootNode, out List<string> outline, out HashSet<string> nodes )
        {
            var outlines = new List<string>();
            var visitedNodes = new HashSet<string>();
            var allNodes = new HashSet<string>();
            var seenPaths = new List<string>();
            AppendParents( catalogId, null, rootNode, visitedNodes, allNodes, seenPaths );
            foreach ( var path in seenPaths )
            {
                outlines.Add( catalogName + "/" + path );
            }

            outline = outlines;
            nodes = allNodes;
        }

        private void AppendParents( int catalogId, string path, CatalogNode node, HashSet<string> visitedNodes, HashSet<string> allNodes, List<string> fullPaths )
        {
            visitedNodes.Add( node.ID );
            allNodes.Add( node.ID );
            path = path == null ? node.ID : node.ID + "/" + path;
            var parentNodes = GetParentNodes( catalogId, node );
            if ( parentNodes.Count == 0 )
            {
                fullPaths.Add( path );
            }

            foreach ( var parentNode in parentNodes )
            {
                if ( !visitedNodes.Contains( parentNode.ID ) )
                {
                    var visitedNodesInBranch = new HashSet<string>();
                    visitedNodesInBranch.UnionWith( visitedNodes );
                    AppendParents( catalogId, path, parentNode, visitedNodesInBranch, allNodes, fullPaths );
                }
            }
        }

        private IEnumerable<Attribute> GetPrices( CatalogEntryDto.CatalogEntryRow entry )
        {
            return GetLowestPricePerName( new CatalogKey( entry ) );
        }

        private IEnumerable<Attribute> GetLowestPricePerName( CatalogKey key )
        {
            return _priceService.GetCatalogEntryPrices( key )
                                .SelectMany( CreatePrices )
                                .GroupBy( pt => pt.Item1 )
                                .Select( ptg => ptg.OrderBy( pt => pt.Item2 ).First() )
                                .Select( pt => NewAttribute( pt.Item1, pt.Item2 ) );
        }

        private static IEnumerable<Tuple<string, decimal>> CreatePrices( IPriceValue price )
        {
            const string listPrice = "listprice";
            const string salePrice = "saleprice";
            var amount = price.UnitPrice.Amount;
            yield return new Tuple<string, decimal>( GetPriceFieldName( salePrice, price, true ), amount );
            yield return new Tuple<string, decimal>( GetPriceFieldName( salePrice, price, false ), amount );
            if ( price.MinQuantity == 0 && price.CustomerPricing.PriceTypeId == CustomerPricing.PriceType.AllCustomers )
            {
                yield return new Tuple<string, decimal>( GetPriceFieldName( listPrice, price, true ), amount );
                yield return new Tuple<string, decimal>( GetPriceFieldName( listPrice, price, false ), amount );
            }
        }

        private static string GetPriceFieldName( string prefix, IPriceValue price, bool includeMarket )
        {
            return ( includeMarket
                         ? string.Format( "{0}{1}_{2}", prefix, price.UnitPrice.Currency.CurrencyCode, price.MarketId.Value )
                         : string.Format( "{0}{1}", prefix, price.UnitPrice.Currency.CurrencyCode ) )
                .ToLowerInvariant();
        }

        private Attribute NewAttribute( string name, object value )
        {
            return new Attribute( name, _formatter.Format( value ) );
        }

        private Attribute NewAttribute( string name, IEnumerable<object> values )
        {
            return new Attribute( name, values.Select( v => _formatter.Format( v ) ) );
        }

        private static string ToFacetName( string attributeName )
        {
            return attributeName + "__facet";
        }

        private static string FromFacetName( string attributeName )
        {
            return attributeName.EndsWith( "__facet" ) ? attributeName.Substring( 0, attributeName.Length - 7 ) : attributeName;
        }
    }
}
