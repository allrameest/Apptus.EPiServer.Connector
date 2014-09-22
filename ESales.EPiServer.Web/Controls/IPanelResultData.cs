using Apptus.ESales.Connector;

namespace Apptus.ESales.EPiServer.Web.Controls
{
    interface IPanelResultData<out T> where T : Result
    {
        T Result { get; }
    }
}
