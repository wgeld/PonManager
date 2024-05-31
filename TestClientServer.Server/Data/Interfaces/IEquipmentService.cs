using TestClientServer.Shared.Models;

namespace TestClientServer.Server.Data.Interfaces;

public interface IEquipmentService
{
    Task AddPonWcfEquipments(IEnumerable<WcfMgmtEquipment> newRecord);
    Task<DateTime?> GetEquipmentByEquipId(string equipId);

    Task<List<WcfMgmtEquipment>> GetNewEquipmentRecords(DateTime? createdDate);
}