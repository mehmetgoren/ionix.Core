namespace Ionix.Migration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class MigrationReflection
    {
        protected readonly List<Assembly> Assemblies = new List<Assembly>();


        public void LookForMigrationsInAssemblyOfType<T>()
        {
            var assembly = typeof(T).GetTypeInfo().Assembly;
            LookForMigrationsInAssembly(assembly);
        }

        public void LookForMigrationsInAssembly(Assembly assembly)
        {
            if (Assemblies.Contains(assembly))
            {
                return;
            }
            Assemblies.Add(assembly);
        }

        public IEnumerable<Migration> GetAllMigrations()
        {
            return Assemblies
                .SelectMany(GetMigrationsFromAssembly)
                .OrderBy(m => m.Version);
        }

        protected IEnumerable<Migration> GetMigrationsFromAssembly(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes()
                    .Where(t => typeof(Migration).GetTypeInfo().IsAssignableFrom(t) && !t.GetTypeInfo().IsAbstract)
                    .Select(Activator.CreateInstance)
                    .OfType<Migration>();
            }
            catch (Exception exception)
            {
                throw new MigrationException("Cannot load migrations from assembly: " + assembly.FullName, exception);
            }
        }

        public MigrationVersion LatestVersion()
        {
            if (!GetAllMigrations().Any())
            {
                return MigrationVersion.Default();
            }
            return GetAllMigrations()
                .Max(m => m.Version);
        }

        public IEnumerable<Migration> GetMigrationsAfter(DatabaseVersionBase currentVersion)
        {
            var migrations = GetAllMigrations();

            if (currentVersion != null)
            {
                migrations = migrations.Where(m => m.Version > currentVersion.Version);
            }

            return migrations.OrderBy(m => m.Version);
        }
    }
}
