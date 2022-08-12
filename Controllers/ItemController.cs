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
    public class ItemController : ControllerBase
    {
        private readonly OnlineStorageContext _context;
        private readonly IJwtAuthManager _jwtAuthManager;
        private readonly ILogger _logger;

        public ItemController(OnlineStorageContext context, IJwtAuthManager jwtAuthManager, ILogger<ItemController> logger)
        {
            _context = context;
            _jwtAuthManager = jwtAuthManager;
            _logger = logger;
        }

        // GET: api/item
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Item>>> Getitem()
        {
            var item = await _context.items.AsSplitQuery().ToListAsync();
            return new JsonResult(new
            {
                status = "success",
                item
            })
            { StatusCode = StatusCodes.Status200OK };
        }

        // GET: api/item/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Item>> Getitem(string id)
        {
            var item = await _context.items.AsQueryable().Where(x => x.ID == id).FirstOrDefaultAsync();
            if (item == null)
            {
                return NotFound();
            }

            return new JsonResult(new
            {
                status = "success",
                item
            })
            { StatusCode = StatusCodes.Status200OK };
        }

        // PUT: api/item/5
        [HttpPut("{id}")]

        public async Task<IActionResult> Putitem(string id, Item item)
        {
            if (id != item.ID)
            {
                return BadRequest();
            }

            var local = _context.Set<Item>().Local.FirstOrDefault(entry => entry.ID.Equals(id));
            if (local != null)
            {
                _context.Entry(local).State = EntityState.Detached;
            }

            _context.Entry(item).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!itemExists(id))
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
                item,
                message = "item updated"
            })
            { StatusCode = StatusCodes.Status200OK };
        }

        // POST: api/item
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Item>> Postitem(Item item)
        {
            var helper = new helpers.GeneralHelper();
            item.ID = helper.generateID("ITM");

            _context.items.Add(item);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new JsonResult(new { status = "failure", message = ex.Message }) { StatusCode = StatusCodes.Status400BadRequest };
            }


            return new JsonResult(new
            {
                status = "success",
                item,
                message = "Articulo agregado",
            })
            { StatusCode = StatusCodes.Status201Created };
        }

        // DELETE: api/item/5
        [HttpDelete("{id}")]
        [Authorize]

        public async Task<ActionResult<Item>> Deleteitem(string id)
        {
            var item = await _context.items.Where(u => u.ID == id).FirstOrDefaultAsync();
            if (item == null)
            {
                return NotFound();
            }

            _context.items.Remove(item);
            await _context.SaveChangesAsync();

            return new JsonResult(new
            {
                status = "success",
                message = "item deleted",
            })
            { StatusCode = StatusCodes.Status202Accepted };
        }


        private bool itemExists(string id)
        {
            return _context.items.AsQueryable().Any(e => e.ID == id);
        }
    }
}
