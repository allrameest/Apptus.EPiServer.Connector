using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Apptus.ESales.EPiServer.Config;
using Apptus.ESales.EPiServer.DataAccess;
using Apptus.ESales.EPiServer.Import;
using Apptus.ESales.EPiServer.Import.Ads;
using Apptus.ESales.EPiServer.Import.Configuration;
using Apptus.ESales.EPiServer.Import.Formatting;
using Apptus.ESales.EPiServer.Import.Products;
using Apptus.ESales.EPiServer.Util;
using Autofac;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Marketing.Dto;
using Moq;
using NUnit.Framework;

namespace ESales.EPiServer.Tests
{
    [TestFixture]
    public class ProductIndexBuilderAttributeTests
    {
        [Test]
        public void ProductUrl()
        {
            var entries = new CatalogEntryDto.CatalogEntryDataTable().WithRow( 1, "CODE1", "foo1" );
            var added = new List<IEntity>();
            var writer = new Mock<IOperationsWriter>();
            writer.Setup( w => w.Add( It.IsAny<IEntity>() ) ).Callback<IEntity>( e => added.Add( e ) );
            var productIndexBuilder = Build( entries, writer.Object );

            productIndexBuilder.Build( false );

            var product = added.Single();
            var productUrl = product.Single( a => a.Name == "product_url" );
            Assert.That( productUrl.Value, Is.EqualTo( "~/foo1.aspx" ) );
        }

        private static ProductIndexBuilder Build( CatalogEntryDto.CatalogEntryDataTable entries, IOperationsWriter writer )
        {
            var appConfig = new Mock<IAppConfig>();
            var catalogSystem = new Mock<ICatalogSystemMapper>();
            var indexSystem = new Mock<IIndexSystemMapper>();
            var keyLookup = new Mock<IKeyLookup>();
            var metaData = new Mock<IMetaDataMapper>();
            var priceService = new Mock<IPriceServiceMapper>();
            var fileSystem = new Mock<IFileSystem>();
            var entryAdditionalData = new Mock<IEntryAdditionalData>();
            entryAdditionalData
                .Setup( ead => ead.GetCatalogItemSeoRows( It.IsAny<CatalogEntryDto.CatalogEntryRow>() ) )
                .Returns<CatalogEntryDto.CatalogEntryRow>( e => e.GetSeo() );

            catalogSystem.Setup( cs => cs.GetCatalogs() ).Returns( new CatalogDto.CatalogDataTable().WithRow( "FooCatalog" ) );
            catalogSystem
                .Setup( cs => cs.StartFindItemsForIndexing( It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>() ) )
                .Returns( entries.Count );
            catalogSystem
                .Setup( cs => cs.ContinueFindItemsForIndexing( It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>() ) )
                .Returns( entries );

            keyLookup
                .Setup( kl => kl.Value( It.IsAny<CatalogEntryDto.CatalogEntryRow>(), It.IsAny<string>() ) )
                .Returns<CatalogEntryDto.CatalogEntryRow, string>( ( e, l ) => AttributeHelper.CreateKey( e.Code, l ) );

            fileSystem
                .Setup( fsm => fsm.Open( It.IsAny<string>(), It.IsAny<FileMode>(), It.IsAny<FileAccess>() ) )
                .Returns( new MemoryStream( new UTF8Encoding( false ).GetBytes( "<SearchConfig><SearchFilters/></SearchConfig>" ) ) );

            var builder = new ContainerBuilder();
            builder.RegisterType<ProductIndexBuilder>();
            builder.Register( c => appConfig.Object );
            builder.Register( c => catalogSystem.Object );
            builder.Register( c => indexSystem.Object );
            builder.Register( c => keyLookup.Object );
            builder.Register( c => writer );
            builder.Register( c => metaData.Object );
            builder.Register( c => priceService.Object );
            builder.Register( c => entryAdditionalData.Object );
            builder.RegisterType<EntryConverter>();
            builder.RegisterType<CatalogEntryProviderFactory>();
            builder.Register( c => new PromotionEntryCodeProvider( c.Resolve<PromotionDataTableMapper>(), Enumerable.Empty<IPromotionEntryCodes>() ) );
            builder.Register( c => new PromotionDataTableMapper(
                                       new PromotionDto.PromotionLanguageDataTable(),
                                       new PromotionDto.PromotionDataTable(),
                                       new CampaignDto.CampaignDataTable() ) );
            builder.RegisterAssemblyTypes( typeof( IFormatRule ).Assembly ).As<IFormatRule>();
            builder.RegisterType<Formatter>();
            builder.RegisterType<Configuration>().As<IConfiguration>();
            builder.Register( c => fileSystem.Object );
            builder.RegisterType<AdAttributeHelper>();
            builder.RegisterType<UrlResolver>().As<IUrlResolver>();
            var container = builder.Build();
            return container.Resolve<ProductIndexBuilder>();
        }
    }
}
