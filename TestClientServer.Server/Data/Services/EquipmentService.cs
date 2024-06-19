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
    /*******************************************************************/
    /*********** Rewrote the Confirmation Query using ADO.NET **********/
    /*********** Return for /Confirmation Page Table *******************/
    /*******************************************************************/
    
    // public async Task<List<WcfMgmtEquipment>> GetNewEquipmentRecords(int olt, int lt, int pon, string town, string fdh, string splitterCard)
    // {
    //     List<WcfMgmtEquipment> equipmentList = [];
    //     
    //     var connectionString = _context.Database.GetDbConnection().ConnectionString;
    //     await using var connection = new SqlConnection(connectionString);
    //     await connection.OpenAsync();
    //
    //     const string oltQuery = "SELECT * FROM [wcfMgmt_test].[dbo].[wcfMgmtEquipments] " +
    //                             "WHERE olt = @olt AND lt = @lt AND pon = @pon AND town = @town";
    //     
    //     var oltCommand = new SqlCommand(oltQuery, connection);
    //     oltCommand.Parameters.AddWithValue("@olt", olt);
    //     oltCommand.Parameters.AddWithValue("@lt", lt);
    //     oltCommand.Parameters.AddWithValue("@pon", pon);
    //     oltCommand.Parameters.AddWithValue("@town", town);
    //     
    //     const string fdhQuery = "SELECT * FROM [wcfMgmt_test].[dbo].[wcfMgmtEquipments] " +
    //                             "WHERE fdh = @fdh AND splitterCard = @splitterCard AND town = @town";
    //
    //     var fdhCommand = new SqlCommand(fdhQuery, connection);
    //     fdhCommand.Parameters.AddWithValue("@fdh", fdh);
    //     fdhCommand.Parameters.AddWithValue("@splitterCard", splitterCard);
    //     fdhCommand.Parameters.AddWithValue("@town", town);
    //     
    //     await using (var oltReader = await oltCommand.ExecuteReaderAsync())
    //     {
    //         while (oltReader.Read())
    //         {
    //             var oltEquipment = new WcfMgmtEquipment
    //             {
    //                 EquId = oltReader["equID"] as string,
    //                 Id = (int)oltReader["Id"],
    //             EquClass = oltReader["equClass"] as string,
    //             Olt = oltReader.IsDBNull(oltReader.GetOrdinal("olt")) ? null : oltReader.GetInt32(oltReader.GetOrdinal("olt")),
    //             Lt = oltReader.IsDBNull(oltReader.GetOrdinal("lt")) ? null : oltReader.GetInt32(oltReader.GetOrdinal("lt")),
    //             Pon = oltReader.IsDBNull(oltReader.GetOrdinal("pon")) ? null : oltReader.GetInt32(oltReader.GetOrdinal("pon")),
    //             Ont = oltReader.IsDBNull(oltReader.GetOrdinal("ont")) ? null : oltReader.GetInt32(oltReader.GetOrdinal("ont")),
    //             CreatedDate = oltReader.IsDBNull(oltReader.GetOrdinal("createdDate")) ? null : (DateTime?)oltReader["createdDate"],
    //             Town = oltReader["town"] as string
    //             };
    //             equipmentList.Add(oltEquipment);
    //         }
    //     }
    //     await using (var fdhReader = await fdhCommand.ExecuteReaderAsync())
    //     {
    //         while (fdhReader.Read())
    //         {
    //             var fdhEquipment = new WcfMgmtEquipment
    //             { 
    //                 EquId = fdhReader["equID"] as string,
    //                 Id = (int)fdhReader["Id"],
    //              EquClass = fdhReader["equClass"] as string, 
    //              Fdh = fdhReader["fdh"] as string,
    //              SplitterCard = fdhReader["splitterCard"] as string,
    //              SplitterTail = fdhReader.IsDBNull(fdhReader.GetOrdinal("splitterTail")) ? null : fdhReader.GetInt32(fdhReader.GetOrdinal("splitterTail")),
    //              Town = fdhReader["town"] as string
    //             };
    //             equipmentList.Add(fdhEquipment);
    //         }
    //     }
    //     await connection.CloseAsync();
    //     
    //     return equipmentList;
    // }
    
}
