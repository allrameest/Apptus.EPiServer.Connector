using Apptus.ESales.EPiServer.Util;
using NUnit.Framework;

namespace ESales.EPiServer.Tests
{
    [TestFixture]
    public class ExtensionsTests
    {
        [Test]
        public void Append()
        {
            Assert.That( new[] { "hello", "hi" }.Append( "hey" ), Is.EqualTo( new[] { "hello", "hi", "hey" } ) );
        }

        [Test]
        public void AppendNullSource()
        {
            string[] nullList = null;
            Assert.That( nullList.Append( "w/e" ), Is.EqualTo( new[] { "w/e" } ) );
        }

        [Test]
        public void AppendEmptyStringSource()
        {
            Assert.That( new[] { "" }.Append( "hey" ), Is.EqualTo( new[] { "", "hey" } ) );
        }

        [Test]
        public void AppendNullItem()
        {
            Assert.That( new[] { "hello" }.Append( null ), Is.EqualTo( new[] { "hello" } ) );
        }
        
        [Test]
        public void AppendNullItemWithAppendEmptyString()
        {
            Assert.That( new[] { "hello" }.Append( null, true ), Is.EqualTo( new[] { "hello" } ) );
        }

        [Test]
        public void AppendDontAppendEmptyStringItem()
        {
            Assert.That( new[] { "hello" }.Append( "" ), Is.EqualTo( new[] { "hello" } ) );
        }

        [Test]
        public void AppendDoAppendEmptyStringItem()
        {
            Assert.That( new[] { "hello" }.Append( "", true ), Is.EqualTo( new[] { "hello", "" } ) );
        }
    }
}
