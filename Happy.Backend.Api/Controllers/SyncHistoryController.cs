using System.Text;
using System.Text.Json.Nodes;
using Happy.Backend.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Happy.Backend.Api.Controllers;

[ApiController]
[Route("sync-history")]
[Tags("Sync History")]
public class SyncHistoryController : ControllerBase
{
    private readonly HappyDbContext _db;

    public SyncHistoryController(HappyDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetSyncHistory(
        [FromQuery] string? app,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] string? phone,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        bool includeFarmer = string.IsNullOrEmpty(app) || app.Equals("Farmer", StringComparison.OrdinalIgnoreCase);
        bool includeStore = string.IsNullOrEmpty(app) || app.Equals("Store", StringComparison.OrdinalIgnoreCase);

        var allItems = new List<object>();
        int total = 0;

        if (includeFarmer)
        {
            var q = _db.SyncRawFarmers.AsQueryable();
            if (fromDate.HasValue) q = q.Where(x => x.SyncTime >= fromDate.Value);
            if (toDate.HasValue) q = q.Where(x => x.SyncTime < toDate.Value.Date.AddDays(1));
            if (!string.IsNullOrWhiteSpace(phone)) q = q.Where(x => x.Phone.Contains(phone.Trim()));

            total += await q.CountAsync();

            var farmers = await q
                .OrderByDescending(x => x.SyncTime)
                .Select(f => new { f.Id, f.Phone, f.SyncTime, f.SyncData, f.Status, f.TotalItems, f.ProcessedItems, f.CreatedAt })
                .ToListAsync();

            allItems.AddRange(farmers.Select(f => new
            {
                f.Id,
                App = "Farmer",
                f.Phone,
                f.SyncTime,
                DataSize = Encoding.UTF8.GetByteCount(f.SyncData),
                f.Status,
                f.TotalItems,
                f.ProcessedItems,
                f.CreatedAt
            }));
        }

        if (includeStore)
        {
            var q = _db.SyncRawStores.AsQueryable();
            if (fromDate.HasValue) q = q.Where(x => x.SyncTime >= fromDate.Value);
            if (toDate.HasValue) q = q.Where(x => x.SyncTime < toDate.Value.Date.AddDays(1));
            if (!string.IsNullOrWhiteSpace(phone)) q = q.Where(x => x.Phone.Contains(phone.Trim()));

            total += await q.CountAsync();

            var stores = await q
                .OrderByDescending(x => x.SyncTime)
                .Select(s => new { s.Id, s.Phone, s.SyncTime, s.SyncData, s.Status, s.TotalItems, s.ProcessedItems, s.CreatedAt })
                .ToListAsync();

            allItems.AddRange(stores.Select(s => new
            {
                s.Id,
                App = "Store",
                s.Phone,
                s.SyncTime,
                DataSize = Encoding.UTF8.GetByteCount(s.SyncData),
                s.Status,
                s.TotalItems,
                s.ProcessedItems,
                s.CreatedAt
            }));
        }

        // Sort and paginate in memory
        var items = allItems
            .OrderByDescending(x => ((dynamic)x).SyncTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Ok(new { total, page, pageSize, items });
    }

    [HttpGet("{app}/{id:guid}")]
    public async Task<IActionResult> GetSyncDetail(string app, Guid id)
    {
        string? syncData = null;
        string? errorDetails = null;
        string? phone = null;
        string? status = null;
        DateTime? syncTime = null;
        int totalItems = 0, processedItems = 0;

        if (app.Equals("Farmer", StringComparison.OrdinalIgnoreCase))
        {
            var record = await _db.SyncRawFarmers.FirstOrDefaultAsync(x => x.Id == id);
            if (record == null) return NotFound();
            syncData = record.SyncData;
            errorDetails = record.ErrorDetails;
            phone = record.Phone;
            status = record.Status;
            syncTime = record.SyncTime;
            totalItems = record.TotalItems;
            processedItems = record.ProcessedItems;
        }
        else if (app.Equals("Store", StringComparison.OrdinalIgnoreCase))
        {
            var record = await _db.SyncRawStores.FirstOrDefaultAsync(x => x.Id == id);
            if (record == null) return NotFound();
            syncData = record.SyncData;
            errorDetails = record.ErrorDetails;
            phone = record.Phone;
            status = record.Status;
            syncTime = record.SyncTime;
            totalItems = record.TotalItems;
            processedItems = record.ProcessedItems;
        }
        else
        {
            return BadRequest("app must be Farmer or Store");
        }

        return Ok(new
        {
            app,
            phone,
            syncTime,
            status,
            totalItems,
            processedItems,
            dataSize = Encoding.UTF8.GetByteCount(syncData ?? ""),
            syncData = JsonNode.Parse(syncData ?? "{}"),
            errorDetails = string.IsNullOrEmpty(errorDetails)
                ? null
                : JsonNode.Parse(errorDetails!)
        });
    }
}
