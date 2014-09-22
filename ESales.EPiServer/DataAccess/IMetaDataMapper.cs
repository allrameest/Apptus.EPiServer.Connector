using System.Collections;
using System.Collections.Generic;
using Mediachase.Commerce.Catalog.Dto;

namespace Apptus.ESales.EPiServer.DataAccess
{
    internal interface IMetaDataMapper
    {
        MetaClassEx LoadMetaClassCached( int metaClassId, string language );
        Hashtable GetMetaFieldValues( CatalogEntryDto.CatalogEntryRow row );
        IEnumerable<MetaClassEx> GetAll();
    }
}