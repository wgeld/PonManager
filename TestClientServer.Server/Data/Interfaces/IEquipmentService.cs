using TestClientServer.Shared.Models.Server;

namespace TestClientServer.Server.Data.Interfaces;
public interface IEquipmentService
{
    Task AddPonWcfEquipments(IEnumerable<WcfMgmtEquipment?> newRecord);
    Task<List<WcfMgmtEquipment>> GetNewEquipmentRecords(int olt, int lt, int pon, string town, string fdh, string splitterCard);
    Task<WcfMgmtEquipment?> WcfGetOltDetailsAsync(int olt, int lt, int pon, string town);
    Task<WcfMgmtEquipment?> WcfGetFdhDetailsAsync(string fdh, string splitterCard, string town);
    Task DeletePonTagRecordEquip(IEnumerable<WcfMgmtEquipment?> deleteRecords);

}