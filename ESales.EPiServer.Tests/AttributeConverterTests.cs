using System.Linq;
using Apptus.ESales.EPiServer.Import.Configuration;
using Apptus.ESales.EPiServer.Resources;
using Autofac;
using NUnit.Framework;

namespace ESales.EPiServer.Tests
{
    [TestFixture]
    public class AttributeConverterTests
    {
        private IContainer _container;

        [SetUp]
        public void Init()
        {
            var builder = new ContainerBuilder();
            builder.Register( c => new configuration
                {
                    formats = Loader.Load<formats>().format,
                    tokenizations = Loader.Load<tokenizations>().tokenization,
                    normalizations = Loader.Load<normalizations>().normalization
                } );
            builder.RegisterType<ConfigurationOptions>();
            builder.RegisterType<AttributeConverter>();
            _container = builder.Build();
        }

        [Test]
        public void AttributeNoOptions()
        {
            var attribute = _container.Resolve<AttributeConverter>().Convert( new ConfigurationAttribute( "no_options", type.@string, Present.No ) );
            Assert.That( attribute.name, Is.EqualTo( "no_options" ) );
            Assert.That( attribute.present, Is.Null );
            Assert.That( attribute.filter_attributes, Is.Not.Null );
            Assert.That( attribute.filter_attributes.Length, Is.EqualTo( 0 ) );
            Assert.That( attribute.search_attributes, Is.Not.Null );
            Assert.That( attribute.search_attributes.Length, Is.EqualTo( 0 ) );
            Assert.That( attribute.sort_attributes, Is.Not.Null );
            Assert.That( attribute.sort_attributes.Length, Is.EqualTo( 0 ) );
        }

        [Test]
        public void AttributePresentYes()
        {
            var attribute = _container.Resolve<AttributeConverter>().Convert( new ConfigurationAttribute( "no_options", type.@string, Present.Yes ) );
            Assert.That( attribute.name, Is.EqualTo( "no_options" ) );
            Assert.That( attribute.present, Is.Not.Null );
            Assert.That( attribute.present.xml, Is.False );
            Assert.That( attribute.filter_attributes, Is.Not.Null );
            Assert.That( attribute.filter_attributes.Length, Is.EqualTo( 0 ) );
            Assert.That( attribute.search_attributes, Is.Not.Null );
            Assert.That( attribute.search_attributes.Length, Is.EqualTo( 0 ) );
            Assert.That( attribute.sort_attributes, Is.Not.Null );
            Assert.That( attribute.sort_attributes.Length, Is.EqualTo( 0 ) );
        }

        [Test]
        public void AttributePresentYesXml()
        {
            var attribute = _container.Resolve<AttributeConverter>().Convert( new ConfigurationAttribute( "no_options", type.@string, Present.YesXml ) );
            Assert.That( attribute.name, Is.EqualTo( "no_options" ) );
            Assert.That( attribute.present, Is.Not.Null );
            Assert.That( attribute.present.xml, Is.True );
            Assert.That( attribute.filter_attributes, Is.Not.Null );
            Assert.That( attribute.filter_attributes.Length, Is.EqualTo( 0 ) );
            Assert.That( attribute.search_attributes, Is.Not.Null );
            Assert.That( attribute.search_attributes.Length, Is.EqualTo( 0 ) );
            Assert.That( attribute.sort_attributes, Is.Not.Null );
            Assert.That( attribute.sort_attributes.Length, Is.EqualTo( 0 ) );
        }

        [Test]
        public void AttributeSearch()
        {
            const string name = "searchAttribute";
            var attribute =
                _container.Resolve<AttributeConverter>()
                          .Convert( new ConfigurationAttribute( name, type.@string, Present.No, new SearchOptions( "sv_SE", Format.None, true, true ) ) );
            Assert.That( attribute.name, Is.EqualTo( name ) );
            Assert.That( attribute.present, Is.Null );
            Assert.That( attribute.filter_attributes, Is.Not.Null );
            Assert.That( attribute.filter_attributes.Length, Is.EqualTo( 0 ) );
            Assert.That( attribute.sort_attributes, Is.Not.Null );
            Assert.That( attribute.sort_attributes.Length, Is.EqualTo( 0 ) );

            Assert.That( attribute.search_attributes, Is.Not.Null );
            var search = new search_attribute();
            Assert.DoesNotThrow( () => search = attribute.search_attributes.Single() );
            Assert.That( search.format, Is.Not.Null );
            Assert.That( search.format.name, Is.EqualTo( "(no format)" ) );
            Assert.That( search.locale, Is.EqualTo( "sv_SE" ) );
            Assert.That( search.match_suffix, Is.True );
            Assert.That( search.suggest, Is.True );
            Assert.That( search.name, Is.EqualTo( name ) );
        }

        [Test]
        public void AttributeFilter()
        {
            const string name = "filterAttribute";
            var attribute =
                _container.Resolve<AttributeConverter>()
                          .Convert( new ConfigurationAttribute( name, type.@string, Present.No,
                                                   filterOptions: new FilterOptions( Format.CommaSeparated, Tokenization.ModelDesignation ) ) );
            Assert.That( attribute.name, Is.EqualTo( name ) );
            Assert.That( attribute.present, Is.Null );
            Assert.That( attribute.search_attributes, Is.Not.Null );
            Assert.That( attribute.search_attributes.Length, Is.EqualTo( 0 ) );
            Assert.That( attribute.sort_attributes, Is.Not.Null );
            Assert.That( attribute.sort_attributes.Length, Is.EqualTo( 0 ) );

            Assert.That( attribute.filter_attributes, Is.Not.Null );
            var filter = new filter_attribute();
            Assert.DoesNotThrow( () => filter = attribute.filter_attributes.Single() );
            Assert.That( filter.format, Is.Not.Null );
            Assert.That( filter.format.name, Is.EqualTo( "Comma-separated list (e.g. a,b,c)" ) );
            Assert.That( filter.name, Is.EqualTo( name ) );
            Assert.That( filter.tokenization, Is.Not.Null );
            Assert.That( filter.tokenization.name, Is.EqualTo( "Model designation (e.g. MB12a)" ) );
            Assert.That( filter.type, Is.EqualTo( type.@string ) );
        }

        [Test]
        public void AttributeSort()
        {
            const string name = "sortAttribute";
            var attribute =
                _container.Resolve<AttributeConverter>()
                          .Convert( new ConfigurationAttribute( name, type.@string, Present.No, sortOptions: new SortOptions( Normalization.DigitsLetters ) ) );
            Assert.That( attribute.name, Is.EqualTo( name ) );
            Assert.That( attribute.present, Is.Null );
            Assert.That( attribute.filter_attributes, Is.Not.Null );
            Assert.That( attribute.filter_attributes.Length, Is.EqualTo( 0 ) );
            Assert.That( attribute.search_attributes, Is.Not.Null );
            Assert.That( attribute.search_attributes.Length, Is.EqualTo( 0 ) );

            Assert.That( attribute.sort_attributes, Is.Not.Null );
            var sort = new sort_attribute();
            Assert.DoesNotThrow( () => sort = attribute.sort_attributes.Single() );
            Assert.That( sort.name, Is.EqualTo( name ) );
            Assert.That( sort.normalization, Is.Not.Null );
            Assert.That( sort.normalization.name, Is.EqualTo( "Keep digits and letters (e.g. Text123)" ) );
            Assert.That( sort.type, Is.EqualTo( type.@string ) );
        }
    }
}
