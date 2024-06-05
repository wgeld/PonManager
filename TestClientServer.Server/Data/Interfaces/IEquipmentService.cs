using TestClientServer.Shared.Models;

namespace TestClientServer.Server.Data.Interfaces;

public interface IEquipmentService
{
    Task AddPonWcfEquipments(IEnumerable<WcfMgmtEquipment> newRecord);

    Task<List<WcfMgmtEquipment>> GetNewEquipmentRecords(int olt, int lt, int pon, string town, string fdh,
        string splitterCard);

    Task<WcfMgmtEquipment?> WcfGetPonDetailsAsync(int olt, int lt, int pon, string town);
}