using Xunit;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using CloudCMS;

namespace CloudCMS.Tests
{
    public class NodeTest : AbstractWithRepositoryTest
    {

        public NodeTest(RepositoryFixture fixture) : base(fixture)
        {

        }

        private async Task<INode> createFile(IBranch branch, INode parent, string filename, bool isFolder)
        {
            JObject nodeObj = new JObject(new JProperty("title", filename));
            INode node = (INode) await branch.CreateNodeAsync(nodeObj);
            JObject fileObj = new JObject();
            fileObj.Add("filename", filename);
            await node.AddFeatureAsync("f:filename", fileObj);
            if (isFolder)
            {
                await node.AddFeatureAsync("f:container", new JObject());
            }

            await parent.AssociateAsync(node, QName.create("a:child"), Directionality.DIRECTED);

            return node;
        }

        [Fact]
        public async void TestNodeCrud()
        {
            IBranch branch = await Fixture.Repository.ReadBranchAsync("master");

            JObject nodeObj = new JObject(
                new JProperty("title", "MyNode")
            );
            IBaseNode node = await branch.CreateNodeAsync(nodeObj);
            string expectedRef = "node://" + node.PlatformId + "/" + node.RepositoryId + "/" + node.BranchId + "/" + node.Id;
            Assert.Equal(expectedRef, node.Ref.Ref);
            
            Assert.NotNull(node.QName);
            Assert.Equal("n:node", node.TypeQName.ToString());

            IBaseNode nodeRead = await branch.ReadNodeAsync(node.Id);
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
        public async void TestNodeQuerySearchFind()
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

            IBaseNode node1 = await branch.CreateNodeAsync(nodeObj1);
            IBaseNode node2 = await branch.CreateNodeAsync(nodeObj2);
            IBaseNode node3 = await branch.CreateNodeAsync(nodeObj3);
            IBaseNode node4 = await branch.CreateNodeAsync(nodeObj4);

            // Wait for nodes to index
            Thread.Sleep(5000);

            JObject query = new JObject(
                new JProperty("meal", "lunch")
            );
            List<IBaseNode> queryNodes = await branch.QueryNodesAsync(query);
            var queryNodesIds = queryNodes.Select(node => node.Id);
            Assert.Equal(3, queryNodes.Count);
            Assert.Contains(node1.Id, queryNodesIds);
            Assert.Contains(node2.Id, queryNodesIds);
            Assert.Contains(node3.Id, queryNodesIds);

            JObject find = new JObject(
                new JProperty("search", "burger")
            );
            List<IBaseNode> findNodes = await branch.FindNodesAsync(find);
            var findNodesIds = findNodes.Select(node => node.Id);
            Assert.Equal(2, findNodes.Count);
            Assert.Contains(node1.Id, findNodesIds);
            Assert.Contains(node2.Id, findNodesIds);
            
            // search
            List<IBaseNode> searchNodes = await branch.SearchNodesAsync("burger");
            var searchNodesIds = searchNodes.Select(node => node.Id);
            Assert.Equal(2, searchNodes.Count);
            Assert.Contains(node1.Id, searchNodesIds);
            Assert.Contains(node2.Id, searchNodesIds);

            await node1.DeleteAsync();
            await node2.DeleteAsync();
            await node3.DeleteAsync();
            await node4.DeleteAsync();
        }

        [Fact]
        public async void TestFeatures()
        {
            IBranch branch = await Fixture.Repository.ReadBranchAsync("master");

            IBaseNode node = await branch.CreateNodeAsync(new JObject());
            List<string> featureIds = node.GetFeatureIds();
            Assert.NotEmpty(featureIds);
            
            JObject filenameObj = new JObject(
                new JProperty("filename", "file1")
                );
            await node.AddFeatureAsync("f:filename", filenameObj);

            featureIds = node.GetFeatureIds();
            Assert.Contains("f:filename", featureIds);
            Assert.True(node.HasFeature("f:filename"));
            JObject featureObj = node.GetFeature("f:filename");
            Assert.Equal("file1", featureObj.GetValue("filename"));

            await node.RemoveFeatureAsync("f:filename");
            featureIds = node.GetFeatureIds();
            Assert.DoesNotContain("f:filename", featureIds);
            Assert.False(node.HasFeature("f:filename"));
            Assert.Null(node.GetFeature("f:filename"));
        }

        [Fact]
        public async void TestTraverse()
        {
            /*
             * folder1
             * file1
             * folder1/folder2
             * folder1/file2
             * folder1/file4
             * folder1/folder2/file3
             * folder1/folder2/file5
             */
            IBranch branch = await Fixture.Repository.ReadBranchAsync("master");
            INode rootNode = await branch.RootNodeAsync();

            INode folder1 = await createFile(branch, rootNode, "folder1", true);
            INode file1 = await createFile(branch, rootNode, "file1", false);
            INode folder2 = await createFile(branch, folder1, "folder2", true);
            INode file2 = await createFile(branch, folder1, "file2", false);
            INode file3 = await createFile(branch, folder2, "file3", false);
            INode file4 = await createFile(branch, folder1, "file4", false);
            INode file5 = await createFile(branch, folder2, "file5", false);

            JObject traverse = new JObject(
                new JProperty("depth", 1),
                new JProperty("filter", "ALL_BUT_START_NODE"),
                new JProperty("associations", new JObject(
                    new JProperty("a:child", "ANY")
                )));
            Thread.Sleep(5000);

            TraversalResults results = await rootNode.TraverseAsync(traverse);
            
            Assert.Equal(2, results.Nodes.Count);
            Assert.Equal(2, results.Associations.Count);
            
            JObject paths = await file5.ResolvePathsAsync();
            Assert.True(paths.Count > 0);
            
            // test path resolves 
            string path = await file5.ResolvePathAsync();
            Assert.Equal("/folder1/folder2/file5", path);

            
        }

        [Fact]
        public async void TestTranslations()
        {
            IBranch branch = await Fixture.Repository.ReadBranchAsync("master");
            INode rootNode = await branch.RootNodeAsync();

            INode node = await createFile(branch, rootNode, "theNode", false);
            
            JObject germanObj1 = new JObject(new JProperty("title", "german1"));
            JObject spanishObj1 = new JObject(new JProperty("title", "spanish1"));
            JObject spanishObj2 = new JObject(new JProperty("title", "spanish2"));

            INode german1 = await node.CreateTranslationAsync("de_DE", "1.0", germanObj1);
            Assert.NotNull(german1);
            INode spanish1 = await node.CreateTranslationAsync("es_MX", "1.0", spanishObj1);
            INode spanish2 = await node.CreateTranslationAsync("es_MX", "2.0", spanishObj2);

            List<string> editions = await node.GetTranslationEditionsAsync();
            Assert.Equal(2, editions.Count);

            List<string> locales = await node.GetTranslationLocales("1.0");
            Assert.Equal(2, locales.Count);

            INode translation = await node.ReadTranslationAsync("es_MX", "1.0");
            Assert.Equal("spanish1", translation.GetString("title"));
        }

        [Fact]
        public async void TestChangeQName()
        {
            IBranch master = await Fixture.Repository.MasterAsync();
            JObject nodeObj = new JObject(
                new JProperty("_type", "n:node"),
                new JProperty("title", "Test Node")
            );

            IBaseNode node = await master.CreateNodeAsync(nodeObj);
            await node.ChangeQNameAsync(QName.create("o:blah"));
            await node.ReloadAsync();

            QName newQName = node.QName;
            Assert.Equal("o:blah", newQName.ToString());
        }

        [Fact]
        public async void TestVersions()
        {
            IBranch master = await Fixture.Repository.MasterAsync();
            JObject nodeObj = new JObject(
                new JProperty("title", "Test Node")
            );
            
            IBaseNode node = await master.CreateNodeAsync(nodeObj);
            string firstChangeset = node.Data.SelectToken("_system.changeset").ToString();
            node.Data["title"] = "new stuff";
            await node.UpdateAsync();
            await node.ReloadAsync();
            Assert.Equal("new stuff", node.Data.GetValue("title").ToString());

            List<IBaseNode> versions = await node.ListVersionsAsync();
            Assert.Equal(2, versions.Count);

            IBaseNode firstVersion = await node.ReadVersionAsync(firstChangeset);
            Assert.Equal("Test Node", firstVersion.Data["title"].ToString());

            IBaseNode restoredVersion = await node.RestoreVersionAsync(firstChangeset);
            Assert.Equal("Test Node", restoredVersion.Data["title"].ToString());

        }
        
        
    }
}