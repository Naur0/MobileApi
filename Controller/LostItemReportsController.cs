using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MobileApi.Data;
using MobileApi.Models;

namespace MobileApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LostItemReportsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LostItemReportsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var reports = await _context.LostItemReports
                .OrderByDescending(report => report.CreatedAtUtc)
                .ToListAsync(cancellationToken);

            return Ok(reports);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var report = await _context.LostItemReports.FindAsync([id], cancellationToken);
            return report == null ? NotFound() : Ok(report);
        }

        [HttpPost]
        public async Task<IActionResult> Create(LostItemReport report, CancellationToken cancellationToken)
        {
            report.Id = 0;
            report.CreatedAtUtc = DateTime.UtcNow;
            report.Status = string.IsNullOrWhiteSpace(report.Status) ? "Open" : report.Status.Trim();

            _context.LostItemReports.Add(report);
            await _context.SaveChangesAsync(cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = report.Id }, report);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, LostItemReport updated, CancellationToken cancellationToken)
        {
            var report = await _context.LostItemReports.FindAsync([id], cancellationToken);
            if (report == null)
            {
                return NotFound();
            }

            report.ItemName = updated.ItemName;
            report.Description = updated.Description;
            report.Category = updated.Category;
            report.LostDate = updated.LostDate;
            report.LastSeenLocation = updated.LastSeenLocation;
            report.ContactName = updated.ContactName;
            report.ContactMethod = updated.ContactMethod;
            report.Status = string.IsNullOrWhiteSpace(updated.Status) ? report.Status : updated.Status.Trim();

            await _context.SaveChangesAsync(cancellationToken);
            return Ok(report);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var report = await _context.LostItemReports.FindAsync([id], cancellationToken);
            if (report == null)
            {
                return NotFound();
            }

            _context.LostItemReports.Remove(report);
            await _context.SaveChangesAsync(cancellationToken);

            return NoContent();
        }
    }
}
