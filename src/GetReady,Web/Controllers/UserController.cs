namespace GetReady.Web.Controllers
{
    using System;
    using GetReady.Services.Contracts;
    using GetReady.Services.Models.UserModels;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService userService;
        private readonly IJwtService jwtService;

        public UserController(IUserService userService, IJwtService jwtService)
        {
            this.userService = userService;
            this.jwtService = jwtService;
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Login ([FromBody] UserLoginDto data)
        {
            try
            {
                var result = userService.Login(data);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Register([FromBody] UserRegisterDto data)
        {
            try
            {
                var result = userService.Register(data);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}