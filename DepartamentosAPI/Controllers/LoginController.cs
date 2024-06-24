using DepartamentosAPI.Helpers;
using DepartamentosAPI.Models.DTOS;
using DepartamentosAPI.Models.Validators;
using DepartamentosAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace DepartamentosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly DepartamentoRepository departamentoRepository;
        private readonly JWTHelper _jwthelper;
        public LoginController(DepartamentoRepository departamentoRepo, JWTHelper jwtHelper )
        {
            departamentoRepository = departamentoRepo;
            _jwthelper = jwtHelper;
        }
        [HttpPost]
        public IActionResult IniciarSesion(LoginDTO user)
        {
            var results = LoginValidator.Validate(user);
            if (results.IsValid)
            {
                var usuario = departamentoRepository.GetUsuario(user.Nombre,Encryption.StringToSHA512( user.Contrasena));
                if (usuario == null)
                {
                    return Unauthorized();
                }
                var token = _jwthelper.GetToken(usuario.Username, usuario.IdSuperior == null ? "Admin" : "User", usuario.Id, new List<Claim>()
                { new Claim("Id", usuario.Id.ToString())}); 
                return Ok(token);
            }
            else 
            {
                return BadRequest(results.Errors.Select(x => x.ErrorMessage));
            }
        }
    }
}
