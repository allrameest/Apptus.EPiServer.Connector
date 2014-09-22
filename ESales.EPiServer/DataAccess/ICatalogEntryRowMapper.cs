using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mediachase.Commerce.Catalog.Dto;

namespace Apptus.ESales.EPiServer.DataAccess
{
    internal interface ICatalogEntryRowMapper
    {
        int CatalogId { get; set; }
        CatalogEntryDto.InventoryRow InventoryRow { get; set; }

        INodeEntryRelationRowMapper[] GetNodeEntryRelationRows();
    }
}
