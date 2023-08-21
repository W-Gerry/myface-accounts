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
            if(Request.Headers.TryGetValue("Authorization", out var authorization) && authorization.ToString().StartsWith("Basic "))
            {
                var encodedUsernameAndPassword = authorization.ToString().Substring(6);
                var newUsernameAndPasswordBytes = Convert.FromBase64String(encodedUsernameAndPassword);
                var decodedUsernameAndPassword = System.Text.Encoding.UTF8.GetString(newUsernameAndPasswordBytes);
                string[] separatedUsernameAndPassword = decodedUsernameAndPassword.Split(':');
                var username = separatedUsernameAndPassword[0];
                var password = separatedUsernameAndPassword[1];
                var matchingUser = _users.GetByUsername(username);

                if (matchingUser == null || PasswordHelper.GenerateHash(password, matchingUser.Salt) != matchingUser.HashedPassword)
                {
                    return Unauthorized();
                }
            } else{
                return Unauthorized();
            }            
            var users = _users.Search(searchRequest);
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