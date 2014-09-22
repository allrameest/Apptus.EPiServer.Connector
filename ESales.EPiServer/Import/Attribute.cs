using System;
using System.Collections.Generic;
using System.Linq;

namespace Apptus.ESales.EPiServer.Import
{
    /// <summary>
    /// An eSales attribute in the for of a name and an enumerable of values.
    /// </summary>
    public class Attribute
    {
        /// <summary>
        /// Creates a new attribute.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="values">Zero to multiple values.</param>
        public Attribute( string name, params string[] values )
        {
            if ( string.IsNullOrWhiteSpace( name ) )
            {
                throw new ArgumentNullException( "name" );
            }
            Name = name;
            Values = ( values ?? Enumerable.Empty<string>() ).Where( v => !string.IsNullOrWhiteSpace( v ) );
        }

        /// <summary>
        /// Creates a new attribute.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="values">An enumerable of values.</param>
        public Attribute( string name, IEnumerable<string> values ) : this( name, ( values ?? new string[0] ).ToArray() )
        {
        }

        /// <summary>
        /// The name of the attribute.
        /// </summary>
        public string Name { get; private set; }
        
        /// <summary>
        /// The enumerable of values.
        /// </summary>
        public IEnumerable<string> Values { get; private set; }

        /// <summary>
        /// The joined result of the values.
        /// </summary>
        public string Value
        {
            get { return string.Join( "|", Values ); }
        }
    }
}