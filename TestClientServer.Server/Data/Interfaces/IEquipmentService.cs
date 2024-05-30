using TestClientServer.Shared.Models;

namespace TestClientServer.Server.Data.Interfaces;

public interface IEquipmentService
{
    Task AddPonWcfEquipments(IEnumerable<WcfMgmtEquipment> newRecord);
    Task<WcfMgmtEquipment> GetEquipmentByEquipId(string equipId);
    Task<List<WcfMgmtEquipment>> GetEquipmentByCreatedDate(DateTime? createdDate);
}