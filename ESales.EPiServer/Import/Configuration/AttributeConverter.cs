namespace Apptus.ESales.EPiServer.Import.Configuration
{
    internal class AttributeConverter
    {
        private readonly ConfigurationOptions _configurationOptions;

        public AttributeConverter( ConfigurationOptions configurationOptions )
        {
            _configurationOptions = configurationOptions;
        }

        public attribute Convert( ConfigurationAttribute ca )
        {
            var a = new attribute
                {
                    name = ca.Name,
                    present = ca.Present == Present.No
                                  ? null
                                  : new present { xml = ca.Present == Present.YesXml, xmlSpecified = true},
                    search_attributes = ca.SearchOptions != null
                                            ? new[]
                                                {
                                                    new search_attribute
                                                        {
                                                            name = ca.Name,
                                                            format = _configurationOptions.Format( ca.SearchOptions.Format ),
                                                            locale = ca.SearchOptions.Locale,
                                                            match_suffix = ca.SearchOptions.MatchSuffix,
                                                            suggest = ca.SearchOptions.Suggest
                                                        }
                                                }
                                            : new search_attribute[0],
                    filter_attributes = ca.FilterOptions != null
                                            ? new[]
                                                {
                                                    new filter_attribute
                                                        {
                                                            name = ca.Name,
                                                            format = _configurationOptions.Format( ca.FilterOptions.Format ),
                                                            tokenization = _configurationOptions.Tokenization( ca.FilterOptions.Tokenization ),
                                                            type = ca.Type
                                                        }
                                                }
                                            : new filter_attribute[0],
                    sort_attributes = ca.SortOptions != null
                                          ? new[]
                                              {
                                                  new sort_attribute
                                                      {
                                                          name = ca.Name,
                                                          normalization = _configurationOptions.Normalization( ca.SortOptions.Normalization ),
                                                          type = ca.Type
                                                      }
                                              }
                                          : new sort_attribute[0]
                };
            return a;
        }
    }
}
