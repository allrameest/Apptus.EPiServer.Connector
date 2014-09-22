using System;
using System.Web.UI;

namespace Apptus.ESales.EPiServer.Web.Controls.PanelTypes
{
    internal interface IPanelType
    {
        PanelTemplateContainer CreatePanelTemplateContainer( PanelHierarchyData panelHierarchyData );
        ITemplate SelectFooterTemplate( IHierarchyData hierarchyData );
        ITemplate SelectHeaderTemplate( IHierarchyData hierarchyData );
        ITemplate SelectPanelTemplate( IHierarchyData hierarchyData );
    }
}
