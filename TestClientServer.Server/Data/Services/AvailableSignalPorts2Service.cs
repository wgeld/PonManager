using Microsoft.EntityFrameworkCore;
using TestClientServer.Server.Data.Interfaces;
using TestClientServer.Shared.Models;
using TestClientServer.Shared.Models.DBContext;
using TestClientServer.Shared.Models.Server;

namespace TestClientServer.Server.Data.Services;

public class AvailableSignalPorts2Service : IAvailableSignalPorts2Service
{
    private readonly WcfMgmtTestContext _context;
    public AvailableSignalPorts2Service(WcfMgmtTestContext context)
    {
        _context = context;
    }
    public async Task<AvailableSignalPorts2?> Asp2GetPonDetailsAsync(int olt, int lt, int pon, string town, string fdh, string splitter)
    {
        return await _context.AvailableSignalPorts2s.FirstOrDefaultAsync(x =>
            x.Olt == olt && x.Lt == lt && x.Pon == pon && x.Town == town && x.Fdh == fdh && x.Splitter == splitter);
    }

    // Method to add a new PON path
    public async Task<AvailableSignalPorts2?> AddNewPonPathAsp2(AvailableSignalPorts2? newRecord)
    {
        // Add the new record to the DbSet
        var addedEntity = _context.AvailableSignalPorts2s.Add(newRecord);
        // Save changes to the database
        await _context.SaveChangesAsync();
        // Return the added entity, which now includes the generated ID (if applicable)
        return addedEntity.Entity;
    }
}