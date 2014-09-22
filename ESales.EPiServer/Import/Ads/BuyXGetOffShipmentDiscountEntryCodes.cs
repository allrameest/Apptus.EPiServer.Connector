using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Apptus.ESales.EPiServer.Util;

namespace Apptus.ESales.EPiServer.Import.Ads
{
    internal class BuyXGetOffShipmentDiscountEntryCodes : IPromotionEntryCodes
    {
        public string PromotionType { get { return "BuyXGetOffShipmentDiscount"; } }

        public IEnumerable<string> Included( Promotion promotion )
        {
            var settings =
                (Apps_Marketing_Promotions_BuyXGetDiscountedShipipng.Settings)
                new XmlSerializer( typeof( Apps_Marketing_Promotions_BuyXGetDiscountedShipipng.Settings ) ).Deserialize( new MemoryStream( promotion.PromotionRow.Params ) );
            if ( settings == null )
            {
                return Enumerable.Empty<string>();
            }
            var separator = settings.SkuDelimiter.ToString();
            return settings.RuleSkuSet.Split( separator, StringSplitOptions.RemoveEmptyEntries );
        }

        public IEnumerable<string> Excluded( Promotion promotion )
        {
            return Enumerable.Empty<string>();
        }
    }
}