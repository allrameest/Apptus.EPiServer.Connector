using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Apptus.ESales.Connector;
using Apptus.ESales.EPiServer.Util;
using Apptus.ESales.EPiServer.Web.Controls.PanelTypes;

namespace Apptus.ESales.EPiServer.Web.Controls
{
    [ParseChildren(true), PersistChildren(false)]
    public class PanelView : HierarchicalDataBoundControl
    {
        private PanelHierarchyData _root;

        // Header templates
        private PanelHeaderTemplate _headerTemplate = new PanelHeaderTemplate(null);
        private PanelHeaderTemplate _productHeaderTemplate;
        private PanelHeaderTemplate _valuesHeaderTemplate;
        private PanelHeaderTemplate _countHeaderTemplate;
        private PanelHeaderTemplate _correctionsHeaderTemplate;
        private PanelHeaderTemplate _completionsHeaderTemplate;

        // Footer templates
        private PanelFooterTemplate _footerTemplate = new PanelFooterTemplate(null);
        private PanelFooterTemplate _productFooterTemplate;
        private PanelFooterTemplate _valuesFooterTemplate;
        private PanelFooterTemplate _countFooterTemplate;
        private PanelFooterTemplate _correctionsFooterTemplate;
        private PanelFooterTemplate _completionsFooterTemplate;

        //Panel types
        private IDictionary<PanelType, IPanelType> _panelTypes;

        public string SubPath { get; set; }
        
        private bool _panelDiv = true;
        public bool PanelDiv
        {
            get { return _panelDiv; }
            set { _panelDiv = value; }
        }

        public bool ShowEmpty { get; set; }

        public bool SingletonValues { get; set; }

        protected override void OnInit(EventArgs e)
        {
            InitPanelTypes();

            base.OnInit(e);
        }

        private void InitPanelTypes()
        {
            _panelTypes = new Dictionary<PanelType, IPanelType>
                              {
                                  {PanelType.Ads, new AdsPanelType( this )},
                                  {PanelType.Completions, new CompletionsPanelType( this )},
                                  {PanelType.Corrections, new CorrectionsPanelType( this )},
                                  {PanelType.Count, new CountPanelType( this )},
                                  {PanelType.Empty, new EmptyPanelType( this )},
                                  {PanelType.Products, new ProductsPanelType( this )},
                                  {PanelType.Unknown, new UnknownPanelType( this )},
                                  {PanelType.Values, new ValuesPanelType( this )},
                                  {PanelType.Zone, new ZonePanelType( this )},
                              };
        }

        protected override void  PerformDataBinding()
        {
            //if ( SubPath == null )
            //{
            //    throw new ArgumentException( "'SubPath' is a required property" );
            //}

            base.PerformDataBinding();
            // Do not attempt to bind data if there is no
            // data source set.
            if (!IsBoundUsingDataSourceID && (DataSource == null))
            {
                return;
            }
            InitPanelSourceRoot();

            if (_root == null) return;

            Controls.Clear();
            ClearChildViewState();
            CreateChildControls();
            ChildControlsCreated = true;
            TrackViewState();

            for (var i = 0; i < Controls.Count; i++)
            {
                Controls[i].DataBind();
            }
        }

        private void InitPanelSourceRoot()
        {
            PanelHierarchicalEnumerable enumerable = GetData(SubPath).Select() as PanelHierarchicalEnumerable;
            if (enumerable == null) return;

            var enumerator = enumerable.GetEnumerator();
            if (!enumerator.MoveNext()) return;

            _root = enumerator.Current;
        }

        protected override void CreateChildControls()
        {
            if (_root == null)
            {
                InitPanelSourceRoot();
                if (_root == null) return;
            }

            var tree = CreatePruningInformation(_root);
            if (!tree.IsVisible) return;
            CreateTemplateControl(_root, HeaderTemplate);
            CreateRecursiveTemplates(_root, tree);
            CreateTemplateControl(_root, FooterTemplate);
        }

        private PruningTree CreatePruningInformation(IHierarchyData root)
        {
            PruningTree tree = new PruningTree();
            if (root.HasChildren)
            {
                var anyVisible = ShowEmpty;
                foreach (var child in root.GetChildren())
                {
                    var childTree = CreatePruningInformation((IHierarchyData) child);
                    anyVisible = anyVisible || childTree.IsVisible;
                    tree.Children.Add(childTree);
                }
                tree.IsVisible = anyVisible;
            } else
            {
                var pc = (PanelContent) root.Item;
                tree.IsVisible = (ShowEmpty || pc.HasResult) && SelectPanelTemplate(root) != null;
                tree.IsVisible = tree.IsVisible &&
                                 (SingletonValues || 
                                 GetPanelTypeEnum(root) != PanelType.Values ||
                                 pc.ResultAsValues().Size > 1);
            }
            return tree;
        }

        private void CreateRecursiveTemplates(PanelHierarchyData panelHierarchy, PruningTree tree)
        {
            var panelHeaderTemplate = SelectHeaderTemplate(panelHierarchy);
            ((PanelHeaderTemplate) panelHeaderTemplate).PanelDiv = PanelDiv;
            CreateTemplateControl(panelHierarchy, panelHeaderTemplate);

            var panelTemplate = SelectPanelTemplate(panelHierarchy);
            CreateTemplateControl(panelHierarchy, panelTemplate);

            if (panelHierarchy.HasChildren)
            {
                var childIdx = 0;
                CreateTemplateControl(panelHierarchy, DescendTemplate);
                foreach (var childHierarchy in panelHierarchy.GetChildren())
                {
                    var childTree = tree.Children[childIdx++];
                    if (childTree.IsVisible)
                    {
                        CreateRecursiveTemplates((PanelHierarchyData) childHierarchy, childTree);
                    }
                }
                CreateTemplateControl(panelHierarchy, AscendTemplate);
            }

            var panelFooterTemplate = SelectFooterTemplate(panelHierarchy);
            ((PanelFooterTemplate) panelFooterTemplate).PanelDiv = PanelDiv;
            CreateTemplateControl(panelHierarchy, panelFooterTemplate);
        }

        private ITemplate SelectHeaderTemplate(IHierarchyData panel)
        {
            return GetPanelType( panel ).SelectHeaderTemplate( panel );
        }

        private ITemplate SelectFooterTemplate(IHierarchyData panel)
        {
            return GetPanelType( panel ).SelectFooterTemplate( panel );
        }

        private void CreateTemplateControl(PanelHierarchyData panel, ITemplate template)
        {
            if (template == null) return;

            var container = CreatePanelTemplateContainer(panel);
            template.InstantiateIn(container);
            Controls.Add(container);
        }

        private PanelTemplateContainer CreatePanelTemplateContainer(PanelHierarchyData panel)
        {
            return GetPanelType( panel ).CreatePanelTemplateContainer( panel );
        }

        private ITemplate SelectPanelTemplate(IHierarchyData panelHierarchy)
        {
            var panelTemplate = GetPanelType( panelHierarchy ).SelectPanelTemplate( panelHierarchy );
            return panelTemplate;
        }

        private static PanelType GetPanelTypeEnum(IHierarchyData panelHierarchy)
        {
            var o = Enum.Parse(typeof (PanelType), panelHierarchy.Type);
            return o is PanelType ? (PanelType) o : PanelType.Unknown;
        }

        private IPanelType GetPanelType( IHierarchyData panelHierarchy )
        {
            var panelTypeEnum = GetPanelTypeEnum( panelHierarchy );
            IPanelType panelType;
            if ( _panelTypes.TryGetValue( panelTypeEnum, out panelType ) )
            {
                return panelType;
            }
            return _panelTypes[PanelType.Unknown];
        }

        protected override void RenderChildren(HtmlTextWriter writer)
        {
            if (!HasControls()) return;

            for (int i = 0, end = Controls.Count; i < end; ++i)
            {
                Controls[i].RenderControl(writer);
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            RenderChildren(writer);
        }

        #region View Meta Templates

        [Browsable(false), TemplateContainer(typeof(PanelTemplateContainer)), PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate HeaderTemplate { get; set; }

        [Browsable(false), TemplateContainer(typeof(PanelTemplateContainer)), PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate FooterTemplate { get; set; }

        #endregion // View Meta Templates

        #region Panel Structure Meta Templates

        [Browsable(false), TemplateContainer(typeof(PanelTemplateContainer)), PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate DescendTemplate { get; set; }

        [Browsable(false), TemplateContainer(typeof(PanelTemplateContainer)), PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate AscendTemplate { get; set; }

        #endregion // Panel Structure Meta Templates

        #region Panel Meta Templates

        [Browsable(false), TemplateContainer(typeof(PanelTemplateContainer)), PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate PanelHeaderTemplate
        {
            get
            {
                return _headerTemplate;
            } 
            set
            {
                _headerTemplate = new PanelHeaderTemplate(value);
            }
        }

        [Browsable(false), TemplateContainer(typeof(PanelTemplateContainer)), PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate ProductPanelHeaderTemplate
        {
            get
            {
                return _productHeaderTemplate;
            }
            set
            {
                _productHeaderTemplate = new PanelHeaderTemplate(value);
            }
        }

        [Browsable(false), TemplateContainer(typeof(PanelTemplateContainer)), PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate ValuesPanelHeaderTemplate
        {
            get
            {
                return _valuesHeaderTemplate;
            }
            set
            {
                _valuesHeaderTemplate = new PanelHeaderTemplate(value);
            }
        }

        [Browsable(false), TemplateContainer(typeof(PanelTemplateContainer)), PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate CountPanelHeaderTemplate
        {
            get
            {
                return _countHeaderTemplate;
            }
            set
            {
                _countHeaderTemplate = new PanelHeaderTemplate(value);
            }
        }

        [Browsable(false), TemplateContainer(typeof(PanelTemplateContainer)), PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate CorrectionsPanelHeaderTemplate
        {
            get
            {
                return _correctionsHeaderTemplate;
            }
            set
            {
                _correctionsHeaderTemplate = new PanelHeaderTemplate(value);
            }
        }

        [Browsable(false), TemplateContainer(typeof(PanelTemplateContainer)), PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate CompletionsPanelHeaderTemplate
        {
            get
            {
                return _completionsHeaderTemplate;
            }
            set
            {
                _completionsHeaderTemplate = new PanelHeaderTemplate(value);
            }
        }

        [Browsable(false), TemplateContainer(typeof(PanelTemplateContainer)), PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate PanelFooterTemplate
        {
            get
            {
                return _footerTemplate;
            }
            set
            {
                _footerTemplate = new PanelFooterTemplate(value);
            }
        }

        [Browsable(false), TemplateContainer(typeof(PanelTemplateContainer)), PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate ProductPanelFooterTemplate
        {
            get
            {
                return _productFooterTemplate;
            }
            set
            {
                _productFooterTemplate = new PanelFooterTemplate(value);
            }
        }

        [Browsable(false), TemplateContainer(typeof(PanelTemplateContainer)), PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate ValuesPanelFooterTemplate
        {
            get
            {
                return _valuesFooterTemplate;
            }
            set
            {
                _valuesFooterTemplate = new PanelFooterTemplate(value);
            }
        }

        [Browsable(false), TemplateContainer(typeof(PanelTemplateContainer)), PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate CountPanelFooterTemplate
        {
            get
            {
                return _countFooterTemplate;
            }
            set
            {
                _countFooterTemplate = new PanelFooterTemplate(value);
            }
        }

        [Browsable(false), TemplateContainer(typeof(PanelTemplateContainer)), PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate CorrectionsPanelFooterTemplate
        {
            get
            {
                return _correctionsFooterTemplate;
            }
            set
            {
                _correctionsFooterTemplate = new PanelFooterTemplate(value);
            }
        }

        [Browsable(false), TemplateContainer(typeof(PanelTemplateContainer)), PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate CompletionsPanelFooterTemplate
        {
            get
            {
                return _completionsFooterTemplate;
            }
            set
            {
                _completionsFooterTemplate = new PanelFooterTemplate(value);
            }
        }

        #endregion // Panel Meta Templates

        #region Panel Templates

        [Browsable(false), TemplateContainer(typeof(PanelTemplateContainer)), PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate EmptyPanelTemplate { get; set; }

        [Browsable(false), TemplateContainer(typeof(PanelTemplateContainer)), PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate ZonePanelTemplate { get; set; }

        [Browsable(false), TemplateContainer(typeof(ProductsPanelTemplateContainer)), PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate ProductsPanelTemplate { get; set; }
        
        [Browsable(false), TemplateContainer(typeof(AdsPanelTemplateContainer)), PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate AdsPanelTemplate { get; set; }

        [Browsable(false), TemplateContainer(typeof(CompletionsPanelTemplateContainer)), PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate CompletionsPanelTemplate { get; set; }

        [Browsable(false), TemplateContainer(typeof(CorrectionsPanelTemplateContainer)), PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate CorrectionsPanelTemplate { get; set; }

        [Browsable(false), TemplateContainer(typeof(CountPanelTemplateContainer)), PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate CountPanelTemplate { get; set; }

        [Browsable(false), TemplateContainer(typeof(ValuesPanelTemplateContainer)), PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate ValuesPanelTemplate { get; set; }

        [Browsable(false), TemplateContainer(typeof(PanelTemplateContainer)), PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate UnknownPanelTemplate { get; set; }

        #endregion // Panel Templates
    }

    class PruningTree
    {
        internal bool IsVisible { get; set; }

        private readonly IList<PruningTree> _children = new List<PruningTree>();
        internal IList<PruningTree> Children { get { return _children; } }
    }

    public class ValuesPanelTemplateContainer : PanelTemplateContainer, IPanelResultData<Result.Values>
    {
        internal ValuesPanelTemplateContainer(PanelContent panel, int indent, bool hasChildren) : base(panel, indent, hasChildren) { }

        public Result.Values Result
        {
            get { return PanelContent.ResultAsValues(); }
        }
    }

    public class CountPanelTemplateContainer : PanelTemplateContainer, IPanelResultData<Result.Count>
    {
        internal CountPanelTemplateContainer(PanelContent panel, int indent, bool hasChildren) : base(panel, indent, hasChildren) { }

        public Result.Count Result
        {
            get { return PanelContent.ResultAsCount(); }
        }
    }

    public class CorrectionsPanelTemplateContainer : PanelTemplateContainer, IPanelResultData<Result.Corrections>
    {
        internal CorrectionsPanelTemplateContainer(PanelContent panel, int indent, bool hasChildren) : base(panel, indent, hasChildren) { }

        public Result.Corrections Result
        {
            get { return PanelContent.ResultAsCorrections(); }
        }
    }

    public class ProductsPanelTemplateContainer : PanelTemplateContainer, IPanelResultData<Result.Products>
    {
        internal ProductsPanelTemplateContainer(PanelContent panel, int indent, bool hasChildren) : base(panel, indent, hasChildren) { }

        public Result.Products Result
        {
            get { return PanelContent.ResultAsProducts(); }
        }
    }

    public class AdsPanelTemplateContainer : PanelTemplateContainer, IPanelResultData<Result.Ads>
    {
        internal AdsPanelTemplateContainer(PanelContent panel, int indent, bool hasChildren) : base(panel, indent, hasChildren) { }

        public Result.Ads Result
        {
            get
            {
                var resultAsAds = PanelContent.ResultAsAds();
                return resultAsAds;
            }
        }
    }

    public class CompletionsPanelTemplateContainer : PanelTemplateContainer, IPanelResultData<Result.Completions>
    {
        internal CompletionsPanelTemplateContainer(PanelContent panel, int indent, bool hasChildren) : base(panel, indent, hasChildren) { }

        public Result.Completions Result
        {
            get { return PanelContent.ResultAsCompletions(); }
        }
    }

    class PanelHeaderTemplate : ITemplate
    {

        private readonly ITemplate _inner;

        public bool PanelDiv { get; set; }

        public PanelHeaderTemplate(ITemplate inner)
        {
            _inner = inner;
        }

        public void InstantiateIn(Control container)
        {
            if (_inner != null)
            {
                _inner.InstantiateIn(container);
            }
            if (PanelDiv)
            {
                Literal ticket = new Literal {EnableViewState = false};
                ticket.DataBinding += TicketDataBinding;
                container.Controls.Add(ticket);
            }
        }

        private static void TicketDataBinding(object sender, EventArgs e)
        {
            Literal ticket = (Literal) sender;
            IPanelContentSource panelContent = ticket.NamingContainer as IPanelContentSource;
            if (panelContent != null)
            {
                ticket.Text = string.Format("<div class=\"eS-panel eS-t-{0}\">", panelContent.PanelContent.Ticket);
            }
        }
    }

    class PanelFooterTemplate : ITemplate
    {

        private readonly ITemplate _inner;

        public bool PanelDiv { get; set; }

        public PanelFooterTemplate(ITemplate inner)
        {
            _inner = inner;
        }

        public void InstantiateIn(Control container)
        {
            if (PanelDiv)
            {
                Literal ticket = new Literal {EnableViewState = false};
                ticket.DataBinding += TicketDataBinding;
                container.Controls.Add(ticket);
            }
            if (_inner != null)
            {
                _inner.InstantiateIn(container);
            }
        }

        private static void TicketDataBinding(object sender, EventArgs e)
        {
            Literal ticket = (Literal) sender;
            IPanelContentSource panelContent = ticket.NamingContainer as IPanelContentSource;
            if (panelContent != null)
            {
                ticket.Text = "</div>";
            }
        }
    }
}
