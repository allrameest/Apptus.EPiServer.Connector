using System;
using System.Collections.Generic;
using System.Linq;
using Apptus.ESales.Connector;
using Apptus.ESales.EPiServer.Config;
using Apptus.ESales.EPiServer.Import.Configuration;
using Mediachase.Search;
using Mediachase.Search.Extensions;

namespace Apptus.ESales.EPiServer.Util
{
    ///<summary>
    /// Helper class for converting values between ESales classes and Mediachase classes.
    ///</summary>
    internal class ValueConverter
    {

        ///<summary>
        /// Data holder for value/count pairs.
        ///</summary>
        [Serializable]
        public class ValueResult
        {
            public string Value;
            public int Count;
        }

        ///<summary>
        /// Data holder for result for one attribute.
        ///</summary>
        [Serializable]
        public class AttributeResult
        {
            public string Attribute;
            public List<ValueResult> ValueResults;
        }

        ///<summary>
        /// Converts a <c>ComponentResult.NavigationPane</c> into an array of <c>ISearchFacetGroup</c> 
        /// for use against Mediachase APIs.
        ///</summary>
        ///<param name="conf">The active configuration</param>
        ///<param name="navResult">The navigation result from the eSales query processors</param>
        ///<param name="lang">The language to use when creating <c>ISearchFacetGroup</c>s.</param>
        /// <param name="currency">The currency to use when creating saleprice <c>ISearchFacetGroup</c>s. 
        ///  If null, no saleprice attribute will be returned. If currency is not set, it is possible 
        ///  to set it to <c>conf.DefaultCurrency</c> if conf is of type <c>Configuration</c>, but
        ///  this must be done explicitly.</param>
        ///<param name="componentLocations">Output parameter, map containing component locations
        /// for notifications corresponding to each attribute.</param>
        ///<returns>An array of <c>ISearchFacetGroup</c>s generated from <c>navResult</c>.</returns>
        public static ISearchFacetGroup[] GetFacetGroups(IConfiguration conf, PanelContent navResult, string lang, string currency, Dictionary<string, string> componentLocations)
        {
            List<AttributeResult> attributeResults = CreateAttributeResultList(navResult);
            return GetFacetGroups(conf, attributeResults, lang, currency);
        }

        ///<summary>
        /// Converts a list of <c>AttributeResult</c>s into an array of <c>ISearchFacetGroup</c> 
        /// for use against Mediachase APIs.
        ///</summary>
        ///<param name="conf">The active configuration</param>
        ///<param name="attributeResults">The result from the eSales query processors in intermittent form.</param>
        ///<param name="lang">The language to use when creating <c>ISearchFacetGroup</c>s.</param>
        /// <param name="currency">The currency to use when creating saleprice <c>ISearchFacetGroup</c>s. 
        ///  If null, no saleprice attribute will be returned. If currency is not set, it is possible 
        ///  to set it to <c>conf.DefaultCurrency</c> if conf is of type <c>Configuration</c>, but
        ///  this must be done explicitly.</param>
        ///<returns>An array of <c>ISearchFacetGroup</c>s generated from <c>navResult</c>.</returns>
        public static ISearchFacetGroup[] GetFacetGroups(IConfiguration conf, List<AttributeResult> attributeResults, string lang, string currency)
        {
            IList<ISearchFacetGroup> groups = new List<ISearchFacetGroup>();
            foreach (AttributeResult ar in attributeResults)
            {
                var valueResults = ar.ValueResults;
                var attribute = ar.Attribute;
                if (attribute == null) continue;
                if (attribute.StartsWith("saleprice"))
                {
                    if (currency == null) continue;
                    if (!attribute.StartsWith("saleprice" + currency.ToLower()))
                    {
                        //group in another currency - skip
                        continue;
                    }
                }

                ISearchFacetGroup group = GetFacetGroup(attribute, lang, conf, valueResults);

                if (group != null)
                {
                    groups.Add(group);
                }
            }
            return groups.ToArray();
        }

        ///<summary>
        /// Converts a list of <c>ComponentResult.NavigationPane</c>s into an list of <c>AttributeResult</c>s.
        /// The list is serializable, for storage in e.g. view state.
        ///</summary>
        ///<param name="navResult">The navigation result from the eSales query processors</param>
        ///<returns>A list of <c>AttributeResult</c>s generated from <c>navResult</c>.</returns>
        public static List<AttributeResult> CreateAttributeResultList(PanelContent navResult)
        {
            return (from r in navResult.Subpanels
                    let values = (Result.Values) r.Result()
                    let attribute = r.Attributes["display_name"]
                    let vrList = values.Select(v => new ValueResult {Count = v.Count, Value = v.Text}).ToList()
                    select new AttributeResult {Attribute = attribute, ValueResults = vrList}).ToList();
        }


        private static ISearchFacetGroup GetFacetGroup(string attribute, string locale, IConfiguration conf, IEnumerable<ValueResult> values)
        {
            if ( attribute.EndsWith( "__facet" ) )
            {
                attribute = attribute.Substring(0, attribute.Length - "__facet".Length);
            }

            var fa = conf.FacetAttributes.FirstOrDefault( a => a.Name == attribute );
            
            if ( fa == null )
            {
                return null;
            }

            string desc = null;
            if (locale != null)
            {
                desc = fa.GetDescription(locale);
            }
            if (desc == null) desc = fa.GetDescription();
            var group = attribute.StartsWith("saleprice") ? new FacetGroup("saleprice", desc) : new FacetGroup(attribute, desc);
            IList<string> keys = new List<string>();
            IDictionary<string, int> quantities = new Dictionary<string, int>();
            foreach (var value in values)
            {
                var key = value.Value;
                var f = fa.GetFacetFromKey(key);
                if (f == null) continue;
                keys.Add(key);
                quantities.Add(key, value.Count);
            }
            var ka = keys.ToArray();
            fa.Reorder(ka);
            foreach (var key in ka)
            {
                var facet = fa.GetFacetFromKey(key);
                string description = null;
                if (locale != null)
                {
                    description = facet.GetDescription(locale);
                }
                if (description == null) description = facet.GetDescription();
                group.Facets.Add(new Facet(group, facet.Key, description, quantities[key]));
            }
            return group;
        }
    }
}
