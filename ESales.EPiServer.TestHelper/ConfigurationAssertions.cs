using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ESales.EPiServer.TestHelper
{
    public class ConfigurationAssertions
    {
        public static void AssertAttributesEquivalent( attribute[] actual, attribute[] expected )
        {
            if ( !AssertNullEquivalent( actual, expected ) )
            {
                return;
            }

            IEnumerable<attribute> missing;
            if ( ( missing = expected.Where( e => !actual.Select( a => a.name ).Contains( e.name ) ) ).Any() )
            {
                Assert.Fail( "Missing attributes: " + string.Join( ", ", missing.Select( m => m.name ) ) );
            }
            IEnumerable<attribute> unexpected;
            if ( ( unexpected = actual.Where( a => !expected.Select( e => e.name ).Contains( a.name ) ) ).Any() )
            {
                Assert.Fail( "Unexpected attributes: " + string.Join( ", ", unexpected.Select( u => u.name ) ) );
            }

            foreach ( var a in actual )
            {
                try
                {
                    AssertAttributesEquivalent( a, expected );
                }
                catch ( Exception )
                {
                    Console.WriteLine( "Error in attribute: " + a.name );
                    throw;
                }
            }
        }

        private static void AssertAttributesEquivalent( attribute attribute, IEnumerable<attribute> expected )
        {
            var match = expected.Single( a => a.name == attribute.name );
            if ( AssertNullEquivalent( attribute.present, match.present ) )
            {
                Assert.That( attribute.present.xml, Is.EqualTo( match.present.xml ) );
            }
            AssertFiltersEquivalent( attribute.filter_attributes, match.filter_attributes );
            AssertSortEquivalent( attribute.sort_attributes, match.sort_attributes );
            AssertSearchEquivalent( attribute.search_attributes, match.search_attributes );
        }

        private static void AssertSearchEquivalent( search_attribute[] actual, search_attribute[] expected )
        {
            if ( !AssertNullEquivalent(actual, expected) )
            {
                return;
            }
            Assert.That( actual.Length, Is.EqualTo( expected.Length ) );
            Array.ForEach( actual, sa => AssertSearchEquivalent( sa, expected ) );
        }

        private static void AssertSearchEquivalent( search_attribute actual, IEnumerable<search_attribute> expected )
        {
            var match = expected.Single( a => a.name == actual.name );
            Assert.That( actual.format.name, Is.EqualTo( match.format.name ) );
            Assert.That( actual.locale, Is.EqualTo( match.locale ) );
            Assert.That( actual.match_suffix, Is.EqualTo( match.match_suffix ) );
            Assert.That( actual.suggest, Is.EqualTo( match.suggest ) );
        }

        private static void AssertSortEquivalent( sort_attribute[] actual, sort_attribute[] expected )
        {
            if ( !AssertNullEquivalent( actual, expected ) )
            {
                return;
            }
            Assert.That( actual.Length, Is.EqualTo( expected.Length ) );
            Array.ForEach( actual, l => AssertSortEquivalent( l, expected ) );
        }

        private static void AssertSortEquivalent( sort_attribute actual, IEnumerable<sort_attribute> expected )
        {
            var match = expected.Single( a => a.name == actual.name );
            Assert.That( actual.normalization.name, Is.EqualTo( match.normalization.name ) );
            Assert.That( actual.type, Is.EqualTo( match.type ) );
        }

        private static void AssertFiltersEquivalent( filter_attribute[] actual, filter_attribute[] expected )
        {
            if ( !AssertNullEquivalent( actual, expected ) )
            {
                return;
            }

            Assert.That( actual.Length, Is.EqualTo( expected.Length ) );
            Array.ForEach( actual, fa => AssertFiltersEquivalent( fa, expected ) );
        }

        private static void AssertFiltersEquivalent( filter_attribute actual, IEnumerable<filter_attribute> expected )
        {
            var match = expected.Single( a => a.name == actual.name );
            Assert.That( actual.type, Is.EqualTo( match.type ) );
            Assert.That( actual.format.name, Is.EqualTo( match.format.name ) );
            Assert.That( actual.tokenization.name, Is.EqualTo( match.tokenization.name ) );
        }


// ReSharper disable UnusedParameter.Local
        private static bool AssertNullEquivalent( object actual, object expected )
// ReSharper restore UnusedParameter.Local
        {
            if ( actual == null && expected != null )
            {
                Assert.Fail( "Actual is null but expected is not." );
            }
            if ( actual != null && expected == null )
            {
                Assert.Fail( "Actual is not null but expected is." );
            }
            if ( actual == null )
            {
                // Both are null
                return false;
            }
            return true;
        }
    }
}
