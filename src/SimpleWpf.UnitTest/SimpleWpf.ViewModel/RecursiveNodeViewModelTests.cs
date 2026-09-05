using SimpleWpf.UI.ViewModel.FileTreeView;

namespace SimpleWpf.UnitTest.SimpleWpf.ViewModel
{
    public class RecursiveNodeViewModelTests
    {
        const string TEST_FOLDER = "SimpleWpf_UnitTest_ViewModel";
        const string TEST_FILE = "TestFile.txt";

        private string _rootDirectory;
        private string _testDirectory;
        private string _testFilePath;

        [SetUp]
        public void Setup()
        {
            _rootDirectory = Environment.CurrentDirectory;
            _testDirectory = Path.Combine(Environment.CurrentDirectory, TEST_FOLDER);
            _testFilePath = Path.Combine(_testDirectory, TEST_FILE);

            Directory.CreateDirectory(_testDirectory);
            File.WriteAllText(_testFilePath, "This is a test file for SimpleWpf.UnitTest project. This may be deleted.");
        }

        [Test]
        public void InstantiatePathNode()
        {
            // Test Folder
            var root = new FileTreeNodeViewModel(Environment.CurrentDirectory, _rootDirectory, 0);

            // -> Root
            var rootNode = new FileTreeViewModel("*.txt", root);

            // -> Root -> Test (Dir)
            var testDirectoryNode = rootNode.Add(new FileTreeNodeViewModel(_rootDirectory, _testDirectory, 0)) as FileTreeViewModel;

            // -> Root -> Test (Dir) -> Test (File)
            var fileNode = testDirectoryNode.Add(new FileTreeNodeViewModel(_rootDirectory, _testFilePath, 0)) as FileTreeViewModel;

            Assert.That(rootNode.GetNodeValue().FullPath == _rootDirectory);
            Assert.That(testDirectoryNode.GetNodeValue().FullPath == _testDirectory);
            Assert.That(fileNode.GetNodeValue().FullPath == _testFilePath);

            Assert.That(rootNode.GetNodeValue().BaseDirectory == _rootDirectory);
            Assert.That(testDirectoryNode.GetNodeValue().BaseDirectory == _rootDirectory);
            Assert.That(fileNode.GetNodeValue().BaseDirectory == _rootDirectory);

            Assert.That(rootNode.GetNodeValue().IsDirectory);
            Assert.That(testDirectoryNode.GetNodeValue().IsDirectory);
            Assert.That(fileNode.GetNodeValue().IsDirectory);
        }

        [Test]
        public void RecursiveIteration()
        {
            // Test Folder
            var root = new FileTreeNodeViewModel(Environment.CurrentDirectory, _rootDirectory, 0);

            // -> Root
            var rootNode = new FileTreeViewModel("*.txt", root);
            var testNodeValue = new FileTreeNodeViewModel(_rootDirectory, _testDirectory, 0);
            var testFileNodeValue = new FileTreeNodeViewModel(_rootDirectory, _testFilePath, 0);

            // -> Root -> Test (Dir)
            var testDirectoryNode = rootNode.Add(testNodeValue);

            // -> Root -> Test (Dir) -> Test (File)
            var fileNode = testDirectoryNode.Add(testFileNodeValue);

            var rootFound = false;
            var testFound = false;
            var fileFound = false;

            rootNode.RecurseForEach(node =>
            {
                if (node.NodeValue == root)
                    rootFound = true;

                else if (node.NodeValue == testNodeValue)
                    testFound = true;

                else if (node.NodeValue == testFileNodeValue)
                    fileFound = true;

                else
                    Assert.Fail("Missing node from recursive iterator");
            });

            Assert.That(rootFound);
            Assert.That(testFound);
            Assert.That(fileFound);
        }

        [Test]
        public void RecursiveNodeEvents()
        {
            // Test Folder
            var root = new FileTreeNodeViewModel(Environment.CurrentDirectory, _rootDirectory, 0);

            // -> Root
            var rootNode = new FileTreeViewModel("*.txt", root);
            var testNodeValue = new FileTreeNodeViewModel(_rootDirectory, _testDirectory, 0);
            var testFileNodeValue = new FileTreeNodeViewModel(_rootDirectory, _testFilePath, 0);

            // -> Root -> Test (Dir)
            var testDirectoryNode = rootNode.Add(testNodeValue);

            // -> Root -> Test (Dir) -> Test (File)
            var fileNode = testDirectoryNode.Add(testFileNodeValue);

            var eventFired = false;

            rootNode.ItemPropertyChanged += (sender, e) =>
            {
                eventFired = true;
            };

            // -> Root Event Fired
            rootNode.NodeValue.IsSelected = true;

            Assert.That(eventFired);

            eventFired = false;

            // -> Test Event Fired
            testDirectoryNode.NodeValue.IsSelected = true;

            Assert.That(eventFired);

            eventFired = false;

            // -> Leaf Event Fired
            fileNode.NodeValue.IsSelected = true;

            Assert.That(eventFired);
        }

        [TearDown]
        public void Teardown()
        {
            if (Directory.Exists(_testDirectory))
                Directory.Delete(_testDirectory, true);
        }
    }
}