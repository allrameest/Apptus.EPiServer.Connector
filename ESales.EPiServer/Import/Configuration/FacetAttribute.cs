using System;
using System.Collections.Generic;
using System.Globalization;

namespace Apptus.ESales.EPiServer.Import.Configuration
{
    ///<summary>
    /// Information about a facet, by which is meant a key/value-pair with descriptions mapped from data values.
    ///</summary>
    public interface IFacet
    {
        ///<summary>
        /// Determines whether a data value maps to this facet.
        ///</summary>
        ///<param name="dataValue">The value from the raw data.</param>
        ///<returns>True if the data value maps to this facet, false otherwise.</returns>
        bool Matches(string dataValue);
        
        ///<summary>
        /// The facet key, i.e. the unique data token used for fetching this facet from the configuration.
        ///</summary>
        string Key { get; }

        ///<summary>
        /// The facet value, i.e. the data token used for indexing in eSales.
        ///</summary>
        string Value { get; }

        ///<summary>
        /// Fetches the description for a given language.
        ///</summary>
        ///<param name="locale">The locale to fetch descriptions for.</param>
        ///<returns>The description, if it exists, null otherwise.</returns>
        string GetDescription(string locale);

        ///<summary>
        /// Fetches the default description.
        ///</summary>
        ///<returns>The description, if it exists, null otherwise.</returns>
        string GetDescription();
    }


    ///<summary>
    /// Class representing a facet attribute, i.e. a named attribute tied to a collection of <c>IFacet</c>s.
    ///</summary>
    public abstract class FacetAttribute
    {
        readonly IDictionary<string, IFacet> _facetDict;
        
        ///<summary>
        /// the name of the facet attribute.
        ///</summary>
        public string Name { get; private set; }
        private readonly Dictionary<string, string> _descriptions;
        private readonly string _defaultLocale;

        ///<summary>
        /// Facet for strings, using exact string matching to determine mappings.
        ///</summary>
        public class StringFacet : IFacet
        {
            readonly string _key;
            readonly string _value;
            readonly string _defaultLocale;
            readonly Dictionary<string, string> _descriptions;
            public string Key
            {
                get
                {
                    return _key;
                }
            }
            public string Value
            {
                get
                {
                    return _value;
                }
            }

            ///<summary>
            /// Create a string facet.
            ///</summary>
            ///<param name="key">The facet key.</param>
            ///<param name="value">The facet value.</param>
            ///<param name="descriptions">Map with descriptions for locales.</param>
            ///<param name="defaultLocale">The locale to use as default.</param>
            public StringFacet(string key, string value, Dictionary<string, string> descriptions, string defaultLocale)
            {
                _key = key;
                _value = value;
                _descriptions = descriptions;
                _defaultLocale = defaultLocale;
            }

            ///<summary>
            /// Determines whether a data value maps to this facet by comparing the data value
            /// with the <c>value</c> parameter given at creation. The comparison is case insensitive.
            ///</summary>
            ///<param name="dataValue">The value from the raw data.</param>
            ///<returns>True if the data value maps to this facet, false otherwise.</returns>
            public bool Matches(string dataValue)
            {
                return _value.Equals(dataValue, StringComparison.InvariantCultureIgnoreCase);
            }
            public string GetDescription(string locale)
            {
                string desc;
                _descriptions.TryGetValue(locale, out desc);
                return desc;
            }
            public string GetDescription()
            {
                string desc;
                _descriptions.TryGetValue(_defaultLocale, out desc);
                return desc;
            }

        }

        ///<summary>
        /// Facet for numeric values, using ranges to determine mappings.
        ///</summary>
        public class RangeFacet : IFacet
        {
            readonly decimal _lb;
            readonly decimal _hb;
            readonly bool _includeLb;
            readonly bool _includeHb;
            readonly string _key;
            readonly string _defaultLocale;
            readonly Dictionary<string, string> _descriptions;
            public string Key
            {
                get
                {
                    return _key;
                }
            }
            public string Value
            {
                get
                {
                    return _key;
                }
            }

            ///<summary>
            /// Create a range facet.
            ///</summary>
            ///<param name="lb">The lower bound of the facet.</param>
            ///<param name="hb">The higher bound of the facet.</param>
            ///<param name="includeLb">Whether to include the lower bound in the facet range.</param>
            ///<param name="includeHb">Whether to include the higher bound in the facet range.</param>
            ///<param name="key">The facet key.</param>
            ///<param name="descriptions">Map with descriptions for locales.</param>
            ///<param name="defaultLocale">The locale to use as default.</param>
            public RangeFacet(decimal lb, decimal hb, bool includeLb, bool includeHb, string key,Dictionary<string, string> descriptions, string defaultLocale)
            {
                _lb = lb;
                _hb = hb;
                _includeHb = includeHb;
                _includeLb = includeLb;
                _key = key;
                _descriptions = descriptions;
                _defaultLocale = defaultLocale;
            }

            ///<summary>
            /// Determines whether a data value maps to this facet, by parsing value to decimal and checking
            /// higher and lower bound.
            ///</summary>
            ///<param name="dataValue">The value from the raw data.</param>
            ///<returns>True if the data value is in range for this facet, false otherwise.</returns>
            public bool Matches(string dataValue)
            {
                try
                {
                    decimal d = Decimal.Parse(dataValue, CultureInfo.InvariantCulture);
                    bool included = (_includeLb ? d >= _lb : d > _lb);
                    included = included && (_includeHb ? d <= _hb : d < _hb);
                    return included;
                }
                catch (Exception)
                {
                    return false;
                }
                
            }

            public string GetDescription(string locale)
            {
                string desc;
                _descriptions.TryGetValue(locale, out desc);
                return desc;
            }
            public string GetDescription()
            {
                string desc;
                _descriptions.TryGetValue(_defaultLocale, out desc);
                return desc;
            }
        }


        protected FacetAttribute(String name, IEnumerable<IFacet> facets, Dictionary<string, string> descriptions, string defaultLocale)
        {
            Name = name;
            _facetDict = new Dictionary<string, IFacet>();
            foreach (IFacet f in facets)
            {
                _facetDict[f.Key] = f;
            }
            _descriptions = descriptions;
            _defaultLocale = defaultLocale;
        }

        ///<summary>
        /// Fetches the facet attribute description for a given language.
        ///</summary>
        ///<param name="locale">The locale to fetch descriptions for.</param>
        ///<returns>The description, if it exists, null otherwise.</returns>
        public string GetDescription(string locale)
        {
            string desc;
            _descriptions.TryGetValue(locale, out desc);
            return desc;
        }

        ///<summary>
        /// Fetches the default description.
        ///</summary>
        ///<returns>The description, if it exists, null otherwise.</returns>
        public string GetDescription()
        {
            string desc;
            _descriptions.TryGetValue(_defaultLocale, out desc);
            return desc;
        }


        ///<summary>
        /// Fetches a facet mapping to a given data value. Returns the first facet found.
        ///</summary>
        ///<param name="dataValue">The value to map for.</param>
        ///<returns>A matching facet, or null if no matching facet was found.</returns>
        public IFacet GetFacetFromDataValue(string dataValue)
        {
            foreach (IFacet f in _facetDict.Values)
            {
                if (f.Matches(dataValue))
                {
                    return f;
                }
            }
            return null;
        }


        ///<summary>
        /// Fetches a facet with the given key.
        ///</summary>
        ///<param name="key">The key of the facet.</param>
        ///<returns>The facet with the given key, or null if no such facet exists.</returns>
        public IFacet GetFacetFromKey(string key)
        {
            IFacet f;
            _facetDict.TryGetValue(key, out f);
            return f;
        }


        ///<summary>
        /// If applicable, reorder the array of facet keys in a proper order for presentation.
        ///</summary>
        ///<param name="keys">An array of facet keys.</param>
        public abstract void Reorder(string[] keys);

    }

    ///<summary>
    /// A facet attribute of type string, containing <c>StringFacet</c>s.
    ///</summary>
    public class StringFacetAttribute : FacetAttribute
    {
        ///<summary>
        /// Create a string facet attribute.
        ///</summary>
        ///<param name="name">The name of the facet attribute.</param>
        ///<param name="facets">The facets in this attribute.</param>
        ///<param name="descriptions">Descriptions mapped from locales for the attribute.</param>
        ///<param name="defaultLocale">The default locale for descriptions.</param>
        public StringFacetAttribute(String name, IEnumerable<IFacet> facets, Dictionary<string, string> descriptions, string defaultLocale) :
            base(name, facets, descriptions, defaultLocale)
        {
        }

        ///<summary>
        /// This implementation will not reorder the array.
        ///</summary>
        ///<param name="keys">An array of facet keys.</param>
        public override void Reorder(string[] keys)
        {
            //nothing;
        }
    }

    ///<summary>
    /// A facet attribute of numeric type, containing <c>RangeFacet</c>s.
    ///</summary>
    public class RangeFacetAttribute : FacetAttribute
    {
        readonly Dictionary<string, int> _ordinals = new Dictionary<string, int>();
        private readonly RangeComparer _rc;
        ///<summary>
        /// Create a range facet attribute.
        ///</summary>
        ///<param name="name">the name of the facet attribute.</param>
        ///<param name="facets">The facets in this attribute.</param>
        ///<param name="descriptions">Descriptions mapped from locales for the attribute.</param>
        ///<param name="defaultLocale">The default locale for descriptions.</param>
        public RangeFacetAttribute(String name, IEnumerable<IFacet> facets, Dictionary<string, string> descriptions, string defaultLocale) :
            base(name, facets, descriptions, defaultLocale)
        {
            int count = 0;
            foreach (IFacet f in facets)
            {
                _ordinals[f.Key] = count++;
            }
             _rc = new RangeComparer(_ordinals);
        }

        class RangeComparer : IComparer<string>
        {
            private readonly Dictionary<string, int> _ordinals;
            internal RangeComparer(Dictionary<string, int> ordinals)
            {
                _ordinals = ordinals;
            }
            public int Compare(string x, string y)
            {
                return _ordinals[x] - _ordinals[y];
            }
        }


        ///<summary>
        /// This implementation will reorder the keys to the order in the enumerable given to the constructor. 
        ///</summary>
        ///<param name="keys">An array of facet keys.</param>
        public override void Reorder(string[] keys)
        {
            Array.Sort(keys, _rc);
        }
    }
}
