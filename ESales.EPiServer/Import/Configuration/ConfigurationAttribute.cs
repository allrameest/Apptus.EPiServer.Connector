namespace Apptus.ESales.EPiServer.Import.Configuration
{
    internal class ConfigurationAttribute
    {
        public ConfigurationAttribute( string name, type type, Present present, SearchOptions searchOptions = null, FilterOptions filterOptions = null,
                          SortOptions sortOptions = null )
        {
            Name = name;
            Type = type;
            Present = present;
            SearchOptions = searchOptions;
            FilterOptions = filterOptions;
            SortOptions = sortOptions;
        }

        public string Name { get; private set; }
        public Present Present { get; private set; }
        public SearchOptions SearchOptions { get; private set; }
        public FilterOptions FilterOptions { get; private set; }
        public SortOptions SortOptions { get; private set; }
        public type Type { get; private set; }
    }
}