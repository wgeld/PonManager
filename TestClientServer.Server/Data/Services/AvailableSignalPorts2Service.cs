using Microsoft.EntityFrameworkCore;
using TestClientServer.Server.Data.Interfaces;
using TestClientServer.Shared.Models.DBContext;
using TestClientServer.Shared.Models.Server;

namespace TestClientServer.Server.Data.Services;
public class AvailableSignalPorts2Service(WcfMgmtTestContext context) : IAvailableSignalPorts2Service
{
    /*******************************************************************/
    /********* Search AvailableSignalPorts for existing PON Path *******/
    /*******************************************************************/
    public async Task<AvailableSignalPorts2?> Asp2GetPonDetailsAsync(int olt, int lt, int pon, string town, string fdh, string splitter)
    {
        return await context.AvailableSignalPorts2s.FirstOrDefaultAsync(x =>
            x.Olt == olt && x.Lt == lt && x.Pon == pon && x.Town == town && x.Fdh == fdh && x.Splitter == splitter);
    }
    /*******************************************************************/
    /********* Add List of Equipment Records to WCFEquip Table *********/
    /*******************************************************************/
    public async Task<AvailableSignalPorts2?> AddNewPonPathAsp2(AvailableSignalPorts2? newRecord)
    {
        var addedEntity = context.AvailableSignalPorts2s.Add(newRecord!);
        await context.SaveChangesAsync();
        return addedEntity.Entity;
    }
}