using System.Collections.Generic;
using System.Linq;
using Apptus.ESales.EPiServer.Import.Ads;
using Mediachase.Commerce.Marketing.Dto;

namespace Apptus.ESales.EPiServer.DataAccess
{
    internal class PromotionDataTableMapper
    {
        private readonly PromotionDto.PromotionDataTable _promotionDataTable;
        private readonly PromotionDto.PromotionLanguageDataTable _promotionLanguageDataTable;
        private readonly CampaignDto.CampaignDataTable _campaignDataTable;

        public PromotionDataTableMapper( PromotionDto.PromotionLanguageDataTable promotionLanguageTable, PromotionDto.PromotionDataTable promotionTable,
                                         CampaignDto.CampaignDataTable campaignTable )
        {
            _promotionDataTable = promotionTable;
            _promotionLanguageDataTable = promotionLanguageTable;
            _campaignDataTable = campaignTable;
        }

        public PromotionDto.PromotionDataTable PromotionDataTable { get { return _promotionDataTable; } }

        public PromotionDto.PromotionLanguageDataTable PromotionLanguageDataTable { get { return _promotionLanguageDataTable; } }

        public CampaignDto.CampaignDataTable CampaignDataTable { get { return _campaignDataTable; } }

        public IEnumerable<Promotion> Promotions 
        {
            get
            {
                var promotionLanugagePromotionCampaign =
                from plp in
                    ( from pl in _promotionLanguageDataTable.Rows.Cast<PromotionDto.PromotionLanguageRow>()
                      join p in _promotionDataTable.Rows.Cast<PromotionDto.PromotionRow>() on int.Parse( pl["PromotionId"].ToString() ) equals int.Parse( p["PromotionId"].ToString() )
                      select new { PromotionLanguage = pl, Promotion = p } )
                join c in _campaignDataTable.Rows.Cast<CampaignDto.CampaignRow>() on int.Parse( plp.Promotion["CampaignId"].ToString() ) equals int.Parse( c["CampaignId"].ToString() )
                select new { plp.PromotionLanguage, plp.Promotion, Campaign = c };
                return promotionLanugagePromotionCampaign.Select( plpc => new Promotion( plpc.PromotionLanguage, plpc.Promotion, plpc.Campaign ) );
            }
        }
    }
}