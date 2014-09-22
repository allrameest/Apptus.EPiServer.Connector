using System.Xml.Linq;
using NUnit.Framework;

namespace ESales.EPiServer.Tests
{
    [TestFixture]
    public class XNodeExtensionsTests
    {
        [Test]
        public void ElementsEquivalentToIdentical()
        {
            var doc1 = new XElement(
                new XElement(
                    "root",
                    new XElement(
                        "child",
                        new XElement(
                            "grandchild",
                            "content" ) ) )
                );

            var identical = new XElement(
                new XElement(
                    "root",
                    new XElement(
                        "child",
                        new XElement(
                            "grandchild",
                            "content" ) ) )
                );

            Assert.That( doc1.ElementsEquivalentTo( identical ) );
        }

        [Test]
        public void ElementsEquivalentToDifferentContent()
        {
            var doc1 = new XElement(
                new XElement(
                    "root",
                    new XElement(
                        "child",
                        new XElement(
                            "grandchild",
                            "content" ) ) )
                );

            var identical = new XElement(
                new XElement(
                    "root",
                    new XElement(
                        "child",
                        new XElement(
                            "grandchild",
                            "content2" ) ) )
                );

            Assert.That( !doc1.ElementsEquivalentTo( identical ) );
        }

        [Test]
        public void ElementsEquivalentToDifferentElements()
        {
            var doc1 = new XElement(
                new XElement(
                    "root",
                    new XElement(
                        "child",
                        new XElement(
                            "grandchild",
                            "content" ) ) )
                );

            var identical = new XElement(
                new XElement(
                    "root",
                    new XElement(
                        "child" ) ) );

            Assert.That( !doc1.ElementsEquivalentTo( identical ) );
        }

        [Test]
        public void ElementsEquivalentToIsNotContainedIn()
        {
            var doc1 = new XElement(
                new XElement(
                    "root",
                    new XElement(
                        "child",
                        new XElement(
                            "grandchild" ) ) ) );

            var identical = new XElement(
                new XElement(
                    "root",
                    new XElement(
                        "child" ) ) );

            Assert.That( !identical.ElementsEquivalentTo( doc1 ) );
        }
    }
}
