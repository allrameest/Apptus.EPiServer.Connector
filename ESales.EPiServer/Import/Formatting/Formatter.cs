using System.Collections.Generic;
using System.Linq;

namespace Apptus.ESales.EPiServer.Import.Formatting
{
    internal class Formatter
    {
        private readonly Dictionary<System.Type, IFormatRule> _formatters;

        public Formatter( IEnumerable<IFormatRule> formatters )
        {
            _formatters = formatters.ToDictionary( f => f.FromType );
        }

        public string Format( object v )
        {
            if ( v == null )
            {
                return "";
            }
            IFormatRule formatRule;
            return _formatters.TryGetValue( v.GetType(), out formatRule ) ? formatRule.Format( v ) : v.ToString();
        }
    }
}