using System;
using System.Collections.Generic;
using System.Linq;
using Apptus.ESales.EPiServer.Import;
using Apptus.ESales.EPiServer.Import.Products;
using Mediachase.Commerce.Catalog.Dto;
using NUnit.Framework;

namespace ESales.EPiServer.Tests
{
    [TestFixture]
    public class ESalesVariantHelperTests
    {
        [Test]
        public void MultipleParents()
        {
            var relations = new CatalogRelationDto();
            relations.CatalogEntryRelation.AddCatalogEntryRelationRow( 1, 10, "", 0, "", 0 );
            relations.CatalogEntryRelation.AddCatalogEntryRelationRow( 2, 10, "", 0, "", 0 );
            Assert.Throws<ArgumentException>( () => new ESalesVariantHelper( relations.CatalogEntryRelation ) );
        }

        [Test]
        public void FindVariants()
        {
            var relations = new CatalogRelationDto();
            relations.CatalogEntryRelation.AddCatalogEntryRelationRow( 1, 10, "", 0, "", 0 );
            relations.CatalogEntryRelation.AddCatalogEntryRelationRow( 1, 11, "", 0, "", 0 );
            relations.CatalogEntryRelation.AddCatalogEntryRelationRow( 2, 12, "", 0, "", 0 );
            var helper = new ESalesVariantHelper( relations.CatalogEntryRelation );
            Assert.That( helper.GetVariants( 1 ).ToArray(), Is.EquivalentTo( new[] { 10, 11 } ) );
        }

        [Test]
        public void FindNoVariants()
        {
            var relations = new CatalogRelationDto();
            relations.CatalogEntryRelation.AddCatalogEntryRelationRow( 1, 10, "", 0, "", 0 );
            relations.CatalogEntryRelation.AddCatalogEntryRelationRow( 1, 11, "", 0, "", 0 );
            relations.CatalogEntryRelation.AddCatalogEntryRelationRow( 2, 12, "", 0, "", 0 );
            var helper = new ESalesVariantHelper( relations.CatalogEntryRelation );
            Assert.That( !helper.GetVariants( 3 ).Any() );
        }

        [Test]
        public void FindParentProduct()
        {
            var relations = new CatalogRelationDto();
            relations.CatalogEntryRelation.AddCatalogEntryRelationRow( 1, 10, "", 0, "", 0 );
            relations.CatalogEntryRelation.AddCatalogEntryRelationRow( 1, 11, "", 0, "", 0 );
            relations.CatalogEntryRelation.AddCatalogEntryRelationRow( 2, 12, "", 0, "", 0 );
            var helper = new ESalesVariantHelper( relations.CatalogEntryRelation );
            Assert.That( helper.GetParentProduct( 12 ), Is.EqualTo( 2 ) );
        }

        [Test]
        public void FindNoParentProduct()
        {
            var relations = new CatalogRelationDto();
            relations.CatalogEntryRelation.AddCatalogEntryRelationRow( 1, 10, "", 0, "", 0 );
            relations.CatalogEntryRelation.AddCatalogEntryRelationRow( 1, 11, "", 0, "", 0 );
            relations.CatalogEntryRelation.AddCatalogEntryRelationRow( 2, 12, "", 0, "", 0 );
            var helper = new ESalesVariantHelper( relations.CatalogEntryRelation );
            Assert.Throws<KeyNotFoundException>( () => helper.GetParentProduct( 13 ) );
        }
    }
}
