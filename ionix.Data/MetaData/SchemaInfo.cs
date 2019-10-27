namespace Ionix.Data
{
    using Utils;
    using System;

    public sealed class SchemaInfo : IEquatable<SchemaInfo>, ILockable, IPrototype<SchemaInfo>
    {
        public SchemaInfo(string columnName, Type dataType, bool isNullable)
        {
            if (String.IsNullOrEmpty(columnName))
                throw new ArgumentNullException(nameof(columnName));
            if (null == dataType)
                throw new ArgumentNullException(nameof(dataType));

            this.columnName = new Locked<string>(columnName);
            this.dataType = new Locked<Type>(dataType);
            this.isNullable = new Locked<bool>(isNullable);

            this.maxLength = new Locked<int>(-1);
        }
        public SchemaInfo(string columnName, Type dataType)
            : this(columnName, dataType, true)
        {

        }
        public SchemaInfo(string columnName)
            : this(columnName, CachedTypes.String)
        {

        }

        private Locked<string> columnName;
        public string ColumnName
        {
            get
            {
                if (null == this.columnName.Value)
                    return String.Empty;
                return this.columnName.Value;
            }
            set => this.columnName.Value = value;
        }

        private Locked<Type> dataType;
        public Type DataType
        {
            get => this.dataType.Value;
            set => this.dataType.Value = value;
        }

        private Locked<bool> isNullable;
        public bool IsNullable
        {
            get => this.isNullable.Value;
            set => this.isNullable.Value = value;
        }

        private Locked<bool> isKey;
        public bool IsKey
        {
            get => this.isKey.Value;
            set => this.isKey.Value = value;
        }

        private Locked<bool> readOnly;
        public bool ReadOnly
        {
            get => this.readOnly.Value;
            set => this.readOnly.Value = value;
        }

        private Locked<StoreGeneratedPattern> databaseGeneratedOption;
        public StoreGeneratedPattern DatabaseGeneratedOption
        {
            get => this.databaseGeneratedOption.Value;
            set => this.databaseGeneratedOption.Value = value;
        }

        //i.e. getdate(), deleted(0), sequence.nextVal. But nopt db computed giving by client. and Non-parameter field
        private Locked<string> defaultValue;
        public string DefaultValue
        {
            get
            {
                if (null == this.defaultValue.Value)
                    return String.Empty;
                return this.defaultValue.Value;
            }
            set => this.defaultValue.Value = value;
        }


        private Locked<int> maxLength;
        public int MaxLength
        {
            get => this.maxLength.Value;
            set => this.maxLength.Value = value;
        }

        private Locked<int> order;
        public int Order
        {
            get => this.order.Value;
            internal set => this.order.Value = value;
        }



        private Locked<SqlValueType> sqlValueType;
        public SqlValueType SqlValueType
        {
            get => this.sqlValueType.Value;
            set => this.sqlValueType.Value = value;
        }


        #region equals
        public bool Equals(SchemaInfo other)
        {
            if (null != other)
                return this.ColumnName.Equals(other.ColumnName);
            return false;
        }
        public override bool Equals(object obj)
        {
            return this.Equals(obj as SchemaInfo);
        }
        public override int GetHashCode()
        {
            return this.ColumnName.GetHashCode();
        }
        public override string ToString()
        {
            return this.ColumnName;
        }
        #endregion

        public void Lock()
        {
            this.columnName.Lock();
            this.dataType.Lock();
            this.isNullable.Lock();

            this.isKey.Lock();
            this.readOnly.Lock();

            this.databaseGeneratedOption.Lock();
            this.defaultValue.Lock();

            this.maxLength.Lock();
            this.order.Lock();

            this.sqlValueType.Lock();

            this.isLocked = true;
        }
        public void Unlock()
        {
            this.columnName.Unlock();
            this.dataType.Unlock();
            this.isNullable.Unlock();

            this.isKey.Unlock();
            this.readOnly.Unlock();

            this.databaseGeneratedOption.Unlock();
            this.defaultValue.Unlock();

            this.maxLength.Unlock();
            this.order.Unlock();

            //table unlock edilmeyecek. Tek seferde set edilecek. SchemaInfoCollection tarafından. Zaten Internal.

            this.sqlValueType.Unlock();

            this.isLocked = false;
        }
        private bool isLocked;
        public bool IsLocked => this.isLocked;


        public SchemaInfo Copy()
        {
            SchemaInfo copy = new SchemaInfo(this.ColumnName, this.DataType, this.IsNullable);
            copy.IsKey = this.IsKey;
            copy.ReadOnly = this.ReadOnly;

            copy.DatabaseGeneratedOption = this.DatabaseGeneratedOption;
            copy.DefaultValue = this.DefaultValue;

            copy.MaxLength = this.MaxLength;
            copy.Order = this.Order;
            copy.SqlValueType = this.SqlValueType;

            return copy;
        }
    }
}
