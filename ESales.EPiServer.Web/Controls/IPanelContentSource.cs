using System.Collections.Generic;
using Apptus.ESales.Connector;

namespace Apptus.ESales.EPiServer.Web.Controls
{
    public interface IPanelContentSource
    {
        PanelContent PanelContent { get; }
        IList<PanelContent> Subpanels { get; }
        bool HasResult { get; }
        bool IsZone { get; }
        string Path { get; }
    }
}