using System;
using EPiServer;
using EPiServer.Shell.Navigation;
using EPiServer.Shell.WebForms;

namespace Apptus.ESales.EPiServer.Navigation
{
	public partial class ESalesManager : WebFormsBase
    {
		protected string IFrameSource { get; set; }

		/// <summary>
		/// Raises the <see cref="E:System.Web.UI.Page.PreInit"/> event at the beginning of page initialization.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
		protected override void OnPreInit(EventArgs e)
		{
			base.OnPreInit(e);

			MasterPageFile = UriSupport.ResolveUrlFromUIBySettings("MasterPages/Frameworks/Framework.Master");
		}

		/// <summary>
		/// Raises the Init event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			ESalesManagerShellMenu.SelectionPath = string.Format("{0}/esales/manager", MenuPaths.Global);
			ESalesManagerShellMenu.Area = "CMS";
		}
        
    }
}