using System;

namespace Apptus.ESales.EPiServer.Config
{
    internal class AppConfigCleaner
    {
        public static string CleanupClusterUrl( string url, string saasUser = null, string saasPassword = null )
        {
            if ( url == null )
            {
                return null;
            }

            string scheme;
            url = GetRightPart( "://", url, out scheme );

            string credentials;
            url = GetRightPart( "@", url, out credentials );
            if ( !string.IsNullOrWhiteSpace( saasUser ) && !string.IsNullOrWhiteSpace( saasPassword ) )
            {
                credentials = saasUser + ":" + saasPassword + "@";
            }
            return scheme + credentials + url;
        }

        private static string GetRightPart( string partSeparator, string url, out string partAndLeft )
        {
            var split = url.Split( new[] { partSeparator }, 2, StringSplitOptions.RemoveEmptyEntries );
            if ( split.Length != 2 )
            {
                partAndLeft = null;
                return url;
            }
            partAndLeft = split[0] + partSeparator;
            var right = split[1];
            return right;
        }
    }
}