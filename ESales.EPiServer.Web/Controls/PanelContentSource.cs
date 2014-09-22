using System.Web.UI;
using Apptus.ESales.Connector;

namespace Apptus.ESales.EPiServer.Web.Controls
{
    [PersistChildren( false ), ParseChildren( ChildrenAsProperties = true )]
    public class PanelContentSource : HierarchicalDataSourceControl
    {
        public PanelContent RootPanel { get; set; }

        protected override HierarchicalDataSourceView GetHierarchicalView( string viewPath )
        {
            return new HierarchicalPanelDataSourceView( RootPanel, viewPath );
        }
    }
}
