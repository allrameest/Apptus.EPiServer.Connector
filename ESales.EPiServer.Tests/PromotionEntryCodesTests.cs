using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Apptus.ESales.EPiServer.DataAccess;
using Apptus.ESales.EPiServer.Import.Ads;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Marketing.Dto;
using Moq;
using NUnit.Framework;

namespace ESales.EPiServer.Tests
{
    [TestFixture]
    class PromotionEntryCodesTests
    {
        private static readonly Guid ApplicationGuid = Guid.Parse( "024B3344-402A-4100-8764-384263B729C1" );
        

        [Test]
        public void BuyXGetYDiscounted()
        {
            var promotion = GetMockedPromotionWithSettings( 
                XDocument.Parse( 
                    @"<?xml version=""1.0""?>
                      <Settings xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
                        <EntryXFilter>62012B</EntryXFilter>
                        <EntryYFilter>61608B</EntryYFilter>
                        <SourceQuantity>1</SourceQuantity>
                        <TargetQuantity>1</TargetQuantity>
                        <RewardType>EachAffectedEntry</RewardType>
                        <AmountOff>20.00</AmountOff>
                        <AmountType>Percentage</AmountType>
                      </Settings>" ) );
            Assert.That( new BuyXGetYDiscountedEntryCodes().Included( promotion ).ToArray(), Is.EqualTo( new[] { "62012B", "61608B" } ) );
            Assert.That( new BuyXGetYDiscountedEntryCodes().Excluded(promotion).ToArray(), Is.EqualTo( new string[0] ) );
        }

        [Test]
        public void BuyXGetOffDiscountEntries()
        {
            var promotion =
                GetMockedPromotionWithSettings( XDocument.Parse( 
                    @"<?xml version=""1.0""?>
                        <Settings xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
                        <RuleSkuSet>76691B,80932B,76695B,80934B,76384B,76385B,80587B,79177B,80989B,76383B,74341B,76382B,77524B,76387B,76386B,70904B,73836B,70903B,72294B,70903M,73834M,73834B,71040B,74343B,77522B,74339B,80320B,76006B,77707B,77523B,71039B,77705B,78441B,72498B,76696B,80935B,72499B,76698B,70902M,70902B,73835B,80936B,74139M,74139B,82898B,80863B,76381B,83405B,73838B,78035B,79158B,76013B,76012B,76009B,78036B,76003B,80299B,73830B,76002B,73831B,80414B,78034B,79159B,77708B,77151B,79785B,76829B,00045B,82447B,80933B</RuleSkuSet>
                        <SkuDelimiter>44</SkuDelimiter>
                        <MinQuantity>10</MinQuantity>
                        <RewardType>EachAffectedEntry</RewardType>
                        <AmountOff>10.00</AmountOff>
                        <AmountType>Percentage</AmountType>
                      </Settings>" ) );
            Assert.That( new BuyXGetOffDiscountEntryCodes().Included( promotion ).ToArray(),
                         Is.EqualTo( new[]
                             {
                                 "76691B", "80932B", "76695B", "80934B", "76384B", "76385B", "80587B", "79177B", "80989B", "76383B", "74341B", "76382B", "77524B", "76387B", "76386B", "70904B",
                                 "73836B", "70903B", "72294B", "70903M", "73834M", "73834B", "71040B", "74343B", "77522B", "74339B", "80320B", "76006B", "77707B", "77523B", "71039B",
                                 "77705B", "78441B", "72498B", "76696B", "80935B", "72499B", "76698B", "70902M", "70902B", "73835B", "80936B", "74139M", "74139B", "82898B", "80863B",
                                 "76381B", "83405B", "73838B", "78035B", "79158B", "76013B", "76012B", "76009B", "78036B", "76003B", "80299B", "73830B", "76002B", "73831B", "80414B",
                                 "78034B", "79159B", "77708B", "77151B", "79785B", "76829B", "00045B", "82447B", "80933B"
                             } ) );
        }

        [Test]
        public void BuyXGetNofYatReducedPriceEntriesExcluded()
        {
            var promotion = GetMockedPromotionWithSettings(
                XDocument.Parse(
                    @"<?xml version=""1.0""?>
                        <Settings xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
                          <EntryXSkuSet>76691B,80932B,76384B,80587B</EntryXSkuSet>
                          <EntryYFilter>62012B</EntryYFilter>
                          <EntryXSkuDelimiter>59</EntryXSkuDelimiter>
                          <MaxYQuantity>2</MaxYQuantity>
                          <RewardType>EachAffectedEntry</RewardType>
                          <AmountOff>10</AmountOff>
                          <AmountType>Percentage</AmountType>
                          <EqualityOper>&amp;lt;</EqualityOper>
                          <Exclude>true</Exclude>
                        </Settings>" ) );

            Assert.That( new BuyXGetNofYatReducedPriceEntryCodes().Included( promotion ).ToArray(), Is.EqualTo( new string[0] ) );
            Assert.That( new BuyXGetNofYatReducedPriceEntryCodes().Excluded( promotion ).ToArray(), Is.EqualTo( new[] { "76691B", "80932B", "76384B", "80587B", "62012B" } ) );
        }

        [Test]
        public void BuyXGetNofYatReducedPriceEntriesIncluded()
        {
            var promotion = GetMockedPromotionWithSettings(
                XDocument.Parse(
                    @"<?xml version=""1.0""?>
                        <Settings xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
                          <EntryXSkuSet>76691B,80932B,76384B,80587B</EntryXSkuSet>
                          <EntryYFilter>62012B</EntryYFilter>
                          <EntryXSkuDelimiter>59</EntryXSkuDelimiter>
                          <MaxYQuantity>2</MaxYQuantity>
                          <RewardType>EachAffectedEntry</RewardType>
                          <AmountOff>10</AmountOff>
                          <AmountType>Percentage</AmountType>
                          <EqualityOper>&amp;lt;</EqualityOper>
                          <Exclude>false</Exclude>
                        </Settings>" ) );

            Assert.That( new BuyXGetNofYatReducedPriceEntryCodes().Included( promotion ).ToArray(), Is.EqualTo( new[] { "76691B", "80932B", "76384B", "80587B", "62012B" } ) );
            Assert.That( new BuyXGetNofYatReducedPriceEntryCodes().Excluded( promotion ).ToArray(), Is.EqualTo( new string[0] ) );
        }

        [Test]
        public void BuySKUFromCategoryXGetDiscountedShippingEntries()
        {
            var catalogSystemMapperMock = new Mock<ICatalogSystemMapper>();
            catalogSystemMapperMock
                .Setup( csm => csm.GetCatalogRelations( It.IsAny<int>(), It.IsAny<int>() ) )
                .Returns<int, int>(
                    ( c, cn ) => new CatalogRelationDto.NodeEntryRelationDataTable()
                                     .WithRow( 1, 15 )
                                     .WithRow( 2, 15 )
                                     .WithRow( 4, 15 )
                                     .WithRow( 100, 15 ) );
            catalogSystemMapperMock
                .Setup( csm => csm.GetCatalogNode( It.IsAny<string>() ) )
                .Returns<string>( c => new CatalogNode( new CatalogNodeDto.CatalogNodeDataTable().WithRow(15, "NZ").First( t => t.Code == c ), CatalogNodeResponseGroup.ResponseGroup.CatalogNodeInfo ) );
            var entryTable = new CatalogEntryDto.CatalogEntryDataTable()
                .WithRow( 1, "ABC1" )
                .WithRow( 2, "ABC2" )
                .WithRow( 4, "ABC4" )
                .WithRow( 100, "ABC100" );

            catalogSystemMapperMock
                .Setup( csm => csm.GetCatalogEntry( It.IsAny<int>() ) )
                .Returns<int>( e => entryTable.First( r => r.CatalogEntryId == e ) );

            var promotion = GetMockedPromotionWithSettings(
                XDocument.Parse(
                    @"<?xml version=""1.0""?>
                        <Settings xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
                          <CategoryCode>NZ</CategoryCode>
                          <MinimumQuantity>1</MinimumQuantity>
                          <ShippingMethodId>a796ace2-e147-4534-a910-55ef188a6adb</ShippingMethodId>
                          <RewardType>WholeOrder</RewardType>
                          <AmountOff>10</AmountOff>
                          <AmountType>Percentage</AmountType>
                        </Settings>" ) );

            Assert.That( new BuySKUFromCategoryXGetDiscountedShippingEntryCodes( catalogSystemMapperMock.Object ).Included( promotion ).ToArray(),
                         Is.EqualTo( new[] { "ABC1", "ABC2", "ABC4", "ABC100" } ) );
            Assert.That( new BuySKUFromCategoryXGetDiscountedShippingEntryCodes( catalogSystemMapperMock.Object ).Excluded( promotion ).ToArray(), Is.EqualTo( new string[0] ) );
        }

        [Test]
        public void BuyXGetOffShipmentDiscountEntries()
        {
            var promotion = GetMockedPromotionWithSettings(
                XDocument.Parse(
                    @"<?xml version=""1.0""?>
                        <Settings xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
                          <RuleSkuSet>62012B;61608B;33228B;</RuleSkuSet>
                          <SkuDelimiter>59</SkuDelimiter>
                          <MinimumQuantity>1</MinimumQuantity>
                          <ShippingMethodId>a796ace2-e147-4534-a910-55ef188a6adb</ShippingMethodId>
                          <RewardType>WholeOrder</RewardType>
                          <AmountOff>10</AmountOff>
                          <AmountType>Percentage</AmountType>
                        </Settings>" ) );

            Assert.That( new BuyXGetOffShipmentDiscountEntryCodes().Included( promotion ).ToArray(), Is.EqualTo( new[] { "62012B", "61608B", "33228B" } ) );
            Assert.That( new BuyXGetOffShipmentDiscountEntryCodes().Excluded( promotion ).ToArray(), Is.EqualTo( new string[0] ) );
        }

        private static Promotion GetMockedPromotionWithSettings( XDocument settingsXml )
        {
            var stream = new MemoryStream();
            settingsXml.Save( stream );
            var settings = stream.ToArray();

            var promotionDto = new PromotionDto();
            var promotionTable = new PromotionDto.PromotionDataTable();
            promotionDto.Tables.Add( promotionTable );
            var promotionRow = promotionTable.AddPromotionRow( "", ApplicationGuid, "", DateTime.MinValue, DateTime.MaxValue, "", 0, 0, "", 0, "", 0, DateTime.MinValue,
                                                               DateTime.MinValue, "", "", 0, 0, settings, 0, 0 );
            var promotionLanguageTable = new PromotionDto.PromotionLanguageDataTable();
            promotionDto.Tables.Add( promotionLanguageTable );
            var promotionLanguageRow = promotionLanguageTable.AddPromotionLanguageRow( "", "en-US", promotionRow );
            var campaignDto = new CampaignDto();
            var campaignTable = new CampaignDto.CampaignDataTable();
            campaignDto.Tables.Add( campaignTable );
            var campaignRow = campaignTable.AddCampaignRow( ApplicationGuid, DateTime.MinValue, DateTime.MaxValue, "", DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, "", true, false, "" );

            return new Promotion( promotionLanguageRow, promotionRow, campaignRow );
        }
    }
}
