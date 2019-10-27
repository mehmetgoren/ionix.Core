namespace Ionix.Rest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Utils.Extensions;

    public interface IAuthorizationValidator
    {
        void RefreshStorageAndCachedData();
        bool IsAuthenticated(string role, string controller, string action);
        bool IsNeedToBeAuthenticated(string controller);
    }
    //Singleton. Hem Web api hem de mvc api' yi denetleyecek.
    public abstract class AuthorizationValidatorBase<TAuthenticationManager> : IAuthorizationValidator
        where TAuthenticationManager : AuthorizationValidatorBase<TAuthenticationManager>, new()
    {
        protected abstract ControllerActionsList CreateControllerActionsList();
        private HashSet<string> _controllerList;
        private HashSet<string> ControllerList => this._controllerList ?? (this._controllerList = this.CreateControllerActionsList().Select(i => i.ControllerType.Name.Replace("Controller", "").ToLower()).ToHashSet());

        public bool IsNeedToBeAuthenticated(string controller)
        {
            if (!String.IsNullOrEmpty(controller))
                return this.ControllerList.Contains(controller.ToLower());
            return false;
        }

        protected abstract IRoleStorageProvider CreateRoleStorageProvider();
        private IRoleStorageProvider _roleStorageProvider;
        private IRoleStorageProvider RoleStorageProvider => this._roleStorageProvider ?? (this._roleStorageProvider = this.CreateRoleStorageProvider());

        private IndexedRoles _indexedRoles;
        private IndexedRoles IndexedRoles => this._indexedRoles ?? (this._indexedRoles = IndexedRoles.Create(this.RoleStorageProvider.GetAll()));

        public virtual void RefreshStorageAndCachedData()
        {
            this._roleStorageProvider = null;
            this._indexedRoles = null;
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
