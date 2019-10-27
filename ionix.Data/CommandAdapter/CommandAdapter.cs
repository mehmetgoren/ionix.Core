namespace Ionix.Data
{
    using System;


    public sealed class CommandAdapter : CommandAdapterBase
    {
        private readonly Func<IEntityMetaDataProvider> providerMethod;

        public CommandAdapter(ICommandFactory factory, Func<IEntityMetaDataProvider> providerMethod)
            : base(factory)
        {
            if (null == providerMethod)
                throw new ArgumentNullException(nameof(providerMethod));

            this.providerMethod = providerMethod;
        }

        protected override IEntityMetaDataProvider CreateProvider()
        {
            return this.providerMethod();
        }
    }

    //EntityMetaDataProvider lar cache lediği için buna gerek yok.
    //public sealed class CommandAdapterCache : CommandAdapterBase
    //{
    //    private readonly ProxyEntityMetaDataProvider proxy;

    //    public CommandAdapterCache(ICommandFactory factory, Func<IEntityMetaDataProvider> providerMethod)
    //        : base(factory)
    //    {
    //        if (null == providerMethod)
    //            throw new ArgumentNullException(nameof(providerMethod));

    //        this.proxy = new ProxyEntityMetaDataProvider(providerMethod());
    //    }


    //    protected override IEntityMetaDataProvider CreateProvider()
    //    {
    //        return this.proxy;
    //    }



    //    //Strategy Pattern Fonksiyon işaretçileiryle sağlanıyor, polimorfizim ile değil.
    //    private static readonly Dictionary<Type, IEntityMetaData> cache = new Dictionary<Type, IEntityMetaData>();// Type lar performans için cache leniyor ve Strategy Pattern ile esneklik sağlanıyor.
    //    private static readonly object sync = new object();
    //    private sealed class ProxyEntityMetaDataProvider : IEntityMetaDataProvider
    //    {
    //        private IEntityMetaDataProvider Provider { get; }

    //        internal ProxyEntityMetaDataProvider(IEntityMetaDataProvider provider)
    //        {
    //            this.Provider = provider;
    //        }

    //        IEntityMetaData IEntityMetaDataProvider.CreateEntityMetaData(Type entityType)
    //        {
    //            IEntityMetaData metaData;
    //            lock (sync)
    //            {
    //                if (!cache.TryGetValue(entityType, out metaData))
    //                {
    //                    metaData = this.Provider.CreateEntityMetaData(entityType);
    //                    if (null == metaData)
    //                        throw new NullReferenceException("Func<Type, IEntityMetaData> providerMethod returns null");

    //                    foreach (PropertyMetaData p in metaData.Properties)
    //                    {
    //                        if (!p.Schema.IsLocked)
    //                            p.Schema.Lock();
    //                    }

    //                    cache.Add(entityType, metaData);
    //                }
    //            }

    //            return metaData;
    //        }
    //    }
    //}
}
