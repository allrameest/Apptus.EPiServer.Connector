using System.Globalization;

namespace Apptus.ESales.EPiServer.Import.Formatting
{
    internal class DoubleFormatRule : IFormatRule
    {
        public System.Type FromType { get { return typeof( double ); } }

        public string Format( object value )
        {
            return ( (double) value ).ToString( "F", CultureInfo.InvariantCulture );
        }
    }
}