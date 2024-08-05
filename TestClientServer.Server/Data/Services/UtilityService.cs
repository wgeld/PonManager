using TestClientServer.Server.Data.Interfaces;
using TestClientServer.Shared.Models.Server;

namespace TestClientServer.Server.Data.Services;
public class UtilityService : IUtilityService
{
    /*******************************************************************/
    /********* Create the Equipment ID for an ONT Path *****************/
    /*******************************************************************/
    public string CreateEquipIdOnt(int olt, int lt, int pon, int nextAvailOnt, string town)
    {
        return town switch
        {
            "Westfield" => $"N{olt:00}.{lt:00}.{pon:00}.{nextAvailOnt:00}",
            "WestSpringfield" => $"WS{olt:00}.{lt:00}.{pon:00}.{nextAvailOnt:00}",
            _ => throw new ArgumentException("Invalid town specified")
        };
    }
    /*******************************************************************/
    /********* Create the Equipment ID for a FDH Path ******************/
    /*******************************************************************/
    public string CreateEquipIdFdh(string fdh, string splitterCard, int splitterTail, string town)
    {
        return town switch
        {
            "Westfield" => $"F{fdh}.{splitterCard}.{splitterTail:00}",
            "WestSpringfield" => $"WS-{fdh}.{splitterCard}.{splitterTail:00}",
            _ => throw new ArgumentException("Invalid town specified")
        };
    }
    /*******************************************************************/
    /******** Split the Splitter Input to get the SplitterCard *********/
    /*******************************************************************/
    public string CreateSplitterCard(string splitter)
    {
        var parts = splitter.Split('.'); // Split the splitter input
        var splitterCard = parts.Length > 1 ? parts[1] : ""; //splitterCard is the input after the .
        return splitterCard;
    }
    /*******************************************************************/
    /************* Create the PonPath for ASP2 Table *******************/
    /*******************************************************************/
    private static string CreatePonPath(int olt, int lt, int pon)
    {
        return $"{olt:00}.{lt:00}.{pon:00}";
    }
    /*******************************************************************/
    /********* Create the New ONT Path to add to the List **************/
    /********** This list gets sent to the Equipments Table ************/
    /*******************************************************************/
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
    /*******************************************************************/
    /********* Create the New FDH Path to add to the List **************/
    /********** This list gets sent to the Equipments Table ************/
    /*******************************************************************/
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
    /*******************************************************************/
    /********* Create the New ASP2 Path to add to the List *************/
    /********** This Path gets sent to the ASP2 Table ******************/
    /*******************************************************************/
    public AvailableSignalPorts2? CreateNewPonPathAsp2(int olt, int lt, int pon, string town, string fdh, string splitter)
    {
        var ponPath = CreatePonPath(olt, lt, pon);
        var splitterCard = CreateSplitterCard(splitter);
        var nextOntEquipId = CreateEquipIdOnt(olt, lt, pon, 1, town);
        var nextTailEquipId = CreateEquipIdFdh(fdh, splitterCard, 1, town);
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