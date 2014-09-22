using System.IO;
using System.Text;
using Apptus.ESales.EPiServer.Config;
using Apptus.ESales.EPiServer.DataAccess;
using Apptus.ESales.EPiServer.Import.Ads;
using Apptus.ESales.EPiServer.Import.Configuration;
using Apptus.ESales.EPiServer.Resources;
using Autofac;
using ESales.EPiServer.TestHelper;
using Mediachase.MetaDataPlus.Configurator;
using Moq;
using NUnit.Framework;

namespace ESales.EPiServer.Tests
{
    [TestFixture]
    public class ConfigurationTests
    {
        [Test]
        public void ProductAttributes()
        {
            var appConfigMock = new Mock<IAppConfig>();
            var metaDataMock = new Mock<IMetaDataMapper>();
            metaDataMock.Setup( mdm => mdm.GetAll() ).Returns(
                () =>
                new[]
                    {
                        new MetaClassEx( 1, "FooBar", new[]
                            {
                                new MetaFieldEx( "testns", "Foo", "Foo", "FooDesc", MetaDataType.NVarChar, 10, true, true, true, false, true, true, true ),
                                new MetaFieldEx( "testns", "Bar", "Bar", "BarDesc", MetaDataType.Int, 4, true, true, true, false, true, true, true )
                            } )
                    } );
            var fileSystemMock = new Mock<IFileSystem>();
            var writtenConfig = new MemoryStream();
            fileSystemMock
                .Setup( fsm => fsm.Open( It.IsAny<string>(), It.IsAny<FileMode>(), It.IsAny<FileAccess>() ) )
                .Returns<string, FileMode, FileAccess>(
                    ( p, fm, fa ) =>
                    fa == FileAccess.Read
                        ? new MemoryStream( new UTF8Encoding( false ).GetBytes( "<SearchConfig><SearchFilters></SearchFilters></SearchConfig>" ) )
                        : writtenConfig );
            var container = Build( appConfigMock.Object, metaDataMock.Object, fileSystemMock.Object );

            var writer = container.Resolve<ConfigurationWriter>();
            writer.WriteConfiguration();
            var actual = Loader.Deserialize<configuration>( new MemoryStream( writtenConfig.ToArray() ) );

            configuration expected;
            using ( var stream = new FileStream( "configuration-product-attributes.xml", FileMode.Open, FileAccess.Read ) )
            {
                expected = Loader.Deserialize<configuration>( stream );
            }

            ConfigurationAssertions.AssertAttributesEquivalent( actual.product_attributes, expected.product_attributes );
        }

        [Test]
        public void FacetAttributes()
        {
            var appConfigMock = new Mock<IAppConfig>();
            var metaDataMock = new Mock<IMetaDataMapper>();
            metaDataMock.Setup( mdm => mdm.GetAll() ).Returns(
                () =>
                new[]
                    {
                        new MetaClassEx( 1, "ColorClass", new[]
                            {
                                new MetaFieldEx( "testns", "Color", "Color", "ColorDesc", MetaDataType.NVarChar, 10, true, true, true, false, true, true, true )
                            } )
                    } );
            var fileSystemMock = new Mock<IFileSystem>();
            var writtenConfig = new MemoryStream();
            fileSystemMock
                .Setup( fsm => fsm.Open( It.IsAny<string>(), It.IsAny<FileMode>(), It.IsAny<FileAccess>() ) )
                .Returns<string, FileMode, FileAccess>(
                    ( p, fm, fa ) =>
                    fa == FileAccess.Read
                        ? new MemoryStream( new UTF8Encoding( false ).GetBytes( 
                            @"<SearchConfig>
                                <SearchFilters>
                                    <Filter field=""color"">
                                        <Descriptions defaultLocale=""en-us"">
                                            <Description locale=""en-us"">Color</Description>
                                        </Descriptions>
                                        <Values>
                                            <SimpleValue key=""white"" value=""white"">
                                                <Descriptions defaultLocale=""en-us"">
                                                    <Description locale=""en-us"">White</Description>
                                                </Descriptions>
                                            </SimpleValue>
                                            <SimpleValue key=""red"" value=""red"">
                                                <Descriptions defaultLocale=""en-us"">
                                                    <Description locale=""en-us"">Red</Description>
                                                </Descriptions>
                                            </SimpleValue>
                                            <SimpleValue key=""rosé"" value=""rosé"">
                                                <Descriptions defaultLocale=""en-us"">
                                                    <Description locale=""en-us"">Rosé</Description>
                                                </Descriptions>
                                            </SimpleValue>
                                        </Values>
                                    </Filter>
                                </SearchFilters>
                            </SearchConfig>" ) )
                        : writtenConfig );
            var container = Build( appConfigMock.Object, metaDataMock.Object, fileSystemMock.Object );

            var writer = container.Resolve<ConfigurationWriter>();
            writer.WriteConfiguration();
            var actual = Loader.Deserialize<configuration>( new MemoryStream( writtenConfig.ToArray() ) );

            configuration expected;
            using ( var stream = new FileStream( "configuration-facet-attributes.xml", FileMode.Open, FileAccess.Read ) )
            {
                expected = Loader.Deserialize<configuration>( stream );
            }

            ConfigurationAssertions.AssertAttributesEquivalent( actual.product_attributes, expected.product_attributes );
        }

        [Test]
        public void AdAttributes()
        {
            var appConfigMock = new Mock<IAppConfig>();
            appConfigMock.Setup( acm => acm.AdsSource ).Returns( "commerce" );
            appConfigMock.Setup( acm => acm.EnableAds ).Returns( true );
            var metaDataMock = new Mock<IMetaDataMapper>();
            var fileSystemMock = new Mock<IFileSystem>();
            var writtenConfig = new MemoryStream();
            fileSystemMock
                .Setup( fsm => fsm.Open( It.IsAny<string>(), It.IsAny<FileMode>(), It.IsAny<FileAccess>() ) )
                .Returns<string, FileMode, FileAccess>(
                    ( p, fm, fa ) =>
                    fa == FileAccess.Read
                        ? new MemoryStream( new UTF8Encoding( false ).GetBytes( "<SearchConfig><SearchFilters></SearchFilters></SearchConfig>" ) )
                        : writtenConfig );
            var container = Build( appConfigMock.Object, metaDataMock.Object, fileSystemMock.Object );

            var writer = container.Resolve<ConfigurationWriter>();
            writer.WriteConfiguration();
            var actual = Loader.Deserialize<configuration>( new MemoryStream( writtenConfig.ToArray() ) );

            configuration expected;
            using ( var stream = new FileStream( "configuration-ad-attributes.xml", FileMode.Open, FileAccess.Read ) )
            {
                expected = Loader.Deserialize<configuration>( stream );
            }

            ConfigurationAssertions.AssertAttributesEquivalent( actual.ad_attributes, expected.ad_attributes );
        }

        private static IContainer Build( IAppConfig appConfig, IMetaDataMapper metaData, IFileSystem fileSystem )
        {
            var builder = new ContainerBuilder();
            builder.Register( c => appConfig );
            builder.Register( c => metaData );
            builder.Register( c => fileSystem );
            builder.RegisterType<AdAttributeHelper>();
            builder.Register( c => new ConfigurationOptions( Loader.LoadBaseConfiguration() ) );
            builder.RegisterType<AttributeConverter>();
            builder.RegisterType<Configuration>().As<IConfiguration>();
            builder.RegisterType<FileHelper>();
            builder.RegisterType<ConfigurationWriter>();
            return builder.Build();
        }
    }
}


