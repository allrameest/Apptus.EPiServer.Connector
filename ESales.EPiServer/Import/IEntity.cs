using System.Collections.Generic;

namespace Apptus.ESales.EPiServer.Import
{
    /// <summary>
    /// An eSales entity.
    /// </summary>
    public interface IEntity : IEnumerable<Attribute>
    {
        /// <summary>
        /// The name of the entity.
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// The key of the entity.
        /// </summary>
        Attribute Key { get; }
    }
}
