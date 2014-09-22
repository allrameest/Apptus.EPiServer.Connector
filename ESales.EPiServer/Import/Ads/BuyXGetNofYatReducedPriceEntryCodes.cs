using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Apptus.ESales.EPiServer.Util;

namespace Apptus.ESales.EPiServer.Import.Ads
{
    internal class BuyXGetNofYatReducedPriceEntryCodes : IPromotionEntryCodes
    {
        private IEnumerable<string> _entries;
        private bool _exclude;

        public string PromotionType { get { return "BuyXGetNofYatReducedPrice"; } }

        public IEnumerable<string> Included( Promotion promotion )
        {
            if ( _entries == null )
            {
                DeserializeFrom( promotion );
            }

            return !_exclude ? _entries : Enumerable.Empty<string>();
        }

        public IEnumerable<string> Excluded( Promotion promotion )
        {
            if ( _entries == null )
            {
                DeserializeFrom( promotion );
            }

            return _exclude ? _entries : Enumerable.Empty<string>();
        }

        private void DeserializeFrom( Promotion promotion )
        {
            var settings =
                (Apps_Marketing_Promotions_BuyXGetNofYatReducedPrice.Settings)
                new XmlSerializer( typeof( Apps_Marketing_Promotions_BuyXGetNofYatReducedPrice.Settings ) ).Deserialize( new MemoryStream( promotion.PromotionRow.Params ) );

            if ( settings == null )
            {
                _entries = Enumerable.Empty<string>();
                return;
            }
            _entries = settings.EntryXSkuSet.Split( ",", StringSplitOptions.RemoveEmptyEntries ).Append( settings.EntryYFilter );
            _exclude = settings.Exclude;
        }
    }
}