using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Apptus.ESales.EPiServer.Config;
using Apptus.ESales.EPiServer.DataAccess;
using Apptus.ESales.EPiServer.Import.Ads;
using Autofac;
using Mediachase.Commerce.Marketing.Dto;
using Moq;
using NUnit.Framework;

namespace ESales.EPiServer.Tests
{
    [TestFixture]
    internal class ESalesCatalogIndexBuilderTests
    {
        [Test]
        public void IndexAds()
        {
            var builder = new ContainerBuilder();

            var appConfigMock = new Mock<IAppConfig>();
            const string applicationName = "Test";
            const string scope = "TestScope";
            appConfigMock.Setup( c => c.ApplicationId ).Returns( () => applicationName );
            appConfigMock.Setup( c => c.Scope ).Returns( () => scope );
            appConfigMock.Setup( c => c.Enable ).Returns( () => true );
            appConfigMock.Setup( c => c.EnableAds ).Returns( () => true );
            builder.Register( c => appConfigMock.Object );

            var fileSystemMock = new Mock<IFileSystem>();
            var xmlStream = new MemoryStream();
            fileSystemMock.Setup( fsm => fsm.Open( It.IsAny<string>(), It.IsAny<FileMode>(), It.IsAny<FileAccess>() ) ).Returns( xmlStream );
            builder.Register( c => fileSystemMock.Object );

            builder.RegisterType<FileHelper>();

            var promotionLanguageDataTable = new PromotionDto.PromotionLanguageDataTable().WithRow( 1, 1000001, "en-us", "20% off on Chateau" );
            const string promotionTypeInclude = "BuyXGetYDiscounted";
            var applicationId = Guid.Parse( "024b3344-402a-4100-8764-384263b729c1" );
            var promotionDataTable = new PromotionDto.PromotionDataTable().WithRow( 1000001, "20% off on Chateau [NO LANGUAGE]", promotionTypeInclude, 1, applicationId,
                                                                                    startDate: new DateTime( 2012, 1, 1, 8, 17, 0, DateTimeKind.Local ),
                                                                                    endDate: new DateTime( 2020, 12, 31, 8, 17, 0, DateTimeKind.Local ),
                                                                                    created: new DateTime( 2012, 9, 21, 7, 19, 11, DateTimeKind.Local ),
                                                                                    modified: new DateTime( 2012, 10, 4, 7, 6, 28, DateTimeKind.Local ) );
            var campaignDataTable = new CampaignDto.CampaignDataTable().WithRow(
                1, "Wine", applicationId, created: new DateTime( 2008, 10, 7, 22, 14, 13, DateTimeKind.Local ),
                exported: new DateTime( 1970, 1, 1, 1, 0, 0, DateTimeKind.Local ), modified: new DateTime( 2012, 10, 4, 7, 5, 9, DateTimeKind.Local ) );
            var promotionDataTableMapper = new PromotionDataTableMapper( promotionLanguageDataTable, promotionDataTable, campaignDataTable );
            builder.Register( c => promotionDataTableMapper );

            var promotionEntryCodes1 = new Mock<IPromotionEntryCodes>();
            promotionEntryCodes1.Setup( pm => pm.PromotionType ).Returns( promotionTypeInclude );
            promotionEntryCodes1
                .Setup( pm => pm.Included( It.IsAny<Promotion>() ) )
                .Returns<Promotion>( p => p.PromotionRow.PromotionType == promotionTypeInclude ? new[] { "WHATEVER" } : Enumerable.Empty<string>() );
            promotionEntryCodes1.Setup( pm => pm.Excluded( It.IsAny<Promotion>() ) ).Returns<Promotion>( p => Enumerable.Empty<string>() );
            builder.Register( c => promotionEntryCodes1.Object );

            builder.RegisterType<PromotionEntryCodeProvider>();

            var indexSystemMock = new Mock<IIndexSystemMapper>();
            builder.Register( c => indexSystemMock.Object );

            builder.RegisterType<AdsIndexBuilder>();

            var expectedXElement = XElement.Parse( @"<operations>
                                                        <clear>
                                                          <ad/>
                                                        </clear>
                                                        <add>
                                                          <ad>
                                                            <campaign_name>Wine</campaign_name>
                                                            <campaign_created>1223410453000</campaign_created>
                                                            <campaign_exported>0</campaign_exported>
                                                            <campaign_modified>1349327109000</campaign_modified>
                                                            <campaign_modified_by>admin</campaign_modified_by>
                                                            <campaign_is_active>True</campaign_is_active>
                                                            <campaign_is_archived>False</campaign_is_archived>
                                                            <name>20% off on Chateau</name>
                                                            <application_id>" + applicationId + @"</application_id>
                                                            <status>active</status>
                                                            <start_time>2012-01-01T07:17:00Z</start_time>
                                                            <end_time>2020-12-31T07:17:00Z</end_time>
                                                            <offer_amount>20.00</offer_amount>
                                                            <offer_type>1</offer_type>
                                                            <promotion_group>entry</promotion_group>
                                                            <campaign_key>1</campaign_key>
                                                            <exclusivity_type>none</exclusivity_type>
                                                            <priority>1</priority>
                                                            <created>1348204751000</created>
                                                            <modified>1349327188000</modified>
                                                            <modified_by>admin</modified_by>
                                                            <promotion_type>BuyXGetYDiscounted</promotion_type>
                                                            <per_order_limit>0</per_order_limit>
                                                            <application_limit>0</application_limit>
                                                            <customer_limit>0</customer_limit>
                                                            <locale>en_US</locale>
                                                            <locale_filter>en_US</locale_filter>
                                                            <ad_key>1000001_en_US</ad_key>
                                                            <included>locale_filter:'en_US' AND ad_key_included:'1000001_en_US'</included>
                                                            <live_products>4</live_products>
                                                          </ad>
                                                        </add>
                                                      </operations>" );

            var container = builder.Build();
            container.Resolve<AdsIndexBuilder>().Build( false );
            XElement xml;
            using ( var reader = new StreamReader( new MemoryStream( xmlStream.ToArray() ) ) )
            {
                xml = XElement.Parse( reader.ReadToEnd() );
            }

            try
            {
                Assert.That( xml.ElementsEquivalentTo( expectedXElement ) );
            }
            catch ( AssertionException )
            {
                Console.WriteLine( "Expected:" );
                Console.WriteLine( expectedXElement );
                Console.WriteLine();
                Console.WriteLine( "Actual:" );
                Console.WriteLine( xml );
                throw;
            }
        }

//        private Mock<IIndexSystemMapper> _mockedBaseBuilder;
//        private Mock<ICatalogSystemMapper> _mockedSystem;
//        private Mock<IConfiguration> _mockedConfig;

//        private class CatalogEntryRowExtra
//        {
//            public CatalogEntryRowExtra( Func<CatalogEntryDto.CatalogEntryRow> createRow, int catalogEntryId, int catalogId, string name, string classTypeId, bool isActive, DateTime modifiedDate )
//            {
//// ReSharper disable SpecifyACultureInStringConversionExplicitly
//                Row = CreateRow( createRow, catalogEntryId, catalogId, DateTime.MinValue, DateTime.MaxValue, name, "Template", catalogEntryId.ToString(), null, classTypeId, 5, isActive,
//// ReSharper restore SpecifyACultureInStringConversionExplicitly
//                                 new byte[0], Guid.Parse( "E6D3F768-7E02-4040-AB50-0FEA47EA5D27" ) );
//                ModifiedDate = modifiedDate;
//            }

//            public CatalogEntryDto.CatalogEntryRow Row { get; private set; }
//            public DateTime ModifiedDate { get; private set; }
//        }

//        private static CatalogEntryDto.CatalogEntryRow CreateRow( Func<CatalogEntryDto.CatalogEntryRow> newRow, CatalogEntryDto.CatalogEntryRow row )
//        {
//            return CreateRow( newRow, row.CatalogEntryId, row.CatalogId, row.StartDate, row.EndDate, row.Name, row.TemplateName, row.Code, row.InventoryRow, row.ClassTypeId,
//                              row.MetaClassId, row.IsActive, row.SerializedData, row.ApplicationId );
//        }

//        private static CatalogEntryDto.CatalogEntryRow CreateRow( Func<CatalogEntryDto.CatalogEntryRow> createRow, int catalogEntryId, int catalogId, DateTime startDate,
//                                                                  DateTime endDate, string name, string templateName, string code, CatalogEntryDto.InventoryRow inventoryRow,
//                                                                  string classTypeId, int metaClassId, bool isActive, byte[] serializedData, Guid applicationId )
//        {
//            var row = createRow();
//            row.CatalogEntryId = catalogEntryId;
//            row.CatalogId = catalogId;
//            row.StartDate = startDate;
//            row.EndDate = endDate;
//            row.Name = name;
//            row.TemplateName = templateName;
//            row.Code = code;
//            row.InventoryRow = inventoryRow;
//            row.ClassTypeId = classTypeId;
//            row.MetaClassId = metaClassId;
//            row.IsActive = isActive;
//            row.SerializedData = serializedData;
//            row.ApplicationId = applicationId;
//            return row;
//        }

//        private static CatalogEntryDto.CatalogEntryDataTable GetEntryTable( IEnumerable<CatalogEntryRowExtra> entries, int catalogId, bool incremental, DateTime? earliest,
//                                                                            DateTime? latest )
//        {
//            var table = new CatalogEntryDto.CatalogEntryDataTable();
//            foreach ( var catalogEntryRow in
//                entries
//                .Where( e => ( e.Row.CatalogId == catalogId ) &&
//                             ( !incremental || ( !earliest.HasValue || earliest <= e.ModifiedDate) && ( !latest.HasValue || latest >= e.ModifiedDate ) ) &&
//                             ( incremental || e.Row.IsActive ) )
//                .Select( e => e.Row ) )
//            {
//                table.AddCatalogEntryRow( CreateRow( table.NewCatalogEntryRow, catalogEntryRow ) );
//            }
//            return table;
//        }

//        [SetUp]
//        public void Init()
//        {
//            _mockedBaseBuilder = new Mock<IIndexSystemMapper>();
//            _mockedBaseBuilder.Setup( b => b.Log( It.IsAny<string>(), It.IsAny<double>(), It.IsAny<object[]>() ) )
//                .Callback<string, double, object[]>( ( m, d, p ) => Console.WriteLine( "Msg: \"{0}\" Progress: {1}%", string.Format( m, p ), d ) );
//            _mockedBaseBuilder.Setup( b => b.CurrentBuildDate ).Returns( new DateTime( 2012, 10, 18 ) );
//            _mockedBaseBuilder.Setup( b => b.LastBuildDate ).Returns( new DateTime( 2012, 10, 15 ) );

//            _mockedSystem = new Mock<ICatalogSystemMapper>();
//            var applicationId = Guid.Parse( "E6D3F768-7E02-4040-AB50-0FEA47EA5D27" );
//            var catalogDto = new CatalogDto();
//            var beerCatalog = catalogDto.Catalog.AddCatalogRow( "Beer", DateTime.MinValue, DateTime.MaxValue, "gbp", "kgs", "en-us", false, true, DateTime.MinValue, DateTime.MinValue, null, null, 0, applicationId, null ).CatalogId;
//            var whiskyCatalog = catalogDto.Catalog.AddCatalogRow( "Whisky", DateTime.MinValue, DateTime.MaxValue, "gbp", "kgs", "en-us", false, true, DateTime.MinValue, DateTime.MinValue, null, null, 0, applicationId, null ).CatalogId;
//            _mockedSystem.Setup( s => s.GetCatalogs() ).Returns( catalogDto.Catalog );

//            _mockedConfig = new Mock<IConfiguration>();
//            _mockedConfig.Setup( c => c.EnableVariants ).Returns( true );

//            var entryTable = new CatalogEntryDto.CatalogEntryDataTable();
//            var createRow = new Func<CatalogEntryDto.CatalogEntryRow>( entryTable.NewCatalogEntryRow );
//            var entries = new[]
//                              {
//                                  new CatalogEntryRowExtra( createRow, 1, beerCatalog, "Lagunitas Maximus IPA", "Variation", true, DateTime.MinValue ),
//                                  new CatalogEntryRowExtra( createRow, 2, beerCatalog, "Mikkeller American Dream", "Variation", true, new DateTime( 2012, 10, 17 ) ),
//                                  new CatalogEntryRowExtra( createRow, 3, beerCatalog, "Alesmith IPA", "Variation", false, new DateTime( 2012, 10, 17 ) ),

//                                  new CatalogEntryRowExtra( createRow, 4, beerCatalog, "Pale Ale", "Variation", true, DateTime.MinValue ),
//                                  new CatalogEntryRowExtra( createRow, 5, beerCatalog, "Sierra Nevada", "Product", true, new DateTime( 2012, 10, 17 ) ),
//                                  new CatalogEntryRowExtra( createRow, 6, beerCatalog, "Torpedo", "Variation", true, DateTime.MinValue ),

//                                  new CatalogEntryRowExtra( createRow, 7, beerCatalog, "Abstrakt", "Variation", true, DateTime.MinValue ),
//                                  new CatalogEntryRowExtra( createRow, 8, beerCatalog, "Punk IPA", "Variation", true, DateTime.MinValue ),
//                                  new CatalogEntryRowExtra( createRow, 9, beerCatalog, "Brewdog", "Product", false, new DateTime( 2012, 10, 17 ) ),

//                                  new CatalogEntryRowExtra( createRow, 10, beerCatalog, "Nils Oscar", "Product", true, DateTime.MinValue ),
//                                  new CatalogEntryRowExtra( createRow, 11, beerCatalog, "Coffee Stout", "Variation", true, new DateTime( 2012, 10, 17 ) ),
//                                  new CatalogEntryRowExtra( createRow, 12, beerCatalog, "Hop Yard IPA", "Variation", true, DateTime.MinValue ),

//                                  new CatalogEntryRowExtra( createRow, 16, beerCatalog, "Extra IPA", "Variation", false, new DateTime( 2012, 10, 17 ) ),
//                                  new CatalogEntryRowExtra( createRow, 17, beerCatalog, "Mohawk", "Product", true, DateTime.MinValue ),
//                                  new CatalogEntryRowExtra( createRow, 18, beerCatalog, "Unfiltered Lager", "Variation", true, DateTime.MinValue ),

//                                  new CatalogEntryRowExtra( createRow, 19, beerCatalog, "Amarillo", "Variation", true, new DateTime( 2012, 10, 17 ) ),
//                                  new CatalogEntryRowExtra( createRow, 20, beerCatalog, "Oppigårds", "Product", false, DateTime.MinValue ),
//                                  new CatalogEntryRowExtra( createRow, 21, beerCatalog, "Golden Ale", "Variation", true, new DateTime( 2012, 10, 17 ) ),

//                                  new CatalogEntryRowExtra( createRow, 22, beerCatalog, "Yeti Imperial Stout", "Variation", true, new DateTime( 2012, 10, 17 ) ),
//                                  new CatalogEntryRowExtra( createRow, 23, beerCatalog, "Great Divide", "Product", true, new DateTime( 2012, 10, 17 ) ),
//                                  new CatalogEntryRowExtra( createRow, 24, beerCatalog, "Titan IPA", "Variation", true, new DateTime( 2012, 10, 17 ) ),

//                                  new CatalogEntryRowExtra( createRow, 25, beerCatalog, "Pilsner", "Variation", true, new DateTime( 2012, 10, 17 ) ),
//                                  new CatalogEntryRowExtra( createRow, 26, beerCatalog, "St Eriks", "Product", true, new DateTime( 2012, 10, 17 ) ),
//                                  new CatalogEntryRowExtra( createRow, 27, beerCatalog, "Porter", "Variation", false, new DateTime( 2012, 10, 17 ) ),

//                                  new CatalogEntryRowExtra( createRow, 28, whiskyCatalog, "Single Malt", "Product", true, new DateTime( 2012, 10, 17 ) ),
//                                  new CatalogEntryRowExtra( createRow, 29, whiskyCatalog, "Ardbeg", "Variation", false, new DateTime( 2012, 10, 17 ) ),
//                                  new CatalogEntryRowExtra( createRow, 30, whiskyCatalog, "Caol Ila", "Variation", false, new DateTime( 2012, 10, 17 ) ),

//                                  new CatalogEntryRowExtra( createRow, 31, whiskyCatalog, "Blended", "Product", false, DateTime.MinValue ),
//                                  new CatalogEntryRowExtra( createRow, 32, whiskyCatalog, "Tullamore Dew", "Variation", true, new DateTime( 2012, 10, 17 ) ),
//                                  new CatalogEntryRowExtra( createRow, 33, whiskyCatalog, "Grants", "Variation", true, new DateTime( 2012, 10, 17 ) ),

//                                  new CatalogEntryRowExtra( createRow, 34, whiskyCatalog, "Bourbon", "Product", true, DateTime.MinValue ),
//                                  new CatalogEntryRowExtra( createRow, 35, whiskyCatalog, "Makers Mark", "Variation", true, new DateTime( 2012, 10, 17 ) ),
//                                  new CatalogEntryRowExtra( createRow, 36, whiskyCatalog, "Knobs Creek", "Variation", false, new DateTime( 2012, 10, 17 ) )
//                              };

//            var entryTablesByGuid = new Dictionary<Guid, CatalogEntryDto.CatalogEntryDataTable>();
//            _mockedSystem
//                .Setup( s => s.StartFindItemsForIndexing( It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>() ) )
//                .Returns<Guid, int, bool, DateTime?, DateTime?>(
//                    ( searchSetId, catalogId, incremental, earliest, latest ) =>
//                        {
//                            var catalogEntryDataTable = GetEntryTable( entries, catalogId, incremental, earliest, latest );
//                            entryTablesByGuid.Add( searchSetId, catalogEntryDataTable );
//                            return catalogEntryDataTable.Count;
//                        } );
//// ReSharper disable ImplicitlyCapturedClosure
//            _mockedSystem.Setup( s => s.ContinueFindItemsForIndexing( It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>() ) ).Returns<Guid, int, int>( (g, s, c) => entryTablesByGuid[g] );
//// ReSharper restore ImplicitlyCapturedClosure

//            var beerRelationDto = new CatalogRelationDto();
//            beerRelationDto.CatalogEntryRelation.AddCatalogEntryRelationRow( 5, 4, "ProductVariation", 1, "default", 0 );
//            beerRelationDto.CatalogEntryRelation.AddCatalogEntryRelationRow( 5, 6, "ProductVariation", 1, "default", 0 );
//            beerRelationDto.CatalogEntryRelation.AddCatalogEntryRelationRow( 9, 7, "ProductVariation", 1, "default", 0 );
//            beerRelationDto.CatalogEntryRelation.AddCatalogEntryRelationRow( 9, 8, "ProductVariation", 1, "default", 0 );
//            beerRelationDto.CatalogEntryRelation.AddCatalogEntryRelationRow( 10, 11, "ProductVariation", 1, "default", 0 );
//            beerRelationDto.CatalogEntryRelation.AddCatalogEntryRelationRow( 10, 12, "ProductVariation", 1, "default", 0 );
//            beerRelationDto.CatalogEntryRelation.AddCatalogEntryRelationRow( 17, 16, "ProductVariation", 1, "default", 0 );
//            beerRelationDto.CatalogEntryRelation.AddCatalogEntryRelationRow( 17, 18, "ProductVariation", 1, "default", 0 );
//            beerRelationDto.CatalogEntryRelation.AddCatalogEntryRelationRow( 20, 19, "ProductVariation", 1, "default", 0 );
//            beerRelationDto.CatalogEntryRelation.AddCatalogEntryRelationRow( 20, 21, "ProductVariation", 1, "default", 0 );
//            beerRelationDto.CatalogEntryRelation.AddCatalogEntryRelationRow( 23, 22, "ProductVariation", 1, "default", 0 );
//            beerRelationDto.CatalogEntryRelation.AddCatalogEntryRelationRow( 23, 24, "ProductVariation", 1, "default", 0 );
//            beerRelationDto.CatalogEntryRelation.AddCatalogEntryRelationRow( 26, 25, "ProductVariation", 1, "default", 0 );
//            beerRelationDto.CatalogEntryRelation.AddCatalogEntryRelationRow( 26, 27, "ProductVariation", 1, "default", 0 );

//            var whiskyRelationDto = new CatalogRelationDto();
//            whiskyRelationDto.CatalogEntryRelation.AddCatalogEntryRelationRow( 28, 29, "ProductVariation", 1, "default", 0 );
//            whiskyRelationDto.CatalogEntryRelation.AddCatalogEntryRelationRow( 28, 30, "ProductVariation", 1, "default", 0 );
//            whiskyRelationDto.CatalogEntryRelation.AddCatalogEntryRelationRow( 31, 32, "ProductVariation", 1, "default", 0 );
//            whiskyRelationDto.CatalogEntryRelation.AddCatalogEntryRelationRow( 31, 33, "ProductVariation", 1, "default", 0 );
//            whiskyRelationDto.CatalogEntryRelation.AddCatalogEntryRelationRow( 34, 35, "ProductVariation", 1, "default", 0 );
//            whiskyRelationDto.CatalogEntryRelation.AddCatalogEntryRelationRow( 34, 36, "ProductVariation", 1, "default", 0 );
//            var relationTables = new Dictionary<int, CatalogRelationDto.CatalogEntryRelationDataTable>
//                                     {
//                                         {beerCatalog, beerRelationDto.CatalogEntryRelation},
//                                         {whiskyCatalog, whiskyRelationDto.CatalogEntryRelation}
//                                     };
//            _mockedSystem.Setup( s => s.GetCatalogRelations( It.IsAny<int>() ) ).Returns<int>( c => relationTables[c] );

//// ReSharper disable ImplicitlyCapturedClosure
//            _mockedSystem.Setup( s => s.GetCatalogEntry( It.IsAny<int>() ) ).Returns<int>( eId => entries.First( e => e.Row.CatalogEntryId == eId ).Row );
//// ReSharper restore ImplicitlyCapturedClosure
//        }

//        [Test]
//        public void BuildIndexRebuildNoVariations()
//        {
//            var expectedEntries = new[]
//                                      {
//                                          "Lagunitas Maximus IPA",
//                                          "Mikkeller American Dream",
//                                          "Pale Ale",
//                                          "Sierra Nevada",
//                                          "Torpedo",
//                                          "Abstrakt",
//                                          "Punk IPA",
//                                          "Nils Oscar",
//                                          "Coffee Stout",
//                                          "Hop Yard IPA",
//                                          "Mohawk",
//                                          "Unfiltered Lager",
//                                          "Amarillo",
//                                          "Golden Ale",
//                                          "Yeti Imperial Stout",
//                                          "Great Divide",
//                                          "Titan IPA",
//                                          "Pilsner",
//                                          "St Eriks",
//                                          "Single Malt",
//                                          "Tullamore Dew",
//                                          "Grants",
//                                          "Bourbon",
//                                          "Makers Mark"
//                                      };

//            var catalogEntries = new List<string>();
//            _mockedBaseBuilder
//                .Setup( b => b.IndexCatalogEntryDto(
//                    It.IsAny<Func<IndexBuilder, CatalogEntryDto.CatalogEntryRow, string, string[], int>>(),
//                    It.IsAny<CatalogEntryDto.CatalogEntryRow>(),
//                    It.IsAny<string>(),
//                    It.IsAny<string[]>() ) )
//                .Callback<
//                    Func<IndexBuilder, CatalogEntryDto.CatalogEntryRow, string, string[], int>,
//                    CatalogEntryDto.CatalogEntryRow,
//                    string,
//                    string[]>(
//                        ( i, e, c, l ) => catalogEntries.Add( e.Name ) );

//            var mockedConfig = new Mock<IConfiguration>();
//            mockedConfig.Setup( c => c.EnableVariants ).Returns( false );

//            var builder = new ESalesCatalogIndexBuilder( _mockedSystem.Object, _mockedBaseBuilder.Object, mockedConfig.Object );
//            builder.BuildIndex( true );

//            _mockedBaseBuilder.Verify( b => b.DeleteAllContent(), Times.Once() );
//            Assert.That( catalogEntries.ToArray(), Is.EqualTo( expectedEntries ) );
//        }

//        [Test]
//        public void BuildIndexRebuild()
//        {
//            var table = new DataTable();
//            table.Columns.AddRange( new[] { new DataColumn( "Name", typeof( string ) ) } );
//            table.Rows.Add( "Lagunitas Maximus IPA" );
//            table.Rows.Add( "Mikkeller American Dream" );
//            table.Rows.Add( "Sierra Nevada" );
//            table.Rows.Add( "Pale Ale" );
//            table.Rows.Add( "Torpedo" );
//            table.Rows.Add( "Nils Oscar" );
//            table.Rows.Add( "Coffee Stout" );
//            table.Rows.Add( "Hop Yard IPA" );
//            table.Rows.Add( "Mohawk" );
//            table.Rows.Add( "Unfiltered Lager" );
//            table.Rows.Add( "Great Divide" );
//            table.Rows.Add( "Yeti Imperial Stout" );
//            table.Rows.Add( "Titan IPA" );
//            table.Rows.Add( "St Eriks" );
//            table.Rows.Add( "Pilsner" );
//            table.Rows.Add( "Single Malt" );
//            table.Rows.Add( "Bourbon" );
//            table.Rows.Add( "Makers Mark" );
//            var expectedCatalogEntries = table.Rows.Cast<DataRow>().Select( r => r["Name"].ToString() ).ToArray();

//            var catalogEntries = new List<string>();
//            _mockedBaseBuilder
//                .Setup( b => b.IndexCatalogEntryDto(
//                    It.IsAny<Func<IndexBuilder, CatalogEntryDto.CatalogEntryRow, string, string[], int>>(), 
//                    It.IsAny<CatalogEntryDto.CatalogEntryRow>(),
//                    It.IsAny<string>(),
//                    It.IsAny<string[]>() ) )
//                .Callback<
//                    Func<IndexBuilder, CatalogEntryDto.CatalogEntryRow, string, string[], int>,
//                    CatalogEntryDto.CatalogEntryRow,
//                    string,
//                    string[]>(
//                        ( i, e, c, l ) => catalogEntries.Add( e.Name ) );

//            var builder = new ESalesCatalogIndexBuilder( _mockedSystem.Object, _mockedBaseBuilder.Object, _mockedConfig.Object );
//            builder.BuildIndex( true );

//            _mockedBaseBuilder.Verify( b => b.DeleteAllContent(), Times.Once() );
//            Assert.That( catalogEntries.ToArray(), Is.EqualTo( expectedCatalogEntries ) );
//        }

//        [Test]
//        public void BuildIndexIncrementalAdded()
//        {
//            var table = new DataTable();
//            table.Columns.AddRange( new[] { new DataColumn( "Name", typeof( string ) ) } );
//            table.Rows.Add( "Mikkeller American Dream" );

//            table.Rows.Add( "Sierra Nevada" );
//            table.Rows.Add( "Pale Ale" );
//            table.Rows.Add( "Torpedo" );

//            table.Rows.Add( "Coffee Stout" );

//            table.Rows.Add( "Great Divide" );
//            table.Rows.Add( "Yeti Imperial Stout" );
//            table.Rows.Add( "Titan IPA" );

//            table.Rows.Add( "St Eriks" );
//            table.Rows.Add( "Pilsner" );

//            table.Rows.Add( "Single Malt" );

//            table.Rows.Add( "Makers Mark" );
//            var expectedCatalogEntryNames = table.Rows.Cast<DataRow>().Select( r => r["Name"].ToString() ).ToArray();

//            var addedCatalogEntries = new List<string>();
//            _mockedBaseBuilder
//                .Setup( b => b.IndexCatalogEntryDto(
//                    It.IsAny<Func<IndexBuilder, CatalogEntryDto.CatalogEntryRow, string, string[], int>>(),
//                    It.IsAny<CatalogEntryDto.CatalogEntryRow>(),
//                    It.IsAny<string>(),
//                    It.IsAny<string[]>() ) )
//                .Callback<
//                    Func<IndexBuilder, CatalogEntryDto.CatalogEntryRow, string, string[], int>,
//                    CatalogEntryDto.CatalogEntryRow,
//                    string,
//                    string[]>(
//                        ( i, e, c, l ) => addedCatalogEntries.Add( e.Name ) );

//            var builder = new ESalesCatalogIndexBuilder( _mockedSystem.Object, _mockedBaseBuilder.Object, _mockedConfig.Object );
//            builder.BuildIndex( false );

//            Assert.That( addedCatalogEntries.ToArray(), Is.EqualTo( expectedCatalogEntryNames ) );
//        }

//        [Test]
//        public void BuildIndexIncrementalDeletedProducts()
//        {
//            var expectedCatalogEntryIds2 = new[]
//                                               {
//                                                   2,
//                                                   3,

//                                                   5,
//                                                   4,
//                                                   6,

//                                                   9,
//                                                   7,
//                                                   8,

//                                                   11,

//                                                   16,

//                                                   19,
//                                                   21,

//                                                   22,
//                                                   23,
//                                                   24,

//                                                   25,
//                                                   26,
//                                                   27,

//                                                   28,
//                                                   29,
//                                                   30,

//                                                   32,
//                                                   33,

//                                                   35,
//                                                   36
//                                               };

//            var deletedCatalogEntries = new List<int>();
//            _mockedBaseBuilder.Setup( b => b.DeleteProduct( It.IsAny<int>() ) ).Callback<int>( deletedCatalogEntries.Add );

//            var builder = new ESalesCatalogIndexBuilder( _mockedSystem.Object, _mockedBaseBuilder.Object, _mockedConfig.Object );
//            builder.BuildIndex( false );

//            Assert.That( deletedCatalogEntries.ToArray(), Is.EqualTo( expectedCatalogEntryIds2 ) );
//        }

//        [Test]
//        public void BuildIndexIncrementalDeletedVariants()
//        {
//            var expectedCatalogEntryIds = new[]
//                                              {
//                                                  4,
//                                                  6,

//                                                  11,

//                                                  16,

//                                                  22,
//                                                  24,

//                                                  25,
//                                                  27,

//                                                  29,
//                                                  30,

//                                                  35,
//                                                  36
//                                              };

//            var deletedCatalogEntries = new List<int>();
//            _mockedBaseBuilder.Setup( b => b.DeleteVariant( It.IsAny<int>() ) ).Callback<int>( deletedCatalogEntries.Add );

//            var builder = new ESalesCatalogIndexBuilder( _mockedSystem.Object, _mockedBaseBuilder.Object, _mockedConfig.Object );
//            builder.BuildIndex( false );

//            Assert.That( deletedCatalogEntries.ToArray(), Is.EqualTo( expectedCatalogEntryIds ) );
//        }
    }
}