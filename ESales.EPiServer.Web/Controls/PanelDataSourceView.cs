using System;
using System.Web.UI;
using Apptus.ESales.Connector;

namespace Apptus.ESales.EPiServer.Web.Controls
{
    public class PanelDataSourceView : HierarchicalDataSourceView
    {
        private readonly PanelContent _rootPanel;
        private readonly string _viewPath;

        public PanelDataSourceView(PanelContent rootPanel, string viewPath)
        {
            if (rootPanel == null)
            {
                throw new ArgumentException("No root panel specified");
            }

            _rootPanel = rootPanel;
            _viewPath = viewPath ?? string.Empty;
        }

        public override IHierarchicalEnumerable Select()
        {
            return null;
        }
    }
}
