using System;
using Microsoft.AspNetCore.Http;
using MyFace.Repositories;
using MyFace.Models;
using MyFace.Models.Request;
using System.Reflection.Metadata;

namespace MyFace.Helpers
{
    public static class AuthHelper
    {
        public static bool IsAuthenticated(HttpRequest request, IUsersRepo users)
        {
            if(request.Headers.TryGetValue("Authorization", out var authorization) && authorization.ToString().StartsWith("Basic "))
            {
                var encodedUsernameAndPassword = authorization.ToString().Substring(6);
                var newUsernameAndPasswordBytes = Convert.FromBase64String(encodedUsernameAndPassword);
                var decodedUsernameAndPassword = System.Text.Encoding.UTF8.GetString(newUsernameAndPasswordBytes);
                string[] separatedUsernameAndPassword = decodedUsernameAndPassword.Split(':');
                var username = separatedUsernameAndPassword[0];
                var password = separatedUsernameAndPassword[1];
                return VerifyCredentials(username, password, users);
            } else
            {
                return false;
            }       
        }

        public static bool VerifyCredentials(string username, string password, IUsersRepo usersRepo)
        {
            var matchingUser = usersRepo.GetByUsername(username);

            if (matchingUser == null || PasswordHelper.GenerateHash(password, matchingUser.Salt) != matchingUser.HashedPassword)
            {
                return false;
            }
            return true;
        }
        
    }
}