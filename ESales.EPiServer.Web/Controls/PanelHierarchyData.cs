using System.Web.UI;
using Apptus.ESales.Connector;
using Apptus.ESales.EPiServer.Web.Controls.PanelTypes;

namespace Apptus.ESales.EPiServer.Web.Controls
{
    public class PanelHierarchyData : IHierarchyData
    {
        private readonly int _indent;
        public int Indent
        {
            get { return _indent; }
        }

        private readonly PanelHierarchyData _parentZone;
        private readonly PanelContent _panelContent;

        public PanelHierarchyData(PanelHierarchyData parentZone, PanelContent panelContent, int indent)
        {
            _indent = indent;
            _parentZone = parentZone;
            _panelContent = panelContent;
        }

        public IHierarchicalEnumerable GetChildren()
        {
            return new PanelHierarchicalEnumerable(this, _panelContent.Subpanels);
        }

        public IHierarchyData GetParent()
        {
            return _parentZone;
        }

        public string DisplayName
        {
            get { return _panelContent.Attributes["display_name"]; }
        }

        public string Ticket
        {
            get { return _panelContent.Ticket; }
        }

        public bool HasChildren
        {
            get { return _panelContent != null && (_panelContent.IsZone && _panelContent.Subpanels.Count > 0); }
        }

        public string Path
        {
            get { return _panelContent.Path.ToString(); }
        }

        public object Item
        {
            get { return _panelContent; }
        }

        public string Type
        {
            get 
            {
                // marker for a zone panel
                if (_panelContent.IsZone)
                {
                    return PanelType.Zone.ToString();
                }

                // marker for an empty panel
                if (!_panelContent.HasResult)
                {
                    return PanelType.Empty.ToString();
                }

                // use the result type
                var type = PanelType.Unknown;
                switch (_panelContent.Result().Type)
                {
                    case Result.ResultType.Completions:
                        type = PanelType.Completions;
                        break;
                    case Result.ResultType.Corrections:
                        type = PanelType.Corrections;
                        break;
                    case Result.ResultType.Count:
                        type = PanelType.Count;
                        break;
                    case Result.ResultType.Products:
                        type = PanelType.Products;
                        break;
                    case Result.ResultType.Ads:
                        type = PanelType.Ads;
                        break;
                    case Result.ResultType.Values:
                        type = PanelType.Values;
                        break;
                }
                return type.ToString();
            }
        }
    }
}