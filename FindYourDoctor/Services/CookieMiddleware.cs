using System.Collections.Concurrent;
using FindYourDoctor.Data.Domain;
using Microsoft.AspNetCore.Identity;

namespace FindYourDoctor.Services;

public class CookieMiddleware
{
    private readonly RequestDelegate _next;

    public static IDictionary<Guid, LoginInfo> Logins { get; private set; }
        = new ConcurrentDictionary<Guid, LoginInfo>();


    public CookieMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, SignInManager<User> signInMgr)
    {
        if (context.Request.Path == "/login_user" && context.Request.Query.ContainsKey("key") 
                                             && Guid.TryParse(context.Request.Query["key"], out var key))
        {
            var info = Logins[key];

            var result = await signInMgr.PasswordSignInAsync(info.UserName, info.Password, info.IsPersistent, lockoutOnFailure: false);
            info.Password = string.Empty;
            
            if (result.Succeeded)
            {
                Logins.Remove(key);
                context.Response.Redirect("/");
                return;
            }
            
            context.Response.Redirect("/login_failed");
            return;
        }
        
        if (context.Request.Path == "/logout")
        {
            await signInMgr.SignOutAsync();
            context.Response.Redirect("/");
            return;
        }
        
        await _next.Invoke(context);
    }
}

public class LoginInfo
{
    public string UserName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
    
    public bool IsPersistent { get; set; }
}