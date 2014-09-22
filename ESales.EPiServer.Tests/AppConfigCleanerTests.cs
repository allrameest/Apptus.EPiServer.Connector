using Apptus.ESales.EPiServer.Config;
using NUnit.Framework;

namespace ESales.EPiServer.Tests
{
    [TestFixture]
    internal class AppConfigCleanerTests
    {
        [Test]
        public void ClusterUrlPlain()
        {
            Assert.That( AppConfigCleaner.CleanupClusterUrl( "foo.bar" ), Is.EqualTo( "foo.bar" ) );
        }

        [Test]
        public void ClusterUrlWithScheme()
        {
            Assert.That( AppConfigCleaner.CleanupClusterUrl( "esales://foo.bar" ), Is.EqualTo( "esales://foo.bar" ) );
        }

        [Test]
        public void ClusterUrlWithCredentialsInUrl()
        {
            Assert.That( AppConfigCleaner.CleanupClusterUrl( "user:pass@foo.bar" ), Is.EqualTo( "user:pass@foo.bar" ) );
        }

        [Test]
        public void ClusterUrlWithSchemeAndCredentialsInUrl()
        {
            Assert.That( AppConfigCleaner.CleanupClusterUrl( "esales://user:pass@foo.bar" ), Is.EqualTo( "esales://user:pass@foo.bar" ) );
        }

        [Test]
        public void ClusterUrlWithCredentialsInArg()
        {
            Assert.That( AppConfigCleaner.CleanupClusterUrl( "foo.bar", "user", "pass" ), Is.EqualTo( "user:pass@foo.bar" ) );
        }

        [Test]
        public void ClusterUrlWithSchemeAndCredentialsInArg()
        {
            Assert.That( AppConfigCleaner.CleanupClusterUrl( "esales://foo.bar", "user", "pass" ), Is.EqualTo( "esales://user:pass@foo.bar" ) );
        }

        [Test]
        public void ClusterUrlWithCredentialsInArgOverwrite()
        {
            Assert.That( AppConfigCleaner.CleanupClusterUrl( "esales://user1:pass1@foo.bar", "user2", "pass2" ), Is.EqualTo( "esales://user2:pass2@foo.bar" ) );
        }

        [Test]
        public void ClusterUrlWithPartialCredentialsInArg()
        {
            Assert.That( AppConfigCleaner.CleanupClusterUrl( "esales://user1:pass1@foo.bar", "user2" ), Is.EqualTo( "esales://user1:pass1@foo.bar" ) );
        }

        [Test]
        public void ClusterUrlWithOtherQueryParams()
        {
            Assert.That( AppConfigCleaner.CleanupClusterUrl( "esales://user1:pass1@foo.bar?ignore_certificate=true" ), Is.EqualTo( "esales://user1:pass1@foo.bar?ignore_certificate=true" ) );
        }
    }
}
