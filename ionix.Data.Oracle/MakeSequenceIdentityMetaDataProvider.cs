namespace Ionix.Data.Oracle
{
    using System;

    //Oracle da NonIdentity ve Otomatik Sequence olan Alanlar İçin.
    //Proxy Pattern
    public sealed class MakeSequenceIdentityMetaDataProvider : IEntityMetaDataProvider
    {
        private readonly IEntityMetaDataProvider concrete;
        public MakeSequenceIdentityMetaDataProvider(IEntityMetaDataProvider concrete)
        {
            if (null == concrete)
                throw new ArgumentNullException(nameof(concrete));

            this.concrete = concrete;
        }

        internal static void MakeKeyIdentity(IEntityMetaData metaData)
        {
            PropertyMetaData primaryKey = metaData.GetPrimaryKey();
            if (null != primaryKey)//has no or multiple primary key found means no primary key.
            {
                SchemaInfo schema = primaryKey.Schema;
                StoreGeneratedPattern pattern = schema.DatabaseGeneratedOption;
                if (pattern != StoreGeneratedPattern.AutoGenerateSequence)
                {
                    bool isLocked = schema.IsLocked;
                    if (isLocked)
                        schema.Unlock();
                    schema.DatabaseGeneratedOption = StoreGeneratedPattern.AutoGenerateSequence;
                    if (isLocked)
                        schema.Lock();
                }
            }
        }

        public IEntityMetaData CreateEntityMetaData(Type entityType)
        {
            IEntityMetaData metaData = this.concrete.CreateEntityMetaData(entityType);

            MakeKeyIdentity(metaData);

            return metaData;
        }
    }
}
