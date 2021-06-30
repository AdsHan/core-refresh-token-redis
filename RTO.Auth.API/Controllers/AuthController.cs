using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using RTO.Auth.API.DTO;
using RTO.Auth.API.Extensions;
using RTO.Auth.API.Service;
using System.Threading.Tasks;

namespace RTO.Auth.API.Controllers
{
    [Produces("application/json")]
    [Route("api/[Controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        public AuthController()
        {

        }

        // POST api/auth
        /// <summary>
        /// Efetua o login do usuário
        /// </summary>   
        /// <remarks>
        /// Exemplo request:
        ///
        ///     USU: adshan@gmail.com SEN: 123456 (Este registro é criado automaticamente toda vez que a Api sobe)
        ///     
        /// 
        ///     POST Login
        ///     
        ///     {
        ///         "email": "adshan@gmail.com",
        ///         "password": "123456",
        ///         "typeAcess": 0        
        ///     }
        ///     
        ///     POST Refresh Token
        ///
        ///     {
        ///         "email": "adshan@gmail.com",
        ///         "refreshToken": "9949ad3f35f444fbb7a8fc28334edf79",
        ///         "typeAcess": 1        
        ///     }        
        ///
        /// </remarks>        
        /// <returns>Token de autenticação</returns>                
        /// <response code="200">Foi realizado o login corretamente</response>                
        /// <response code="400">Falha na requisição</response>         
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SignInAsync([FromBody] AccessCredentialsDTO credentials, [FromServices] AuthService _authService)
        {

            var isValid = await _authService.ValidateCredentialsAsync(credentials);

            if (!isValid.ValidateOperation())
            {
                ModelState.AddModelError("Messages", isValid.GetErrorsMessages());
                return BadRequest(ModelState);
            }

            var response = _authService.GenerateToken(credentials.Email);
            return Ok(response);
        }
    }
}
