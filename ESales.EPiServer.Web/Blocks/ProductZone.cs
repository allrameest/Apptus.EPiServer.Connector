using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using Apptus.ESales.Connector;
using Apptus.ESales.EPiServer.ConnectorExtensions;
using Apptus.ESales.EPiServer.Util;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using Mediachase.Commerce.Core;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Security;

namespace Apptus.ESales.EPiServer.Web.Blocks
{
    [ContentType( DisplayName = "Apptus Product Zone", 
        GUID = "71e92f15-229f-4280-a21a-e0fa3feb3483", 
        GroupName = "Apptus" )]

    public class ProductZone : Panel
    {
        private static readonly ArgumentHelper ArgumentHelper = new ArgumentHelper( new SiteContextMapper() );

        [Ignore]
        protected override Dictionary<string, string> Arguments
        {
            get
            {
                var defaultCart = OrderContext.Current.GetCart( Cart.DefaultName, SecurityContext.Current.CurrentUserId );
                var languageName = SiteContext.Current.LanguageName;
                var productKeys = defaultCart.OrderForms
                                             .Cast<OrderForm>()
                                             .SelectMany(
                                                 of => of.LineItems
                                                         .Cast<LineItem>()
                                                         .Select( li => AttributeHelper.CreateKey( li.ParentCatalogEntryId, languageName ) ) );

                var cart = string.Join( ",", productKeys );

                var products = ArgumentHelper.CodesToProducts( Codes );

                return new Dictionary<string, string>
                    {
                        {"filter", Filter},
                        {"window_first", "1"},
                        {"window_last", NumberOfProducts.ToString(CultureInfo.InvariantCulture)},
                        {"search_phrase", SearchPhrase},
                        {"search_attributes", SearchAttributes},
                        {"sort_by", SortBy},
                        {"customer_key", SecurityContext.Current.CurrentUserName},
                        {"product_key", ProductKey},
                        {"cart", cart},
                        {"products", products},
                    };
            }
        }

        public IEnumerable<PanelResult<Result.Products>> GetPanelResult()
        {
            var panels = new List<PanelResult<Result.Products>>();
            try
            {
                var root = GetPanelContent();
                ExtractPanelResult( root, panels );
            }
            catch ( Exception exception )
            {
                var panelResult = new PanelResult<Result.Products>(exception.Message);
                panels.Add( panelResult );
            }
            return panels;
        }

        private static void ExtractPanelResult(PanelContent pc, IList<PanelResult<Result.Products>> result)
        {
            if ( pc.Error != null )
            {
                var panelResult = new PanelResult<Result.Products>( pc.Error );
                result.Add( panelResult );
                return;
            }

            if ( pc.HasResult && pc.Result().Type == Result.ResultType.Products )
            {
                var panelResult = new PanelResult<Result.Products>( pc.Attributes, pc.ResultAsProducts(), pc.Ticket );
                result.Add( panelResult );
                return;
            }
            
            foreach (var subPanel in pc.Subpanels)
            {
                ExtractPanelResult(subPanel, result);
            }
        }

        [Display(Name = "Filter")]
        public virtual string Filter { get; set; }

        [Display (Name = "Number of products")]
        public virtual int NumberOfProducts { get; set; }

        [Display(Name = "Search phrase")]
        public virtual string SearchPhrase { get; set; }

        [Display(Name = "Search attributes")]
        public virtual string SearchAttributes { get; set; }

        [Display(Name = "Sort by" )]
        public virtual string SortBy { get; set; }
        
        [Display(Name = "Codes" )]
        public virtual string Codes { get; set; }

        [Ignore]
        [ScaffoldColumn(false)]
        public virtual string ProductKey { get; set; }

        public override void SetDefaultValues(ContentType contentType)
        {
            base.SetDefaultValues(contentType);
            Filter = "";
            NumberOfProducts = 3;
            SearchPhrase = "";
            SearchAttributes = "";
            SortBy = "relevance desc";
            ProductKey = "";
        }
    }
}
