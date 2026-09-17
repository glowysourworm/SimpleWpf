using SimpleWpf.Native.IO;

namespace SimpleWpf.Native.Test
{
    public class FastDirectoryIOTests
    {
        const string TEST_FOLDER = "SimpleWpf_UnitTest_NativeIO";
        const string TEST_SUB_FOLDER = "TestSubFolder";
        const string TEST_FILE1 = "TestFile1.txt";
        const string TEST_FILE2 = "TestFile2.txt";
        const string TEST_FILE3 = "TestFile3.log";

        private string _rootDirectory;
        private string _testDirectory;
        private string _testSubFolder;
        private string _testFilePath1;
        private string _testFilePath2;
        private string _testFilePath3;

        [SetUp]
        public void Setup()
        {
            _rootDirectory = Environment.CurrentDirectory;
            _testDirectory = Path.Combine(Environment.CurrentDirectory, TEST_FOLDER);
            _testSubFolder = Path.Combine(_testDirectory, TEST_SUB_FOLDER);
            _testFilePath1 = Path.Combine(_testSubFolder, TEST_FILE1);
            _testFilePath2 = Path.Combine(_testSubFolder, TEST_FILE2);
            _testFilePath3 = Path.Combine(_testSubFolder, TEST_FILE3);

            Directory.CreateDirectory(_testDirectory);
            Directory.CreateDirectory(_testSubFolder);

            File.WriteAllText(_testFilePath1, "This is a test file for SimpleWpf.UnitTest project. This may be deleted.");
            File.WriteAllText(_testFilePath2, "This is a test file for SimpleWpf.UnitTest project. This may be deleted.");
            File.WriteAllText(_testFilePath3, "This is a test file for SimpleWpf.UnitTest project. This may be deleted.");
        }

        [Test]
        public void TopLevelDirectory()
        {
            using (var fileIO = new FastDirectoryIO(_testDirectory, SearchOption.TopDirectoryOnly, "*.txt"))
            {
                var result = fileIO.GetFiles();

                // Includes Directories / Files
                Assert.That(result.Count() == 1);

                // Directory Flag
                Assert.That(result.Count(x => x.Attributes.HasFlag(FileAttributes.Directory)) == 1);
                Assert.That(result.Count(x => x.IsDirectory) == 1);

                // FileName
                Assert.That(result.Count(x => x.FileName == _testSubFolder) == 0);

                // Path
                Assert.That(result.Count(x => x.FullPath == _testSubFolder) == 1);
            }
        }

        [Test]
        public void AllDirectories()
        {
            using (var fileIO = new FastDirectoryIO(_testDirectory, SearchOption.AllDirectories, "*.txt"))
            {
                var result = fileIO.GetFiles();

                // Includes Directories / Files
                Assert.That(result.Count() == 3);

                // Directory Flag
                Assert.That(result.Count(x => x.Attributes.HasFlag(FileAttributes.Directory)) == 1);
                Assert.That(result.Count(x => x.IsDirectory) == 1);

                // FileName
                Assert.That(result.Count(x => x.FileName == _testSubFolder) == 0);
                Assert.That(result.Count(x => x.FileName == TEST_FILE1) == 1);
                Assert.That(result.Count(x => x.FileName == TEST_FILE2) == 1);

                // Path
                Assert.That(result.Count(x => x.FullPath == _testSubFolder) == 1);
                Assert.That(result.Count(x => x.FullPath == _testFilePath1) == 1);
                Assert.That(result.Count(x => x.FullPath == _testFilePath2) == 1);
            }
        }

        [Test]
        public void MultipleFileTypes()
        {
            using (var fileIO = new FastDirectoryIO(_testDirectory, SearchOption.AllDirectories, "*.txt", "*.log"))
            {
                var result = fileIO.GetFiles();

                // Includes Directories / Files
                Assert.That(result.Count() == 4);

                // Directory Flag
                Assert.That(result.Count(x => x.Attributes.HasFlag(FileAttributes.Directory)) == 1);
                Assert.That(result.Count(x => x.IsDirectory) == 1);

                // FileName
                Assert.That(result.Count(x => x.FileName == _testSubFolder) == 0);
                Assert.That(result.Count(x => x.FileName == TEST_FILE1) == 1);
                Assert.That(result.Count(x => x.FileName == TEST_FILE2) == 1);
                Assert.That(result.Count(x => x.FileName == TEST_FILE3) == 1);

                // Path
                Assert.That(result.Count(x => x.FullPath == _testSubFolder) == 1);
                Assert.That(result.Count(x => x.FullPath == _testFilePath1) == 1);
                Assert.That(result.Count(x => x.FullPath == _testFilePath2) == 1);
                Assert.That(result.Count(x => x.FullPath == _testFilePath3) == 1);
            }
        }

        [TearDown]
        public void Teardown()
        {
            if (Directory.Exists(_testDirectory))
                Directory.Delete(_testDirectory, true);
        }
    }
}
