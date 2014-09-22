using System.Collections.Generic;
using System.Linq;
using Apptus.ESales.EPiServer.DataAccess;
using Apptus.ESales.EPiServer.Util;

namespace Apptus.ESales.EPiServer.Import.Ads
{
    internal class PromotionEntryCodeProvider
    {
        private readonly ILookup<string, string> _included;
        private readonly ILookup<string, string> _excluded;
        private readonly HashSet<string> _adsWithIncluded;
        private readonly HashSet<string> _adsWithExcluded;

        public PromotionEntryCodeProvider( PromotionDataTableMapper promotionDataTableMapper , IEnumerable<IPromotionEntryCodes> promotionEntryCodes )
        {
            var promotions = promotionDataTableMapper.Promotions;
            var providers = promotionEntryCodes.ToDictionary( pec => pec.PromotionType );
            var adKeysWithCodes = ( from p in promotions
                                    where providers.ContainsKey( p.PromotionRow.PromotionType )
                                    let provider = providers[p.PromotionRow.PromotionType]
                                    select new
                                        {
                                            AdKey = AttributeHelper.CreateKey( p.PromotionRow.PromotionId.ToString(), p.PromotionLanguageRow.LanguageCode ),
                                            Included = provider.Included( p ),
                                            Excluded = provider.Excluded( p )
                                        } ).ToArray();
            _included = adKeysWithCodes
                .SelectMany( a => a.Included.Select( i => new { Included = i, a.AdKey } ) )
                .ToLookup( i => i.Included, i => i.AdKey );
            _excluded = adKeysWithCodes
                .SelectMany( a => a.Excluded.Select( e => new { Excluded = e, a.AdKey } ) )
                .ToLookup( e => e.Excluded, e => e.AdKey );
            _adsWithIncluded = new HashSet<string>( adKeysWithCodes.Where( a => a.Included.Any() ).Select( a => a.AdKey ) );
            _adsWithExcluded = new HashSet<string>( adKeysWithCodes.Where( a => a.Excluded.Any() ).Select( a => a.AdKey ) );
        }

        public IEnumerable<string> GetIncluded( string code )
        {
            return _included[code];
        }

        public IEnumerable<string> GetExcluded( string code )
        {
            return _excluded[code];
        }

        public bool AdKeyHasIncluded( string adKey )
        {
            return _adsWithIncluded.Contains( adKey );
        }
        
        public bool AdKeyHasExcluded( string adKey )
        {
            return _adsWithExcluded.Contains( adKey );
        }
    }
}
