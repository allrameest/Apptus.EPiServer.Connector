using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apptus.ESales.EPiServer.DataAccess
{
    internal interface INodeEntryRelationRowMapper
    {
        int CatalogId { get; set; }
        int CatalogNodeId { get; set; }
        int SortOrder { get; set; }
    }
}
