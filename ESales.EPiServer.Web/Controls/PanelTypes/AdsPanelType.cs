using System.Web.UI;
using Apptus.ESales.Connector;

namespace Apptus.ESales.EPiServer.Web.Controls.PanelTypes
{
    internal class AdsPanelType : IPanelType
    {
        private readonly PanelView _panelView;

        public AdsPanelType( PanelView panelView )
        {
            _panelView = panelView;
        }

        public PanelTemplateContainer CreatePanelTemplateContainer( PanelHierarchyData panelHierarchyData )
        {
            return new AdsPanelTemplateContainer( (PanelContent) panelHierarchyData.Item, panelHierarchyData.Indent, panelHierarchyData.HasChildren );
        }

        public ITemplate SelectFooterTemplate( IHierarchyData hierarchyData )
        {
            return _panelView.PanelFooterTemplate;
        }

        public ITemplate SelectHeaderTemplate( IHierarchyData hierarchyData )
        {
            return _panelView.PanelHeaderTemplate;
        }

        public ITemplate SelectPanelTemplate( IHierarchyData hierarchyData )
        {
            return _panelView.AdsPanelTemplate;
        }
    }
}