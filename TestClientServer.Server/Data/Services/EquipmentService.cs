using Microsoft.EntityFrameworkCore;
using TestClientServer.Server.Data.Interfaces;
using TestClientServer.Shared;
using TestClientServer.Shared.Models;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace TestClientServer.Server.Data.Services;

public class EquipmentService : IEquipmentService
{
    private readonly WcfMgmtTestContext _context;
    private IEquipmentService _equipmentServiceImplementation;

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
    
    public async Task<DateTime?> GetEquipmentByEquipId(string equipId)
    {
        var connectionString = _context.Database.GetDbConnection().ConnectionString;
        await using var connection = new SqlConnection(connectionString);
 
        var command = new SqlCommand("SELECT createdDate FROM [wcfMgmt_test].[dbo].[wcfMgmtEquipments] WHERE equID = @equipId", connection);
        command.Parameters.AddWithValue("@equipId", equipId);

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();  
        await connection.CloseAsync();
        
        return result as DateTime?;
    }

    public async Task<List<WcfMgmtEquipment>> GetNewEquipmentRecords(DateTime? createdDate)
    {
        var connectionString = _context.Database.GetDbConnection().ConnectionString;
        List<WcfMgmtEquipment> equipmentList = [];
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var query = "SELECT * FROM [wcfMgmt_test].[dbo].[wcfMgmtEquipments] WHERE createdDate = @createdDate";
        var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@createdDate", createdDate.Value);
        await using (var reader = await command.ExecuteReaderAsync())
        {
            while (reader.Read())
            {
                var equipment = new WcfMgmtEquipment
                {
                    EquId = reader["equID"] as string,
                    Id = (int)reader["Id"],
                EquClass = reader["equClass"] as string,
                Serial = reader["serial"] as string,
                Mac = reader["mac"] as string,
                AssetTag = reader["assetTag"] as string,
                Manufacturer = reader["manufacturer"] as string,
                Model = reader["model"] as string,
                Ssid24 = reader["ssid24"] as string,
                Ssid5g = reader["ssid5g"] as string,
                SsidPassword = reader["ssidPassword"] as string,
                Fdh = reader["fdh"] as string,
                SplitterCard = reader["splitterCard"] as string,
                SplitterTail = reader.IsDBNull(reader.GetOrdinal("splitterTail")) ? null : reader.GetInt32(reader.GetOrdinal("splitterTail")),
                Olt = reader.IsDBNull(reader.GetOrdinal("olt")) ? null : reader.GetInt32(reader.GetOrdinal("olt")),
                Lt = reader.IsDBNull(reader.GetOrdinal("lt")) ? null : reader.GetInt32(reader.GetOrdinal("lt")),
                Pon = reader.IsDBNull(reader.GetOrdinal("pon")) ? null : reader.GetInt32(reader.GetOrdinal("pon")),
                Ont = reader.IsDBNull(reader.GetOrdinal("ont")) ? null : reader.GetInt32(reader.GetOrdinal("ont")),
                InstallDate = reader.IsDBNull(reader.GetOrdinal("installDate")) ? null : (DateTime?)reader["installDate"],
                RemovalDate = reader.IsDBNull(reader.GetOrdinal("removalDate")) ? null : (DateTime?)reader["removalDate"],
                ModifiedDate = reader.IsDBNull(reader.GetOrdinal("modifiedDate")) ? null : (DateTime?)reader["modifiedDate"],
                CreatedDate = reader.IsDBNull(reader.GetOrdinal("createdDate")) ? null : (DateTime?)reader["createdDate"],
                CreatedBy = reader["createdBy"] as string,
                ModifiedBy = reader["modifiedBy"] as string,
                RemovedBy = reader["removedBy"] as string,
                Town = reader["town"] as string
                };
                equipmentList.Add(equipment);
            }
        }
        await connection.CloseAsync();

        return equipmentList;
    }
    
}