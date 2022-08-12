using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineStore.models;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace OnlineStore.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly OnlineStorageContext _context;
        private readonly IJwtAuthManager _jwtAuthManager;
        private readonly ILogger _logger;

        public UsersController(OnlineStorageContext context, IJwtAuthManager jwtAuthManager, ILogger<UsersController> logger)
        {
            _context = context;
            _jwtAuthManager = jwtAuthManager;
            _logger = logger;
        }

        // GET: api/Users
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<User>>> Getusers()
        {
            var users = await _context.users.Include(p => p.payments).AsSplitQuery().ToListAsync();
            foreach (var user in users)
            {
                user.pwd = null;
            }
            return new JsonResult(new
            {
                status = "success",
                users
            })
            { StatusCode = StatusCodes.Status200OK };
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<User>> GetUser(string id)
        {
            var user = await _context.users.AsQueryable().Where(x => x.ID == id).Include(p => p.payments).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound();
            }

            return new JsonResult(new
            {
                status = "success",
                user
            })
            { StatusCode = StatusCodes.Status200OK };
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]

        public async Task<IActionResult> PutUser(string id, User user)
        {
            if (id != user.ID)
            {
                return BadRequest();
            }

            var local = _context.Set<User>().Local.FirstOrDefault(entry => entry.ID.Equals(id));
            if (local != null)
            {
                _context.Entry(local).State = EntityState.Detached;
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return new JsonResult(new
            {
                status = "success",
                user,
                message = "User updated"
            })
            { StatusCode = StatusCodes.Status200OK };
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            var ctxUser = (User)HttpContext.Items["User"];
            if ((ctxUser != null && ctxUser.role.ToUpper() != "ADMIN" && user.role.ToUpper() == "ADMIN") || (ctxUser == null && user.role.ToUpper() == "ADMIN"))
            {
                return new JsonResult(new
                {
                    message = "Solos los administradores pueden agregar otros admins",
                })
                { StatusCode = StatusCodes.Status400BadRequest };
            }

            var helper = new helpers.GeneralHelper();
            user.ID = helper.generateID("USR");

            user.enabled = true;
            _context.users.Add(user);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message.Contains("duplicate key"))
                {
                    return new JsonResult(new { status = "failure", message = "Ya existe un usuario con este username" }) { StatusCode = StatusCodes.Status400BadRequest };
                }
                return new JsonResult(new { status = "failure", message = ex.Message }) { StatusCode = StatusCodes.Status400BadRequest };
            }


            user.pwd = null;

            return new JsonResult(new
            {
                status = "success",
                user,
                message = "User created",
            })
            { StatusCode = StatusCodes.Status201Created };
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        [Authorize]

        public async Task<ActionResult<User>> DeleteUser(string id)
        {
            var ctxUser = (User)HttpContext.Items["User"];
            var getAllUser = await _context.users.ToListAsync();
            if (getAllUser.Count == 1)
            {
                return new JsonResult(new
                {
                    status = "failure",
                    message = "No puedes eliminar todos los usuarios",
                })
                { StatusCode = StatusCodes.Status400BadRequest };
            }

            var user = await _context.users.Where(u => u.ID == id).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound();
            }

            if (ctxUser.ID == user.ID)
            {
                return new JsonResult(new
                {
                    status = "failure",
                    message = "No puedes eliminarte a ti mismo",
                })
                { StatusCode = StatusCodes.Status400BadRequest };
            }

            _context.users.Remove(user);
            await _context.SaveChangesAsync();

            return new JsonResult(new
            {
                status = "success",
                message = "User deleted",
            })
            { StatusCode = StatusCodes.Status202Accepted };
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<User>> LoginUser(Login usr)
        {
            string username = usr.userName;
            var user = await _context.users.Where(x => x.userName == username && x.enabled == true).AsSplitQuery().FirstOrDefaultAsync();

            if (user == null)
            {
                return new JsonResult(new
                {
                    status = "failure",
                    message = "Usuario inexistente o deshabilitado",
                })
                { StatusCode = StatusCodes.Status400BadRequest };
            }
            if (user.pwd != usr.password)
            {

                return new JsonResult(new
                {
                    status = "failure",
                    message = "Contraseña incorrecta",
                })
                { StatusCode = StatusCodes.Status400BadRequest };
            }

            Claim[] claimns = new Claim[]{
                    new Claim("ID",user.ID),
                };

            var token = _jwtAuthManager.GenerateTokens(user.ID, claimns, DateTime.Now);
            CookieOptions options = new CookieOptions { Expires = token.RefreshToken.ExpireAt };
            Response.Cookies.Append("jwt", token.AccessToken, options);
            user.pwd = null;

            return new JsonResult(new
            {
                status = "success",
                user,
                message = "User logged",
                token = token.AccessToken,
            })
            { StatusCode = StatusCodes.Status200OK };

        }

        private bool UserExists(string id)
        {
            return _context.users.AsQueryable().Any(e => e.ID == id);
        }
    }
}
