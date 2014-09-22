using System.Web.UI;
using Apptus.ESales.Connector;

namespace Apptus.ESales.EPiServer.Web.Controls.PanelTypes
{
    internal class ProductsPanelType : IPanelType
    {
        private readonly PanelView _panelView;

        public ProductsPanelType( PanelView panelView )
        {
            _panelView = panelView;
        }

        public PanelTemplateContainer CreatePanelTemplateContainer( PanelHierarchyData panelHierarchyData )
        {
            return new ProductsPanelTemplateContainer( (PanelContent) panelHierarchyData.Item, panelHierarchyData.Indent, panelHierarchyData.HasChildren );
        }

        public ITemplate SelectFooterTemplate( IHierarchyData hierarchyData )
        {
            return _panelView.ProductPanelFooterTemplate ?? _panelView.PanelFooterTemplate;
        }

        public ITemplate SelectHeaderTemplate( IHierarchyData hierarchyData )
        {
            return _panelView.ProductPanelHeaderTemplate ?? _panelView.PanelHeaderTemplate;
        }

        public ITemplate SelectPanelTemplate( IHierarchyData hierarchyData )
        {
            return _panelView.ProductsPanelTemplate;
        }
    }
}