using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Apptus.ESales.EPiServer.DataAccess;

namespace Apptus.ESales.EPiServer.Import.Ads
{
    internal class BuySKUFromCategoryXGetDiscountedShippingEntryCodes : IPromotionEntryCodes
    {
        private readonly ICatalogSystemMapper _catalogSystemMapper;

        public BuySKUFromCategoryXGetDiscountedShippingEntryCodes( ICatalogSystemMapper catalogSystemMapper )
        {
            _catalogSystemMapper = catalogSystemMapper;
        }

        public string PromotionType { get { return "BuySKUFromCategoryXGetDiscountedShipping"; } }

        public IEnumerable<string> Included( Promotion promotion )
        {
            var settings =
                (Apps_Marketing_Promotions_BuySKUFromCategoryXGetDiscountedShipping.Settings)
                new XmlSerializer( typeof( Apps_Marketing_Promotions_BuySKUFromCategoryXGetDiscountedShipping.Settings ) ).Deserialize( new MemoryStream( promotion.PromotionRow.Params ) );
            if ( settings == null )
            {
                return Enumerable.Empty<string>();
            }
            var nodeId = _catalogSystemMapper.GetCatalogNode( settings.CategoryCode ).CatalogNodeId;
            var entryIds = _catalogSystemMapper.GetCatalogRelations( 0, nodeId ).Select( r => r.CatalogEntryId );
            return entryIds.Select( ei => _catalogSystemMapper.GetCatalogEntry( ei ) ).Where( e => e != null ).Select( e => e.Code );
        }

        public IEnumerable<string> Excluded( Promotion promotion )
        {
            return Enumerable.Empty<string>();
        }
    }
}