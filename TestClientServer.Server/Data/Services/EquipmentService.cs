using Microsoft.EntityFrameworkCore;
using TestClientServer.Server.Data.Interfaces;
using TestClientServer.Shared;
using TestClientServer.Shared.Models;

namespace TestClientServer.Server.Data.Services;

public class EquipmentService : IEquipmentService
{
    private readonly WcfMgmtTestContext _context;

    public EquipmentService(WcfMgmtTestContext context)
    {
        _context = context;
        _context.Database.SetCommandTimeout(180);
    }
    
    public async Task AddPonWcfEquipments(IEnumerable<WcfMgmtEquipment> newRecords)
    {
        const string disableTriggerSql = "DISABLE TRIGGER calculateAvailablePorts ON wcfMgmtEquipments";
        await _context.Database.ExecuteSqlRawAsync(disableTriggerSql);

        _context.WcfMgmtEquipments.AddRange(newRecords);
        await _context.SaveChangesAsync();

        const string enableTriggerSql = "ENABLE TRIGGER calculateAvailablePorts ON wcfMgmtEquipments";
        await _context.Database.ExecuteSqlRawAsync(enableTriggerSql);

    }
    public async Task<WcfMgmtEquipment> GetEquipmentByEquipId(string equipId)
    {
        return await _context.WcfMgmtEquipments
            .Where(e => e.EquId == equipId)
            .FirstOrDefaultAsync() ?? throw new InvalidOperationException();
    }

    public async Task<List<WcfMgmtEquipment>> GetEquipmentByCreatedDate(DateTime? createdDate)
    {
        return await _context.WcfMgmtEquipments
            .Where(e => e.CreatedDate == createdDate)
            .ToListAsync();
    }
}