namespace Apptus.ESales.EPiServer.Import
{
    internal interface IIndexLogger
    {
        void Log( string message, double percent, params object[] args );
    }
}