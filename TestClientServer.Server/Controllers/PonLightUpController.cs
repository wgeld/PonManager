using Microsoft.AspNetCore.Mvc;
using TestClientServer.Server.Data.Interfaces;
using TestClientServer.Shared.Models.Server;

namespace TestClientServer.Server.Controllers;

[ApiController]
[Route("formdata")]
    public class PonLightUpController(
        IAvailableSignalPorts2Service asp2Service,
        IEquipmentService equipmentService,
        IUtilityService utilityService) 
        : ControllerBase
    {
        /*******************************************************************/
        /*********** Master Controller. PON Form Data Passes Here **********/
        /*******************************************************************/
        [HttpPost("PostPon")]
        public async Task<IActionResult> PostPonDetailsAsync(int olt, int lt, int pon, string town, string fdh, string splitter)
        {
            try
            {
                var checkAsp2Result = await CheckAsp2Path(olt, lt, pon, town, fdh, splitter);
                if (checkAsp2Result is OkObjectResult) 
                    return  Ok("PON exists ASP2");

                var checkWcfMgmtOltResult = await CheckWcfOltPath(olt,lt,pon,town);
                if (checkWcfMgmtOltResult is OkObjectResult)
                    return Ok("PON exists WCFEquip");
                
                //Splitter Cards gets passed as (ex. '23.2A'). Need to split the splitter input at the '.' to extract the splitterCard (ex. '2A'). 
                var splitterCard = utilityService.CreateSplitterCard(splitter);
                var checkWcfMgmtFdhResult = await CheckWcfFdhPath(fdh, splitterCard, town);
                if (checkWcfMgmtFdhResult is OkObjectResult)
                    return Ok("PON exists WCFEquip");
                
                var createResult = await CreateAsp2Path(olt, lt, pon, town, fdh, splitter);
                if (createResult is not OkObjectResult) 
                    return BadRequest("Error creating PON path");

                var newRecords = await AddEquipmentRecords(olt, lt, pon, town, fdh, splitter);
                return Ok(new { Records = newRecords, Message = "PON details processed successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request: " + ex.Message);
            }
        }
        /*******************************************************************/
        /*********** Get Newly Created Equipment Records from Table ********/
        /*******************************************************************/
        [HttpGet("GetPon")] 
        public async Task<IActionResult> GetNewEquipmentRecord(int olt, int lt, int pon, string town, string fdh, string splitterCard)
        {
            try
            {
                var newRecords = await equipmentService.GetNewEquipmentRecords(olt, lt, pon, town, fdh, splitterCard);
                return Ok(newRecords);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        /*******************************************************************/
        /* Delete the Newly Created Equipments Records on Undo Submission **/
        /*******************************************************************/
        [HttpDelete("DeletePon")]
        public async Task<IActionResult> UndoEquipmentRecord(int olt, int lt, int pon, string town, string fdh,
            string splitterCard)
        {
            try
            {
                await asp2Service.DeletePonTagRecord(olt, lt, pon, town, fdh, splitterCard);
                for (int i = 1; i <= 16; i++)
                {
                    await equipmentService.DeletePonTagRecordEquip(olt, lt, pon, town, fdh, splitterCard, i);
                }
                return Ok("Undo Successful");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error");
            }
            
        }
        /*******************************************************************/
        /******* Check WCFEquipments to See If Olt Path Already Exists *****/
        /*******************************************************************/
        private async Task<IActionResult> CheckWcfOltPath(int olt, int lt, int pon, string town)
        {
            var detailsWcf = await equipmentService.WcfGetOltDetailsAsync(olt, lt, pon, town);
            return detailsWcf == null ? NotFound() : Ok(detailsWcf);
        }
        /*******************************************************************/
        /******* Check WCFEquipments to See If Fdh Path Already Exists *****/
        /*******************************************************************/
        private async Task<IActionResult> CheckWcfFdhPath(string fdh, string splitterCard, string town)
        {
            var detailsWcf = await equipmentService.WcfGetFdhDetailsAsync(fdh, splitterCard, town);
            return detailsWcf == null ? NotFound() : Ok(detailsWcf);
        }
        /*******************************************************************/
        /*********** Check ASP2 to See If Path Already Exists **************/
        /*******************************************************************/
    private async Task<IActionResult> CheckAsp2Path(int olt, int lt, int pon, string town, string fdh, string splitter)
    {
        var detailsAsp2 = await asp2Service.Asp2GetPonDetailsAsync(olt, lt, pon, town, fdh, splitter);
        return detailsAsp2 == null ? NotFound() : Ok(detailsAsp2);
    }
        /*******************************************************************/
        /*********** Create PON Path Information in ASP2 *******************/
        /*******************************************************************/
    private async Task<IActionResult> CreateAsp2Path(int olt, int lt, int pon, string town, string fdh, string splitter)
    {
        var newPonPath = utilityService.CreateNewPonPathAsp2(olt, lt, pon, town, fdh, splitter);
        await asp2Service.AddNewPonPathAsp2(newPonPath);
        return Ok(newPonPath);
    }
        /*******************************************************************/
        /***** Create & Add the 32 New Equip Records to Equip Table ********/
        /*******************************************************************/
    private async Task<List<WcfMgmtEquipment?>> AddEquipmentRecords(int olt, int lt, int pon, string town, string fdh, string splitter)
    {
        var newRecord = new List<WcfMgmtEquipment?>();
        for (var i = 1; i <= 16; i++)
        {
            newRecord.Add(CreateOntPath(olt, lt, pon,i, town));
            newRecord.Add(CreateFdhPath(fdh, splitter, i, town));
        }
        await equipmentService.AddPonWcfEquipments(newRecord);
        return newRecord;
    }
        /*******************************************************************/
        /*********** Create the ONT PON Path for Equip Table ***************/
        /*******************************************************************/
    private WcfMgmtEquipment? CreateOntPath(int olt, int lt, int pon, int nextAvailOnt, string town)
    {
        var ontEquId = utilityService.CreateEquipIdOnt(olt, lt, pon, nextAvailOnt);
        return utilityService.CreateOntPathWcfMgmtEquipment(olt, lt, pon, town, ontEquId, nextAvailOnt);
    }
        /*******************************************************************/
        /*********** Create the FDH PON Path for Equip Table ***************/
        /*******************************************************************/
    private WcfMgmtEquipment? CreateFdhPath(string fdh, string splitter, int splitterTail, string town)
    {
        var fdhSplitterCard = utilityService.CreateSplitterCard(splitter);
        var fdhEquId = utilityService.CreateEquipIdFdh(fdh, fdhSplitterCard, splitterTail);
        return utilityService.CreateFdhPathWcfMgmtEquipment(fdh, town, fdhSplitterCard, fdhEquId, splitterTail);
    }
}

