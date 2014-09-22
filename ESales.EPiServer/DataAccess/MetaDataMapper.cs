using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Storage;
using Mediachase.MetaDataPlus.Configurator;

namespace Apptus.ESales.EPiServer.DataAccess
{
    internal class MetaDataMapper : IMetaDataMapper
    {
        public MetaClassEx LoadMetaClassCached( int metaClassId, string language )
        {
            var context = CatalogContext.MetaDataContext;
            context.UseCurrentThreadCulture = false;
            context.Language = language;
            var metaClass = MetaHelper.LoadMetaClassCached( context, metaClassId );
            return new MetaClassEx( metaClass );
        }

        public Hashtable GetMetaFieldValues( CatalogEntryDto.CatalogEntryRow row )
        {
            return ObjectHelper.GetMetaFieldValues( row );
        }

        public IEnumerable<MetaClassEx> GetAll()
        {
            var context = CatalogContext.MetaDataContext;
            var metaClassCollection = MetaClass.GetList(context, "Mediachase.Commerce.Catalog", true);
            return metaClassCollection.Cast<MetaClass>().Select( mc => new MetaClassEx( mc ) );
        }
    }
}