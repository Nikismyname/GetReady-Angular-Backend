namespace GetReady.Services.Implementations
{
    using Contracts;
    using GetReady.Data.Models.UserModels;
    using System;
    using GetReady.Services.Models.UserModels;
    using System.Security.Cryptography;
    using Microsoft.AspNetCore.Cryptography.KeyDerivation;
    using GetReady.Data;
    using System.Linq;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.IdentityModel.Tokens.Jwt;
    using GetReady.Services.Exceptions;

    public class UserService : IUserService
    {
        const int HashingIterationsCount = 1000;
        const int SaltNumberOfBytes = 128 / 8;
        const int PasswordNumberOfBytes = 256 / 8;
        const KeyDerivationPrf Algorithm = KeyDerivationPrf.HMACSHA1;

        private readonly GetReadyDbContext context;
        private readonly IConfiguration configuration;
        private readonly IQuestionSheetService questionSheetService;

        public UserService(GetReadyDbContext context,
            IConfiguration configuration,
            IQuestionSheetService questionSheetService)
        {
            this.context = context;
            this.configuration = configuration;
            this.questionSheetService = questionSheetService;
        }

        public UserWithTokenDto Register(UserRegisterDto data, bool admin = false)
        {
            if (data.Password != data.RepeatPassword)
            {
                throw new ServiceException("Password and Repeate Password must match");
            };

            var existingUser = context.Users
                .SingleOrDefault(x => x.Username == data.Username);

            if (existingUser != null)
            {
                throw new ServiceException("User with the given name already Exists!");
            }

            byte[] salt = new byte[SaltNumberOfBytes];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            var saltString = Convert.ToBase64String(salt);

            string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2
                (
                    password: data.Password,
                    salt: salt,
                    prf: Algorithm,
                    iterationCount: HashingIterationsCount,
                    numBytesRequested: PasswordNumberOfBytes
                 )
            );

            var user = new User()
            {
                Username = data.Username,
                FirstName = data.FirstName,
                LastName = data.LastName,
                HashedPassword = hashedPassword,
                Salt = saltString,
                Role = admin ? "Admin" : "User",
            };

            try
            {
                context.Users.Add(user);
                context.SaveChanges();
            }
            catch
            {
                throw new ServiceException("Interna Error!");
            }

            ///Creating The Personal Question Sheet Root for the user; 
            this.questionSheetService.CreateRoot(user.Id);

            var token = GenerateToken(user);

            var userWithToken = new UserWithTokenDto
            {
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                Token = token,
            };

            return userWithToken;
        }

        public UserWithTokenDto Login(UserLoginDto data)
        {
            var user = context.Users.SingleOrDefault(x => x.Username == data.Username);
            if (user == null)
            {
                throw new ServiceException("Invalid username or password!");
            }

            var salt = Convert.FromBase64String(user.Salt);
            var hashedPassword = user.HashedPassword;

            string hashedIncomingPassword = Convert.ToBase64String(
                KeyDerivation.Pbkdf2(
                    password: data.Password,
                    salt: salt,
                    prf: Algorithm,
                    iterationCount: HashingIterationsCount,
                    numBytesRequested: PasswordNumberOfBytes
                )
            );

            if (hashedPassword != hashedIncomingPassword)
            {
                throw new ServiceException("Invalid username or password!");
            }

            var token = GenerateToken(user);

            var userWithToken = new UserWithTokenDto
            {
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role == null ? "User" : user.Role,
                Token = token,
            };

            return userWithToken;
        }

        private string GenerateToken(User user)
        {
            var signingKey = Convert.FromBase64String(configuration["Jwt:SigningSecret"]);
            var expiryDuration = int.Parse(configuration["Jwt:ExpiryDuration"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = null,              // Not required as no third-party is involved
                Audience = null,            // Not required as no third-party is involved
                IssuedAt = DateTime.UtcNow,
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(expiryDuration),
                Subject = new ClaimsIdentity(new List<Claim> {
                        new Claim("userid", user.Id.ToString()),
                        new Claim("role", user.Role == null? "User": user.Role)
                    }),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(signingKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = jwtTokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            var token = jwtTokenHandler.WriteToken(jwtToken);
            return token;
        }
    }
}
