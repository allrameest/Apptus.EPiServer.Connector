using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Apptus.ESales.Connector;
using Apptus.ESales.EPiServer.Config;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Core;
using ResponseGroup = Mediachase.Commerce.Catalog.Managers.CatalogEntryResponseGroup.ResponseGroup;

namespace Apptus.ESales.EPiServer.Util
{
    public class Products
    {
        private const ResponseGroup DefaultResponseGroup = ResponseGroup.CatalogEntryInfo;
        private static readonly IAppConfig AppConfig = new AppConfig();

        public static IEnumerable<ProductESalesEntry> GetProductEntries(Result.Products products)
        {
            return GetProductEntries(products.ProductList);
        }

        public static IEnumerable<ProductESalesEntry> GetProductEntries(IEnumerable<Result.Product> products, ResponseGroup responseGroup = DefaultResponseGroup)
        {
            products = products.ToList();
            var entryResponseGroup = new CatalogEntryResponseGroup( responseGroup );
            var entries = CatalogContext.Current.GetCatalogEntries( products
                                                                        .SelectMany( p => new[] { int.Parse( p.GetValue( "_id" ) ) }
                                                                                              .Concat( p.VariantList.EmptyIfNull().Select( v => int.Parse( v.GetValue( "_id" ) ) ) ) )
                                                                        .ToArray(),
                                                                    false, new TimeSpan(), entryResponseGroup)
                                        .Entry
                                        .EmptyIfNull()
                                        .ToDictionary( e => e.CatalogEntryId, e => e );

            return products.Select( p => new ProductESalesEntry(
                                             entries[int.Parse( p.GetValue( "_id" ) )],
                                             double.Parse( p.GetValue( "proportion", "0" ) ),
                                             p.Ticket,
                                             p.GetValue( "_outline", "" ).Split( "|", StringSplitOptions.RemoveEmptyEntries ),
                                             p.VariantList.EmptyIfNull()
                                                 .Select( v => new ESalesEntry(
                                                                   entries[int.Parse( v.GetValue( "_id" ) )],
                                                                   double.Parse( v.GetValue( "proportion", "0" ) ),
                                                                   v.Ticket,
                                                                   v.GetValue( "_outline", "" ).Split( "|", StringSplitOptions.RemoveEmptyEntries ), 
                                                                   v.ToDictionary(a => a.Name, a => a.Value)
                                                                   )),
                                             p.ToDictionary(a => a.Name, a => a.Value)));
        }

        /// <summary>
        /// Gets an eSales product key from an EPiServer id.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static string ProductKey( Entry entry )
        {
            return entry == null
                       ? string.Empty
                       : AttributeHelper.CreateKey( AppConfig.UseCodeAsKey ? entry.ID : entry.CatalogEntryId.ToString( CultureInfo.InvariantCulture ), GetUserLang() );
        }

        private static string GetUserLang()
        {
            var lang = SiteContext.Current.LanguageName;
            return !string.IsNullOrEmpty(lang) ? lang.ToLower() : "en-us";
        }
    }
}
