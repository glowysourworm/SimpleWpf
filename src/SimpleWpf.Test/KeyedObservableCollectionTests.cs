using System.ComponentModel;

using SimpleWpf.Extensions.ObservableCollection;

namespace SimpleWpf.Test
{
    public class KeyedObservableCollectionTests
    {
        public class TestClass : INotifyPropertyChanged
        {
            string _name;

            public event PropertyChangedEventHandler? PropertyChanged;

            public string Name
            {
                get { return _name; }
                set
                {
                    _name = value;

                    if (this.PropertyChanged != null)
                        this.PropertyChanged(this, new PropertyChangedEventArgs("Name"));
                }
            }

            public TestClass(string name)
            {
                _name = name;
            }

            public override int GetHashCode()
            {
                return _name.GetHashCode();
            }
        }

        public class TestClassComparer : Comparer<TestClass>
        {
            public TestClassComparer()
            {
            }

            public override int Compare(TestClass? object1, TestClass? object2)
            {
                return object1.GetHashCode().CompareTo(object2.GetHashCode());
            }
        }


        KeyedObservableCollection<int, TestClass> _collection;
        KeyedObservableCollection<TestClass, TestClass> _hashCollection;

        [SetUp]
        public void Setup()
        {
            _collection = new KeyedObservableCollection<int, TestClass>();
            _hashCollection = new KeyedObservableCollection<TestClass, TestClass>();

            var item1 = new TestClass("Item1");
            var item2 = new TestClass("Item2");
            var item3 = new TestClass("Item3");
            var item4 = new TestClass("Item4");

            _collection.Add(1, item1);
            _collection.Add(2, item2);
            _collection.Add(3, item3);
            _collection.Add(4, item4);

            _hashCollection.Add(item1, item1);
            _hashCollection.Add(item2, item2);
            _hashCollection.Add(item3, item3);
            _hashCollection.Add(item4, item4);
        }

        [Test]
        public void AddItem()
        {
            var item5 = new TestClass("Item5");

            _collection.Add(5, item5);

            Assert.That(_collection.Count == 5);
            Assert.That(_collection[5].Name == item5.Name);

            _hashCollection.Add(item5, item5);

            Assert.That(_hashCollection.Count == 5);
            Assert.That(_hashCollection[item5].Name == item5.Name);
        }

        [Test]
        public void RemoveItem()
        {
            var item4 = _collection[4];
            var item3 = _collection[3];
            var item1 = _collection[1];

            _collection.Remove(4);

            Assert.That(_collection.Count == 3);
            Assert.That(!_collection.ContainsKey(4));

            _collection.Remove(2);

            Assert.That(_collection.Count == 2);
            Assert.That(!_collection.ContainsKey(2));

            _collection.Remove(1);

            Assert.That(_collection.Count == 1);
            Assert.That(!_collection.ContainsKey(1));

            _hashCollection.Remove(item4);

            Assert.That(_hashCollection.Count == 3);
            Assert.That(!_hashCollection.ContainsKey(item4));

            _hashCollection.Remove(item3);

            Assert.That(_hashCollection.Count == 2);
            Assert.That(!_hashCollection.ContainsKey(item3));

            _hashCollection.Remove(item1);

            Assert.That(_hashCollection.Count == 1);
            Assert.That(!_hashCollection.ContainsKey(item1));
        }

        [Test]
        public void RemoveItemsWithPredicate()
        {
            var item1 = _collection[1];

            _collection.Remove(x => x.Name == "Item1");

            Assert.That(_collection.Count == 3);
            Assert.That(!_collection.ContainsKey(1));

            _hashCollection.Remove(x => x.Name == "Item1");

            Assert.That(_hashCollection.Count == 3);
            Assert.That(!_hashCollection.ContainsKey(item1));
        }
    }
}
