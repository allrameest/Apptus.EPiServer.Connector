using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Apptus.ESales.Connector;

namespace Apptus.ESales.EPiServer.Web.Controls
{
    [PersistChildren(false), ParseChildren(ChildrenAsProperties=true)]
    public class PanelDataSource : HierarchicalDataSourceControl
    {

        private PanelContent _rootPanel;
        private ParameterCollection _selectParameters;

        public string Path
        {
            get
            {
                string val = ViewState["Path"] as string;
                return val ?? string.Empty;
            }
            set { ViewState["Path"] = value; }
        }

        protected override HierarchicalDataSourceView GetHierarchicalView(string viewPath)
        {
            LoadRootPanelContent();
            return new HierarchicalPanelDataSourceView(_rootPanel, viewPath);
        }

        private void LoadRootPanelContent()
        {
            if (_rootPanel == null)
            {
                var parameters = BuildArguments( SelectParameters.GetValues( HttpContext.Current, this ) );
                _rootPanel = Util.ESales.GetPanelContent( Path, parameters );
            }
        }

        private static ArgMap BuildArguments(IDictionary args)
        {
            ArgMap a = new ArgMap();
            foreach (var argument in args.Keys.Cast<string>().Where(argument => argument != null))
            {
                var value = args[argument];
                if (value != null) a.SafeAdd(argument, value);
            }
            return a;
        }

        [Editor("System.Web.UI.Design.WebControls.ParameterCollectionEditor, System.Design", "System.Drawing.Design.UITypeEditor, System.Drawing"), MergableProperty(false), Category("Data"), PersistenceMode(PersistenceMode.InnerProperty), DefaultValue((string)null)]
        public ParameterCollection SelectParameters
        {
            get
            {
                return _selectParameters ?? (_selectParameters = new ParameterCollection());
            }
        }
    }
}
