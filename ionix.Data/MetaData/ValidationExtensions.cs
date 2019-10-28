namespace Ionix.Data
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;
    using Utils.Extensions;


    public static class ValidationExtensions
    {
        public static bool IsModelValid<TEntity>(this TEntity entity)
        {
            bool ret = null != entity;
            if (ret)
            {
                foreach (PropertyInfo pi in typeof(TEntity).GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    object value = pi.GetValue(entity);

                    foreach (var item in pi.GetCustomAttributes())
                    {
                        ValidationAttribute validation = item as ValidationAttribute;
                        if (null != validation)
                        {
                            if (!validation.IsValid(value))
                            {
                                ret = false;
                                goto Endfunc;
                            }
                        } 
                    }
                }
            }

            Endfunc:
            return ret;
        }


        public static bool IsModelListValid<TEntity>(this IEnumerable<TEntity> entityList)
        {
            bool ret = !entityList.IsNullOrEmpty();
            if (ret)
            {
                foreach (var entity in entityList)
                {
                    ret = IsModelValid(entity);
                    if (!ret) return false;
                }
            }
            return ret;
        }
    }
}
