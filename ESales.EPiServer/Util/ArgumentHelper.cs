using System;
using System.Linq;
using Apptus.ESales.EPiServer.DataAccess;

namespace Apptus.ESales.EPiServer.Util
{
    public class ArgumentHelper
    {
        private readonly ISiteContextMapper _siteContext;

        public ArgumentHelper( ISiteContextMapper siteContext )
        {
            _siteContext = siteContext;
        }

        public string CodesToProducts( string codes )
        {
            var productList = codes
                .Split( ",", StringSplitOptions.None )
                .Select( code => AttributeHelper.CreateKey( code, _siteContext.LanguageName ) );
            var products = string.Join( ",", productList );
            return products;
        }
    }
}