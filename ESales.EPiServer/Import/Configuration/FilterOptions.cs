namespace Apptus.ESales.EPiServer.Import.Configuration
{
    internal class FilterOptions
    {
        public FilterOptions( Format format, Tokenization tokenization )
        {
            Format = format;
            Tokenization = tokenization;
        }

        public Format Format { get; private set; }
        public Tokenization Tokenization { get; private set; }
    }
}