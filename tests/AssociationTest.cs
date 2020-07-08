using System.Collections.Generic;
using CloudCMS;
using Newtonsoft.Json.Linq;
using Xunit;

namespace CloudCMS.Tests
{
    public class AssociationTest : AbstractWithRepositoryTest
    {
        public AssociationTest(RepositoryFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async void TestAssociateUnassociate()
        {
            IBranch master = await Fixture.Repository.MasterAsync();
            
            JObject nodeObj1 = new JObject(
                new JProperty("title", "Node1")
            );
            INode node1 = (INode) await master.CreateNodeAsync(nodeObj1);
            
            JObject nodeObj2 = new JObject(
                new JProperty("title", "Node1")
            );
            INode node2 = (INode) await master.CreateNodeAsync(nodeObj2);
            
            JObject nodeObj3 = new JObject(
                new JProperty("title", "Node1")
            );
            INode node3 = (INode) await master.CreateNodeAsync(nodeObj3);
            
            // Associate node 1 directed to node 2 with a:child
            IAssociation association1 = await node1.AssociateAsync(node2, QName.create("a:child"));
            string expectedRef = "association://" + association1.PlatformId + "/" + association1.RepositoryId + "/" + association1.BranchId + "/" + association1.Id;
            Assert.Equal(expectedRef, association1.Ref.Ref);
            Assert.Equal(Directionality.DIRECTED, association1.Directionality);
            Assert.Equal(node1.Id ,association1.SourceNodeId);
            Assert.Equal(node2.Id, association1.TargetNodeId);
            
            INode source = await association1.ReadSourceNodeAsync();
            Assert.Equal(node1.Id, source.Id);
            INode target = await association1.ReadTargetNodeAsync();
            Assert.Equal(node2.Id, target.Id);

            // Associate node 1 undirected to node 3 with a:linked
            JObject associationData = new JObject(
                new JProperty("test", "field")
            );
            IAssociation association2 = await node1.AssociateAsync(node3, QName.create("a:linked"), Directionality.UNDIRECTED, associationData);
            Assert.Equal(Directionality.UNDIRECTED, association2.Directionality);
            Assert.Equal(node1.Id ,association2.SourceNodeId);
            Assert.Equal(node3.Id, association2.TargetNodeId);
            
            // Check all associations
            List<IAssociation> allAssociations = await node1.AssociationsAsync();
            Assert.Equal(3, allAssociations.Count); // will include a has_role association
            
            // Outgoing associations
            List<IAssociation> outgoingAssociations = await node1.AssociationsAsync(Direction.OUTGOING);
            Assert.Equal(2, outgoingAssociations.Count); // undirected counts as incoming
            
            // Incoming associations
            List<IAssociation> incomingAssociations = await node1.AssociationsAsync(Direction.INCOMING);
            Assert.Equal(2, incomingAssociations.Count); // will include has_role
            
            // Child Associations
            List<IAssociation> childAssociations = await node1.AssociationsAsync(QName.create("a:child"));
            Assert.Single(childAssociations);
            
            // Unassociate the two associations
            await node1.UnassociateAsync(node2, QName.create("a:child"));
            await node1.UnassociateAsync(node3, QName.create("a:linked"), Directionality.UNDIRECTED);
            
            allAssociations = await node1.AssociationsAsync();
            Assert.Single(allAssociations); // will include a has_role association
        }
    }
}