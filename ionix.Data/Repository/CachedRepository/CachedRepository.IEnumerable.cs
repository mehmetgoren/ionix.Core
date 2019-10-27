namespace Ionix.Data
{
    using System.Collections;
    using System.Collections.Generic;

    partial class CachedRepository<TEntity> : IEnumerable<TEntity>
    {
        public IEnumerator<TEntity> GetEnumerator()
        {
            return this.List.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public int Count => this.List.Count;
    }
}
