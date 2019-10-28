namespace Ionix.Rest
{
    using System;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Utils.Extensions;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class TokenTableAuthAttribute : Attribute
    {
        
    }

    public class TokenTableMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ITokenTable _tokenTable;
        private readonly IAuthorizationValidator _validator;

        public TokenTableMiddleware(RequestDelegate next, ITokenTable tokenTable, IAuthorizationValidator validator)
        {
            this._next = next ?? throw new ArgumentNullException(nameof(next));
            this._tokenTable = tokenTable ?? throw new ArgumentNullException(nameof(tokenTable));
            this._validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public virtual async Task Invoke(HttpContext context)
        {
            bool flag = true;
            var request = context.Request;
            if (request.Method != HttpMethods.Options) //? sec hole
            {
                (string controller, string action) = GetControllerActionNames(context.Request);

                if (this._validator.IsNeedToBeAuthenticated(controller))
                {
                    flag = false;
                    var headerValue = context.Request.Headers["Authorization"];
                    if (!headerValue.IsNullOrEmpty())
                    {
                        string[] splits = headerValue[0]?.Split(' ');
                        if (!splits.IsNullOrEmpty() && splits.Length == 2 && splits[0] == "Token")
                        {
                            string base64Token = splits[1];
                            string strToken = Encoding.ASCII.GetString(Convert.FromBase64String(base64Token));
                            if (Guid.TryParse(strToken, out Guid token))
                            {
                                flag = this._tokenTable.TryAuthenticateToken(token, out User user) &&
                                       this.AuthenticateUser(controller, action, user);
                            }
                        }
                    }

                    if (!flag)
                    {
                        context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                        await context.Response.WriteAsync("Access Denied 401");
                        //throw new AuthenticationException("401 Access Denied");
                    }
                }
            }
          
            if (flag)
                await _next.Invoke(context);
        }

        protected virtual bool AuthenticateUser(string controller, string action, User user)
        {
            if (user.IsAdmin) return true;

            return this._validator.IsAuthenticated(user.Role, controller, action);
        }

        private static (string, string) GetControllerActionNames(HttpRequest request)
        {
            string path = request.Path.Value;
            if (!String.IsNullOrEmpty(path))
            {
                string[] arr = path.Split('/');
                if (null != arr && arr.Length > 1)
                {
                    string controller = arr[arr.Length - 2];
                    string action = arr[arr.Length - 1].Split('?')[0];

                    return (controller, action);
                }
            }

            throw new ArgumentException($"'{request.Path}' is not valid");
        }
    }

    public static class TokenTableMiddlewareExtensions
    {
        public static IApplicationBuilder UseTokenTableAuthentication(this IApplicationBuilder builder, ITokenTable tokenTable, IAuthorizationValidator validator)
        {
            return builder.UseMiddleware<TokenTableMiddleware>(tokenTable, validator);
        }
    }
}
