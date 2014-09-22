using System.Globalization;

namespace Apptus.ESales.EPiServer.Import.Formatting
{
    internal class FloatFormatRule : IFormatRule
    {
        public System.Type FromType { get { return typeof( float ); } }

        public string Format( object value )
        {
            return ( (float) value ).ToString( "F", CultureInfo.InvariantCulture );
        }
    }
}