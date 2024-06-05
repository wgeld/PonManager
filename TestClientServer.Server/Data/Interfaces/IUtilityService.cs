using TestClientServer.Shared.Models;
using TestClientServer.Shared.Models.Server;

namespace TestClientServer.Server.Data.Interfaces;

public interface IUtilityService
{
    string CreateEquipIdOnt(int olt, int lt, int pon, int nextAvailOnt);
    string CreateEquipIdFdh(string fdh, string splitterCard, int splitterTail);
    string CreateSplitterCard(string splitter);
    WcfMgmtEquipment? CreateOntPathWcfMgmtEquipment(int olt, int lt, int pon, string town, string ontEquId, int ont);
    WcfMgmtEquipment? CreateFdhPathWcfMgmtEquipment(string fdh, string town, string fdhSplitterCard, string fdhEquId, int splitterTail);
    AvailableSignalPorts2? CreateNewPonPathAsp2(int olt, int lt, int pon, string town, string fdh, string splitter);
}
