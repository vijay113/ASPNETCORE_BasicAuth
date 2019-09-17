using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BasicAuthSample.BasicAuth
{
    /// <summary>
    /// Accepts either username or email as user identifier for sign in with Http Basic authentication
    /// </summary>
    public class BasicAuthenticationMiddleware
    {
        public BasicAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        private readonly RequestDelegate _next;

        public async Task Invoke(HttpContext context)
        {           
                var basicAuthenticationHeader = GetBasicAuthenticationHeaderValue(context);
                if (basicAuthenticationHeader.IsValidBasicAuthenticationHeaderValue)
                {
                    //  Sign in 
                    var authenticationManager = new BasicAuthenticationSignInManager(context, basicAuthenticationHeader);
                    var isvalidUser = await authenticationManager.TrySignInUser();
                    if (isvalidUser)
                    {
                        // valid User.
                        await _next.Invoke(context);
                    }
                    else
                    {                       
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        await context.Response.WriteAsync("Invalid UserName and Password.");
                        return;
                    }
                }
                else
                {
                    // Code if any other Auth, Not BasicAuthentication                    
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsync("Provide Valid Basic Authentication Header Value.");
                    return;
                }            
        }

        private BasicAuthenticationHeaderValue GetBasicAuthenticationHeaderValue(HttpContext context)
        {
            var basicAuthenticationHeader = context.Request.Headers["Authorization"]
                .FirstOrDefault(header => header.StartsWith("Basic", StringComparison.OrdinalIgnoreCase));
            var decodedHeader = new BasicAuthenticationHeaderValue(basicAuthenticationHeader);
            return decodedHeader;
        }
    }
}
