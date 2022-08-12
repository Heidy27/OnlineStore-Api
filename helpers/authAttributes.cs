using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using OnlineStore.models;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = (User)context.HttpContext.Items["User"];
        if (user == null)
        {
            // not logged in
            context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
        }
    }
}

public class accessAttribute : Attribute, IAuthorizationFilter
{
    string access;
    public accessAttribute(string accessTo)
    {
        access = accessTo;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = (User)context.HttpContext.Items["User"];
        bool allowed = false;
        if (access.ToUpper().Contains(user.role))
        {
            // user allowed
            allowed = true;
        }

        if (!allowed)
        {
            context.Result = new JsonResult(new { message = "Role not admited" }) { StatusCode = StatusCodes.Status403Forbidden };
        }
    }

}