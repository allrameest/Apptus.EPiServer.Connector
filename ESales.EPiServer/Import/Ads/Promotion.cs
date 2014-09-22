using System;
using Mediachase.Commerce.Marketing.Dto;

namespace Apptus.ESales.EPiServer.Import.Ads
{
    /// <summary>
    /// One promotion for one language as defined by the EPiServer Commerce dto's.
    /// </summary>
    public class Promotion
    {
        /// <summary>
        /// Creates a new language specific promotion.
        /// </summary>
        /// <param name="promotionLanguageRow">The language specific data.</param>
        /// <param name="promotionRow">The promotion specific data.</param>
        /// <param name="campaignRow">The campaign specific data.</param>
        public Promotion( PromotionDto.PromotionLanguageRow promotionLanguageRow, PromotionDto.PromotionRow promotionRow, CampaignDto.CampaignRow campaignRow )
        {
            if ( promotionLanguageRow == null )
            {
                throw new ArgumentNullException( "promotionLanguageRow" );
            }
            if ( promotionRow == null )
            {
                throw new ArgumentNullException( "promotionRow" );
            }
            if ( campaignRow == null )
            {
                throw new ArgumentNullException( "campaignRow" );
            }
            PromotionLanguageRow = promotionLanguageRow;
            PromotionRow = promotionRow;
            CampaignRow = campaignRow;
        }

        /// <summary>
        /// The language specific data.
        /// </summary>
        public PromotionDto.PromotionLanguageRow PromotionLanguageRow { get; private set; }

        /// <summary>
        /// The promotion specific data.
        /// </summary>
        public PromotionDto.PromotionRow PromotionRow { get; private set; }

        /// <summary>
        /// The campaign specific data.
        /// </summary>
        public CampaignDto.CampaignRow CampaignRow { get; private set; }
    }
}
