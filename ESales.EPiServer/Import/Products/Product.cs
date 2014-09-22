using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Apptus.ESales.EPiServer.Import.Products
{
    /// <summary>
    /// A product entity.
    /// </summary>
    public class Product : IEntity
    {
        private const string KeyName = "product_key";
        private readonly IEnumerable<Attribute> _attributes;

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="key">A <c>product_key</c>.</param>
        /// <param name="attributes">An enumerable of attributes.</param>
        public Product( string key, IEnumerable<Attribute> attributes = null )
            : this( ( attributes ?? new Attribute[0] ).Concat( new[] { new Attribute( KeyName, key ) } ) )
        {
        }

        /// <summary>
        /// Creates a new product entity.
        /// </summary>
        /// <param name="attributes">An enumerable of values, must at least contain one attribute named <c>product_key</c> with non-empty value.</param>
        public Product( IEnumerable<Attribute> attributes )
        {
            _attributes = ValidateAttributes( attributes );
            Name = "product";
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
            if ( Key == null )
            {
                throw new ArgumentException( "Missing product_key." );
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
        /// The name of this entity, i.e. <c>product</c>.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The key of this product, with the key name <c>product_key</c>.
        /// </summary>
        public Attribute Key { get; private set; }
    }
}