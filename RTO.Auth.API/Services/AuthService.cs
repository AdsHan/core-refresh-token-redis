using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using RTO.Auth.API.DTO;
using RTO.Auth.API.Entities;
using RTO.Auth.API.Enums;
using RTO.Auth.API.Extensions;
using RTO.Auth.API.Services;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RTO.Auth.API.Service
{
    public class AuthService
    {
        private readonly TokenConfigurations _tokenConfigurations;
        private readonly IDistributedCache _cache;
        private readonly UserManager<UserModel> _userManager;
        private readonly SignInManager<UserModel> _signInManager;

        public AuthService(TokenConfigurations tokenConfigurations, IDistributedCache cache, UserManager<UserModel> userManager, SignInManager<UserModel> signInManager)
        {
            _tokenConfigurations = tokenConfigurations;
            _cache = cache;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<ValidateResult> ValidateCredentialsAsync(AccessCredentialsDTO credentials)
        {
            var validate = new ValidateResult();

            // Verifica se o usuário existe
            var isIncluded = await _userManager.FindByEmailAsync(credentials.Email);

            if (isIncluded == null)
            {
                validate.AddProcessingError("Este usuário não existe!");
                return validate;
            }

            if (credentials.TypeAcess == TypeAcess.Password)
            {
                var result = await _signInManager.PasswordSignInAsync(credentials.Email, credentials.Password, isPersistent: false, lockoutOnFailure: true);

                if (!result.Succeeded)
                {
                    validate.AddProcessingError("Usuário ou Senha incorretos!");
                }

                if (result.IsLockedOut)
                {
                    validate.AddProcessingError("Usuário temporariamente bloqueado por tentativas inválidas");
                }

                return validate;

            }
            else if (credentials.TypeAcess == TypeAcess.Refresh_Token)
            {
                if (!String.IsNullOrWhiteSpace(credentials.RefreshToken))
                {
                    RefreshTokenModel refreshTokenModel = null;

                    string strTokenArmazenado = _cache.GetString(credentials.RefreshToken);

                    if (!String.IsNullOrWhiteSpace(strTokenArmazenado))
                    {
                        refreshTokenModel = JsonSerializer.Deserialize<RefreshTokenModel>(strTokenArmazenado);
                    }

                    bool accessCredentialsValid = (refreshTokenModel != null && credentials.Email == refreshTokenModel.Email && credentials.RefreshToken == refreshTokenModel.RefreshToken);
                    
                    if (accessCredentialsValid)
                    {
                        // Elimina o token de refresh uma vez que será gerado um novo
                        _cache.Remove(credentials.RefreshToken);

                    }
                    else
                    {
                        validate.AddProcessingError("Refresh Token Inválido!");
                    }
                }
                else
                {
                    validate.AddProcessingError("Não foi infornado Refresh Token!");
                }

                return validate;

            }
            validate.AddProcessingError("Informe um tipo de acesso!");
            return validate;
        }

        public AuthTokenDTO GenerateToken(string Email)
        {
            // Define as claims do usuário (não é obrigatório mas cria mais chaves no Payload)
            var claims = new[]
            {
                 new Claim(JwtRegisteredClaimNames.UniqueName, Email),
                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
             };

            // Gera uma chave
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenConfigurations.SecretJWTKey));

            // Gera a assinatura digital do token
            var credenciais = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Tempo de expiracão do token            
            var expiration = DateTime.UtcNow.AddSeconds(_tokenConfigurations.ExpireSeconds);

            // Monta as informações do token
            JwtSecurityToken token = new JwtSecurityToken(
              issuer: _tokenConfigurations.Issuer,
              audience: _tokenConfigurations.Audience,
              claims: claims,
              expires: expiration,
              signingCredentials: credenciais);

            // Retorna o token e demais informações
            var response = new AuthTokenDTO
            {
                Authenticated = true,
                Expiration = expiration,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = Guid.NewGuid().ToString().Replace("-", String.Empty),
                Message = "Token JWT OK",
                UserToken = new UserTokenDTO
                {
                    Email = Email
                }
            };

            // Armazena o refresh token em cache através do Redis 
            var refreshTokenModel = new RefreshTokenModel()
            {
                RefreshToken = response.RefreshToken,
                Email = response.UserToken.Email
            };

            // Validade do refresh token (O Redis irá invalidar o registro automaticamente de acordo com a validade)            
            TimeSpan finalExpiration = TimeSpan.FromSeconds(_tokenConfigurations.FinalExpirationSeconds);

            DistributedCacheEntryOptions optionsCache = new DistributedCacheEntryOptions();
            optionsCache.SetAbsoluteExpiration(finalExpiration);

            // Grava o Refresh Token no cache
            _cache.SetString(response.RefreshToken, JsonSerializer.Serialize(refreshTokenModel), optionsCache);

            return response;

        }
    }
}
