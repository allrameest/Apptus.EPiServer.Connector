using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
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
using Attribute = Apptus.ESales.EPiServer.Import.Attribute;

namespace ESales.EPiServer.Tests
{
    [TestFixture]
    public class PluginTests
    {
        [Test]
        public void ProductConverter()
        {
            var builder = Builder();
            var writer = new Mock<IOperationsWriter>();
            var added = new List<IEntity>();
            writer.Setup( w => w.Add( It.IsAny<IEntity>() ) ).Callback<IEntity>( e => added.Add( e ) );
            builder.Register( c => writer.Object );
            var container = builder.Build();

            container.Resolve<ProductIndexBuilder>().Build( false );
            Assert.That( added.Single( a => a.Key.Value == "CODE1_sv_SE" ).Single( a => a.Name == "name" ).Value, Is.EqualTo( "foo1" ) );

            builder = Builder();
            builder.Register( c => writer.Object );
            var nameConverter = new Mock<IProductConverter>();
            nameConverter.Setup( pc => pc.Convert( It.IsAny<IEntity>() ) )
                            .Returns<IEntity>( e => new Product( e.Where( a => a.Name != "name" ).Concat( new[] { new Attribute( "name", "bar" ) } ) ) );
            builder.Register( c => nameConverter.Object );
            container = builder.Build();
            added.Clear();
            container.Resolve<ProductIndexBuilder>().Build( false );
            Assert.That( added.Single( a => a.Key.Value == "CODE1_sv_SE" ).Single( a => a.Name == "name" ).Value, Is.EqualTo( "bar" ) );
        }

        [Test]
        public void AdConverter()
        {
            var fileSystem = new Mock<IFileSystem>();
            var adsStream = new MemoryStream();
            fileSystem.Setup( fs => fs.Open( It.IsAny<string>(), FileMode.CreateNew, FileAccess.Write ) ).Returns( () => ( adsStream = new MemoryStream() ) );

            var builder = Builder();
            builder.Register( c => fileSystem.Object );
            var container = builder.Build();
            var adsIndexBuilder = container.Resolve<AdsIndexBuilder>();
            adsIndexBuilder.Build( false );
            var adsBytes = adsStream.ToArray();
            var ads = Encoding.UTF8.GetString( adsBytes, 3, adsBytes.Length - 3 ); // Remove BOM
            var adsXml = XDocument.Parse( ads );

            Assert.That( adsXml.Descendants( "name" ).Single().Value, Is.EqualTo( "FooAd-SE" ) );

            builder = Builder();
            builder.Register( c => fileSystem.Object );
            var nameConverter = new Mock<IAdConverter>();
            nameConverter.Setup( ac => ac.Convert( It.IsAny<IEntity>() ) )
                            .Returns<IEntity>( e => new Ad( e.Where( a => a.Name != "name" ).Concat( new[] { new Attribute( "name", "bar" ) } ) ) );
            builder.Register( c => nameConverter.Object );
            container = builder.Build();
            adsIndexBuilder = container.Resolve<AdsIndexBuilder>();
            adsIndexBuilder.Build( false );
            adsBytes = adsStream.ToArray();
            ads = Encoding.UTF8.GetString( adsBytes, 3, adsBytes.Length - 3 ); // Remove BOM
            adsXml = XDocument.Parse( ads );

            Assert.That( adsXml.Descendants( "name" ).Single().Value, Is.EqualTo( "bar" ) );
        }

        [Test]
        public void ProductsAppender()
        {
            var builder = Builder();
            var writer = new Mock<IOperationsWriter>();
            var added = new List<IEntity>();
            writer.Setup( w => w.Add( It.IsAny<IEntity>() ) ).Callback<IEntity>( e => added.Add( e ) );
            builder.Register( c => writer.Object );
            var container = builder.Build();

            container.Resolve<ProductIndexBuilder>().Build( false );
            Assert.That( added.Count(), Is.EqualTo( 2 ) );

            builder = Builder();
            builder.Register( c => writer.Object );
            var appender = new Mock<IProductsAppender>();
            appender.Setup( pc => pc.Append( It.IsAny<bool>() ) ).Returns( () => new[] { new Product( "Ext1", new[] { new Attribute( "name", "ExternalFoo" ) } ) } );
            builder.Register( c => appender.Object );
            container = builder.Build();
            added.Clear();
            container.Resolve<ProductIndexBuilder>().Build( false );
            Assert.That( added.Count(), Is.EqualTo( 3 ) );
            var appended = added.Single( p => p.Key.Value == "Ext1" );
            Assert.That( appended.Single( a => a.Name == "name" ).Value, Is.EqualTo( "ExternalFoo" ) );
        }

        [Test]
        public void AdsFullWithAdsNoAppender()
        {
            var fileSystem = new Mock<IFileSystem>();
            var adsStream = new MemoryStream();
            fileSystem.Setup(fs => fs.Open(It.IsAny<string>(), FileMode.CreateNew, FileAccess.Write)).Returns(() => (adsStream = new MemoryStream()));

            var builder = Builder();
            builder.Register(c => fileSystem.Object);
            var container = builder.Build();
            var adsIndexBuilder = container.Resolve<AdsIndexBuilder>();

            adsIndexBuilder.Build(false);
            var adsBytes = adsStream.ToArray();
            var ads = Encoding.UTF8.GetString(adsBytes, 3, adsBytes.Length - 3); // Remove BOM
            var adsXml = XDocument.Parse(ads);

            Assert.That(adsXml.Element("operations").Element("add").Elements("ad").Count(), Is.EqualTo(1));
        }

        [Test]
        public void AdsFullNoAdsNoAppender()
        {
            var fileSystem = new Mock<IFileSystem>();
            var adsStream = new MemoryStream();
            fileSystem.Setup(fs => fs.Open(It.IsAny<string>(), FileMode.CreateNew, FileAccess.Write)).Returns(() => (adsStream = new MemoryStream()));

            var builder = Builder();
            builder.Register(c => fileSystem.Object);
            builder.Register(c => new PromotionDto.PromotionLanguageDataTable());
            builder.Register(c => new PromotionDto.PromotionDataTable());
            builder.Register(c => new CampaignDto.CampaignDataTable());
            builder.RegisterType<PromotionDataTableMapper>();
            builder.RegisterType<PromotionEntryCodeProvider>();
            var container = builder.Build();
            var adsIndexBuilder = container.Resolve<AdsIndexBuilder>();

            adsIndexBuilder.Build(false);
            var adsBytes = adsStream.ToArray();
            var ads = Encoding.UTF8.GetString(adsBytes, 3, adsBytes.Length - 3); // Remove BOM
            var adsXml = XDocument.Parse(ads);

            Assert.That(adsXml.Element("operations").Element("add").Elements("ad").Any(), Is.False);
        }

        [Test]
        public void AdsFullWithAdsWithAppender()
        {
            var fileSystem = new Mock<IFileSystem>();
            var adsStream = new MemoryStream();
            fileSystem.Setup(fs => fs.Open(It.IsAny<string>(), FileMode.CreateNew, FileAccess.Write)).Returns(() => (adsStream = new MemoryStream()));

            var builder = Builder();
            builder.Register(c => fileSystem.Object);
            var appender = new Mock<IAdsAppender>();
            appender.Setup(ac => ac.Append(It.IsAny<bool>())).Returns(() => new[] { new Ad("Ext1", new[] { new Attribute("name", "ExtFooAd1") }) });
            builder.Register(c => appender.Object);
            var container = builder.Build();
            var adsIndexBuilder = container.Resolve<AdsIndexBuilder>();

            adsIndexBuilder.Build(false);
            var adsBytes = adsStream.ToArray();
            var ads = Encoding.UTF8.GetString(adsBytes, 3, adsBytes.Length - 3); // Remove BOM
            var adsXml = XDocument.Parse(ads);

            Assert.That(adsXml.Element("operations").Element("add").Elements("ad").Count(), Is.EqualTo(2));
            var matchingAd = adsXml.Element("operations").Element("add").Elements("ad").Single(a => a.Element("ad_key").Value == "Ext1");
            Assert.That(matchingAd.Element("name").Value, Is.EqualTo("ExtFooAd1"));
        }

        [Test]
        public void AdsFullNoAdsWithAppender()
        {
            var fileSystem = new Mock<IFileSystem>();
            var adsStream = new MemoryStream();
            fileSystem.Setup(fs => fs.Open(It.IsAny<string>(), FileMode.CreateNew, FileAccess.Write)).Returns(() => (adsStream = new MemoryStream()));

            var builder = Builder();
            builder.Register(c => fileSystem.Object);
            builder.Register(c => new PromotionDto.PromotionLanguageDataTable());
            builder.Register(c => new PromotionDto.PromotionDataTable());
            builder.Register(c => new CampaignDto.CampaignDataTable());
            builder.RegisterType<PromotionDataTableMapper>();
            builder.RegisterType<PromotionEntryCodeProvider>();
            var appender = new Mock<IAdsAppender>();
            appender.Setup(ac => ac.Append(It.IsAny<bool>())).Returns(() => new[] { new Ad("Ext1", new[] { new Attribute("name", "ExtFooAd1") }) });
            builder.Register(c => appender.Object);
            var container = builder.Build();
            var adsIndexBuilder = container.Resolve<AdsIndexBuilder>();

            adsIndexBuilder.Build(false);
            var adsBytes = adsStream.ToArray();
            var ads = Encoding.UTF8.GetString(adsBytes, 3, adsBytes.Length - 3); // Remove BOM
            var adsXml = XDocument.Parse(ads);

            Assert.That(adsXml.Element("operations").Element("add").Elements("ad").Count(), Is.EqualTo(1));
            var matchingAd = adsXml.Element("operations").Element("add").Elements("ad").Single(a => a.Element("ad_key").Value == "Ext1");
            Assert.That(matchingAd.Element("name").Value, Is.EqualTo("ExtFooAd1"));
        }

        [Test]
        public void AdsIncrementalWithChangesNoAppender()
        {
            var fileSystem = new Mock<IFileSystem>();
            var adsStream = new MemoryStream();
            fileSystem.Setup(fs => fs.Open(It.IsAny<string>(), FileMode.CreateNew, FileAccess.Write)).Returns(() => (adsStream = new MemoryStream()));

            var builder = Builder();
            builder.Register(c => fileSystem.Object);
            var container = builder.Build();
            var adsIndexBuilder = container.Resolve<AdsIndexBuilder>();

            adsIndexBuilder.Build(true);
            var adsBytes = adsStream.ToArray();

            Assert.That(adsBytes.Any(), Is.False);
        }

        [Test]
        public void AdsIncrementalWithChangesWithAppender()
        {
            var fileSystem = new Mock<IFileSystem>();
            var adsStream = new MemoryStream();
            fileSystem.Setup(fs => fs.Open(It.IsAny<string>(), FileMode.CreateNew, FileAccess.Write)).Returns(() => (adsStream = new MemoryStream()));

            var builder = Builder();
            builder.Register(c => fileSystem.Object);
            var appender = new Mock<IAdsAppender>();
            appender.Setup(ac => ac.Append(It.IsAny<bool>())).Returns(() => new[] { new Ad("Ext1", new[] { new Attribute("name", "ExtFooAd1") }) });
            builder.Register(c => appender.Object);
            var container = builder.Build();
            var adsIndexBuilder = container.Resolve<AdsIndexBuilder>();

            adsIndexBuilder.Build(true);
            var adsBytes = adsStream.ToArray();
            var ads = Encoding.UTF8.GetString(adsBytes, 3, adsBytes.Length - 3); // Remove BOM
            var adsXml = XDocument.Parse(ads);

            Assert.That(adsXml.Element("operations").Element("add").Elements("ad").Count(), Is.EqualTo(1));
            var matchingAd = adsXml.Element("operations").Element("add").Elements("ad").Single(a => a.Element("ad_key").Value == "Ext1");
            Assert.That(matchingAd.Element("name").Value, Is.EqualTo("ExtFooAd1"));
        }

        [Test]
        public void AdsIncrementalNoChangesNoAppender()
        {
            var fileSystem = new Mock<IFileSystem>();
            var adsStream = new MemoryStream();
            fileSystem.Setup(fs => fs.Open(It.IsAny<string>(), FileMode.CreateNew, FileAccess.Write)).Returns(() => (adsStream = new MemoryStream()));

            var builder = Builder();
            builder.Register(c => fileSystem.Object);
            builder.Register(c => new PromotionDto.PromotionLanguageDataTable());
            builder.Register(c => new PromotionDto.PromotionDataTable());
            builder.Register(c => new CampaignDto.CampaignDataTable());
            builder.RegisterType<PromotionDataTableMapper>();
            builder.RegisterType<PromotionEntryCodeProvider>();
            var container = builder.Build();
            var adsIndexBuilder = container.Resolve<AdsIndexBuilder>();

            adsIndexBuilder.Build(true);
            var adsBytes = adsStream.ToArray();

            Assert.That(adsBytes.Any(), Is.False);
        }

        [Test]
        public void AdsIncrementalNoChangesWithAppender()
        {
            var fileSystem = new Mock<IFileSystem>();
            var adsStream = new MemoryStream();
            fileSystem.Setup(fs => fs.Open(It.IsAny<string>(), FileMode.CreateNew, FileAccess.Write)).Returns(() => (adsStream = new MemoryStream()));

            var builder = Builder();
            builder.Register(c => fileSystem.Object);
            builder.Register(c => new PromotionDto.PromotionLanguageDataTable());
            builder.Register(c => new PromotionDto.PromotionDataTable());
            builder.Register(c => new CampaignDto.CampaignDataTable());
            builder.RegisterType<PromotionDataTableMapper>();
            builder.RegisterType<PromotionEntryCodeProvider>();
            var appender = new Mock<IAdsAppender>();
            appender.Setup(ac => ac.Append(It.IsAny<bool>())).Returns(() => new[] { new Ad("Ext1", new[] { new Attribute("name", "ExtFooAd1") }) });
            builder.Register(c => appender.Object);
            var container = builder.Build();
            var adsIndexBuilder = container.Resolve<AdsIndexBuilder>();

            adsIndexBuilder.Build(true);
            var adsBytes = adsStream.ToArray();
            var ads = Encoding.UTF8.GetString(adsBytes, 3, adsBytes.Length - 3); // Remove BOM
            var adsXml = XDocument.Parse(ads);

            Assert.That(adsXml.Element("operations").Element("add").Elements("ad").Count(), Is.EqualTo(1));
            var matchingAd = adsXml.Element("operations").Element("add").Elements("ad").Single(a => a.Element("ad_key").Value == "Ext1");
            Assert.That(matchingAd.Element("name").Value, Is.EqualTo("ExtFooAd1"));
        }


        private static ContainerBuilder Builder()
        {
            var appConfig = new Mock<IAppConfig>();
            appConfig.Setup( ac => ac.EnableAds ).Returns( true );
            var catalogSystem = new Mock<ICatalogSystemMapper>();
            catalogSystem.Setup( cs => cs.GetCatalogs() ).Returns( new CatalogDto.CatalogDataTable().WithRow( "FooCatalog" ) );
            catalogSystem
                .Setup( cs => cs.StartFindItemsForIndexing( It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>() ) )
                .Returns( 1 );
            catalogSystem.Setup( cs => cs.ContinueFindItemsForIndexing( It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>() ) )
                         .Returns( new CatalogEntryDto.CatalogEntryDataTable().WithRow( 1, "CODE1", "foo1" ).WithRow( 2, "CODE2", "foo2" ) );
            var metaData = new Mock<IMetaDataMapper>();
            var priceService = new Mock<IPriceServiceMapper>();
            var keyLookup = new Mock<IKeyLookup>();
            keyLookup.Setup( kl => kl.Value( It.IsAny<CatalogEntryDto.CatalogEntryRow>(), It.IsAny<string>() ) )
                     .Returns<CatalogEntryDto.CatalogEntryRow, string>( ( e, l ) => AttributeHelper.CreateKey( e.Code, l ) );
            var configuration = new Mock<IConfiguration>();
            configuration.Setup( c => c.ProductAttributes ).Returns( new[] { new ConfigurationAttribute( "name", type.@string, Present.Yes ) } );
            var indexSystem = new Mock<IIndexSystemMapper>();
            var urlResolver = new Mock<IUrlResolver>();

            var builder = new ContainerBuilder();
            builder.RegisterType<ProductIndexBuilder>();
            builder.RegisterType<EntryConverter>();
            builder.RegisterType<CatalogEntryProviderFactory>();
            builder.Register( c => catalogSystem.Object );
            builder.Register( c => metaData.Object );
            builder.Register( c => priceService.Object );
            builder.Register( c => keyLookup.Object );
            builder.Register( c => configuration.Object );
            builder.RegisterType<Formatter>();
            builder.RegisterAssemblyTypes( typeof( Formatter ).Assembly ).As<IFormatRule>();
            builder.RegisterType<AdsIndexBuilder>();
            builder.Register( c => appConfig.Object );
            builder.RegisterType<FileHelper>();
            builder.Register( c => new PromotionDto.PromotionLanguageDataTable().WithRow( 1, 2, "sv-se", "FooAd-SE" ) );
            builder.Register( c => new PromotionDto.PromotionDataTable().WithRow( 2, "FooAd", "FooBarType", 3 ) );
            builder.Register( c => new CampaignDto.CampaignDataTable().WithRow( 3, "FooCampaign" ) );
            builder.RegisterType<PromotionDataTableMapper>();
            builder.RegisterType<PromotionEntryCodeProvider>();
            builder.Register( c => indexSystem.Object );
            builder.Register( c => new ESalesVariantHelper( Enumerable.Empty<CatalogRelationDto.CatalogEntryRelationRow>() ) );
            builder.Register( c => urlResolver.Object );

            return builder;
        }
    }
}
