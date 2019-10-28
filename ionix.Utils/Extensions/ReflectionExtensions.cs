namespace Ionix.Utils.Extensions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Extensions;

    public static class ReflectionExtensions
    {
        private static readonly MethodInfo ToNullable_MethodInfo = typeof(ConversionExtensions).GetTypeInfo().GetMethod(nameof(ConversionExtensions.ToNullable), BindingFlags.Static | BindingFlags.Public);

        private static readonly MethodInfo ToNullableSafely_MethodInfo = typeof(ConversionExtensions).GetTypeInfo().GetMethod(nameof(ConversionExtensions.ToNullableSafely), BindingFlags.Static | BindingFlags.Public);

        private static readonly MethodInfo Convert_MethodInfo = typeof(ConversionExtensions).GetTypeInfo().GetMethod(nameof(ConversionExtensions.Convert), BindingFlags.Static | BindingFlags.Public);

        private static MethodInfo convertSafely_MethodInfo;
        private static readonly object forLock = new object();
        private static MethodInfo ConvertSafely_MethodInfo
        {
            get
            {
                if (null == convertSafely_MethodInfo)
                {
                    lock (forLock)
                    {
                        if (null == convertSafely_MethodInfo)
                        {
                            convertSafely_MethodInfo = ReflectionExtensions.FixAmbiguousMatch(typeof(ConversionExtensions).GetTypeInfo().GetMethods(BindingFlags.Static | BindingFlags.Public), nameof(ConversionExtensions.ConvertSafely), 1, 1);

                            if (null == convertSafely_MethodInfo)
                                throw new NotFoundException($"{nameof(ConversionExtensions)}.{nameof(ConversionExtensions.ConvertSafely)}. method not found");
                        }
                    }
                }

                return convertSafely_MethodInfo;
            }
        }
        public static MethodInfo FixAmbiguousMatch(MethodInfo[] methods, string methodName, int parameterCount, int genericParameterCount)
        {
            if (!methods.IsNullOrEmpty() && !String.IsNullOrEmpty(methodName))
            {
                foreach (MethodInfo mi in methods)
                {
                    if (String.Equals(mi.Name, methodName))
                    {
                        var pars = mi.GetParameters();
                        if (!pars.IsNullOrEmpty() && pars.Count() == parameterCount)
                        {
                            var genericPars = mi.GetGenericArguments();
                            if (!genericPars.IsNullOrEmpty() && genericPars.Count() == genericParameterCount)
                            {
                                return mi;
                            }
                        }
                    }
                }
            }

            return null;
        }

        public static void SetValueConvert(this PropertyInfo pi, object entity, object propertyValue)
        {
            TypeInfo propertyTypeInfo = pi.PropertyType.GetTypeInfo();
            MethodInfo mi;
            if (propertyTypeInfo.IsGenericType && propertyTypeInfo.GetGenericTypeDefinition() == CachedTypes.PureNullableType)
            {
                mi = ReflectionExtensions.ToNullable_MethodInfo.MakeGenericMethod(propertyTypeInfo.GetGenericArguments()[0]);
            }
            else
            {
                mi = ReflectionExtensions.Convert_MethodInfo.MakeGenericMethod(pi.PropertyType);
            }
            pi.SetValue(entity, mi.Invoke(null, new object[] { propertyValue }));// Convert.ChangeType(dbValue, pi.PropertyType));
        }

        public static void SetValueConvertSafely(this PropertyInfo pi, object entity, object propertyValue)
        {
            TypeInfo propertyTypeInfo = pi.PropertyType.GetTypeInfo();
            MethodInfo mi;
            if (propertyTypeInfo.IsGenericType && propertyTypeInfo.GetGenericTypeDefinition() == CachedTypes.PureNullableType)
            {
                mi = ReflectionExtensions.ToNullableSafely_MethodInfo.MakeGenericMethod(propertyTypeInfo.GetGenericArguments()[0]);
            }
            else
            {
                mi = ReflectionExtensions.ConvertSafely_MethodInfo.MakeGenericMethod(pi.PropertyType);
            }
            pi.SetValue(entity, mi.Invoke(null, new object[] { propertyValue }));// Convert.ChangeType(dbValue, pi.PropertyType));
        }

        public static IList CreateGenericList(this Type elementType)
        {
            if (null != elementType)
            {
                Type genericType = typeof(List<>);
                Type listType = genericType.MakeGenericType(elementType);
                return Activator.CreateInstance(listType) as IList;
            }
            return null;
        }

        public static object GetDefault(this Type type)
        {
            if (null != type)
            {
                if (type.GetTypeInfo().IsValueType)
                {
                    return Activator.CreateInstance(type);
                }
            }
            return null;
        }


        public static void CopyPropertiesFrom(this object destObject, object sourceObject)
        {
            if (null == destObject)
                throw new ArgumentNullException(nameof(destObject));
            if (null == sourceObject)
                throw new ArgumentNullException(nameof(sourceObject));

            Type destObjectType = destObject.GetType();
            foreach (PropertyInfo sourcePi in sourceObject.GetType().GetTypeInfo().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                PropertyInfo destPi = destObjectType.GetTypeInfo().GetProperty(sourcePi.Name);
                if (null != destPi && null != destPi.SetMethod)
                {
                    object sourcePropertyValue = sourcePi.GetValue(sourceObject);

                    destPi.SetValueConvertSafely(destObject, sourcePropertyValue);
                }
            }
        }

        public static void CopyPropertiesFrom<T>(this T destObject, T sourceObject)
        {
            if (null == destObject)
                throw new ArgumentNullException(nameof(destObject));
            if (null == sourceObject)
                throw new ArgumentNullException(nameof(sourceObject));

            foreach (PropertyInfo pi in typeof(T).GetTypeInfo().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (null != pi.SetMethod)
                {
                    object sourcePropertyValue = pi.GetValue(sourceObject);
                    pi.SetValue(destObject, sourcePropertyValue, null);
                }
            }
        }

        public static Type GetType(string typeName)//Type.GetType("TypeName,DllName");
        {
            if (String.IsNullOrEmpty(typeName))
                throw new ArgumentNullException(nameof(typeName));

            return Type.GetType(typeName, true, true);
        }



        public static MethodInfo GetMethod(Type objectType, string methodName, int parameterCount)
        {
            if (null != objectType && !String.IsNullOrEmpty(methodName))
            {
                MethodInfo method;
                try
                {
                    method = objectType.GetTypeInfo().GetMethod(methodName);
                }
                catch (AmbiguousMatchException)
                {
                    method = ReflectionExtensions.FixAmbiguousMatch(objectType.GetTypeInfo().GetMethods(BindingFlags.Public | BindingFlags.Instance), methodName, parameterCount, 1);
                }
                return method;
            }

            return null;
        }
        public static MethodInfo GetGenericMethod(Type objectType, string methodName, Type genericType, int parameterCount)
        {
            if (null != genericType)
            {
                MethodInfo method = GetMethod(objectType, methodName, parameterCount);
                if (null != method)
                {
                    MethodInfo generic = method.MakeGenericMethod(genericType);
                    return generic;
                }
            }

            return null;
        }
        public static object InvokeGeneric(this object obj, string methodName, Type genericType, params object[] pars)
        {
            if (null != obj)
            {
                MethodInfo generic = GetGenericMethod(obj.GetType(), methodName, genericType, (pars == null ? 0 : pars.Length));
                if (null != generic)
                    return generic.Invoke(obj, pars);
            }
            return null;
        }



        public static PropertyInfo GetPropertyInfo(Expression exp)
        {
            PropertyInfo pi = null;
            MemberExpression me = exp as MemberExpression;
            if (null != me)
            {
                pi = (PropertyInfo)me.Member;
            }
            else
            {
                UnaryExpression ue = exp as UnaryExpression;
                if (null != ue)
                {
                    return GetPropertyInfo(ue.Operand);
                }
                else
                {
                    LambdaExpression lax = exp as LambdaExpression;
                    if (null != lax)
                    {
                        return GetPropertyInfo(lax.Body);
                    }
                }
            }
            return pi;
        }

        public static TAttribute GetCustomAttribute<TAttribute>(this Expression exp, bool inherit)
            where TAttribute : Attribute
        {
            if (null != exp)
            {
                PropertyInfo pi = GetPropertyInfo(exp);
                if (null != pi)
                    return pi.GetCustomAttribute<TAttribute>(inherit);
            }
            return null;
        }
        public static TAttribute GetCustomAttribute<TAttribute>(this Expression exp)
            where TAttribute : Attribute
        {
            return GetCustomAttribute<TAttribute>(exp, false);
        }


        private static readonly HashSet<Type> PrimitiveTypes = new HashSet<Type>()
        {
            CachedTypes.String,
            CachedTypes.Boolean, CachedTypes.Byte,CachedTypes.ByteArray,CachedTypes.Char,CachedTypes.DateTime,CachedTypes.Decimal,CachedTypes.Double, CachedTypes.Single,
            CachedTypes.Guid,CachedTypes.Int16,CachedTypes.Int32,CachedTypes.Int64, CachedTypes.UInt16, CachedTypes.UInt32, CachedTypes.UInt64, CachedTypes.SByte,
            CachedTypes.Nullable_Boolean, CachedTypes.Nullable_Byte, CachedTypes.Nullable_Char,CachedTypes.Nullable_DateTime, CachedTypes.Nullable_Single,
            CachedTypes.Nullable_Decimal,CachedTypes.Nullable_Double,CachedTypes.Nullable_Guid,CachedTypes.Nullable_Int16,CachedTypes.Nullable_Int32,CachedTypes.Nullable_Int64
            ,CachedTypes.Nullable_UInt16, CachedTypes.Nullable_UInt32, CachedTypes.Nullable_UInt64, CachedTypes.Nullable_SByte, CachedTypes.ObjectType,
            CachedTypes.ExpandoObjectType, CachedTypes.DateTimeOffset, CachedTypes.Nullable_DateTimeOffset, CachedTypes.TimeSpan, CachedTypes.Nullable_TimeSpan
        };
        public static bool IsPrimitiveType(Type propertyType)
        {
            return PrimitiveTypes.Contains(propertyType);
        }

        private static readonly TypeInfo IEnumerableTypeInfo = typeof(IEnumerable).GetTypeInfo();
        public static bool IsEnumerable(Type type)
        {
            return (IEnumerableTypeInfo.IsAssignableFrom(type) && (type != CachedTypes.String && type != CachedTypes.ByteArray));
        }

        public static bool IsNullableType(this Type type)
        {
            if (null != type)
            {
                return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == CachedTypes.PureNullableType;
            }

            return false;
        }

    }
}
