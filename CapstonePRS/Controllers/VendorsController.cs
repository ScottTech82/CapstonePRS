using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CapstonePRS.Models;
using System.Drawing.Text;

namespace CapstonePRS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorsController : ControllerBase
    {
        public static string APPROVED = "APPROVED";

        private readonly AppDbContext _context;

        public VendorsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Vendors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vendor>>> GetVendors()
        {
            return await _context.Vendors.ToListAsync();
        }

        // GET: api/Vendors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Vendor>> GetVendor(int id)
        {
            var vendor = await _context.Vendors.FindAsync(id);

            if (vendor == null)
            {
                return NotFound();
            }

            return vendor;
        }

        // GET: api/Vendors/po/5
        [HttpGet("po/{vendorId}")]
        public async Task<ActionResult<Po>> CreatePo(int vendorId)
        {
            
            var xPo = new Po();
                   //need to solve the null or use GetVendor method above?
                   //GetVendor wont convert and Single causes an error instead of returning null.
                   //could do if statement after to return exception.
            
            xPo.Vendor = await _context.Vendors.SingleOrDefaultAsync(x => x.Id == vendorId);

            if (xPo.Vendor == null)
            {
                throw new Exception("vendor Id does not exist");
            }

            var xPoline = from v in _context.Vendors
                           join p in _context.Products
                                 on v.Id equals p.VendorId
                           join rl in _context.RequestLines
                                 on p.Id equals rl.ProductId
                           join r in _context.Requests
                                 on rl.RequestId equals r.Id
                           where r.Status == APPROVED && v.Id == vendorId
                           select new { p.Id, Product = p.Name, rl.Quantity, p.Price, LineTotal = p.Price * rl.Quantity };


            var sortedLines = new SortedList<int, Poline>();
            foreach(var line in xPoline)
            {

                    if (!sortedLines.ContainsKey(line.Id))
                    {
                        var poline = new Poline()
                        {
                            Product = line.Product,   
                            Quantity = 0,
                            Price = line.Price,
                            LineTotal = line.LineTotal
                        };
                        sortedLines.Add(line.Id, poline);
                    }

                sortedLines[line.Id].Quantity += line.Quantity;
                        
            }

            xPo.Polines = sortedLines.Values;
            xPo.PoTotal = xPoline.Sum(x => x.LineTotal);
            return xPo;

            
            


        }
        

        // PUT: api/Vendors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVendor(int id, Vendor vendor)
        {
            if (id != vendor.Id)
            {
                return BadRequest();
            }

            _context.Entry(vendor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VendorExists(id))
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

        // POST: api/Vendors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Vendor>> PostVendor(Vendor vendor)
        {
            _context.Vendors.Add(vendor);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVendor", new { id = vendor.Id }, vendor);
        }

        // DELETE: api/Vendors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVendor(int id)
        {
            var vendor = await _context.Vendors.FindAsync(id);
            if (vendor == null)
            {
                return NotFound();
            }

            _context.Vendors.Remove(vendor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VendorExists(int id)
        {
            return _context.Vendors.Any(e => e.Id == id);
        }
    }
}
