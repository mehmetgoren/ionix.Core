namespace Ionix.Data
{
    using System;

    public enum StoreGeneratedPattern : int //DatabaseGeneratedOption
    {
        None = 0,//Guid, Manuel Sequence Value. (insert list de olacak)
        Identity = 1,//Identity Column. (insert list de olmayacak). Kısıt olarak IEntityMetaData.Properties de mutlaka tekil olmalı.
        Computed = 2,//Column with Default Value(i.e getdate(), deleted 0,), Guid as DefaultValue, Next Sequence Value as Default Value.
        AutoGenerateSequence = 3//Classical Oracle Sequence and returning next value like identity. Kısıt olarak IEntityMetaData.Properties de mutlaka tekil olmalı.
    }


    public enum SqlValueType : int
    {
        Parameterized = 0,
        Text
    }

}
