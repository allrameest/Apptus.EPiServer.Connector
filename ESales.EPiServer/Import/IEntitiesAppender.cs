using System.Collections.Generic;

namespace Apptus.ESales.EPiServer.Import
{
    /// <summary>
    /// Base interface for appending additional entities in an eSales import.
    /// </summary>
    public interface IEntitiesAppender
    {
        /// <summary>
        /// Creates additional entites that will be appended in an eSales import.
        /// </summary>
        /// <param name="incremental"><c>true</c> if this is an incremental import, <c>false</c> if it is a full import.</param>
        /// <returns>An enumerable of entities.</returns>
        IEnumerable<IEntity> Append( bool incremental );
    }
}
