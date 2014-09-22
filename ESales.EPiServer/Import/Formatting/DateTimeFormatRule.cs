using System;
using System.Globalization;

namespace Apptus.ESales.EPiServer.Import.Formatting
{
    internal class DateTimeFormatRule : IFormatRule
    {
        public System.Type FromType { get { return typeof( DateTime ); } }

        public string Format( object value )
        {
            return ( (DateTime) value - new DateTime( 1970, 1, 1, 0, 0, 0, 0 ).ToLocalTime() ).TotalSeconds.ToString( "F", CultureInfo.InvariantCulture );
        }
    }
}