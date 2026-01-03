using SimpleWpf.ViewModel;

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
            var root = new PathViewModel(Environment.CurrentDirectory, _rootDirectory);

            // -> Root
            var rootNode = new PathNodeViewModel("*.txt", root);

            // -> Root -> Test (Dir)
            var testDirectoryNode = rootNode.Add(new PathViewModel(_rootDirectory, _testDirectory));

            // -> Root -> Test (Dir) -> Test (File)
            var fileNode = testDirectoryNode.Add(new PathViewModel(_rootDirectory, _testFilePath));

            Assert.That(rootNode.NodeValue.FullPath == _rootDirectory);
            Assert.That(testDirectoryNode.NodeValue.FullPath == _testDirectory);
            Assert.That(fileNode.NodeValue.FullPath == _testFilePath);

            Assert.That(rootNode.NodeValue.BaseDirectory == _rootDirectory);
            Assert.That(testDirectoryNode.NodeValue.BaseDirectory == _rootDirectory);
            Assert.That(fileNode.NodeValue.BaseDirectory == _rootDirectory);

            Assert.That(rootNode.NodeValue.IsDirectory);
            Assert.That(testDirectoryNode.NodeValue.IsDirectory);
            Assert.That(fileNode.NodeValue.IsDirectory);
        }

        [Test]
        public void RecursiveIteration()
        {
            // Test Folder
            var root = new PathViewModel(Environment.CurrentDirectory, _rootDirectory);

            // -> Root
            var rootNode = new PathNodeViewModel("*.txt", root);
            var testNodeValue = new PathViewModel(_rootDirectory, _testDirectory);
            var testFileNodeValue = new PathViewModel(_rootDirectory, _testFilePath);

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
            var root = new PathViewModel(Environment.CurrentDirectory, _rootDirectory);

            // -> Root
            var rootNode = new PathNodeViewModel("*.txt", root);
            var testNodeValue = new PathViewModel(_rootDirectory, _testDirectory);
            var testFileNodeValue = new PathViewModel(_rootDirectory, _testFilePath);

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