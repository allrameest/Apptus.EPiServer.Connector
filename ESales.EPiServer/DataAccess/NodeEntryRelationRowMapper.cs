using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mediachase.Commerce.Catalog.Dto;


namespace Apptus.ESales.EPiServer.DataAccess
{
    internal class NodeEntryRelationRowMapper : INodeEntryRelationRowMapper
    {
        private readonly CatalogEntryDto.NodeEntryRelationRow _nodeEntryRelationRow;

        public NodeEntryRelationRowMapper( CatalogEntryDto.NodeEntryRelationRow nodeEntryRelationRow )
        {
            _nodeEntryRelationRow = nodeEntryRelationRow;
        }

        public int CatalogId
        {
            get { return _nodeEntryRelationRow.CatalogId; }
            set { _nodeEntryRelationRow.CatalogId = value; }
        }

        public int CatalogNodeId
        {
            get { return _nodeEntryRelationRow.CatalogNodeId; }
            set { _nodeEntryRelationRow.CatalogNodeId = value; }
        }

        public int SortOrder
        {
            get { return _nodeEntryRelationRow.SortOrder; }
            set { _nodeEntryRelationRow.SortOrder = value; }
        }
    }
}
