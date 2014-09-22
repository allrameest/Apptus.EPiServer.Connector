using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using Apptus.ESales.Connector;

namespace Apptus.ESales.EPiServer.Web.Controls
{
    [ToolboxItem(false)]
    public class PanelTemplateContainer : Control, INamingContainer, IPanelContentSource
    {

        private readonly PanelContent _panel;
        private readonly int _indent;
        private readonly bool _hasChildren;

        internal PanelTemplateContainer(PanelContent panel, int indent, bool hasChildren)
        {
            _panel = panel;
            _indent = indent;
            _hasChildren = hasChildren;
        }

        public IDictionary<string, string> Attributes
        {
            get { return _panel.Attributes; }
        }

        public string DisplayName
        {
            get { return Attributes["display_name"]; }
        }

        public string Ticket
        {
            get { return _panel.Ticket; }
        }

        public bool HasChildren
        {
            get { return _hasChildren; }
        }

        public PanelContent PanelContent
        {
            get { return _panel; }
        }

        public IList<PanelContent> Subpanels
        {
            get { return _panel.Subpanels; }
        }

        public bool HasResult
        {
            get { return _panel.HasResult; }
        }

        public bool IsZone
        {
            get { return _panel.IsZone; }
        }

        public string Path
        {
            get { return _panel.Path.ToString(); }
        }

        public int Indent
        {
            get { return _indent; }
        }

        public object DataItem
        {
            get { return _panel; }
        }
    }
}
