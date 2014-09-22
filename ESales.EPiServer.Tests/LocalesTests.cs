using Apptus.ESales.EPiServer.Util;
using NUnit.Framework;

namespace ESales.EPiServer.Tests
{
    [TestFixture]
    class LocalesTests
    {
        [Test]
        public void ToESalesLocaleStandard()
        {
            Assert.That( "sv-SE".ToESalesLocale(), Is.EqualTo( "sv_SE" ) );
        }

        [Test]
        public void ToESalesLocaleNonStandard()
        {
            Assert.That( "sv-EN".ToESalesLocale(), Is.EqualTo( "sv-EN" ) );
        }
    }
}
