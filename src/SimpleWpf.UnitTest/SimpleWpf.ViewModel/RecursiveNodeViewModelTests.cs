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

            Assert.IsTrue(rootNode.NodeValue.Path == _rootDirectory);
            Assert.IsTrue(testDirectoryNode.NodeValue.Path == _testDirectory);
            Assert.IsTrue(fileNode.NodeValue.Path == _testFilePath);

            Assert.IsTrue(rootNode.NodeValue.BaseDirectory == _rootDirectory);
            Assert.IsTrue(testDirectoryNode.NodeValue.BaseDirectory == _rootDirectory);
            Assert.IsTrue(fileNode.NodeValue.BaseDirectory == _rootDirectory);

            Assert.IsTrue(rootNode.NodeValue.IsDirectory);
            Assert.IsTrue(testDirectoryNode.NodeValue.IsDirectory);
            Assert.IsFalse(fileNode.NodeValue.IsDirectory);
        }

        [TearDown]
        public void Teardown()
        {
            if (Directory.Exists(_testDirectory))
                Directory.Delete(_testDirectory, true);
        }
    }
}