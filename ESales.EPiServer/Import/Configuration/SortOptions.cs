namespace Apptus.ESales.EPiServer.Import.Configuration
{
    internal class SortOptions
    {
        public SortOptions( Normalization normalization )
        {
            Normalization = normalization;
        }

        public Normalization Normalization { get; private set; }
    }
}