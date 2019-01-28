using Xunit;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using CloudCMS.Branches;
using CloudCMS.Nodes;

namespace CloudCMS.Tests
{
    public class NodeTest : AbstractWithRepositoryTest
    {

        public NodeTest(RepositoryFixture fixture) : base(fixture)
        {

        }

        [Fact]
        public async void TestNodeCrud()
        {
            IBranch branch = await Fixture.Repository.ReadBranchAsync("master");

            JObject nodeObj = new JObject(
                new JProperty("title", "MyNode")
            );
            INode node = await branch.CreateNodeAsync(nodeObj);

            INode nodeRead = await branch.ReadNodeAsync(node.Id);
            Assert.Equal(node.Data, nodeRead.Data);

            node.Data["title"] = "New title";
            await node.UpdateAsync();

            await nodeRead.ReloadAsync();
            Assert.Equal(node.Data["title"], nodeRead.Data["title"]);

            await node.DeleteAsync();
            
            nodeRead = await branch.ReadNodeAsync(node.Id);
            Assert.Null(nodeRead);
        }

        [Fact]
        public async void TestNodeQueryFind()
        {
            IBranch branch = await Fixture.Repository.ReadBranchAsync("master");

            JObject nodeObj1 = new JObject(
                new JProperty("title", "Cheese burger"),
                new JProperty("meal", "lunch")
            );
            JObject nodeObj2 = new JObject(
                new JProperty("title", "Ham burger"),
                new JProperty("meal", "lunch")
            );
            JObject nodeObj3 = new JObject(
                new JProperty("title", "Turkey sandwich"),
                new JProperty("meal", "lunch")
            );
            JObject nodeObj4 = new JObject(
                new JProperty("title", "Oatmeal"),
                new JProperty("meal", "breakfast")
            );

            INode node1 = await branch.CreateNodeAsync(nodeObj1);
            INode node2 = await branch.CreateNodeAsync(nodeObj2);
            INode node3 = await branch.CreateNodeAsync(nodeObj3);
            INode node4 = await branch.CreateNodeAsync(nodeObj4);

            // Wait for nodes to index
            Thread.Sleep(5000);

            JObject query = new JObject(
                new JProperty("meal", "lunch")
            );
            List<INode> queryNodes = await branch.QueryNodesAsync(query);
            var queryNodesIds = queryNodes.Select(node => node.Id);
            Assert.Equal(3, queryNodes.Count);
            Assert.Contains(node1.Id, queryNodesIds);
            Assert.Contains(node2.Id, queryNodesIds);
            Assert.Contains(node3.Id, queryNodesIds);

            JObject find = new JObject(
                new JProperty("search", "burger")
            );
            List<INode> findNodes = await branch.FindNodesAsync(find);
            var findNodesIds = findNodes.Select(node => node.Id);
            Assert.Equal(2, findNodes.Count);
            Assert.Contains(node1.Id, findNodesIds);
            Assert.Contains(node2.Id, findNodesIds);
            
            await node1.DeleteAsync();
            await node2.DeleteAsync();
            await node3.DeleteAsync();
            await node4.DeleteAsync();
        }
    }
}