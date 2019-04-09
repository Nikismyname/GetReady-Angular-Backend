#region Init
namespace GetReady.Web.Controllers
{
    using System;
    using GetReady.Services.Contracts;
    using GetReady.Services.Models.QuestionSsheetModels;
    using GetReady.Services.Utilities;
    using GetReady.Web.Middleware;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class QuestionSheetController : Controller
    {
        private readonly IJwtService jwtService;
        private readonly IQuestionSheetService questionSheetService;

        public QuestionSheetController(IJwtService jwtService, IQuestionSheetService questionSheetService)
        {
            this.jwtService = jwtService;
            this.questionSheetService = questionSheetService;
        }
        #endregion

        #region getOne
        [HttpGet("GetOnePublic/{id:int}")]
        public IActionResult GetOnePublic(int id)
        {
            try
            {
                var result = questionSheetService.GetOnePublic(id);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [ClaimRequirement(Constants.RoleType, "User")]
        [HttpGet("GetOnePersonal/{id:int}")]
        public IActionResult GetOnePersonal(int id)
        {
            try
            {
                var userData = jwtService.ParseData(this.User);
                var result = questionSheetService.GetOnePersonal(id, userData.UserId);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        #endregion

        #region Create
        [HttpPost]
        [Route("[action]")]
        [ClaimRequirement(Constants.RoleType, "Admin")]
        public IActionResult CreateGlobalSheet([FromBody] QuestionSheetCreate data)
        {
            try
            {
                var result = questionSheetService.CreateGlobal(data);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        [ClaimRequirement(Constants.RoleType, "User")]
        public IActionResult CreatePersonalSheet([FromBody] QuestionSheetCreate data)
        {
            try
            {
                var userData = jwtService.ParseData(this.User);
                var result = questionSheetService.CreatePersonal(data, userData.UserId);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        #endregion

        #region Edit
        [HttpPost]
        [Route("[action]")]
        [ClaimRequirement(Constants.RoleType, "Admin")]
        public IActionResult EditGlobal([FromBody] QuestionSheetEdit data)
        {
            try
            {
                questionSheetService.EditGlobal(data);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        [ClaimRequirement(Constants.RoleType, "User")]
        public IActionResult EditPersonal([FromBody] QuestionSheetEdit data)
        {
            try
            {
                var userData = jwtService.ParseData(this.User);
                questionSheetService.EditPersonal(data, userData.UserId);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        #endregion

        #region GetAll 
        [HttpGet("GetAllGlobal")]
        public IActionResult GetAllGlobal()
        {
            try
            {
                var result = questionSheetService.GetAllItemsGlobal();
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetAllFoldersGlobal")]
        public IActionResult GetAllFoldersGlobal()
        {
            try
            {
                var result = questionSheetService.GetAllFoldersGlobal();
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [ClaimRequirement(Constants.RoleType, "User")]
        [HttpGet("GetAllPersonal")]
        public IActionResult GetAllPersonal()
        {
            try
            {
                var userData = jwtService.ParseData(this.User);
                var result = questionSheetService.GetAllFoldersPersonal(userData.UserId);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        #endregion

        #region Get Index
        [HttpGet("GetGlobalIndex/{id:int}")]
        public IActionResult GetGlobalIndex(int id)
        {
            try
            {
                var result = questionSheetService.GetGlobalSheetIndex(id);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [ClaimRequirement(Constants.RoleType, "User")]
        [HttpGet("GetPersonalIndex/{id:int}")]
        public IActionResult GetPersonalIndex(int id)
        {
            try
            {
                var userData = jwtService.ParseData(this.User);
                var result = questionSheetService.GetPersonalSheetIndex(id, userData.UserId);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        #endregion

        #region Delete
        [HttpPost]
        [Route("[action]")]
        [ClaimRequirement(Constants.RoleType, "Admin")]
        public IActionResult DeleteGlobal([FromBody] int id)
        {
            try
            {
                questionSheetService.DeleteGlobal(id);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        [ClaimRequirement(Constants.RoleType, "User")]
        public IActionResult DeletePersonal([FromBody] int id)
        {
            try
            {
                var userData = jwtService.ParseData(this.User);
                questionSheetService.DeletePersonal(id, userData.UserId);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        #endregion

        #region Reorder
        [HttpPost]
        [Route("[action]")]
        [ClaimRequirement(Constants.RoleType, "Admin")]
        public IActionResult ReorderGlobal([FromBody] ReorderSheet data)
        {
            try
            {
                questionSheetService.ReorderGlobal(data);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        [ClaimRequirement(Constants.RoleType, "User")]
        public IActionResult ReorderPersonal([FromBody] ReorderSheet data)
        {
            try
            {
                var userData = jwtService.ParseData(this.User);
                questionSheetService.ReorderPesonal(data, userData.UserId);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        #endregion

        #region Get Q Ids for Sheeth
        [ClaimRequirement(Constants.RoleType, "User")]
        [HttpGet("GetQuestionIdsForSheet/{id:int}")]
        public IActionResult GetQuestionIdsForSheet(int id)
        {
            try
            {
                var userData = jwtService.ParseData(this.User);
                var result = questionSheetService.GetQuestionIds(id, userData.UserId);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        #endregion
    }
}