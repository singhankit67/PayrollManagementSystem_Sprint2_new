﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem_API.Models;

namespace PayrollManagementSystem_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveMastersController : ControllerBase
    {
        private readonly PayrollManagementSystemMVCContext _context;

        public LeaveMastersController(PayrollManagementSystemMVCContext context)
        {
            _context = context;
        }

        // GET: api/LeaveMasters
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeaveMaster>>> GetLeaveMasters()
        {
            LeaveCount();
            return await _context.LeaveMasters.ToListAsync();
        }

        public async Task<IActionResult> LeaveCount()                                       //CHANGE -4 08/09/2021
        {
            _context.Database.ExecuteSqlInterpolated($"exec USP_Leave_Count");
            await _context.SaveChangesAsync();
            return Ok();
        }

        // GET: api/LeaveMasters/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LeaveMaster>> GetLeaveMaster(string id)
        {
            LeaveCount();
            var leaveMaster = await _context.LeaveMasters.FindAsync(id);

            if (leaveMaster == null)
            {
                return NotFound();
            }

            return leaveMaster;
        }

        // PUT: api/LeaveMasters/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLeaveMaster(string id, LeaveMaster leaveMaster)
        {
            ResetLeaveTable();
            if (id != leaveMaster.EmployeeId)
            {
                return BadRequest();
            }

            _context.Entry(leaveMaster).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LeaveMasterExists(id))
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

        // POST: api/LeaveMasters
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
       
        [HttpPost]
        public async Task<ActionResult<LeaveMaster>> PostLeaveMaster(LeaveMaster leaveMaster)
        {
            ResetLeaveTable();
            _context.LeaveMasters.Add(leaveMaster);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (LeaveMasterExists(leaveMaster.EmployeeId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetLeaveMaster", new { id = leaveMaster.EmployeeId }, leaveMaster);
        }

        public void ResetLeaveTable()
        {
            _context.Database.ExecuteSqlInterpolated($"exec USP_Reset_Leave_Master");            
        }

        

        // DELETE: api/LeaveMasters/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLeaveMaster(string id)
        {
            var leaveMaster = await _context.LeaveMasters.FindAsync(id);
            if (leaveMaster == null)
            {
                return NotFound();
            }

            _context.LeaveMasters.Remove(leaveMaster);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LeaveMasterExists(string id)
        {
            return _context.LeaveMasters.Any(e => e.EmployeeId == id);
        }
    }
}
