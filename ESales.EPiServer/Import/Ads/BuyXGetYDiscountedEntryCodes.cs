using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Apptus.ESales.EPiServer.Import.Ads
{
    internal class BuyXGetYDiscountedEntryCodes : IPromotionEntryCodes
    {
        public string PromotionType { get { return "BuyXGetYDiscounted"; } }

        public IEnumerable<string> Included( Promotion promotion )
        {
            var settings =
                (Apps_Marketing_Promotions_Paired_ConfigControl.Settings)
                new XmlSerializer( typeof( Apps_Marketing_Promotions_Paired_ConfigControl.Settings ) ).Deserialize( new MemoryStream( promotion.PromotionRow.Params ) );

            if ( settings == null )
            {
                yield break;
            }

            if ( !string.IsNullOrWhiteSpace( settings.EntryXFilter ) )
            {
                yield return settings.EntryXFilter;
            }

            if ( !string.IsNullOrWhiteSpace( settings.EntryYFilter ) )
            {
                yield return settings.EntryYFilter;
            }

        }

        public IEnumerable<string> Excluded( Promotion promotion )
        {
            return Enumerable.Empty<string>();
        }
    }
}