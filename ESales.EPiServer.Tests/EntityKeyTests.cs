using Apptus.ESales.EPiServer.Util;
using NUnit.Framework;

namespace ESales.EPiServer.Tests
{
    [TestFixture]
    public class EntityKeyTests
    {
        [Test]
        public void EscapeIllegalCharacters()
        {
            var actual = AttributeHelper.CreateKey( "foo bar", "sv-se" );
            Assert.That( actual, Is.EqualTo( "foo+bar_sv_SE" ) );
        }

        [Test]
        public void EscapeEscapeCharacters()
        {
            var actual = AttributeHelper.CreateKey( "foo+bar", "sv-se" );
            Assert.That( actual, Is.EqualTo( "foo++bar_sv_SE" ) );
        }

        [Test]
        public void EscapeIllegalAndEscapeCharacters()
        {
            var actual = AttributeHelper.CreateKey( "fee fi fo+fum", "sv-se" );
            Assert.That( actual, Is.EqualTo( "fee+fi+fo++fum_sv_SE" ) );
        }
    }
}
