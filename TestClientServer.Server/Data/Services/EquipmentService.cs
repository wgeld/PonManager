﻿using System.Diagnostics.CodeAnalysis;
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

        var sql = $"DELETE FROM [wcfMgmt].[dbo].[wcfMgmtEquipments] WHERE {string.Join(" OR ", conditions)}";
        
        await _context.Database.ExecuteSqlRawAsync(sql, parameters);
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
