using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SimpleWpf.NativeIO;
using SimpleWpf.ViewModel;

namespace SimpleWpf.UnitTest.SimpleWpf.NativeIO
{
    public class FastDirectoryIOTests
    {
        const string TEST_FOLDER = "SimpleWpf_UnitTest_NativeIO";
        const string TEST_SUB_FOLDER = "TestSubFolder";
        const string TEST_FILE1 = "TestFile1.txt";
        const string TEST_FILE2 = "TestFile2.txt";

        private string _rootDirectory;
        private string _testDirectory;
        private string _testSubFolder;
        private string _testFilePath1;
        private string _testFilePath2;

        [SetUp]
        public void Setup()
        {
            _rootDirectory = Environment.CurrentDirectory;
            _testDirectory = Path.Combine(Environment.CurrentDirectory, TEST_FOLDER);
            _testSubFolder = Path.Combine(_testDirectory, TEST_SUB_FOLDER);
            _testFilePath1 = Path.Combine(_testSubFolder, TEST_FILE1);
            _testFilePath2 = Path.Combine(_testSubFolder, TEST_FILE2);

            Directory.CreateDirectory(_testDirectory);
            Directory.CreateDirectory(_testSubFolder);

            File.WriteAllText(_testFilePath1, "This is a test file for SimpleWpf.UnitTest project. This may be deleted.");
            File.WriteAllText(_testFilePath2, "This is a test file for SimpleWpf.UnitTest project. This may be deleted.");
        }

        [Test]
        public void TopLevelDirectory()
        {
            using (var fileIO = new FastDirectoryIO(_testDirectory, "*.txt", SearchOption.TopDirectoryOnly))
            {
                var result = fileIO.GetFiles();

                // Includes Directories / Files
                Assert.IsTrue(result.Count() == 1);

                // Directory Flag
                Assert.IsTrue(result.Count(x => x.Attributes.HasFlag(FileAttributes.Directory)) == 1);
                Assert.IsTrue(result.Count(x => x.IsDirectory) == 1);

                // FileName
                Assert.IsTrue(result.Count(x => x.FileName == _testSubFolder) == 0);

                // Path
                Assert.IsTrue(result.Count(x => x.Path == _testSubFolder) == 1);
            }
        }

        [Test]
        public void AllDirectories()
        {
            using (var fileIO = new FastDirectoryIO(_testDirectory, "*.txt", SearchOption.AllDirectories))
            {
                var result = fileIO.GetFiles();

                // Includes Directories / Files
                Assert.IsTrue(result.Count() == 3);

                // Directory Flag
                Assert.IsTrue(result.Count(x => x.Attributes.HasFlag(FileAttributes.Directory)) == 1);
                Assert.IsTrue(result.Count(x => x.IsDirectory) == 1);

                // FileName
                Assert.IsTrue(result.Count(x => x.FileName == _testSubFolder) == 0);
                Assert.IsTrue(result.Count(x => x.FileName == TEST_FILE1) == 1);
                Assert.IsTrue(result.Count(x => x.FileName == TEST_FILE2) == 1);
                
                // Path
                Assert.IsTrue(result.Count(x => x.Path == _testSubFolder) == 1);
                Assert.IsTrue(result.Count(x => x.Path == _testFilePath1) == 1);
                Assert.IsTrue(result.Count(x => x.Path == _testFilePath2) == 1);
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
