namespace Ionix.Rest
{
    using Utils.Extensions;
    using System;
    using System.Collections.Generic;

    //Amaç RoleControllerActionEntity entityi parçalayıup indexlemek hızlı check işlemleri için.
    public sealed class IndexedRoles
    {
        public static bool IgnoreCase { get; set; }

        public static IndexedRoles Create(IEnumerable<RoleControllerActionEntity> list)
        {
            IndexedRoles ret = new IndexedRoles();
            if (!list.IsNullOrEmpty())
            {
                foreach (var entity in list)
                {
                    ret.Add(entity);
                }
            }
            return ret;
        }

        private sealed class StringIgnoreCaseComparer : IEqualityComparer<string>
        {
            internal static readonly StringIgnoreCaseComparer Instance = new StringIgnoreCaseComparer();
            private StringIgnoreCaseComparer()
            {
                
            }
            public bool Equals(string x, string y)
            {
                return String.Equals(x, y, StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode(string obj)
            {
                return obj.ToLower().GetHashCode();
            }
        }

        private readonly Dictionary<string, Dictionary<string, Dictionary<string, RoleControllerActionEntity>>> dic;//role / controller / action + RoleControllerActionEntity
        private readonly StringIgnoreCaseComparer comparer;
        private IndexedRoles()
        {
            this.comparer = IgnoreCase ? StringIgnoreCaseComparer.Instance : null;
            this.dic = new Dictionary<string, Dictionary<string, Dictionary<string, RoleControllerActionEntity>>>(this.comparer);
        }

        private void Add(RoleControllerActionEntity entity)
        {
            if (null == entity)
                throw new ArgumentNullException(nameof(entity));
            if (String.IsNullOrEmpty(entity.RoleName))
                throw new ArgumentException("entity.RoleName is null");
            if (String.IsNullOrEmpty(entity.ControllerName))
                throw new ArgumentException("entity.ControllerName is null");
            if (String.IsNullOrEmpty(entity.ActionName))
                throw new ArgumentException("entity.ActionName is null");

            Dictionary<string, Dictionary<string, RoleControllerActionEntity>> controllers;
            if (!this.dic.TryGetValue(entity.RoleName, out controllers))
            {
                controllers = new Dictionary<string, Dictionary<string, RoleControllerActionEntity>>(this.comparer);
                this.dic.Add(entity.RoleName, controllers);
            }

            Dictionary<string, RoleControllerActionEntity> actions;
            if (!controllers.TryGetValue(entity.ControllerName, out actions))
            {
                actions = new Dictionary<string, RoleControllerActionEntity>(this.comparer);
                controllers.Add(entity.ControllerName, actions);
            }
            actions.Add(entity.ActionName, entity);// Not support overloading.
        }

        //need to be benchmarked.
        public RoleControllerActionEntity Find(string role, string controller, string action)
        {
            RoleControllerActionEntity entity = null;
            Dictionary<string, Dictionary<string, RoleControllerActionEntity>> controllers;
            if (this.dic.TryGetValue(role, out controllers))
            {
                Dictionary<string, RoleControllerActionEntity> actions;
                if (controllers.TryGetValue(controller, out actions))
                    actions.TryGetValue(action, out entity);
            }
            return entity;
        }
    }
}
