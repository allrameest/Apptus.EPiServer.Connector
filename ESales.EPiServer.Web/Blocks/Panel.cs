using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Apptus.ESales.Connector;
using EPiServer.Core;
using EPiServer.DataAnnotations;
using eSales = Apptus.ESales.EPiServer.Util.ESales;

namespace Apptus.ESales.EPiServer.Web.Blocks
{
    public abstract class Panel : BlockData
    {
        private PanelContent _panelContent;

        [Required]
        public virtual string Path { get; set; }
        
        [Ignore]
		[ScaffoldColumnAttribute(false)]
        protected abstract Dictionary<string, string> Arguments { get; }

        public PanelContent GetPanelContent()
        {
            _panelContent = eSales.GetPanelContent(Path, Arguments);
            return _panelContent;
        }

        public string GetPanelClass()
        {
            if ( _panelContent == null )
            {
                return "";
            }

            return CssClass.Get().ForSiteOverlay(_panelContent.Ticket);
        }
    }
}
