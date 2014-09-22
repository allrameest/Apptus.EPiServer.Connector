using System.Collections.Generic;
using System.Linq;
using Apptus.ESales.EPiServer.DataAccess;
using Apptus.ESales.EPiServer.Import.Ads;
using Apptus.ESales.EPiServer.Import.Configuration;
using Apptus.ESales.EPiServer.Import.Formatting;
using Apptus.ESales.EPiServer.Import.Products;
using Apptus.ESales.EPiServer.Util;
using Autofac;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Marketing.Dto;
using Moq;
using NUnit.Framework;

namespace ESales.EPiServer.Tests
{
    [TestFixture]
    public class EntryConverterTests
    {

        [Test]
        public void SimpleOutline()
        {
            var catalogSystem = new Mock<ICatalogSystemMapper>();

            var nodeRelationTable = new CatalogRelationDto.CatalogNodeRelationDataTable();
            nodeRelationTable.AddCatalogNodeRelationRow( 1, 27, 23, 0 );
            nodeRelationTable.AddCatalogNodeRelationRow( 1, 32, 60, 0 );

            catalogSystem.Setup( cs => cs.GetCatalogNodeRelations( It.IsAny<int>() ) ).Returns( nodeRelationTable );

            catalogSystem.Setup( cs => cs.GetCatalogNode( 1 ) ).Returns( new CatalogNode { ID = "Departments", CatalogNodeId = 1, ParentNodeId = 0 } );
            catalogSystem.Setup( cs => cs.GetCatalogNode( 2 ) ).Returns( new CatalogNode { ID = "Beauty", CatalogNodeId = 2, ParentNodeId = 1 } );
            catalogSystem.Setup( cs => cs.GetCatalogNode( 3 ) ).Returns( new CatalogNode { ID = "SkinCare", CatalogNodeId = 3, ParentNodeId = 2 } );
            catalogSystem.Setup( cs => cs.GetCatalogNode( 4 ) ).Returns( new CatalogNode { ID = "LipCare", CatalogNodeId = 4, ParentNodeId = 3 } );

            var catalogEntry = MockedCatalogEntryRowMapper(new[] { 4 });
            var entryConverter = Build( catalogSystem.Object );
            var attributes = entryConverter.GetNodeEntryRelations( "Quanta", catalogEntry.Object );
            foreach ( var attribute in attributes )
            {
                if ( attribute.Name.Equals( "_outline" ) )
                {
                    Assert.That( attribute.Value, Is.EqualTo( "Quanta/Departments/Beauty/SkinCare/LipCare" ) );
                    return;
                }
            }
            Assert.Fail( "Attribute _outline was not found." );
        }
        
        [Test]
        public void OutlineForLinkedLeafNode()
        {
            var catalogSystem = new Mock<ICatalogSystemMapper>();

            var nodeRelationTable = new CatalogRelationDto.CatalogNodeRelationDataTable();
            nodeRelationTable.AddCatalogNodeRelationRow( 1, 27, 23, 0 );
            nodeRelationTable.AddCatalogNodeRelationRow( 1, 32, 60, 0 );

            catalogSystem.Setup( cs => cs.GetCatalogNodeRelations( It.IsAny<int>() ) ).Returns( nodeRelationTable );

            catalogSystem.Setup( cs => cs.GetCatalogNode( 1 ) ).Returns( new CatalogNode { ID = "Departments", CatalogNodeId = 1, ParentNodeId = 0 } );
            catalogSystem.Setup( cs => cs.GetCatalogNode( 2 ) ).Returns( new CatalogNode { ID = "Beauty", CatalogNodeId = 2, ParentNodeId = 1 } );
            catalogSystem.Setup( cs => cs.GetCatalogNode( 3 ) ).Returns( new CatalogNode { ID = "SkinCare", CatalogNodeId = 3, ParentNodeId = 2 } );
            catalogSystem.Setup( cs => cs.GetCatalogNode( 27 ) ).Returns( new CatalogNode { ID = "MakeUp", CatalogNodeId = 27, ParentNodeId = 2 } );
            catalogSystem.Setup( cs => cs.GetCatalogNode( 23 ) ).Returns( new CatalogNode { ID = "LipCare", CatalogNodeId = 23, ParentNodeId = 3 } );

            var catalogEntry = MockedCatalogEntryRowMapper(new[] { 23 });
            var entryConverter = Build( catalogSystem.Object );
            var attributes = entryConverter.GetNodeEntryRelations( "Quanta", catalogEntry.Object );
            foreach ( var attribute in attributes )
            {
                if ( attribute.Name.Equals( "_outline" ) )
                {
                    Assert.That( attribute.Value, Is.EqualTo( "Quanta/Departments/Beauty/SkinCare/LipCare|Quanta/Departments/Beauty/MakeUp/LipCare" ) );
                    return;
                }
            }
            Assert.Fail( "Attribute _outline was not found." );
        }
        
        [Test]
        public void OutlineForLinkedIntermediateNodes()
        {
            var catalogSystem = new Mock<ICatalogSystemMapper>();

            var nodeRelationTable = new CatalogRelationDto.CatalogNodeRelationDataTable();
            nodeRelationTable.AddCatalogNodeRelationRow( 1, 27, 23, 0 );
            nodeRelationTable.AddCatalogNodeRelationRow( 1, 32, 60, 0 );

            catalogSystem.Setup( cs => cs.GetCatalogNodeRelations( It.IsAny<int>() ) ).Returns( nodeRelationTable );

            catalogSystem.Setup( cs => cs.GetCatalogNode( 1 ) ).Returns( new CatalogNode { ID = "Departments", CatalogNodeId = 1, ParentNodeId = 0 } );
            catalogSystem.Setup( cs => cs.GetCatalogNode( 32 ) ).Returns( new CatalogNode { ID = "Electronics", CatalogNodeId = 32, ParentNodeId = 1 } );
            catalogSystem.Setup( cs => cs.GetCatalogNode( 60 ) ).Returns( new CatalogNode { ID = "Appliances", CatalogNodeId = 60, ParentNodeId = 1 } );
            catalogSystem.Setup( cs => cs.GetCatalogNode( 61 ) ).Returns( new CatalogNode { ID = "Microwaves", CatalogNodeId = 61, ParentNodeId = 60 } );

            var catalogEntry = MockedCatalogEntryRowMapper(new[] { 61 });
            var entryConverter = Build( catalogSystem.Object );
            var attributes = entryConverter.GetNodeEntryRelations( "Quanta", catalogEntry.Object );
            foreach ( var attribute in attributes )
            {
                if ( attribute.Name.Equals( "_outline" ) )
                {
                    Assert.That( attribute.Value, Is.EqualTo( "Quanta/Departments/Appliances/Microwaves|Quanta/Departments/Electronics/Appliances/Microwaves" ) );
                    return;
                }
            }
            Assert.Fail( "Attribute _outline was not found." );
        }
        
        [Test]
        public void OutlineForNodesWithCircularDependencies()
        {
            var catalogSystem = new Mock<ICatalogSystemMapper>();

            var nodeRelationTable = new CatalogRelationDto.CatalogNodeRelationDataTable();
            nodeRelationTable.AddCatalogNodeRelationRow( 1, 60, 32, 0 );
            nodeRelationTable.AddCatalogNodeRelationRow( 1, 32, 60, 0 );

            catalogSystem.Setup( cs => cs.GetCatalogNodeRelations( It.IsAny<int>() ) ).Returns( nodeRelationTable );

            catalogSystem.Setup( cs => cs.GetCatalogNode( 1 ) ).Returns( new CatalogNode { ID = "Departments", CatalogNodeId = 1, ParentNodeId = 0 } );
            catalogSystem.Setup( cs => cs.GetCatalogNode( 32 ) ).Returns( new CatalogNode { ID = "Electronics", CatalogNodeId = 32, ParentNodeId = 1 } );
            catalogSystem.Setup( cs => cs.GetCatalogNode( 60 ) ).Returns( new CatalogNode { ID = "Appliances", CatalogNodeId = 60, ParentNodeId = 1 } );
            catalogSystem.Setup( cs => cs.GetCatalogNode( 61 ) ).Returns( new CatalogNode { ID = "Microwaves", CatalogNodeId = 61, ParentNodeId = 60 } );

            var catalogEntry = MockedCatalogEntryRowMapper(new[] { 61 });
            var entryConverter = Build( catalogSystem.Object );
            var attributes = entryConverter.GetNodeEntryRelations( "Quanta", catalogEntry.Object );
            foreach ( var attribute in attributes )
            {
                if ( attribute.Name.Equals( "_outline" ) )
                {
                    Assert.That( attribute.Value, Is.EqualTo( "Quanta/Departments/Appliances/Microwaves|Quanta/Departments/Electronics/Appliances/Microwaves" ) );
                    return;
                }
            }
            Assert.Fail( "Attribute _outline was not found." );
        }

        [Test]
        public void SimpleNode()
        {
            var catalogSystem = new Mock<ICatalogSystemMapper>();

            var nodeRelationTable = new CatalogRelationDto.CatalogNodeRelationDataTable();
            nodeRelationTable.AddCatalogNodeRelationRow( 1, 27, 23, 0 );
            nodeRelationTable.AddCatalogNodeRelationRow( 1, 32, 60, 0 );

            catalogSystem.Setup( cs => cs.GetCatalogNodeRelations( It.IsAny<int>() ) ).Returns( nodeRelationTable );

            catalogSystem.Setup( cs => cs.GetCatalogNode( 1 ) ).Returns( new CatalogNode { ID = "Departments", CatalogNodeId = 1, ParentNodeId = 0 } );
            catalogSystem.Setup( cs => cs.GetCatalogNode( 2 ) ).Returns( new CatalogNode { ID = "Beauty", CatalogNodeId = 2, ParentNodeId = 1 } );
            catalogSystem.Setup( cs => cs.GetCatalogNode( 3 ) ).Returns( new CatalogNode { ID = "SkinCare", CatalogNodeId = 3, ParentNodeId = 2 } );
            catalogSystem.Setup( cs => cs.GetCatalogNode( 4 ) ).Returns( new CatalogNode { ID = "LipCare", CatalogNodeId = 4, ParentNodeId = 3 } );

            var catalogEntry = MockedCatalogEntryRowMapper(new[] { 4 });
            var entryConverter = Build( catalogSystem.Object );
            var attributes = entryConverter.GetNodeEntryRelations( "Quanta", catalogEntry.Object );
            foreach ( var attribute in attributes )
            {
                if(attribute.Name.Equals( "_node" ))
                {
                    Assert.AreEqual( attribute.Values.Count(), 4 );
                    Assert.That( attribute.Values.Contains( "Departments" ) );
                    Assert.That( attribute.Values.Contains( "Beauty" ) );
                    Assert.That( attribute.Values.Contains( "SkinCare" ) );
                    Assert.That( attribute.Values.Contains( "LipCare" ) );
                    return;
                }
            }
            Assert.Fail( "Attribute _node was not found." );
        }
        
        [Test]
        public void NodesForLinkedLeafNode()
        {
            var catalogSystem = new Mock<ICatalogSystemMapper>();

            var nodeRelationTable = new CatalogRelationDto.CatalogNodeRelationDataTable();
            nodeRelationTable.AddCatalogNodeRelationRow( 1, 27, 23, 0 );
            nodeRelationTable.AddCatalogNodeRelationRow( 1, 32, 60, 0 );

            catalogSystem.Setup( cs => cs.GetCatalogNodeRelations( It.IsAny<int>() ) ).Returns( nodeRelationTable );

            catalogSystem.Setup( cs => cs.GetCatalogNode( 1 ) ).Returns( new CatalogNode { ID = "Departments", CatalogNodeId = 1, ParentNodeId = 0 } );
            catalogSystem.Setup( cs => cs.GetCatalogNode( 2 ) ).Returns( new CatalogNode { ID = "Beauty", CatalogNodeId = 2, ParentNodeId = 1 } );
            catalogSystem.Setup( cs => cs.GetCatalogNode( 3 ) ).Returns( new CatalogNode { ID = "SkinCare", CatalogNodeId = 3, ParentNodeId = 2 } );
            catalogSystem.Setup( cs => cs.GetCatalogNode( 27 ) ).Returns( new CatalogNode { ID = "MakeUp", CatalogNodeId = 27, ParentNodeId = 2 } );
            catalogSystem.Setup( cs => cs.GetCatalogNode( 23 ) ).Returns( new CatalogNode { ID = "LipCare", CatalogNodeId = 23, ParentNodeId = 3 } );

            var catalogEntry = MockedCatalogEntryRowMapper(new[] { 23 });
            var entryConverter = Build( catalogSystem.Object );
            var attributes = entryConverter.GetNodeEntryRelations( "Quanta", catalogEntry.Object );
            foreach ( var attribute in attributes )
            {
                if ( attribute.Name.Equals( "_node" ) )
                {
                    Assert.AreEqual( attribute.Values.Count(), 5 );
                    Assert.That( attribute.Values.Contains( "Departments" ) );
                    Assert.That( attribute.Values.Contains( "Beauty" ) );
                    Assert.That( attribute.Values.Contains( "SkinCare" ) );
                    Assert.That( attribute.Values.Contains( "MakeUp" ) );
                    Assert.That( attribute.Values.Contains( "LipCare" ) );
                    return;
                }
            }
            Assert.Fail( "Attribute _node was not found." );
        }
        
        [Test]
        public void NodesForLinkedIntermediateNodes()
        {
            var catalogSystem = new Mock<ICatalogSystemMapper>();

            var nodeRelationTable = new CatalogRelationDto.CatalogNodeRelationDataTable();
            nodeRelationTable.AddCatalogNodeRelationRow( 1, 27, 23, 0 );
            nodeRelationTable.AddCatalogNodeRelationRow( 1, 32, 60, 0 );

            catalogSystem.Setup( cs => cs.GetCatalogNodeRelations( It.IsAny<int>() ) ).Returns( nodeRelationTable );

            catalogSystem.Setup( cs => cs.GetCatalogNode( 1 ) ).Returns( new CatalogNode { ID = "Departments", CatalogNodeId = 1, ParentNodeId = 0 } );
            catalogSystem.Setup( cs => cs.GetCatalogNode( 32 ) ).Returns( new CatalogNode { ID = "Electronics", CatalogNodeId = 32, ParentNodeId = 1 } );
            catalogSystem.Setup( cs => cs.GetCatalogNode( 60 ) ).Returns( new CatalogNode { ID = "Appliances", CatalogNodeId = 60, ParentNodeId = 1 } );
            catalogSystem.Setup( cs => cs.GetCatalogNode( 61 ) ).Returns( new CatalogNode { ID = "Microwaves", CatalogNodeId = 61, ParentNodeId = 60 } );

            var catalogEntry = MockedCatalogEntryRowMapper(new[] { 61 });
            var entryConverter = Build( catalogSystem.Object );
            var attributes = entryConverter.GetNodeEntryRelations( "Quanta", catalogEntry.Object );
            foreach ( var attribute in attributes )
            {
                if ( attribute.Name.Equals( "_node" ) )
                {
                    Assert.AreEqual( attribute.Values.Count(), 4 );
                    Assert.That( attribute.Values.Contains( "Departments" ) );
                    Assert.That( attribute.Values.Contains( "Electronics" ) );
                    Assert.That( attribute.Values.Contains( "Appliances" ) );
                    Assert.That( attribute.Values.Contains( "Microwaves" ) );
                    return;
                }
            }
            Assert.Fail( "Attribute _node was not found." );
        }
        
        [Test]
        public void NodesForNodesWithCircularDependencies()
        {
            var catalogSystem = new Mock<ICatalogSystemMapper>();

            var nodeRelationTable = new CatalogRelationDto.CatalogNodeRelationDataTable();
            nodeRelationTable.AddCatalogNodeRelationRow( 1, 60, 32, 0 );
            nodeRelationTable.AddCatalogNodeRelationRow( 1, 32, 60, 0 );

            catalogSystem.Setup( cs => cs.GetCatalogNodeRelations( It.IsAny<int>() ) ).Returns( nodeRelationTable );

            catalogSystem.Setup( cs => cs.GetCatalogNode( 1 ) ).Returns( new CatalogNode { ID = "Departments", CatalogNodeId = 1, ParentNodeId = 0 } );
            catalogSystem.Setup( cs => cs.GetCatalogNode( 32 ) ).Returns( new CatalogNode { ID = "Electronics", CatalogNodeId = 32, ParentNodeId = 1 } );
            catalogSystem.Setup( cs => cs.GetCatalogNode( 60 ) ).Returns( new CatalogNode { ID = "Appliances", CatalogNodeId = 60, ParentNodeId = 1 } );
            catalogSystem.Setup( cs => cs.GetCatalogNode( 61 ) ).Returns( new CatalogNode { ID = "Microwaves", CatalogNodeId = 61, ParentNodeId = 60 } );

            var catalogEntry = MockedCatalogEntryRowMapper(new[] { 61 });
            var entryConverter = Build( catalogSystem.Object );
            var attributes = entryConverter.GetNodeEntryRelations( "Quanta", catalogEntry.Object );
            foreach ( var attribute in attributes )
            {
                if ( attribute.Name.Equals( "_node" ) )
                {
                    Assert.AreEqual( attribute.Values.Count(), 4 );
                    Assert.That( attribute.Values.Contains( "Departments" ) );
                    Assert.That( attribute.Values.Contains( "Electronics" ) );
                    Assert.That( attribute.Values.Contains( "Appliances" ) );
                    Assert.That( attribute.Values.Contains( "Microwaves" ) );
                    return;
                }
            }
            Assert.Fail( "Attribute _node was not found." );
        }

        private static Mock<ICatalogEntryRowMapper> MockedCatalogEntryRowMapper( IEnumerable<int> connectedNodeIds )
        {
            return MockedCatalogEntryRowMapper(connectedNodeIds.Select( MockedNodeEntryRelationRowMapper ).Select( r => r.Object ).ToArray());
        }

        private static Mock<ICatalogEntryRowMapper> MockedCatalogEntryRowMapper(INodeEntryRelationRowMapper[] entryRelationRows = null)
        {
            var mockRow = new Mock<ICatalogEntryRowMapper>();
            mockRow.Setup( r => r.CatalogId ).Returns( 1 );
            mockRow.Setup( r => r.InventoryRow ).Returns( (CatalogEntryDto.InventoryRow) null );
            mockRow.Setup( r => r.GetNodeEntryRelationRows() ).Returns( entryRelationRows );
            return mockRow;
        }

        private static Mock<INodeEntryRelationRowMapper> MockedNodeEntryRelationRowMapper( int catalogNodeId)
        {
            var mockRow = new Mock<INodeEntryRelationRowMapper>();
            mockRow.Setup( r => r.CatalogId ).Returns( 1 );
            mockRow.Setup( r => r.CatalogNodeId ).Returns( catalogNodeId );
            mockRow.Setup( r => r.SortOrder ).Returns( 0 );
            return mockRow;
        }

        private static EntryConverter Build( ICatalogSystemMapper catalogSystem )
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<EntryConverter>();
            builder.Register( c => catalogSystem );
            builder.Register( c => new Mock<IMetaDataMapper>().Object );
            builder.Register( c => new Mock<IPriceServiceMapper>().Object );
            builder.Register( c => new Mock<IConfiguration>().Object );
            builder.Register( c => new Mock<IUrlResolver>().Object );

            var keyLookup = new Mock<IKeyLookup>();
            keyLookup
                .Setup( kl => kl.Value( It.IsAny<CatalogEntryDto.CatalogEntryRow>(), It.IsAny<string>() ) )
                .Returns<CatalogEntryDto.CatalogEntryRow, string>( ( e, l ) => AttributeHelper.CreateKey( e.Code, l ) );
            builder.Register( c => keyLookup.Object );

            builder.Register( c => new PromotionEntryCodeProvider( c.Resolve<PromotionDataTableMapper>(), Enumerable.Empty<IPromotionEntryCodes>() ) );
            builder.Register( c => new PromotionDataTableMapper(
                                       new PromotionDto.PromotionLanguageDataTable(),
                                       new PromotionDto.PromotionDataTable(),
                                       new CampaignDto.CampaignDataTable() ) );
            
            builder.RegisterAssemblyTypes( typeof( IFormatRule ).Assembly ).As<IFormatRule>();
            builder.RegisterType<Formatter>();

            var container = builder.Build();

            return container.Resolve<EntryConverter>();
        }
    }
}
