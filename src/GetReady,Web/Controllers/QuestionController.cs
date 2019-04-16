#region Inti
namespace GetReady.Web.Controllers
{
    using System;
    using GetReady.Services.Contracts;
    using GetReady.Services.Models.QuestionModels;
    using GetReady.Services.Utilities;
    using GetReady.Web.Middleware;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : Controller
    {
        private readonly IJwtService jwtService;
        private readonly IQuestionService questionService;

        public QuestionController(IJwtService jwtService, IQuestionService questionController)
        {
            this.jwtService = jwtService;
            this.questionService = questionController;
        }
        #endregion

        #region Get 
        [HttpPost]
        [Route("[action]")]
        public IActionResult GetGlobal([FromBody] int id)
        {
            try
            {
                var result = questionService.GetGlobal(id);
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
        public IActionResult GetPersonal([FromBody] int id)
        {
            try
            {
                var userData = jwtService.ParseData(this.User);
                var result = questionService.GetPersonal(id, userData.UserId);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetQuestionIdsForApproval")]
        [ClaimRequirement(Constants.RoleType, "Admin")]
        public IActionResult GetQuestionIdsForApproval()
        {
            try
            {
                var result = questionService.GetQuestionIdsForApproval();
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetAnsweredQuestions")]
        [ClaimRequirement(Constants.RoleType, "User")]
        public IActionResult GetAnsweredQuestions()
        {
            try
            {
                var userData = jwtService.ParseData(this.User);
                var result = questionService.GetAnsweredQuestions(userData.UserId);
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
                int result = questionService.DeleteGlobal(id);
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
        public IActionResult DeletePersonal([FromBody] int id)
        {
            try
            {
                var userData = jwtService.ParseData(this.User);
                int result = questionService.DeletePersonal(id, userData.UserId);
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
        public IActionResult DeleteAllPersonalForSheet([FromBody] int id)
        {
            try
            {
                var userData = jwtService.ParseData(this.User);
                questionService.DeleteAllPersonalForSheet(id, userData.UserId);
                return Ok();
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
        public IActionResult CreateGlobal([FromBody] QuestionCreate data)
        {
            try
            {
                QuestionIndexWithScope result = questionService.CreateGlobal(data);
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
        public IActionResult CreatePersonal([FromBody] QuestionCreate data)
        {
            try
            {
                var userData = jwtService.ParseData(this.User);
                QuestionIndexWithScope result = questionService.CreatePersonal(data, userData.UserId);
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
        public IActionResult EditGlobal([FromBody] QuestionEdit data)
        {
            try
            {
                QuestionIndexWithScope result = questionService.EditGlobal(data);
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
        public IActionResult EditPersonal([FromBody] QuestionEdit data)
        {
            try
            {
                var userData = jwtService.ParseData(this.User);
                QuestionIndexWithScope result = questionService.EditPersonal(data, userData.UserId);
                return Ok(result);
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
        public IActionResult ReorderGlobal([FromBody] ReorderData data)
        {
            try
            {
                questionService.ReorderGlobal(data);
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
        public IActionResult ReorderPersonal([FromBody] ReorderData data)
        {
            try
            {
                var userData = jwtService.ParseData(this.User);
                questionService.Reorder(data, userData.UserId);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        #endregion

        #region Approve/Reject Question
        [HttpPost]
        [Route("[action]")]
        [ClaimRequirement(Constants.RoleType, "Admin")]
        public IActionResult ApproveQuestion([FromBody] QuestionApprovalData data)
        {
            try
            {
                questionService.ApproveQuestion(data);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        [ClaimRequirement(Constants.RoleType, "Admin")]
        public IActionResult RejectQuestion([FromBody] int id)
        {
            try
            {
                questionService.RejectQuestion(id);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        #endregion

        #region Other
        [HttpPost]
        [Route("[action]")]
        [ClaimRequirement(Constants.RoleType, "User")]
        public IActionResult CopyQuestions([FromBody] CopyQuestions data)
        {
            try
            {
                var userData = jwtService.ParseData(this.User);
                questionService.CopyQuestions(data, userData.UserId);
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
        public IActionResult AddNewScore([FromBody] NewScoreData data)
        {
            try
            {
                var userData = jwtService.ParseData(this.User);
                questionService.AddNewScore(data, userData.UserId);
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
        public IActionResult SuggestForPublishing([FromBody] int questionId)
        {
            try
            {
                var userData = jwtService.ParseData(this.User);
                questionService.SuggestForPublishing(questionId, userData.UserId);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        #endregion
    }
}