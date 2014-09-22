namespace Apptus.ESales.EPiServer.Import.Configuration
{
    internal class SearchOptions
    {
        public SearchOptions( string locale, Format format, bool matchSuffix, bool suggest )
        {
            Locale = locale;
            Format = format;
            MatchSuffix = matchSuffix;
            Suggest = suggest;
        }

        public string Locale { get; private set; }
        public Format Format { get; private set; }
        public bool MatchSuffix { get; private set; }
        public bool Suggest { get; private set; }
    }
}