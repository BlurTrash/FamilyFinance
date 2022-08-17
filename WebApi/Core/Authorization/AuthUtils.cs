using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WebApi.Database.Models;

namespace WebApi.Core.Authorization
{
    public static class AuthUtils
    {
        public static string GetHash(string input)
        {
            using var hashAlgorithm = SHA256.Create();
            //https://docs.microsoft.com/ru-ru/dotnet/api/system.security.cryptography.hashalgorithm.computehash?view=net-6.0
            // Convert the input string to a byte array and compute the hash.
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        /// <summary>
        /// Возвращает jwt token и создает claims для текущего пользлвателя 
        /// </summary>
        public static string GetClaimsIdentity(ApplicationContext db, string login, string password)
        {
            var passwordHash = GetHash(password);

            var user = db.Users.FirstOrDefault(u => u.Login == login && u.Password == passwordHash);

            if (user == null) return null;


            var claims = new List<Claim>();

            // Добавляем к утверждениям логин пользователя
            claims.Add(new Claim(type: ClaimsIdentity.DefaultNameClaimType, user.Login));

            var options = new JsonSerializerOptions()
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true
            };
            // Добавляем к утверждениям информацию о пользователе
            claims.Add(new Claim("user", JsonSerializer.Serialize(user, options)));

            var claimsIdentity = new ClaimsIdentity(
                claims, "jwt",
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType
                );

            var now = DateTime.UtcNow;

            // Создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: claimsIdentity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }


        /// <summary>
        /// Метод расширения извлекающий информацию о текущем пользователе из claims
        /// </summary>
        /// <param name="claimsPrincipal"></param>
        /// <returns></returns>
        public static User CurrentUser(this ClaimsPrincipal claimsPrincipal)
        {
            string claim = claimsPrincipal.FindFirst(p => p.Type == "user")?.Value;

            if (claim != null)
            {
                User user = JsonSerializer.Deserialize<User>(claim);
                return user;
            }
            else
            {
                return null;
            }
        }
    }
}
