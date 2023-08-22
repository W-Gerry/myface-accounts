using Microsoft.AspNetCore.Mvc;
using MyFace.Helpers;
using MyFace.Models.Request;
using MyFace.Models.Response;
using MyFace.Repositories;

namespace MyFace.Controllers
{
    [ApiController]
    [Route("/login")]
    public class LoginController : ControllerBase
    {    
        private readonly IUsersRepo _users;
        public LoginController(IUsersRepo users)
        {
            _users = users;
        }
        
        [HttpPost("")]
        public IActionResult Verify([FromBody] LoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (AuthHelper.VerifyCredentials(loginRequest.Username, loginRequest.Password, _users))
            {
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}