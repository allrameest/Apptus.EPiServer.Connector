using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Apptus.ESales.EPiServer.Util;

namespace Apptus.ESales.EPiServer.Import.Ads
{
    internal class BuyXGetOffDiscountEntryCodes : IPromotionEntryCodes
    {
        public string PromotionType { get { return "BuyXGetOffDiscount"; } }

        public IEnumerable<string> Included( Promotion promotion )
        {
            var settings =
                (Apps_Marketing_Promotions_BuyXGetOffDiscount.Settings)
                new XmlSerializer( typeof( Apps_Marketing_Promotions_BuyXGetOffDiscount.Settings ) ).Deserialize( new MemoryStream( promotion.PromotionRow.Params ) );

            if ( settings == null || string.IsNullOrWhiteSpace( settings.RuleSkuSet ) )
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