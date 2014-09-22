using System;
using System.Web.UI;
using Apptus.ESales.Connector;

namespace Apptus.ESales.EPiServer.Web.Controls
{
    public class HierarchicalPanelDataSourceView : HierarchicalDataSourceView
    {
        private readonly PanelContent _rootPanel;
        private readonly string _viewPath;

        public HierarchicalPanelDataSourceView(PanelContent rootPanel, string viewPath)
        {
            _rootPanel = rootPanel;
            _viewPath = viewPath;
        }

        public override IHierarchicalEnumerable Select()
        {
            if (string.IsNullOrEmpty(_viewPath))
            {
                return new PanelHierarchicalEnumerable(_rootPanel);
            }
            if (!_rootPanel.HasSubpanel(_viewPath))
            {
                return new PanelHierarchicalEnumerable();
            }
            return new PanelHierarchicalEnumerable(_rootPanel.Subpanel(_viewPath));
        }
    }
}