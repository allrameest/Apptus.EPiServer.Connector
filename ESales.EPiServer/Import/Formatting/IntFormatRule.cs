using System.Globalization;

namespace Apptus.ESales.EPiServer.Import.Formatting
{
    internal class IntFormatRule : IFormatRule
    {
        public System.Type FromType
        {
            get { return typeof( int ); }
        }

        public string Format( object value )
        {
            return ( (int) value ).ToString( CultureInfo.InvariantCulture );
        }
    }
}