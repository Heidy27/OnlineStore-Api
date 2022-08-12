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
    public class PaymentController : ControllerBase
    {
        private readonly OnlineStorageContext _context;
        private readonly IJwtAuthManager _jwtAuthManager;
        private readonly ILogger _logger;

        public PaymentController(OnlineStorageContext context, IJwtAuthManager jwtAuthManager, ILogger<PaymentController> logger)
        {
            _context = context;
            _jwtAuthManager = jwtAuthManager;
            _logger = logger;
        }

        // GET: api/Payment
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Payment>>> GetPayment()
        {
            var Payment = await _context.payment.AsSplitQuery().ToListAsync();
            return new JsonResult(new
            {
                status = "success",
                Payment
            })
            { StatusCode = StatusCodes.Status200OK };
        }

        // GET: api/Payment/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Payment>> Getpayment(string id)
        {
            var Payment = await _context.payment.AsQueryable().Where(x => x.ID == id).FirstOrDefaultAsync();
            if (Payment == null)
            {
                return NotFound();
            }

            return new JsonResult(new
            {
                status = "success",
                Payment
            })
            { StatusCode = StatusCodes.Status200OK };
        }

        // PUT: api/Payment/5
        [HttpPut("{id}")]

        public async Task<IActionResult> PutPayment(string id, Payment Payment)
        {
            if (id != Payment.ID)
            {
                return BadRequest();
            }

            var local = _context.Set<Payment>().Local.FirstOrDefault(entry => entry.ID.Equals(id));
            if (local != null)
            {
                _context.Entry(local).State = EntityState.Detached;
            }

            _context.Entry(Payment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaymentExists(id))
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
                Payment,
                message = "Payment updated"
            })
            { StatusCode = StatusCodes.Status200OK };
        }

        // POST: api/Payment
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Payment>> PostPayment(Payment payment)
        {
            var ctxUser = (User)HttpContext.Items["User"];
            
            var helper = new helpers.GeneralHelper();
            payment.ID = helper.generateID("PM");

            payment.userID = ctxUser.ID;
            _context.payment.Add(payment);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message.Contains("duplicate key"))
               
                return new JsonResult(new { status = "failure", message = ex.Message }) { StatusCode = StatusCodes.Status400BadRequest };
            }


            return new JsonResult(new
            {
                status = "success",
                payment,
                message = "Payment created",
            })
            { StatusCode = StatusCodes.Status201Created };
        }

        // DELETE: api/Payment/5
        [HttpDelete("{id}")]
        [Authorize]

        public async Task<ActionResult<Payment>> DeletePayment(string id)
        {
            var ctxPayment = (Payment)HttpContext.Items["Payment"];
            var getAllPayment = await _context.payment.ToListAsync();
            if (getAllPayment.Count == 1)
            {
                return new JsonResult(new
                {
                    status = "failure",
                    message = "No puedes eliminar todos los pagos",
                })
                { StatusCode = StatusCodes.Status400BadRequest };
            }

            var Payment = await _context.payment.Where(u => u.ID == id).FirstOrDefaultAsync();
            if (Payment == null)
            {
                return NotFound();
            }

            if (ctxPayment.ID == Payment.ID)
            {
                return new JsonResult(new
                {
                    status = "failure",
                    message = "No puedes eliminarte a ti mismo",
                })
                { StatusCode = StatusCodes.Status400BadRequest };
            }

            _context.payment.Remove(Payment);
            await _context.SaveChangesAsync();

            return new JsonResult(new
            {
                status = "success",
                message = "Payment deleted",
            })
            { StatusCode = StatusCodes.Status202Accepted };
        }


        private bool PaymentExists(string id)
        {
            return _context.payment.AsQueryable().Any(e => e.ID == id);
        }
    }
}
