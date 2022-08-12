using OnlineStore.models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OnlineStore.helpers
{

    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IJwtAuthManager _jwtAuthManager;
        public JwtMiddleware(RequestDelegate next, IConfiguration configuration, IJwtAuthManager jwtAuthManager)
        {
            _next = next;
            _jwtAuthManager = jwtAuthManager;

        }

        public async Task Invoke(HttpContext context, OnlineStorageContext dbcontext)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (token != null)
            {
                await attachUserToContext(context, dbcontext, token);
            }

            await _next(context);
        }

        private async Task attachUserToContext(HttpContext context, OnlineStorageContext dbcontext, string token)
        {
            try
            {
                if ((User)context.Items["User"] == null)
                {
                    var validatedToken = _jwtAuthManager.DecodeJwtToken(token);

                    var usr = validatedToken.Item1.FindFirst("ID").Value.ToString();

                    var newUser = await dbcontext.users.Where(u => u.ID == usr).FirstOrDefaultAsync();
                    User contextUser = new User()
                    {
                        ID = newUser.ID,
                        role = newUser.role,
                        userName = newUser.userName
                    };

                    context.Items["User"] = contextUser;

                }

            }
            catch { }
        }
    }
}