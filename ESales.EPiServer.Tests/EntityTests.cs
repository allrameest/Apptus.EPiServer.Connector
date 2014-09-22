using System;
using System.Collections.Generic;
using System.Linq;
using Apptus.ESales.EPiServer.Import.Ads;
using Apptus.ESales.EPiServer.Import.Products;
using NUnit.Framework;
using Attribute = Apptus.ESales.EPiServer.Import.Attribute;

namespace ESales.EPiServer.Tests
{
    [TestFixture]
    public class EntityTests
    {
        [Test]
        public void CreateVariantMissingVariantKey()
        {
            Assert.Throws<ArgumentException>( () => new Variant( null, "1", Enumerable.Empty<Attribute>() ) );
            Assert.Throws<ArgumentException>( () => new Variant( new[] { new Attribute( "product_key", "1" ) } ) );
            Assert.Throws<ArgumentException>( () => new Variant( "", "1", Enumerable.Empty<Attribute>() ) );
        }

        [Test]
        public void CreateVariantDuplicateRequiredOnlyAttributes()
        {
            var actual = new Variant( "A", new[]
                {
                    new Attribute( "Attr1", "1" ), 
                    new Attribute( "Attr2", "2" ), 
                    new Attribute( "Attr1", "3" ),
                    new Attribute( "variant_key", "B" ), 
                    new Attribute( "product_key", "2" )
                } );
            var expected = new[]
                {
                    new Attribute( "variant_key", "A" ),
                    new Attribute( "product_key", "2" ),
                    new Attribute( "Attr2", "2" ),
                    new Attribute( "Attr1", "3" )
                };
            AssertAttributesEquivalent( actual, expected );
        }

        [Test]
        public void CreateVariantDuplicateAttributes()
        {
            var actual = new Variant( "A", "1", new[]
                {
                    new Attribute( "Attr1", "1" ), 
                    new Attribute( "Attr2", "2" ), 
                    new Attribute( "Attr1", "3" ),
                    new Attribute( "variant_key", "B" ), 
                    new Attribute( "product_key", "2" )
                } );
            var expected = new[]
                {
                    new Attribute( "variant_key", "A" ),
                    new Attribute( "product_key", "1" ),
                    new Attribute( "Attr2", "2" ),
                    new Attribute( "Attr1", "3" )
                };
            AssertAttributesEquivalent( actual, expected );
        }

        [Test]
        public void CreateProductMissingProductKey()
        {
            Assert.Throws<ArgumentException>( () => new Product( null, Enumerable.Empty<Attribute>() ) );
            Assert.Throws<ArgumentException>( () => new Product( new[] { new Attribute( "foo", "bar" ) } ) );
            Assert.Throws<ArgumentException>( () => new Product( "", Enumerable.Empty<Attribute>() ) );
        }

        [Test]
        public void CreateProductDuplicateAttributes()
        {
            var actual = new Product( "1", new[]
                {
                    new Attribute( "Attr1", "1" ),
                    new Attribute( "Attr2", "2" ),
                    new Attribute( "Attr1", "3" ),
                    new Attribute( "product_key", "2" )
                } );
            var expected = new[]
                {
                    new Attribute( "product_key", "1" ),
                    new Attribute( "Attr2", "2" ),
                    new Attribute( "Attr1", "3" )
                };
            AssertAttributesEquivalent( actual, expected );
        }

        [Test]
        public void CreateAdMissingAdKey()
        {
            Assert.Throws<ArgumentException>( () => new Ad( null, new[] { new Attribute( "included", "UNIVERSE" ) } ) );
            Assert.Throws<ArgumentException>( () => new Ad( null, "UNIVERSE", Enumerable.Empty<Attribute>() ) );
            Assert.Throws<ArgumentException>( () => new Ad( new[] { new Attribute( "included", "UNIVERSE" ), new Attribute( "foo", "bar" ) } ) );
            Assert.Throws<ArgumentException>( () => new Ad( "", new[] { new Attribute( "included", "UNIVERSE" ) } ) );
        }

        [Test]
        public void CreateAdDuplicateOnlyRequiredAttributes()
        {
            var actual = new Ad( "a", new[]
                {
                    new Attribute( "Attr1", "1" ), 
                    new Attribute( "Attr2", "2" ), 
                    new Attribute( "Attr1", "3" ),
                    new Attribute( "ad_key", "b" ), 
                    new Attribute( "included", "UNIVERSE" )
                } );
            var expected = new[]
                {
                    new Attribute( "ad_key", "a" ),
                    new Attribute( "included", "UNIVERSE" ),
                    new Attribute( "Attr2", "2" ),
                    new Attribute( "Attr1", "3" )
                };
            AssertAttributesEquivalent( actual, expected );
        }

        [Test]
        public void CreateAdDuplicateAttributes()
        {
            var actual = new Ad( "a", "UNIVERSE", new[]
                {
                    new Attribute( "Attr1", "1" ), 
                    new Attribute( "Attr2", "2" ), 
                    new Attribute( "Attr1", "3" ),
                    new Attribute( "ad_key", "b" ), 
                    new Attribute( "included", "" )
                } );
            var expected = new[]
                {
                    new Attribute( "ad_key", "a" ),
                    new Attribute( "included", "UNIVERSE" ),
                    new Attribute( "Attr2", "2" ),
                    new Attribute( "Attr1", "3" )
                };
            AssertAttributesEquivalent( actual, expected );
        }

        private static void AssertAttributesEquivalent( IEnumerable<Attribute> actual, IEnumerable<Attribute> expected )
        {
            Assert.That( actual.Count(), Is.EqualTo( expected.Count() ) );
            foreach ( var la in actual )
            {
                var ra = expected.Single( a => a.Name == la.Name );
                Assert.That( la.Values, Is.EquivalentTo( ra.Values ) );
            }
        }
    }
}
