using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestClientServer.Server.Data.Interfaces;
using Microsoft.Data.SqlClient;
using TestClientServer.Shared.Models.DBContext;
using TestClientServer.Shared.Models.Server;

namespace TestClientServer.Server.Data.Services;
public class EquipmentService : IEquipmentService
{
    private readonly WcfMgmtTestContext _context;
    public EquipmentService(WcfMgmtTestContext context)
    {
        _context = context;
        _context.Database.SetCommandTimeout(180);
    }
    /*******************************************************************/
    /*********** Search WcfEquipments for Existing OLT Path ************/
    /*******************************************************************/
    public async Task<WcfMgmtEquipment?> WcfGetOltDetailsAsync(int olt, int lt, int pon, string town)
    {
        if (string.IsNullOrEmpty(town))
        {
            return null;
        }
        return await _context.WcfMgmtEquipments.FirstOrDefaultAsync(x =>
            x.Olt == olt && x.Lt == lt && x.Pon == pon && x.Town == town);
    }
    /*******************************************************************/
    /*********** Search WcfEquipments for Existing FDH Path ************/
    /*******************************************************************/
    public async Task<WcfMgmtEquipment?> WcfGetFdhDetailsAsync(string fdh, string splitterCard, string town)
    {
        if (string.IsNullOrEmpty(fdh) || string.IsNullOrEmpty(splitterCard) || string.IsNullOrEmpty(town))
        {
            return null;
        }
        return await _context.WcfMgmtEquipments.FirstOrDefaultAsync(x =>
            x.Fdh == fdh && x.SplitterCard == splitterCard && x.Town == town);
    }
    
    /*******************************************************************/
    /********* Delete Pon Tag Records from Equipments Table ************/
    /*******************************************************************/
    public async Task DeletePonTagRecordEquip(List<WcfMgmtEquipment?> deleteRecords)
    {
        // Filter out any null entries or entries with null EquId or Town
        var validRecords = deleteRecords.Where(r => r != null && r.EquId != null && r.Town != null).ToList();

        var parameters = new List<SqlParameter>();
        var conditions = new List<string>();

        var paramIndex = 0;
        foreach (var record in validRecords)
        {
            var equIdParamName = $"@equId{paramIndex}";
            var townParamName = $"@town{paramIndex}";

            parameters.Add(new SqlParameter(equIdParamName, record!.EquId));
            parameters.Add(new SqlParameter(townParamName, record.Town));

            conditions.Add($"(EquId = {equIdParamName} AND Town = {townParamName})");
        
            paramIndex++;
        }

        var sql = $"DELETE FROM [wcfMgmt_test].[dbo].[wcfMgmtEquipments] WHERE {string.Join(" OR ", conditions)}";
        
        await _context.Database.ExecuteSqlRawAsync(sql, parameters);
    }
    /*******************************************************************/
    /********* Get a List of the Recently Created Pons  ****************/
    /*******************************************************************/
    public async Task<List<WcfMgmtEquipment?>> GetRecentPonRecords(int recentTimeFrame)
    {
        DateTime startDate = DateTime.Now.AddDays(-recentTimeFrame);
    
        string fdhSql = @"
            SELECT 
                EquClass, 
                Fdh, 
                SplitterCard,  
                MAX(CreatedDate) AS CreatedDate, 
                CreatedBy, 
                Town
            FROM 
                [wcfMgmt_test].[dbo].[wcfMgmtEquipments]
            WHERE 
                EquClass = 'F-FDH' 
                AND CreatedDate >= @startDate
            GROUP BY 
                EquClass, Fdh, SplitterCard, CreatedBy, Town";

        var parameters = new[] { new SqlParameter("@startDate", startDate) };

        var fdhResults = await _context.Set<WcfMgmtEquipment>()
            .FromSqlRaw(fdhSql, parameters)
            .Select(e => new WcfMgmtEquipment
            {
                EquClass = e.EquClass,
                Fdh = e.Fdh,
                SplitterCard = e.SplitterCard,
                CreatedDate = e.CreatedDate,
                CreatedBy = e.CreatedBy,
                Town = e.Town
            })
            .ToListAsync();

        string oltSql = @"
                SELECT 
                    EquClass, 
                    Olt, 
                    Lt, 
                    Pon, 
                    MAX(CreatedDate) AS CreatedDate, 
                    CreatedBy, 
                    Town
                FROM 
                    [wcfMgmt_test].[dbo].[wcfMgmtEquipments]
                WHERE 
                    EquClass = 'F-OLT' 
                    AND CreatedDate >= @startDate
                GROUP BY 
                    EquClass, Olt, Lt, Pon, CreatedBy, Town";
        
        var oltParameters = new[] { new SqlParameter("@startDate", startDate) };
        
        var oltResults = await _context.Set<WcfMgmtEquipment>()
            .FromSqlRaw(oltSql, oltParameters)
            .Select(e => new WcfMgmtEquipment
            {
                EquClass = e.EquClass,
                Olt = e.Olt,
                Lt = e.Lt,
                Pon = e.Pon,
                CreatedDate = e.CreatedDate,
                CreatedBy = e.CreatedBy,
                Town = e.Town
            })
            .ToListAsync();

        return fdhResults.Concat(oltResults).ToList();
    }
    
    /*******************************************************************/
    /********* Add List of Equipment Records to WCFEquip Table *********/
    /*******************************************************************/
    public async Task AddPonWcfEquipments(IEnumerable<WcfMgmtEquipment?> newRecords)
    {
        const string disableTriggerSql = "DISABLE TRIGGER calculateAvailablePorts ON wcfMgmtEquipments";
        await _context.Database.ExecuteSqlRawAsync(disableTriggerSql);

        _context.WcfMgmtEquipments.AddRange(newRecords!);
        await _context.SaveChangesAsync();

        const string enableTriggerSql = "ENABLE TRIGGER calculateAvailablePorts ON wcfMgmtEquipments";
        await _context.Database.ExecuteSqlRawAsync(enableTriggerSql);
    }
    /*******************************************************************/
    /*********** Add Newly Created Equip Records to a List *************/
    /*********** Return for /Confirmation Page Table *******************/
    /*******************************************************************/
    public async Task<List<WcfMgmtEquipment>> GetNewEquipmentRecords(int olt, int lt, int pon, string town, string fdh, string splitterCard)
    {
        var oltEquipmentQuery = _context.WcfMgmtEquipments
            .Where(e => e.Olt == olt && e.Lt == lt && e.Pon == pon && e.Town == town);

        var fdhEquipmentQuery = _context.WcfMgmtEquipments
            .Where(e => e.Fdh == fdh && e.SplitterCard == splitterCard && e.Town == town);
        
        var combinedQuery = oltEquipmentQuery.Concat(fdhEquipmentQuery);
        var equipmentList = await combinedQuery.ToListAsync();

        return equipmentList;
    }

    
}
