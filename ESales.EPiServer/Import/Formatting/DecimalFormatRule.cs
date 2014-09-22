using System.Globalization;

namespace Apptus.ESales.EPiServer.Import.Formatting
{
    internal class DecimalFormatRule : IFormatRule
    {
        public System.Type FromType { get { return typeof( decimal ); } }

        public string Format( object value )
        {
            return ( (decimal) value ).ToString( "F", CultureInfo.InvariantCulture );
        }
    }
}