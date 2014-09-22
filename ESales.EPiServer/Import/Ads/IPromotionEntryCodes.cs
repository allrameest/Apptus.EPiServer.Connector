using System.Collections.Generic;

namespace Apptus.ESales.EPiServer.Import.Ads
{
    /// <summary>
    /// Defines how to extract which entries are related to a specific promotion type. Implement this interface if you create your own promotion type
    /// and would like the entries that are related to it to be exported to eSales.
    /// </summary>
    public interface IPromotionEntryCodes
    {
        /// <summary>
        /// The promotion type, e.g. "BuyXGetOffDiscount". The default promotion types in EPiServer Commerce are implemented out of the box.
        /// </summary>
        string PromotionType { get; }

        /// <summary>
        /// The entry codes that should be included in this promotion type. Example: Assume that promotion P1 and entries E1, E2, E3 exists 
        /// and that P1 is defined to include E1 and E2. Then P1 might be shown on a page where E1 and/or E2 is present.
        /// </summary>
        /// <param name="promotion">A <see cref="Promotion"/> with a matching type.</param>
        /// <returns>An enumerable of entry codes.</returns>
        IEnumerable<string> Included( Promotion promotion );

        /// <summary>
        /// The entry codes that are excluded from this promotion type. Example: Assume that promotion P1 and entries E1, E2, E3 exists and
        /// that P1 is defined to exclude E1 and E2. Then P1 might be shown on a page where E3 is present.
        /// </summary>
        /// <param name="promotion">A <see cref="Promotion"/> with a matching type.</param>
        /// <returns>An enumerable of entry codes.</returns>
        IEnumerable<string> Excluded( Promotion promotion );
    }
}
