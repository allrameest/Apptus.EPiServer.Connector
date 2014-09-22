using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

namespace ESales.EPiServer.Tests
{
    internal static class XNodeExtensions
    {
        public static bool ElementsEquivalentTo( this XElement thisElement, XElement otherElement )
        {
            var thisDocument = new XDocument( thisElement );
            var otherDocument = new XDocument( otherElement );
            return IsContainedIn( thisDocument, otherDocument ) && IsContainedIn( otherDocument, thisDocument );
        }

        private static bool IsContainedIn( XDocument thisDocument, XDocument otherDocument )
        {
            bool isMatchingFound = false;
            foreach ( var element in thisDocument.DescendantNodes().OfType<XElement>() )
            {
                var xpath = element.GetXPath();
                if ( element.Elements().Any() )
                {
                    isMatchingFound = otherDocument.XPathSelectElements( xpath ).Any();
                }
                else
                {
                    var content = element.Value;
                    isMatchingFound = otherDocument.XPathSelectElements( xpath ).Count( x => x.Value == content ) == 1;
                }
                if ( !isMatchingFound )
                {
                    break;
                }
            }
            return isMatchingFound;
        }

        public static string GetXPath( this XElement element )
        {
            var xPath = new StringBuilder();
            foreach ( var node in element.AncestorsAndSelf().Reverse() )
            {
                xPath.Append( "/" ).Append( node.Name.LocalName );
            }
            return xPath.ToString();
        }
    }
}
