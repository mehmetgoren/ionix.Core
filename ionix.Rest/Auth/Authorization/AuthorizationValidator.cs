namespace ionix.Rest
{
    using System;

    public interface IAuthorizationValidator
    {
        void RefreshStorageAndCachedData();
        bool IsAuthenticated(string role, string controller, string action);
    }
    //Singleton. Hem Web api hem de mvc api' yi denetleyecek.
    public abstract class AuthorizationValidatorBase<TAuthenticationManager> : IAuthorizationValidator
        where TAuthenticationManager : AuthorizationValidatorBase<TAuthenticationManager>, new()
    {
        protected abstract IRoleStorageProvider CreateRoleStorageProvider();
        private IRoleStorageProvider roleStorageProvider;
        private IRoleStorageProvider RoleStorageProvider => this.roleStorageProvider ?? (this.roleStorageProvider = this.CreateRoleStorageProvider());

        private IndexedRoles indexedRoles;
        private IndexedRoles IndexedRoles => this.indexedRoles ?? (this.indexedRoles = IndexedRoles.Create(this.RoleStorageProvider.GetAll()));

        public virtual void RefreshStorageAndCachedData()
        {
            this.roleStorageProvider = null;
            this.indexedRoles = null;
        }

        //Önce token' ı geçecek ardından buraya gelecek.
        public virtual bool IsAuthenticated(string role, string controller, string action)
        {
            var entity = this.IndexedRoles.Find(role, controller, action);
            if (null != entity)
                return entity.Enabled;
            return false;
        }

        public static readonly TAuthenticationManager Instance = new TAuthenticationManager();
    }

}
