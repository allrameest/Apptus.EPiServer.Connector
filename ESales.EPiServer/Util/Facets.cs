using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Apptus.ESales.Connector;
using Apptus.ESales.EPiServer.Config;
using Apptus.ESales.EPiServer.DataAccess;
using Apptus.ESales.EPiServer.Import.Ads;
using Apptus.ESales.EPiServer.Import.Configuration;

namespace Apptus.ESales.EPiServer.Util
{
    public class Facets : IEnumerable<Facets.Facet>
    {

        private const string DefaultFacetParameter = "facets";
        private const string Locale = "en-US";
        private readonly IDictionary<string, string> _facets = new Dictionary<string, string>();

        private static readonly Dictionary<string, FacetAttribute> FacetAttributes =
            new Configuration(
                new AppConfig(),
                new AdAttributeHelper(),
                new MetaDataMapper(),
                new FileSystem() )
                .FacetAttributes.ToDictionary( fa => fa.Name );

        public bool IsEmpty { get { return _facets.Count == 0; } }

        public string GetFacet(string attribute)
        {
            string key;
            return _facets.TryGetValue(attribute, out key) ? key : null;
        }

        public void SetFacet(string attribute, string value)
        {
            _facets[attribute] = value;
        }

        public void ClearFacet(string attribute)
        {
            _facets.Remove(attribute);
        }

        public string GetFacetURLParameter()
        {
            return GetFacetURLParameter(DefaultFacetParameter);
        }

        public string GetFacetURLParameter(string parameterName)
        {
            if (_facets.Count == 0) return string.Empty;
            StringBuilder builder = new StringBuilder();
            foreach (var facet in _facets.OrderBy(facet => facet.Key))
            {
                if (builder.Length > 0) builder.Append(';');
                builder.Append(facet.Key).Append(':').Append(facet.Value);
            }
            return string.Format("{0}={1}", HttpUtility.UrlEncode(parameterName), HttpUtility.UrlEncode(builder.ToString()));
        }

        public void Parse(string s)
        {
            if (string.IsNullOrEmpty(s)) return;
            var fs = s.Split(new[] {';'});
            foreach (var f in fs)
            {
                var idx = f.IndexOf(':');
                if (idx == -1) continue;
                _facets[f.Substring(0, idx)] = f.Substring(idx + 1);
            }
        }

        public Filter GetFacetFilter()
        {
            if (IsEmpty) return FilterBuilder.Universe();
            Filter[] filters = new Filter[_facets.Count];
            var idx = 0;
            foreach (var facet in _facets)
            {
                filters[idx++] = FilterBuilder.Attribute(facet.Key, facet.Value);
            }
            return filters.Length == 1 ? filters[0] : FilterBuilder.And(filters);
        }

        public class Facet
        {
            public string GroupKey { get; internal set; }
            public string GroupName { get; internal set; }
            public string Key { get; internal set; }
            public string Name { get; internal set; }
            public int Count { get; internal set; }
            public string Ticket { get; internal set; }
        }

        public IEnumerator<Facet> GetEnumerator()
        {
            
            List<Facet> facets = new List<Facet>();
            foreach (var facet in _facets)
            {
                FacetAttribute fa;
                var attribute = facet.Key;
                if (attribute.EndsWith("__facet")) attribute = attribute.Substring(0, attribute.Length - "__facet".Length);
                if (!FacetAttributes.TryGetValue(attribute, out fa)) continue;
                var desc = fa.GetDescription(Locale) ?? fa.GetDescription();
                var f = fa.GetFacetFromKey(facet.Value);
                if (f == null) continue;
                var facetDesc = f.GetDescription(Locale) ?? f.GetDescription();
                facets.Add(new Facet {GroupKey = facet.Key, Key = facet.Value, GroupName = desc, Name = facetDesc});
            }

            return facets.GetEnumerator();
        }
        
        public static string FacetGroupName(string groupKey)
        {
            if (string.IsNullOrEmpty(groupKey)) return string.Empty;
            if (groupKey.EndsWith("__facet")) groupKey = groupKey.Substring(0, groupKey.Length - "__facet".Length);
            FacetAttribute fa;
            if (!FacetAttributes.TryGetValue(groupKey, out fa)) return string.Empty;
            var desc = fa.GetDescription(Locale) ?? fa.GetDescription();
            return desc;
        }

        public static IEnumerable<Facet> GroupFacets(string groupKey, Result.Values values)
        {
            if (string.IsNullOrEmpty(groupKey)) return new List<Facet>();
            if (groupKey.EndsWith("__facet")) groupKey = groupKey.Substring(0, groupKey.Length - "__facet".Length);
            FacetAttribute fa;
            if (!FacetAttributes.TryGetValue(groupKey, out fa)) return new List<Facet>();
            var groupName = fa.GetDescription(Locale) ?? fa.GetDescription();
            IList<string> keys = new List<string>();
            IDictionary<string, int> quantities = new Dictionary<string, int>();
            IDictionary<string, string> tickets = new Dictionary<string, string>();
            foreach (var value in values)
            {
                var key = value.Text;
                var f = fa.GetFacetFromKey(key);
                if (f == null) continue;
                keys.Add(key);
                quantities.Add(key, value.Count);
                tickets.Add(key, value.Ticket);
            }
            var ka = keys.ToArray();
            fa.Reorder(ka);
            return (
                from key in ka
                let facet = fa.GetFacetFromKey(key)
                let description = facet.GetDescription(Locale) ?? facet.GetDescription()
                select new Facet
                {
                    GroupKey = groupKey, GroupName = groupName, Key = key, Name = description, Count = quantities[key], Ticket = tickets[key]
                }).ToList();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
