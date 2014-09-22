using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Apptus.ESales.EPiServer.Util
{
    public static class AttributeHelper
    {
        /// <summary>
        /// Converts an id and a language into a unique key for eSales. Escapes illegal characters in the id.
        /// The language will be formatted according to <see cref="ToESalesLocale(string)"/>.
        /// </summary>
        /// <param name="id">An entry id.</param>
        /// <param name="language">A language that might be a standard locale.</param>
        /// <returns>A key in the format id_language.</returns>
        public static string CreateKey( string id, string language )
        {
            id = EscapeKey( id );
            return string.Format( "{0}_{1}", id, language.ToESalesLocale() );
        }

        /// <summary>
        /// Escapes keys that are illegal in eSales. Illegal characters are: space, comma, cr, lf and tab.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string EscapeKey( string key )
        {
            return new string( key.SelectMany( Escape ).ToArray() );
        }

        /// <summary>
        /// Formats a language as an eSales locale. If it is a standard locale, such as en-US,
        /// then it will be formatted as en_US, if not then the parameter will be returned as-is.
        /// </summary>
        /// <param name="language">A language that might be a standard locale.</param>
        public static string ToESalesLocale( this string language )
        {
            try
            {
                var culture = CultureInfo.GetCultureInfo( string.IsNullOrWhiteSpace( language ) ? "en-US" : language );
                return culture.ToESalesLocale();
            }
            catch ( CultureNotFoundException )
            {
                return language;
            }
        }

        private static readonly Dictionary<string, string> EsalesLocales = new Dictionary<string, string>
        {
            { "nb-no", "no_NO" },
            { "nn-no", "no_NO_NY" }
        };

        /// <summary>
        /// Formats a culture as an eSales locale, e.g. en-US will be formatted as en_US.
        /// </summary>
        /// <param name="culture">A culture.</param>
        /// <returns>The formatted culture.</returns>
        public static string ToESalesLocale( this CultureInfo culture )
        {

            string eSalesLocale;
            if ( EsalesLocales.TryGetValue( culture.Name.ToLowerInvariant(), out eSalesLocale ) )
            {
                return eSalesLocale;
            }

            return culture.Name.Replace( "-", "_" );
        }

        /// <summary>
        /// Checks if the language is a standard culture.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <returns>True if the language is a standard culture, else false.</returns>
        public static bool IsValidCulture( this string language )
        {
            if ( string.IsNullOrWhiteSpace( language ) )
            {
                return false;
            }

            try
            {
                CultureInfo.GetCultureInfo( language );
            }
            catch ( CultureNotFoundException )
            {
                return false;
            }

            return true;
        }

        private static IEnumerable<char> Escape( char c )
        {
            const string illegalChars = ", \r\n\t";
            const char escapeChar = '+';
            if ( illegalChars.Contains( c ) )
            {
                yield return escapeChar;
                yield break;
            }

            if ( c == escapeChar )
            {
                yield return escapeChar;
            }

            yield return c;
        }
    }
}
