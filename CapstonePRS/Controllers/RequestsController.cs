using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CapstonePRS.Models;

namespace CapstonePRS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestsController : ControllerBase
    {

        public static string NEW = "NEW";
        public static string REVIEW = "REVIEW";
        public static string APPROVED = "APPROVED";
        public static string REJECTED = "REJECTED";
        public static string PAID = "PAID";

        private readonly AppDbContext _context;

        public RequestsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Requests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Request>>> GetRequests()
        {
            return await _context.Requests.ToListAsync();
        }

        // GET: api/Requests/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Request>> GetRequest(int id)
        {
            var request = await _context.Requests
                                            .Include(x => x.RequestLines)!
                                                .ThenInclude(x => x.Product)
                                                .ThenInclude(x => x.Vendor)
                                            .SingleOrDefaultAsync(x => x.Id == id);

            if (request == null)
            {
                return NotFound();
            }

            return request;
        }

        // GET: /api/requests/reviews/{userId}
        [HttpGet("reviews/{userId}")]
        public async Task<ActionResult<IEnumerable<Request>>> GetRequestReview(int userid)
        {
            //need something when the review request returns nothing in review to say none found
            //AND if you enter userid 8 or something that does not exist,
            //it returns all reviewed, need it to return not found again.
           var fred = await _context.Requests.FindAsync(userid);
            if(fred is null)
            {
                return NotFound();
            }
            if(fred.Status != REVIEW)
            {
                return NotFound();
            }

          return await _context.Requests.Where(u => u.UserId != userid && u.Status == REVIEW).ToListAsync();
          
        }



        // PUT: api/Requests/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRequest(int id, Request request)
        {
            if (id != request.Id)
            {
                return BadRequest();
            }

            _context.Entry(request).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RequestExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }



        // PUT: /api/requests/review/5
        [HttpPut("review/{id}")]
        public async Task<IActionResult> Review(int id, Request request)
        {
            if(request.Total <= 50)
            {
                request.Status = APPROVED;
                var reviewx = await PutRequest(id, request);
                return reviewx;
            }
            request.Status = REVIEW;
            var reviewy = await PutRequest(id, request);
            return reviewy;

             // maybe do Ternary instead? - seems to work just fine in testing.          
            //request.Status = (request.Total <= 50) ? APPROVED : REVIEW;
            
        }

        // PUT: /api/requests/approve/5
        [HttpPut("approve/{id}")]
        public async Task<IActionResult> Approve(int id, Request request)
        {
            request.Status = APPROVED;
            var approvex = await PutRequest(id, request);
            return approvex;

        }

        // PUT: /api/requests/reject/5
        [HttpPut("reject/{id}")]
        public async Task<IActionResult> Reject(int id, Request request)
        {
            request.Status = REJECTED;
            var rejectx = await PutRequest(id, request);
            return rejectx;

        }

        // POST: api/Requests
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Request>> PostRequest(Request request)
        {
            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRequest", new { id = request.Id }, request);
        }

        // DELETE: api/Requests/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequest(int id)
        {
            var request = await _context.Requests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            _context.Requests.Remove(request);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RequestExists(int id)
        {
            return _context.Requests.Any(e => e.Id == id);
        }
    }
}
