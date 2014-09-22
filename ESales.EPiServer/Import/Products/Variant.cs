using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Apptus.ESales.EPiServer.Import.Products
{
    /// <summary>
    /// A variant entity.
    /// </summary>
    public class Variant : IEntity
    {
        private const string KeyName = "variant_key";
        private readonly IEnumerable<Attribute> _attributes;

        /// <summary>
        /// Creates a new variant.
        /// </summary>
        /// <param name="key">The <c>variant_key</c>.</param>
        /// <param name="productKey">The <c>product_key</c> that the variant belongs to.</param>
        /// <param name="attributes">An enumerable of attributes.</param>
        public Variant( string key, string productKey, IEnumerable<Attribute> attributes )
            : this( attributes.Concat( new[] { new Attribute( KeyName, key ), new Attribute( "product_key", productKey ) } ) )
        {
        }

        /// <summary>
        /// Creates a new variant.
        /// </summary>
        /// <param name="key">The <c>variant_key</c>.</param>
        /// <param name="attributes">An enumerable of attributes. Must at least contain one attribute with the name <c>product_key</c> with a valid product.</param>
        public Variant( string key, IEnumerable<Attribute> attributes = null )
            : this( (attributes ?? new Attribute[0]).Concat( new[] { new Attribute( KeyName, key ) } ) )
        {
        }

        /// <summary>
        /// Creates a new variant.
        /// </summary>
        /// <param name="attributes">
        /// An enumerable of attributes.
        /// Must at least contain one attribute named <c>variant_key</c> with a non-empty value,
        /// and one attribute named <c>product_key</c> with a valid product.
        /// </param>
        public Variant( IEnumerable<Attribute> attributes )
        {
            _attributes = ValidateAttributes( attributes );
            Name = "variant";
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
                throw new ArgumentException( "Missing variant_key." );
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
        /// The name of this entity, i.e. <c>variant</c>.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The key of this variant, with the key name <c>variant_key</c>.
        /// </summary>
        public Attribute Key { get; private set; }
    }
}