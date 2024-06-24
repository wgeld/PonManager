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
            x.Olt == olt && x.Olt != null 
            && x.Lt == lt && x.Lt != null
            && x.Pon == pon && x.Pon != null
            && x.Town == town && x.Town != null 
            && x.Fdh == fdh && x.Fdh != null
            && x.Splitter == splitter && x.Splitter != null);
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

    public async Task DeletePonTagRecord(int olt, int lt, int pon, string town, string fdh, string splitterCard)
    {
        var ponTag = await context.AvailableSignalPorts2s.FirstOrDefaultAsync(pt =>
            pt.Olt == olt &&
            pt.Lt == lt &&
            pt.Pon == pon &&
            pt.Town == town &&
            pt.Fdh == fdh &&
            pt.SplitterCard == splitterCard);

        if (ponTag != null) context.AvailableSignalPorts2s.Remove(ponTag);
        await context.SaveChangesAsync();
    }
}