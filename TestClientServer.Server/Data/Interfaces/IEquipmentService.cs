using TestClientServer.Shared.Models;

namespace TestClientServer.Server.Data.Interfaces;

public interface IEquipmentService
{
    Task AddPonWcfEquipments(IEnumerable<WcfMgmtEquipment> newRecord);
}