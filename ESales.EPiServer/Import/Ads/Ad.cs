using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Apptus.ESales.EPiServer.Import.Ads
{
    /// <summary>
    /// An ad entity.
    /// </summary>
    public class Ad : IEntity
    {
        private const string KeyName = "ad_key";
        private readonly IEnumerable<Attribute> _attributes;

        /// <summary>
        /// Creates a new ad.
        /// </summary>
        /// <param name="key">An <c>ad_key</c>.</param>
        /// <param name="included">An included filter for the ad.</param>
        /// <param name="attributes">An enumerable of attributes.</param>
        public Ad( string key, string included, IEnumerable<Attribute> attributes )
            : this( attributes.Concat( new[] { new Attribute( KeyName, key ), new Attribute( "included", included ) } ) )
        {
        }

        /// <summary>
        /// Creates a new ad.
        /// </summary>
        /// <param name="key">An <c>ad_key</c>.</param>
        /// <param name="attributes">An enumerable of attributes.</param>
        public Ad( string key, IEnumerable<Attribute> attributes = null ) : this( ( attributes ?? new Attribute[0] ).Concat( new[] { new Attribute( KeyName, key ) } ) )
        {
        }

        /// <summary>
        /// Creates a new ad.
        /// </summary>
        /// <param name="attributes">An enumerable of attributes. Must at least contain one attribute named <c>ad_key</c> with a non-empty value.</param>
        public Ad( IEnumerable<Attribute> attributes )
        {
            _attributes = ValidateAttributes( attributes );
            Name = "ad";
        }

        private IEnumerable<Attribute> ValidateAttributes( IEnumerable<Attribute> attributes )
        {
            var unique = new Dictionary<string, Attribute>();
            foreach ( var attribute in attributes )
            {
                if ( attribute.Name == KeyName && attribute.Values.Count() == 1 )
                {
                    Key = new Attribute( KeyName, attribute.Values.First() );
                }
                if ( unique.ContainsKey( attribute.Name ) )
                {
                    unique.Remove( attribute.Name );
                }
                unique.Add( attribute.Name, attribute );
            }
            if ( Key == null  )
            {
                throw new ArgumentException( "Missing valid ad_key." );
            }
            return unique.Values;
        }

        public IEnumerator<Attribute> GetEnumerator()
        {
            return _attributes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// The name of this entity, i.e. <c>ad</c>.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The key of this ad, with the key name <c>ad_key</c>.
        /// </summary>
        public Attribute Key { get; private set; }
    }
}