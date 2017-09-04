namespace ionix.RestTests
{
    using ionix.Rest;
    using System;

    //for web api.
    public sealed class AuthorizationValidator : AuthorizationValidatorBase<AuthorizationValidator>
    {
       public  static readonly ControllerActionsList ControllerActionsList = ControllerActionsList.Create<ReflectController>(AppDomain.CurrentDomain.GetAssemblies());

        protected override ControllerActionsList CreateControllerActionsList()
        {
            return ControllerActionsList;
        }

        protected override IRoleStorageProvider CreateRoleStorageProvider()
        {
            return SqlRoleStorageProvider.Instance;
        }
    }
}