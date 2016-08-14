using System.Collections.Generic;

namespace WPFGrep.Utilities
{
    public class ThreadSafeList<T> : List<T>
    {
        private readonly object _lock = new object();

        public ThreadSafeList()
        {
        }

        public ThreadSafeList(int capacity) : base(capacity)
        {
        }

        public ThreadSafeList(IEnumerable<T> collection) : base(collection)
        {
        }

        public new void Add(T item)
        {
            lock (_lock)
            {
                base.Add(item);
            }
        }

        public new bool Remove(T item)
        {
            lock (_lock)
            {
                return base.Remove(item);
            }
        }
    }
}