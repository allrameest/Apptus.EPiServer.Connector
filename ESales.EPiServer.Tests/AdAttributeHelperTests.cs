using System;
using System.Collections.Generic;
using System.Linq;
using Apptus.ESales.EPiServer.DataAccess;
using Apptus.ESales.EPiServer.Import;
using Apptus.ESales.EPiServer.Import.Ads;
using Apptus.ESales.EPiServer.Import.Configuration;
using NUnit.Framework;
using Mediachase.Commerce.Marketing.Dto;
using Attribute = Apptus.ESales.EPiServer.Import.Attribute;

namespace ESales.EPiServer.Tests
{
    [TestFixture]
    public class AdAttributeHelperTests
    {
        private IEnumerable<ConfigurationAttribute> _attributes;
        private ConfigurationAttribute[] _expectedConfigurationAttributes;
        private IEnumerable<IEntity> _ads;
        private IEnumerable<IEntity> _expectedAds;
        private AdAttributeHelper _adAttributeHelper;

        [SetUp]
        public void Setup()
        {
            _adAttributeHelper = new AdAttributeHelper();

            _attributes = _adAttributeHelper.ConvertToAttributes( new PromotionDto.PromotionLanguageDataTable(), new PromotionDto.PromotionDataTable(),
                                                                    new CampaignDto.CampaignDataTable() );
            var search = new SearchOptions( null, Format.PipeSeparated, true, true );
            var filter = new FilterOptions( Format.PipeSeparated, Tokenization.CaseInsensitive );
            var sort = new SortOptions( Normalization.CaseInsensitive );
            _expectedConfigurationAttributes = new[]
                {
                    new ConfigurationAttribute( "ad_key", type.@string, Present.Yes, search, filter, sort ),
                    new ConfigurationAttribute( "name", type.@string, Present.Yes, search, filter, sort ),
                    new ConfigurationAttribute( "application_id", type.@string, Present.Yes, search, filter, sort ),
                    new ConfigurationAttribute( "status", type.@string, Present.Yes, search, filter, sort ),
                    new ConfigurationAttribute( "coupon_code", type.@string, Present.Yes, search, filter, sort ),
                    new ConfigurationAttribute( "offer_amount", type.@string, Present.Yes, search, filter, sort ),
                    new ConfigurationAttribute( "offer_type", type.@string, Present.Yes, search, filter, sort ),
                    new ConfigurationAttribute( "promotion_group", type.@string, Present.Yes, search, filter, sort ),
                    new ConfigurationAttribute( "campaign_key", type.@string, Present.Yes, search, filter, sort ),
                    new ConfigurationAttribute( "exclusivity_type", type.@string, Present.Yes, search, filter, sort ),
                    new ConfigurationAttribute( "priority", type.@string, Present.Yes, search, filter, sort ),
                    new ConfigurationAttribute( "created", type.@long, Present.Yes, search, filter, sort ),
                    new ConfigurationAttribute( "modified", type.@long, Present.Yes, search, filter, sort ),
                    new ConfigurationAttribute( "modified_by", type.@string, Present.Yes, search, filter, sort ),
                    new ConfigurationAttribute( "promotion_type", type.@string, Present.Yes, search, filter, sort ),
                    new ConfigurationAttribute( "per_order_limit", type.@int, Present.Yes, search, filter, sort ),
                    new ConfigurationAttribute( "application_limit", type.@int, Present.Yes, search, filter, sort ),
                    new ConfigurationAttribute( "customer_limit", type.@int, Present.Yes, search, filter, sort ),
                    new ConfigurationAttribute( "campaign_name", type.@string, Present.Yes, search, filter, sort ),
                    new ConfigurationAttribute( "campaign_created", type.@long, Present.Yes, search, filter, sort ),
                    new ConfigurationAttribute( "campaign_exported", type.@long, Present.Yes, search, filter, sort ),
                    new ConfigurationAttribute( "campaign_modified", type.@long, Present.Yes, search, filter, sort ),
                    new ConfigurationAttribute( "campaign_modified_by", type.@string, Present.Yes, search, filter, sort ),
                    new ConfigurationAttribute( "campaign_is_active", type.@string, Present.Yes, search, filter, sort ),
                    new ConfigurationAttribute( "campaign_is_archived", type.@string, Present.Yes, search, filter, sort ),
                    new ConfigurationAttribute( "campaign_comments", type.@string, Present.Yes, search, filter, sort ),
                    new ConfigurationAttribute( "locale_filter", type.@string, Present.Yes, search, filter, sort )
                };

            var campaignTable = new CampaignDto.CampaignDataTable();
            var campaignId =
                campaignTable.AddCampaignRow( Guid.Parse( "0351ca45-b49f-4002-8433-1008a1f6fa6b" ), DateTime.MinValue, DateTime.MinValue, "CampaignNameTest",
                                              new DateTime( 2012, 08, 20, 07, 07, 11, DateTimeKind.Local ), new DateTime( 2012, 08, 21, 0, 0, 0, DateTimeKind.Local ),
                                              new DateTime( 2012, 08, 21, 13, 02, 37, DateTimeKind.Local ), "CampaignModifiedByTest", true, false, "CommentsTest" )
                             .CampaignId;
            var promotionTable = new PromotionDto.PromotionDataTable();
            var promotionRow = promotionTable.AddPromotionRow( "NameTest", Guid.Parse( "0351ca45-b49f-4002-8433-1008a1f6fa6b" ), "active",
                                                               new DateTime( 2012, 08, 20, 07, 37, 00, DateTimeKind.Local ),
                                                               new DateTime( 2012, 08, 23, 07, 37, 00, DateTimeKind.Local ), "CouponCodeTest", 100.5m, 1,
                                                               "PromotionGroupTest", campaignId, "ExclusivityTypeTest", 3,
                                                               new DateTime( 2012, 08, 20, 07, 39, 07, DateTimeKind.Local ),
                                                               new DateTime( 2012, 11, 06, 12, 43, 16, DateTimeKind.Local ), "ModifiedByTest", "PromotionTypeTest", 4, 5,
                                                               new byte[] { 1, 2, 3 }, 6, 10 );
            var promotionLanguageTable = new PromotionDto.PromotionLanguageDataTable();
            promotionLanguageTable.AddPromotionLanguageRow( "DisplayNameTestSE", "sv-SE", promotionRow );
            promotionLanguageTable.AddPromotionLanguageRow( "DisplayNameTestFI", "sv-FI", promotionRow );

            var promotionDataTableMapper = new PromotionDataTableMapper( promotionLanguageTable, promotionTable, campaignTable );
            var promotionEntryCodeProvider = new PromotionEntryCodeProvider( promotionDataTableMapper, Enumerable.Empty<IPromotionEntryCodes>() );

            _ads = _adAttributeHelper.ConvertToAds( promotionDataTableMapper, promotionEntryCodeProvider );

            _expectedAds = new[]
                               {
                                   new Ad( new[]
                                               {
                                                   //CampaignTable
                                                   new Attribute( "campaign_name", "CampaignNameTest" ),
                                                   new Attribute( "campaign_created", "1345439231000" ),
                                                   new Attribute( "campaign_exported", "1345500000000" ),
                                                   new Attribute( "campaign_modified", "1345546957000" ),
                                                   new Attribute( "campaign_modified_by", "CampaignModifiedByTest" ),
                                                   new Attribute( "campaign_is_active", "True" ),
                                                   new Attribute( "campaign_is_archived", "False" ),
                                                   new Attribute( "campaign_comments", "CommentsTest" ),

                                                   //PromotionTable
                                                   new Attribute( "application_id", "0351ca45-b49f-4002-8433-1008a1f6fa6b" ),
                                                   new Attribute( "status", "active" ),
                                                   new Attribute( "start_time", "2012-08-20T05:37:00Z" ),
                                                   new Attribute( "end_time", "2012-08-23T05:37:00Z" ),
                                                   new Attribute( "coupon_code", "CouponCodeTest" ),
                                                   new Attribute( "offer_amount", "100.50" ),
                                                   new Attribute( "offer_type", "1" ),
                                                   new Attribute( "promotion_group", "PromotionGroupTest" ),
                                                   new Attribute( "campaign_key", campaignId.ToString() ),
                                                   new Attribute( "exclusivity_type", "ExclusivityTypeTest" ),
                                                   new Attribute( "priority", "3" ),
                                                   new Attribute( "created", "1345441147000" ),
                                                   new Attribute( "modified", "1352202196000" ),
                                                   new Attribute( "modified_by", "ModifiedByTest" ),
                                                   new Attribute( "promotion_type", "PromotionTypeTest" ),
                                                   new Attribute( "per_order_limit", "4" ),
                                                   new Attribute( "application_limit", "5" ),
                                                   new Attribute( "customer_limit", "6" ),
                                                   new Attribute( "max_entry_discount_quantity", "10" ),

                                                   //PromotionLanguageTable
                                                   new Attribute( "ad_key", promotionRow.PromotionId + "_sv_SE" ),
                                                   new Attribute( "name", "DisplayNameTestSE" ),
                                                   new Attribute( "locale", "sv_SE" ),
                                                   new Attribute( "locale_filter", "sv_SE" ),
                                                   new Attribute( "included", "locale_filter:'sv_SE'" ),

                                                   //Static Attributes
                                                   new Attribute( "live_products", "4" )
                                               } ),
                                   new Ad( new[]
                                               {
                                                   //CampaignTable
                                                   new Attribute( "campaign_name", "CampaignNameTest" ),
                                                   new Attribute( "campaign_created", "1345439231000" ),
                                                   new Attribute( "campaign_exported", "1345500000000" ),
                                                   new Attribute( "campaign_modified", "1345546957000" ),
                                                   new Attribute( "campaign_modified_by", "CampaignModifiedByTest" ),
                                                   new Attribute( "campaign_is_active", "True" ),
                                                   new Attribute( "campaign_is_archived", "False" ),
                                                   new Attribute( "campaign_comments", "CommentsTest" ),

                                                   //PromotionTable
                                                   new Attribute( "application_id", "0351ca45-b49f-4002-8433-1008a1f6fa6b" ),
                                                   new Attribute( "status", "active" ),
                                                   new Attribute( "start_time", "2012-08-20T05:37:00Z" ),
                                                   new Attribute( "end_time", "2012-08-23T05:37:00Z" ),
                                                   new Attribute( "coupon_code", "CouponCodeTest" ),
                                                   new Attribute( "offer_amount", "100.50" ),
                                                   new Attribute( "offer_type", "1" ),
                                                   new Attribute( "promotion_group", "PromotionGroupTest" ),
                                                   new Attribute( "campaign_key", campaignId.ToString() ),
                                                   new Attribute( "exclusivity_type", "ExclusivityTypeTest" ),
                                                   new Attribute( "priority", "3" ),
                                                   new Attribute( "created", "1345441147000" ),
                                                   new Attribute( "modified", "1352202196000" ),
                                                   new Attribute( "modified_by", "ModifiedByTest" ),
                                                   new Attribute( "promotion_type", "PromotionTypeTest" ),
                                                   new Attribute( "per_order_limit", "4" ),
                                                   new Attribute( "application_limit", "5" ),
                                                   new Attribute( "customer_limit", "6" ),
                                                   new Attribute( "max_entry_discount_quantity", "10" ),

                                                   //PromotionLanguageTable
                                                   new Attribute( "ad_key", promotionRow.PromotionId + "_sv_FI" ),
                                                   new Attribute( "name", "DisplayNameTestFI" ),
                                                   new Attribute( "locale", "sv_FI" ),
                                                   new Attribute( "locale_filter", "sv_FI" ),
                                                   new Attribute( "included", "locale_filter:'sv_FI'" ),

                                                   //Static Attributes
                                                   new Attribute( "live_products", "4" )
                                               } )
                               };
        }

        [Test]
        public void AttributesDistinctTest()
        {
            var attributes = _adAttributeHelper.ConvertToAttributes( new PromotionDto.PromotionLanguageDataTable(), new PromotionDto.PromotionDataTable(),
                                                                    new CampaignDto.CampaignDataTable() );

            Assert.That( attributes.Count(), Is.EqualTo( attributes.Select( a => a.Name ).Distinct().Count() ) );
        }

        [Test]
        public void AttributesDisallowedNamesTest()
        {
            var disallowedNames = new[] { "params", "promotion_language_id", "promotion_id", "campaign_id", "start_date", "end_date" };
            var disallowedNamesThatShouldNotExist = disallowedNames.Intersect( _attributes.Select( a => a.Name ) );
            foreach ( var d in disallowedNamesThatShouldNotExist )
            {
                Console.WriteLine( d );
            }
            Assert.That( !disallowedNamesThatShouldNotExist.Any() );
        }

        [Test]
        public void AttributesCountTest()
        {
            Assert.That( _attributes.Count(), Is.GreaterThanOrEqualTo( _expectedConfigurationAttributes.Count() ) );
        }

        [Test]
        public void AttributesExpectedTest()
        {
            foreach ( var expectedAttribute in _expectedConfigurationAttributes )
            {
                ConfigurationAttribute expectedAttribute1 = expectedAttribute;
                try
                {
                    _attributes.Single( a => a.Name == expectedAttribute1.Name );
                }
                catch ( Exception )
                {
                    Console.WriteLine( "Test failed with expected attribute [name: {0}].", expectedAttribute1.Name );
                    throw;
                }
            }
        }

        [Test]
        public void AdsExpectedTest()
        {
            foreach ( var expectedAd in _expectedAds )
            {
                var expectedAd1 = expectedAd;
                IEntity matchingAd;
                string expectedAdKey = expectedAd1.Single( ea => ea.Name == "ad_key" ).Value;
                try
                {
                    var matchingAds = _ads.Where( ma => ma.Single( maa => maa.Name == "ad_key" ).Value == expectedAdKey );
                    Assert.That( matchingAds.Count(), Is.EqualTo( 1 ) );
                    matchingAd = matchingAds.First();
                }
                catch ( Exception )
                {
                    Console.WriteLine( "Test failed with ad [ad_key: {0}].", expectedAdKey );
                    throw;
                }

                foreach ( var expectedAttribute in expectedAd1 )
                {
                    try
                    {
                        var expectedAttribute1 = expectedAttribute;
                        var matchingAdAttribute = matchingAd.Single( maa => maa.Name == expectedAttribute1.Name );
                        Assert.That( matchingAdAttribute.Values, Is.EquivalentTo( expectedAttribute1.Values ) );
                    }
                    catch ( Exception )
                    {
                        Console.WriteLine( "Test failed with ad [ad_key: {0}], attribute [attribute name: {1}]", expectedAdKey, expectedAttribute.Name );
                        throw;
                    }
                }
            }
        }

        [Test]
        public void AdsNotInExpectedTest()
        {
            foreach ( var ad in _ads )
            {
                var ad1 = ad;
                IEntity expectedAd;
                var adKey = ad1.Single( ada => ada.Name == "ad_key" ).Value;
                try
                {
                    var expectedAds = _expectedAds.Where( ea => ea.Single( eaa => eaa.Name == "ad_key" ).Value == adKey );
                    Assert.That( expectedAds.Count(), Is.EqualTo( 1 ) );
                    expectedAd = expectedAds.First();
                }
                catch ( Exception )
                {
                    Console.WriteLine( "Test failed with ad [ad_key: {0}].", adKey );
                    throw;
                }

                foreach ( var attribute in ad1 )
                {
                    try
                    {
                        var attribute1 = attribute;
                        var expectedAdAttribute = expectedAd.Single( eaa => eaa.Name == attribute1.Name );
                        Assert.That( expectedAdAttribute.Values, Is.EquivalentTo( attribute1.Values ) );
                    }
                    catch ( Exception )
                    {
                        Console.WriteLine( "Test failed with ad [ad_key: {0}], attribute [attribute name: {1}].", adKey, attribute.Name );
                        throw;
                    }
                }
            }
        }
    }
}
