using Apptus.ESales.EPiServer.Import;
using Mediachase.Search;
using Moq;
using NUnit.Framework;

namespace ESales.EPiServer.Tests
{
    [TestFixture]
    internal class ESalesSearchResultTests
    {
        [Test]
        /** This test is only here because a returned null value causes a System.ArgumentNullException in 
         *  EpiServer Commerce when adding a new Line Item to a cart in the admin view (ES-1163). **/
        public void NullCheckForKeyFieldValues()
        {
            var searchCriteria = new Mock<ISearchCriteria>();
            searchCriteria.Setup( sc => sc.Locale ).Returns( "uknown" );
            searchCriteria.Setup( sc => sc.Currency ).Returns( "uknown" );

            var searchResult = new ESalesSearchResult( searchCriteria.Object );
            Assert.NotNull( searchResult.GetKeyFieldValues<int>());

            searchResult = new ESalesSearchResult( null, null, searchCriteria.Object );
            Assert.NotNull( searchResult.GetKeyFieldValues<int>() );
        }
    }
}


