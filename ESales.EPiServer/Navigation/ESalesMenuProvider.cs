using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Shell.Navigation;

[MenuProvider]
public class ESalesMenuProvider : IMenuProvider
{
	/// <summary>
	/// Gets the menu items.
	/// </summary>
	/// <returns></returns>
	public IEnumerable<MenuItem> GetMenuItems()
	{
		return GetGlobalMenuItems();
	}

	private IEnumerable<MenuItem> GetGlobalMenuItems()
	{
		var menus = new List<MenuItem>();
		menus.Add(new SectionMenuItem("eSales", MenuPaths.Global + "/eSales") { SortIndex = -0, IsStyled = true });
		menus.Add(new UrlMenuItem("Manager", MenuPaths.Global + "/esales/manager", "/Admin/SitePlugin/ESalesManager.aspx") { SortIndex = 10, IsStyled = true });
		return menus;
	}
}
