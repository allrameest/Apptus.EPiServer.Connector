using System.Web.UI;
using Apptus.ESales.Connector;

namespace Apptus.ESales.EPiServer.Web.Controls.PanelTypes
{
    internal class CountPanelType : IPanelType
    {
        private readonly PanelView _panelView;

        public CountPanelType( PanelView panelView )
        {
            _panelView = panelView;
        }

        public PanelTemplateContainer CreatePanelTemplateContainer( PanelHierarchyData panelHierarchyData )
        {
            return new CountPanelTemplateContainer( (PanelContent) panelHierarchyData.Item, panelHierarchyData.Indent, panelHierarchyData.HasChildren );
        }

        public ITemplate SelectFooterTemplate( IHierarchyData hierarchyData )
        {
            return _panelView.CountPanelFooterTemplate ?? _panelView.PanelFooterTemplate;
        }

        public ITemplate SelectHeaderTemplate( IHierarchyData hierarchyData )
        {
            return _panelView.CountPanelHeaderTemplate ?? _panelView.PanelHeaderTemplate;
        }

        public ITemplate SelectPanelTemplate( IHierarchyData hierarchyData )
        {
            return _panelView.CountPanelTemplate;
        }
    }
}