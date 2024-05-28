using Microsoft.AspNetCore.Mvc;
using TestClientServer.Server.Data.Interfaces;
using TestClientServer.Shared.Models;

namespace TestClientServer.Server.Controllers;

[ApiController]
[Route("formdata")]

public class PonTagController(
    IAvailableSignalPorts2Service asp2Service,
    IEquipmentService equipmentService,
    IUtilityService utilityService) 
    : ControllerBase
{
    [HttpPost]
        public async Task<IActionResult> PostPonDetailsAsync(int olt, int lt, int pon, string town, string fdh, string splitter)
    {
        try
        {
            var checkResult = await CheckAsp2Path(olt, lt, pon, town, fdh, splitter);
            if (checkResult is OkObjectResult) 
                return Ok("PON Path already exists in AvailableSignalPorts2.");

            var createResult = await CreateAsp2Path(olt, lt, pon, town, fdh, splitter);
            if (createResult is not OkObjectResult) 
                return BadRequest("Error creating PON path");

            var newRecords = await GenerateAndAddEquipmentRecords(olt, lt, pon, town, fdh, splitter);
            return Ok(new { Records = newRecords, Message = "PON details processed successfully." });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request: " + ex.Message);
        }
    }

    private async Task<IActionResult> CheckAsp2Path(int olt, int lt, int pon, string town, string fdh, string splitter)
    {
        var detailsAsp2 = await asp2Service.Asp2GetPonDetailsAsync(olt, lt, pon, town, fdh, splitter);
        return detailsAsp2 == null ? NotFound() : Ok(detailsAsp2);
    }

    private async Task<IActionResult> CreateAsp2Path(int olt, int lt, int pon, string town, string fdh, string splitter)
    {
        var newPonPath = utilityService.CreateNewPonPathAsp2(olt, lt, pon, town, fdh, splitter);
        await asp2Service.AddNewPonPathAsp2(newPonPath);
        return Ok(newPonPath);
    }

    private async Task<List<WcfMgmtEquipment>> GenerateAndAddEquipmentRecords(int olt, int lt, int pon, string town, string fdh, string splitter)
    {
        var newRecord = new List<WcfMgmtEquipment>();
        for (int i = 1; i <= 16; i++)
        {
            newRecord.Add(CreateOntPath(olt, lt, pon, town, i));
            newRecord.Add(CreateFdhPath(fdh, town, splitter, i));
        }
        await equipmentService.AddPonWcfEquipments(newRecord);
        return newRecord;
    }

    private WcfMgmtEquipment CreateOntPath(int olt, int lt, int pon, string town, int nextAvailOnt)
    {
        var ontEquId = utilityService.CreateEquipIdOnt(olt, lt, pon, nextAvailOnt);
        return utilityService.CreateOntPathWcfMgmtEquipment(olt, lt, pon, town, ontEquId, nextAvailOnt);
    }

    private WcfMgmtEquipment CreateFdhPath(string fdh, string town, string splitter, int splitterTail)
    {
        var fdhSplitterCard = utilityService.CreateSplitterCard(splitter);
        var fdhEquId = utilityService.CreateEquipIdFdh(fdh, fdhSplitterCard, splitterTail);
        return utilityService.CreateFdhPathWcfMgmtEquipment(fdh, town, fdhSplitterCard, fdhEquId, splitterTail);
    }
}

