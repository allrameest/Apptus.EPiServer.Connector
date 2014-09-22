namespace Apptus.ESales.EPiServer.Import
{
    /// <summary>
    /// Base interface for single entity converters.
    /// </summary>
    public interface IEntityConverter
    {
        /// <summary>
        /// Converts one entity.
        /// </summary>
        /// <param name="entity">The entity, i.e. a product, variant or ad, that should be converted.</param>
        /// <returns>A converted entity.</returns>
        IEntity Convert( IEntity entity );
    }
}