using System.ComponentModel;

using SimpleWpf.Extensions.ObservableCollection;

namespace SimpleWpf.Test
{
    public class Tests
    {
        class TestClass : INotifyPropertyChanged
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
        }


        KeyedObservableCollection<int, TestClass> _collection;

        [SetUp]
        public void Setup()
        {
            _collection = new KeyedObservableCollection<int, TestClass>();

            _collection.Add(1, new TestClass("Item1"));
            _collection.Add(2, new TestClass("Item2"));
            _collection.Add(3, new TestClass("Item3"));
            _collection.Add(4, new TestClass("Item4"));
        }

        [Test]
        public void AddItem()
        {
            _collection.Add(5, new TestClass("Item5"));

            Assert.That(_collection.Count == 5);
            Assert.That(_collection[5].Name == "Item5");
        }

        [Test]
        public void RemoveItem()
        {
            _collection.Remove(4);

            Assert.That(_collection.Count == 3);
            Assert.That(!_collection.ContainsKey(4));

            _collection.Remove(2);

            Assert.That(_collection.Count == 2);
            Assert.That(!_collection.ContainsKey(2));
        }
    }
}
