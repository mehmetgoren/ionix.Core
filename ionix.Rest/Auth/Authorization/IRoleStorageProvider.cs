namespace Ionix.Rest
{
    using System;
    using System.Collections.Generic;

    public sealed class RoleControllerActionEntity
    {
        public string RoleName { get; private set; }
        public string ControllerName { get; private set; }
        public string ActionName { get; private set; }

        public bool Enabled { get; set; }

        public RoleControllerActionEntity(string roleName, string controllerName, string actionName)
        {
            if (String.IsNullOrEmpty(roleName))
                throw new ArgumentNullException(nameof(roleName));
            if (String.IsNullOrEmpty(controllerName))
                throw new ArgumentNullException(nameof(controllerName));
            if (String.IsNullOrEmpty(actionName))
                throw new ArgumentNullException(nameof(actionName));

            this.RoleName = roleName;
            this.ControllerName = controllerName;
            this.ActionName = actionName;
        }

        public override string ToString()
        {
            return this.RoleName + ": " + this.ControllerName + "/" + this.ActionName;
        }
    }

    //can be rdbms(Sql Server, Oracle), nosql or etc...
    public interface IRoleStorageProvider
    {
        IEnumerable<RoleControllerActionEntity> GetAll();

        //SaveRoles () RoleControllerActionEntity to RoleApiAction Table
        int Save(IEnumerable<RoleControllerActionEntity> list);

        int ClearNonExistRecords();
    }
}
