using System.Linq;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Engine.Navigation;

namespace Apptus.ESales.EPiServer.DataAccess
{
    internal class UrlResolver : IUrlResolver
    {
        private readonly IEntryAdditionalData _entryAdditionalData;

        public UrlResolver( IEntryAdditionalData entryAdditionalData )
        {
            _entryAdditionalData = entryAdditionalData;
        }

        public string GetEntryUrl( CatalogEntryDto.CatalogEntryRow entry, string language )
        {
            string url;
            var seo = _entryAdditionalData.GetCatalogItemSeoRows( entry ).FirstOrDefault( s => s.LanguageCode == language );
            if ( seo != null && !string.IsNullOrWhiteSpace( seo.Uri ) && !seo.Uri.StartsWith( "~/" ) )
            {
                url = "~/" + seo.Uri;
            }
            else
            {
                url = UrlService.GetUrl( "EntryView", (object) "ec", (object) entry.Code );
            }
            return url;
        }
    }
}