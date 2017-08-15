namespace ionix.RestTests
{
    using ionix.Rest;

    //for web api.
    public sealed class AuthorizationValidator : AuthorizationValidatorBase<AuthorizationValidator>
    {
        protected override IRoleStorageProvider CreateRoleStorageProvider()
        {
            return SqlRoleStorageProvider.Instance;
        }
    }
}