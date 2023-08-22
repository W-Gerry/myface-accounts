using Microsoft.AspNetCore.Mvc;
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
            if (AuthHelper.IsAuthenticated(Request, _users))
            {
                var users = _users.Search(searchRequest);
                var userCount = _users.Count(searchRequest);
                return UserListResponse.Create(searchRequest, users, userCount);
            } else
            {
                return Unauthorized();
            }
        }

        [HttpGet("{id}")]
        public ActionResult<UserResponse> GetById([FromRoute] int id)
        {
            if (AuthHelper.IsAuthenticated(Request, _users))
            {
                var user = _users.GetById(id);
                return new UserResponse(user);               
            } else
            {
                return Unauthorized();
            }
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] CreateUserRequest newUser)
        {   
                
                var user = _users.Create(newUser);

                var url = Url.Action("GetById", new { id = user.Id });
                var responseViewModel = new UserResponse(user);
                return Created(url, responseViewModel);              

        }

        [HttpPatch("{id}/update")]
        public ActionResult<UserResponse> Update([FromRoute] int id, [FromBody] UpdateUserRequest update)
        {
            if (AuthHelper.IsAuthenticated(Request, _users))
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = _users.Update(id, update);
                return new UserResponse(user);        
            } else
            {
                return Unauthorized();
            }
            
        }
        
        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            if (AuthHelper.IsAuthenticated(Request, _users))
            {
                _users.Delete(id);
                return Ok();            
            } else
            {
                return Unauthorized();
            }
        }
    }
}