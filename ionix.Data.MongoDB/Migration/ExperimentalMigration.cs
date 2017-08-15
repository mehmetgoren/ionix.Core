namespace ionix.Data.Mongo.Migration
{
	using System;

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class ExperimentalAttribute : Attribute
	{
	}
}