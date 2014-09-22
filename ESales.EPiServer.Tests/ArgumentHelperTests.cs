using Apptus.ESales.EPiServer.DataAccess;
using Apptus.ESales.EPiServer.Util;
using Moq;
using NUnit.Framework;

namespace ESales.EPiServer.Tests
{
    [TestFixture]
    public class ArgumentHelperTests
    {

        [Test]
        public void CodesToProductsWithSpaces()
        {
            var siteContext = new Mock<ISiteContextMapper>();
            siteContext.Setup( sc => sc.LanguageName ).Returns( "en-us" );
            var argHelper = new ArgumentHelper( siteContext.Object );
            var products = argHelper.CodesToProducts( "my product 1 ,product ,product 2" );
            Assert.AreEqual( "my+product+1+_en_US,product+_en_US,product+2_en_US", products );
        }
    }
}
