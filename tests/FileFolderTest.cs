using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xunit;

namespace CloudCMS.Tests
{
    public class FileFolderTest : AbstractWithRepositoryTest
    {
        public FileFolderTest(RepositoryFixture fixture) : base(fixture)
        {
        }
        
        private async Task<INode> createFile(IBranch branch, INode parent, string filename, bool isFolder)
        {
            INode node = (INode) await branch.CreateNodeAsync();
            JObject fileObj = new JObject();
            fileObj.Add("filename", filename);
            await node.AddFeatureAsync("f:filename", fileObj);
            if (isFolder)
            {
                await node.AddFeatureAsync("f:container", new JObject());
            }

            node.SetString("title", filename);
            await node.UpdateAsync();

            await parent.AssociateAsync(node, QName.create("a:child"), Directionality.DIRECTED);

            return node;
        }
        
        [Fact]
        public async void TestFileFolder()
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

            JObject tree = await rootNode.FileFolderTreeAsync();
            JArray children = (JArray) tree.GetValue("children");
            Assert.Equal(2, children.Count);

            JObject child = (JObject) children[0];

            Assert.NotNull(child.GetValue("filename"));
            Assert.NotNull(child.GetValue("label"));
            Assert.NotNull(child.GetValue("path"));
            Assert.NotNull(child.GetValue("typeQName"));
            Assert.NotNull(child.GetValue("qname"));

            List<IBaseNode> folder2Children = await folder2.ListChildrenAsync();
            Assert.Equal(2, folder2Children.Count);

            List<IBaseNode> folder2Relatives = await folder2.ListRelativesAsync(QName.create("a:child"), Direction.ANY);
            Assert.Equal(3, folder2Relatives.Count);
            
            JObject query = new JObject(new JProperty("title", "file3"));
            List<IBaseNode> folder2RelativesQueried = await folder2.QueryRelativesAsync(QName.create("a:child"), Direction.ANY, query);
            Assert.Single(folder2RelativesQueried);
        }
    }
}