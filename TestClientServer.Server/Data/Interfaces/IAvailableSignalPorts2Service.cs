using TestClientServer.Shared.Models;

namespace TestClientServer.Server.Data.Interfaces;

public interface IAvailableSignalPorts2Service
{
    Task<AvailableSignalPorts2?> Asp2GetPonDetailsAsync(int olt, int lt, int pon, string town, string fdh, string splitter);
    Task<AvailableSignalPorts2?> AddNewPonPathAsp2(AvailableSignalPorts2? newRecord);

    

}