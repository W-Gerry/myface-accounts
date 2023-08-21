using System;
using Microsoft.AspNetCore.Http;
using MyFace.Repositories;
namespace MyFace.Helpers
{
    public interface IAuthHelper 
    {
        //bool IsAuthenticated { HttpRequest request }
    }

    public class AuthHelper
    {
        private readonly IUsersRepo _users;

        public AuthHelper(IUsersRepo users)
        {
            _users = users;
        }

        // public bool IsAuthenticated(HttpRequest request)
        // {
            
        // }
        
    }
}

