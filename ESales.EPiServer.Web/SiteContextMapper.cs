using Apptus.ESales.EPiServer.DataAccess;
using Mediachase.Commerce.Core;

namespace Apptus.ESales.EPiServer.Web
{
    internal class SiteContextMapper : ISiteContextMapper
    {
        public string LanguageName
        {
            get { return SiteContext.Current.LanguageName; }
        }
    }
}
