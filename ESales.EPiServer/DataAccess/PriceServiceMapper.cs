using System.Collections.Generic;
using EPiServer.ServiceLocation;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Pricing;

namespace Apptus.ESales.EPiServer.DataAccess
{
    internal class PriceServiceMapper : IPriceServiceMapper
    {
        private readonly IPriceService _priceService;

        public PriceServiceMapper()
        {
            _priceService = ServiceLocator.Current.GetInstance<IPriceService>();
        }

        public IEnumerable<IPriceValue> GetCatalogEntryPrices( CatalogKey catalogKey )
        {
            return _priceService.GetCatalogEntryPrices( catalogKey );
        }
    }
}