using System.Linq;
using Mediachase.Commerce.Catalog.Dto;

namespace Apptus.ESales.EPiServer.DataAccess
{
    internal class CatalogEntryRowMapper : ICatalogEntryRowMapper
    {
        private readonly CatalogEntryDto.CatalogEntryRow _entry;

        public CatalogEntryRowMapper( CatalogEntryDto.CatalogEntryRow entry )
        {
            _entry = entry;
        }

        public INodeEntryRelationRowMapper[] GetNodeEntryRelationRows()
        {
            return _entry.GetNodeEntryRelationRows().Select( genuineRow => new NodeEntryRelationRowMapper( genuineRow ) ).Cast<INodeEntryRelationRowMapper>().ToArray();
        }

        public int CatalogId
        {
            get { return _entry.CatalogId; }
            set { _entry.CatalogId = value; }
        }

        CatalogEntryDto.InventoryRow ICatalogEntryRowMapper.InventoryRow
        {
            get { return _entry.InventoryRow; }
            set { _entry.InventoryRow = value; } 
        }

    }
}
