using TestClientServer.Server.Data.Interfaces;
using TestClientServer.Shared.Models;

namespace TestClientServer.Server.Data.Services;

public class UtilityService : IUtilityService
{
    public string CreateEquipIdOnt(int olt, int lt, int pon, int nextAvailOnt)
    {
        return $"N{olt:00}.{lt:00}.{pon:00}.{nextAvailOnt:00}";
    }

    public string CreateEquipIdFdh(string fdh, string splitterCard, int splitterTail)
    {
        return $"F{fdh}.{splitterCard}.{splitterTail:00}";
    }

    public string CreateSplitterCard(string splitter)
    {
        var parts = splitter.Split('.'); // Split the splitter input
        var splitterCard = parts.Length > 1 ? parts[1] : ""; //splitterCard is the input after the .
        return splitterCard;
    }
    
    private static string CreatePonPath(int olt, int lt, int pon)
    {
        return $"{olt:00}.{lt:00}.{pon:00}";
    }

    public WcfMgmtEquipment? CreateOntPathWcfMgmtEquipment(int olt, int lt, int pon,
        string town, string ontEquId, int ont)
    {
        const string equClass = "F-OLT";
        const string createdBy = "wcf_app";
        var newRecord = new WcfMgmtEquipment
        {
            EquClass = equClass,
            EquId = ontEquId,
            Olt = olt,
            Lt = lt,
            Pon = pon,
            Ont = ont,
            CreatedBy = createdBy,
            CreatedDate = DateTime.Now,
            Town = town
        };
        return newRecord;
    }

    public WcfMgmtEquipment? CreateFdhPathWcfMgmtEquipment(string fdh, string town, string fdhSplitterCard,
        string fdhEquId, int splitterTail)
    {
        const string equClass = "F-FDH";
        const string createdBy = "wcf_app";
        var newRecord = new WcfMgmtEquipment
        {
            EquClass = equClass,
            EquId = fdhEquId,
            Fdh = fdh,
            SplitterCard = fdhSplitterCard,
            SplitterTail = splitterTail,
            CreatedBy = createdBy,
            CreatedDate = DateTime.Now,
            Town = town,
        };
        return newRecord;
    }


    public AvailableSignalPorts2? CreateNewPonPathAsp2(int olt, int lt, int pon, string town, string fdh, string splitter)
    {
        var ponPath = CreatePonPath(olt, lt, pon);
        var splitterCard = CreateSplitterCard(splitter);
        var nextOntEquipId = CreateEquipIdOnt(olt, lt, pon, 1);
        var nextTailEquipId = CreateEquipIdFdh(fdh, splitterCard, 1);
        var newPonPath = new AvailableSignalPorts2
        {
            Olt = olt,
            Lt = lt,
            Pon = pon,
            Town = town,
            Fdh = fdh,
            NextOntEquid = nextOntEquipId,
            Splitter = splitter,
            PonPath = ponPath,
            SplitterCard = splitterCard,
            OntCount = 16,
            AvailableOnts = 16,
            NextAvailableOnt = 1,
            SplitterTailCount = 16,
            AvailableTails = 16,
            NextAvailableTail = 1,
            NextTailEquid = nextTailEquipId
        };
        return newPonPath;
    }

}