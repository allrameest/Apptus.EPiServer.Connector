namespace Apptus.ESales.EPiServer.Import.Formatting
{
    internal interface IFormatRule
    {
        System.Type FromType { get; }
        string Format( object value );
    }
}
