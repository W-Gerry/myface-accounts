using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using MyFace.Models.Database;
using MyFace.Models.Request;
using MyFace.Models.Response;
using MyFace.Repositories;
using System;
using MyFace.Helpers;

namespace MyFace.Controllers
{
    [ApiController]
    [Route("/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersRepo _users;

        public UsersController(IUsersRepo users)
        {
            _users = users;
        }
        
        [HttpGet("")]
        public ActionResult<UserListResponse> Search([FromQuery] UserSearchRequest searchRequest)
        {
            string username = "";
            string password = "";

            if(Request.Headers.TryGetValue("Authorization", out var authorization))
            {
                if (authorization.ToString().StartsWith("Basic "))
                {
                    var encodedUsernameAndPassword = authorization.ToString().Substring(6);
                    byte[] newUsernameAndPasswordBytes = Convert.FromBase64String(encodedUsernameAndPassword);
                    var decodedUsernameAndPassword = BitConverter.ToString(newUsernameAndPasswordBytes);
                    string[] separatedUsernameAndPassword = decodedUsernameAndPassword.Split(':');
                    username = separatedUsernameAndPassword[0];
                    password = separatedUsernameAndPassword[1];
                }
            } else{
                return Unauthorized();
            }            

            var users = _users.Search(searchRequest);
            foreach(User user in users )
            {
                if (user.Username == username)
                {
                    PasswordHelper passwordHelper = new();
                    var hashSubmittedPassword = passwordHelper.GenerateHash(password, user.Salt);
                    if (hashSubmittedPassword != user.HashedPassword)
                    {
                        return Unauthorized();
                    }
                }
            }

            var userCount = _users.Count(searchRequest);
            return UserListResponse.Create(searchRequest, users, userCount);
        }

        [HttpGet("{id}")]
        public ActionResult<UserResponse> GetById([FromRoute] int id)
        {
            var user = _users.GetById(id);
            return new UserResponse(user);
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] CreateUserRequest newUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var user = _users.Create(newUser);

            var url = Url.Action("GetById", new { id = user.Id });
            var responseViewModel = new UserResponse(user);
            return Created(url, responseViewModel);
        }

        [HttpPatch("{id}/update")]
        public ActionResult<UserResponse> Update([FromRoute] int id, [FromBody] UpdateUserRequest update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = _users.Update(id, update);
            return new UserResponse(user);
        }
        
        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            _users.Delete(id);
            return Ok();
        }
    }
}