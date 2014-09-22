using System.Globalization;
using Apptus.ESales.EPiServer.Util;
using NUnit.Framework;

namespace ESales.EPiServer.Tests
{
    [TestFixture]
    public class AttributeHelperTests
    {

        [Test]
        public void NorwegianEsalesLocales()
        {
            Assert.AreEqual( "no_NO", AttributeHelper.ToESalesLocale( "nb-no" ) );
            Assert.AreEqual( "no_NO", AttributeHelper.ToESalesLocale( "nb-NO" ) );
            Assert.AreEqual( "no_NO_NY", AttributeHelper.ToESalesLocale( "nn-no" ) );
            Assert.AreEqual( "no_NO_NY", AttributeHelper.ToESalesLocale( "nn-NO" ) );

            Assert.AreEqual( "no_NO", AttributeHelper.ToESalesLocale( new CultureInfo( "nb-no" ) ) );
            Assert.AreEqual( "no_NO", AttributeHelper.ToESalesLocale( new CultureInfo( "nb-NO" ) ) );
            Assert.AreEqual( "no_NO_NY", AttributeHelper.ToESalesLocale( new CultureInfo( "nn-no" ) ) );
            Assert.AreEqual( "no_NO_NY", AttributeHelper.ToESalesLocale( new CultureInfo( "nn-NO" ) ) );
        }
    }
}
