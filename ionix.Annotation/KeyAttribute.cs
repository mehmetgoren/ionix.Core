namespace ionix.Annotation
{
    using System;

    //
    // Summary:
    //     Denotes one or more properties that uniquely identify an entity.
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class KeyAttribute : Attribute
    {
    }
}
