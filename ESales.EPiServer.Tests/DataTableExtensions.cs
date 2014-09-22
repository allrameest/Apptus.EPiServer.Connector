using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Marketing.Dto;

namespace ESales.EPiServer.Tests
{
    internal static class DataTableExtensions
    {
        private static readonly Guid ApplicationId = Guid.NewGuid();

        public static CatalogDto.CatalogDataTable WithRow( this CatalogDto.CatalogDataTable table, string name, Guid applicationId = default( Guid ) )
        {
            var row = table.NewCatalogRow();
            row.Name = name;
            row.StartDate = DateTime.MinValue;
            row.EndDate = DateTime.MaxValue;
            row.DefaultCurrency = "sek";
            row.WeightBase = "g";
            row.DefaultLanguage = "sv-se";
            row.IsPrimary = true;
            row.IsActive = true;
            row.Created = DateTime.MinValue;
            row.Modified = DateTime.MinValue;
            row.CreatorId = "foo";
            row.ModifierId = "foo";
            row.SortOrder = 0;
            row.ApplicationId = applicationId == default( Guid ) ? ApplicationId : applicationId;
            row.Owner = "foo";
            table.AddCatalogRow( row );
            return table;
        }

        public static IEnumerable<CatalogNodeDto.CatalogNodeRow> WithRow( this CatalogNodeDto.CatalogNodeDataTable table, int catalogNodeId = 0, string code = null )
        {
            var row = table.NewCatalogNodeRow();

            row.ApplicationId = ApplicationId;
            row.CatalogId = 0;
            row.EndDate = DateTime.MaxValue;
            row.IsActive = true;
            row.MetaClassId = 0;
            row.Name = "";
            row.ParentNodeId = 0;
            row.SortOrder = 0;
            row.StartDate = DateTime.MinValue;
            row.TemplateName = "";

            row.CatalogNodeId = catalogNodeId;
            row.Code = code;
            table.AddCatalogNodeRow( row );
            return table;
        }

        public static CatalogEntryDto.CatalogEntryDataTable WithRow( this CatalogEntryDto.CatalogEntryDataTable table, int catalogEntryId = 0, string code = null,
                                                                     string name = null )
        {
            var row = table.NewCatalogEntryRow();

            row.CatalogId = 0;
            row.ApplicationId = ApplicationId;
            row.StartDate = DateTime.MinValue;
            row.EndDate = DateTime.MaxValue;
            row.Name = name ?? "";
            row.ClassTypeId = "";
            row.IsActive = true;
            row.MetaClassId = 0;
            row.SerializedData = new byte[0];
            row.TemplateName = "";

            row.CatalogEntryId = catalogEntryId;
            row.Code = code;
            table.AddCatalogEntryRow( row );
            return table;
        }

        public static CatalogRelationDto.NodeEntryRelationDataTable WithRow( this CatalogRelationDto.NodeEntryRelationDataTable table, int catalogEntryId = 0,
                                                                             int catalogNodeId = 0 )
        {
            var row = table.NewNodeEntryRelationRow();

            row.CatalogId = 0;
            row.SortOrder = 0;

            row.CatalogEntryId = catalogEntryId;
            row.CatalogNodeId = catalogNodeId;
            table.AddNodeEntryRelationRow( row );
            return table;
        }

        public static PromotionDto.PromotionLanguageDataTable WithRow( this PromotionDto.PromotionLanguageDataTable table, int promotionLanguageId, int promotionId,
                                                                       string languageCode,
                                                                       string displayName )
        {
            var row = table.NewPromotionLanguageRow();

            row.DisplayName = displayName;
            row.LanguageCode = languageCode;
            row.PromotionId = promotionId;
            row.PromotionLanguageId = promotionLanguageId;

            table.AddPromotionLanguageRow( row );
            return table;
        }

        public static PromotionDto.PromotionDataTable WithRow( this PromotionDto.PromotionDataTable table, int promotionId, string name, string promotionType,
                                                               int campaignId, Guid applicationId = default( Guid ), string status = "active",
                                                               DateTime? startDate = null,
                                                               DateTime? endDate = null, string couponCode = "", decimal offerAmount = 20, int offerType = 1,
                                                               string exclusivityType = "none", int priority = 1, DateTime? created = null, DateTime? modified = null,
                                                               string modifiedBy = "admin", string promotionGroup = "entry", int perOrderLimit = 0,
                                                               int applicationLimit = 0, XDocument settingsXml = null, int customerLimit = 0 )
        {
            var row = table.NewPromotionRow();

            row.ApplicationId = applicationId == default( Guid ) ? ApplicationId : applicationId;
            row.ApplicationLimit = applicationLimit;
            row.CampaignId = campaignId;
            row.CouponCode = couponCode;
            row.Created = created ?? DateTime.MinValue;
            row.CustomerLimit = customerLimit;
            row.EndDate = endDate ?? DateTime.MaxValue;
            row.ExclusivityType = exclusivityType;
            row.Modified = modified ?? DateTime.MinValue;
            row.ModifiedBy = modifiedBy;
            row.Name = name;
            row.OfferAmount = offerAmount;
            row.OfferType = offerType;

            if ( settingsXml != null )
            {
                var stream = new MemoryStream();
                settingsXml.Save( stream );
                row.Params = stream.ToArray();
            }
            else
            {
                row.Params = new byte[0];
            }

            row.PerOrderLimit = perOrderLimit;
            row.Priority = priority;
            row.PromotionGroup = promotionGroup;
            row.PromotionId = promotionId;
            row.PromotionType = promotionType;
            row.StartDate = startDate ?? DateTime.MinValue;
            row.Status = status;

            table.AddPromotionRow( row );
            return table;
        }

        public static CampaignDto.CampaignDataTable WithRow( this CampaignDto.CampaignDataTable table, int campaignId, string name, Guid applicationId = default( Guid ),
                                                             DateTime? startDate = null, DateTime? endDate = null, DateTime? created = null, DateTime? exported = null,
                                                             DateTime? modified = null, string modifiedBy = "admin", bool isActive = true, bool isArchived = false,
                                                             string comments = null )

        {
            var row = table.NewCampaignRow();

            row.ApplicationId = applicationId == default( Guid ) ? ApplicationId : applicationId;
            row.CampaignId = campaignId;
            row.Comments = comments;
            row.Created = created ?? DateTime.MinValue;
            row.EndDate = endDate ?? DateTime.MaxValue;
            row.Exported = exported ?? DateTime.MinValue;
            row.IsActive = isActive;
            row.IsArchived = isArchived;
            row.Modified = modified ?? DateTime.MinValue;
            row.ModifiedBy = modifiedBy;
            row.Name = name;
            row.StartDate = startDate ?? DateTime.MinValue;

            table.AddCampaignRow( row );
            return table;
        }

        public static IEnumerable<CatalogEntryDto.CatalogItemSeoRow> GetSeo( this CatalogEntryDto.CatalogEntryRow entry )
        {
            var seo = new CatalogEntryDto.CatalogItemSeoDataTable();
            seo.AddCatalogItemSeoRow( "sv-se", 1, entry, entry.Name + ".aspx", entry.Name, entry.Name, entry.Name, ApplicationId, "" );
            return seo;
        }
    }
}
