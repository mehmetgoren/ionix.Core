namespace Ionix.Utils.Collections
{
    using System.Collections.Generic;

    public sealed class SingleItemList<T> : IEnumerable<T>
    {
        private readonly T item;

        public SingleItemList(T item)
        {
            this.item = item;
        }


        public IEnumerator<T> GetEnumerator()
        {
            yield return this.item;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
