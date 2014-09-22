using System;
using System.Collections.Generic;
using System.Linq;
using Apptus.ESales.EPiServer.Util;
using Mediachase.Commerce.Catalog.Dto;

namespace Apptus.ESales.EPiServer.Import.Products
{
    internal class ESalesVariantHelper
    {
        private readonly Dictionary<int, List<int>> _productsToVariantsRelations;
        private readonly Dictionary<int, int> _variantsToProductsRelations;

        internal ESalesVariantHelper( IEnumerable<CatalogRelationDto.CatalogEntryRelationRow> productVariationRelations )
        {
            _productsToVariantsRelations = new Dictionary<int, List<int>>();
            _variantsToProductsRelations = new Dictionary<int, int>();
            foreach ( var relation in productVariationRelations )
            {
                _productsToVariantsRelations.Add( relation.ParentEntryId, relation.ChildEntryId );
                try
                {
                    _variantsToProductsRelations.Add( relation.ChildEntryId, relation.ParentEntryId );
                }
                catch ( ArgumentException ae )
                {
                    throw new ArgumentException(
                        string.Format( "Variants with multiple parent products are not supported. [Variant: {0}] - [Product 1: {1}] [Product 2: {2}]",
                                       relation.ChildEntryId,
                                       _variantsToProductsRelations[relation.ChildEntryId],
                                       relation.ParentEntryId ), ae );
                }
            }
        }

        internal bool IsVariant( int variant )
        {
            return _variantsToProductsRelations.ContainsKey( variant );
        }

        internal int GetParentProduct( int variant )
        {
            return _variantsToProductsRelations[variant];
        }

        internal IEnumerable<int> GetVariants( int product )
        {
            List<int> variants;
            return _productsToVariantsRelations.TryGetValue( product, out variants ) ? variants.Select( v => v ) : Enumerable.Empty<int>();
        }
    }
}