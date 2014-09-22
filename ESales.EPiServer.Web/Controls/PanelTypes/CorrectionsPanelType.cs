using System.Web.UI;
using Apptus.ESales.Connector;

namespace Apptus.ESales.EPiServer.Web.Controls.PanelTypes
{
    internal class CorrectionsPanelType : IPanelType
    {
        private readonly PanelView _panelView;

        public CorrectionsPanelType( PanelView panelView )
        {
            _panelView = panelView;
        }

        public PanelTemplateContainer CreatePanelTemplateContainer( PanelHierarchyData panelHierarchyData )
        {
            return new CorrectionsPanelTemplateContainer( (PanelContent) panelHierarchyData.Item, panelHierarchyData.Indent, panelHierarchyData.HasChildren );
        }

        public ITemplate SelectFooterTemplate( IHierarchyData hierarchyData )
        {
            return _panelView.CorrectionsPanelFooterTemplate ?? _panelView.PanelFooterTemplate;
        }

        public ITemplate SelectHeaderTemplate( IHierarchyData hierarchyData )
        {
            return _panelView.CorrectionsPanelHeaderTemplate ?? _panelView.PanelHeaderTemplate;
        }

        public ITemplate SelectPanelTemplate( IHierarchyData hierarchyData )
        {
            return _panelView.CorrectionsPanelTemplate;
        }
    }
}