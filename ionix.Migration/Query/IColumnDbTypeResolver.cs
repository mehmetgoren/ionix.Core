namespace ionix.Migration
{
    using Data;

    public interface IColumnDbTypeResolver
    {
        Column GetColumn(PropertyMetaData metaData);
    }
}
