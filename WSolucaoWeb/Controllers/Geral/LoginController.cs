using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using AutoMapper;
using WSolucaoWeb.Dtos.eFolha;
using WSolucaoWeb.Dtos.Geral;
using Persistencia.Models.EFolha;
using Persistencia.DataBases;
using Persistencia.Utils;

namespace WSolucaoWeb.Controllers.Geral
{
    [Authorize]
    [Route("login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly WebApiContext _webApiContext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoginController(WebApiContext webApiContext,
                               UserManager<Usuario> userManager,
                               SignInManager<Usuario> signInManager,
                               IConfiguration configuration,
                               IMapper mapper,
                               IHttpContextAccessor httpContextAccessor)
        {
            _webApiContext = webApiContext;
            _signInManager = signInManager;
            _mapper = mapper;
            _userManager = userManager;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("autenticar")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto login, [FromHeader] string numeroDeSerie)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    string messages = string.Join(" ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                    return BadRequest(Respostas.MensagemDeErroDoModelo(messages));
                }

                SulprogUtil sulprogUtil = new SulprogUtil(_configuration);

                bool baseDeDadosExiste = await sulprogUtil.VerificarSeBaseDeDadosJaExiste(numeroDeSerie);

                if (!baseDeDadosExiste)
                    return BadRequest(Respostas.EscritorioNaoCadastrado());

                await _webApiContext.Database.MigrateAsync();

                Usuario user = await _userManager.FindByEmailAsync(login.Email);

                if (user is null)
                {
                    if (_webApiContext.SaveChanges() == null)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, Respostas.Erro("Erro interno ao salvar registro"));
                    }
                    return Unauthorized(Respostas.EmailNaoEncontrado());
                }

                if (!user.Ativo)
                {
                    if (_webApiContext.SaveChanges() == null)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, Respostas.Erro("Erro interno ao salvar registro"));
                    }
                    return Unauthorized(Respostas.UsuarioInativo());
                }

                // verifica se o password esta correto
                var result = await _signInManager.CheckPasswordSignInAsync(user, login.Senha, false);

                if (result.Succeeded)
                {
                    IQueryable<Usuario> appUserQuery = _userManager.Users
                        .Select(u => new Usuario
                        {
                            Email = u.Email,
                            Id = u.Id,
                            UserName = u.UserName,
                            Celular = u.Celular,
                            DataDaCriacao = u.DataDaCriacao,
                            Imagem = new UsuarioImagem
                            {
                                NomeImagem = u.Imagem.NomeImagem,
                                Uri = u.Imagem.Uri,
                            },
                            UsuariosPerfis = u.UsuariosPerfis
                                .Select(up => new UsuarioPerfil { Perfil = up.Perfil })
                                .ToList(),
                            UsuariosEstabelecimentos = u.UsuariosEstabelecimentos
                        })
                        .AsQueryable();

                    var appUser = await appUserQuery
                        .FirstOrDefaultAsync(u => u.Email == login.Email);

                    var usuario = _mapper.Map<UsuarioDto>(appUser);
                    if (_webApiContext.SaveChanges() == null)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, Respostas.Erro("Erro interno ao salvar registro"));
                    }

                    // Retorna o usuário que conseguiu efetuar o login com sucesso e um token de sessão
                    return Ok(new
                    {
                        token = GenerateJWToken(user).Result,
                        user = usuario
                    });
                }
                if (_webApiContext.SaveChanges() == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, Respostas.Erro("Erro interno ao salvar registro"));
                }

                return Unauthorized(new
                {
                    code = "Atenção",
                    type = "information",
                    message = "Senha incorreta!",
                    detailedMessage = "Senha incorreta!"
                });

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    code = "Erro!",
                    message = ex.Message,
                    detailedMessage = "Favor entrar em contato com o nosso suporte  (42) 3224-3417 / suporte@sulprog.com.br"
                });
            }
        }

        // GET: /login/perfildousuario
        [HttpGet("perfildousuario")]
        public async Task<ActionResult> ConsultarPerfilDoUsuario()
        {
            // Buscando o usuário corrente
            Usuario usuarioCorrente = await _userManager.GetUserAsync(HttpContext.User);

            SulprogUtil sulprogUtil = new SulprogUtil(_configuration);

            IQueryable<object> perfilCorrenteQuery = _webApiContext.UserRoles
                    .Where(up => up.UserId == usuarioCorrente.Id)
                    .Select(up => new
                    {
                        up.Perfil.TipoAcesso,
                        up.Perfil.NivelAcesso,
                    }).AsQueryable();
            object perfilCorrente = await perfilCorrenteQuery.FirstOrDefaultAsync();
            return Ok(perfilCorrente);
        }


        private async Task<string> GenerateJWToken(Usuario user)
        {
            // claims = verifica quais autorizações o usuário possui
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(user);

            // Adiciona quais são os privilégios do usuário
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // cria a chave criptografada
            string numeroDeSerie = _httpContextAccessor.HttpContext.Request.Headers["numeroDeSerie"].ToString();
            string chaveDoToken = $"{numeroDeSerie}{Environment.GetEnvironmentVariable("ChaveDoToken")}";
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(chaveDoToken));

            // qual é o tipo de criptografia que será utilizada
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims), // Permissões associadas
                Expires = DateTime.UtcNow.AddHours(4), // Qual a validade do token
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            // Cria o token
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
