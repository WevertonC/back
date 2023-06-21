using Persistencia.Models.Geral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Google.Cloud.Storage.V1;
using AutoMapper;
using Persistencia.Utils;
using Microsoft.CodeAnalysis;
using WSolucaoWeb.Dtos.eFolha;
using Persistencia.Models.EFolha;
using Persistencia.Models.Enums;
using Persistencia.DataBases;

namespace WSolucaoWeb.Controllers.eFolha
{
    [Authorize]
    [Route("usuario")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<Perfil> _roleManeger;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly WebApiContext _webApiContext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContext;

        //queries
        private IQueryable<Usuario> usuarioQuery;
        private IQueryable<UsuarioPerfil> perfilCorrenteQuery;
        private IQueryable<Perfil> rolesQuery;
        private IQueryable<UsuarioEstabelecimento> usuariosEstQuery;


        public UsuarioController(IMapper mapper,
                                 WebApiContext webApiContext,
                                 IConfiguration configuration,
                                 RoleManager<Perfil> roleManager,
                                 SignInManager<Usuario> signInManager,
                                 UserManager<Usuario> userManager,
                                 IHttpContextAccessor httpContext)
        {
            _mapper = mapper;
            _userManager = userManager;
            _roleManeger = roleManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _webApiContext = webApiContext;
            _httpContext = httpContext;

            usuarioQuery = webApiContext.Usuarios.AsQueryable();
            perfilCorrenteQuery = webApiContext.UserRoles.AsQueryable();
            rolesQuery = roleManager.Roles.AsQueryable();
        }
        //GET: /usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return await usuarioQuery.ToArrayAsync();
        }

        // POST: /usuario/inserir
        [HttpPost("inserir")]
        public async Task<ActionResult> CriarUsuario([FromForm] NovoUsuarioDto novoUsuario)
        {
            try
            {
                // Dispensa a obrigatoriedade dos campos de senha
                ModelState.Remove("Id");

                // Validando o modelo de entrada
                if (!ModelState.IsValid)
                {
                    string messages = string.Join(" ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                    return BadRequest(Respostas.MensagemDeErroDoModelo(messages));
                }
                var nNovoUsuarioDto = novoUsuario;
                var nImagens = novoUsuario.ImagemUsuario;

                nNovoUsuarioDto.ImagemUsuario = null;

                // Buscando o usuário corrente
                Usuario usuarioCorrente = await _userManager.GetUserAsync(HttpContext.User);

                perfilCorrenteQuery = perfilCorrenteQuery
                    .Where(up => up.UserId == usuarioCorrente.Id)
                    .Select(up => new UsuarioPerfil
                    {
                        UserId = up.UserId,
                        Perfil = up.Perfil
                    })
                    .AsQueryable();
                UsuarioPerfil perfilCorrente = await perfilCorrenteQuery.FirstOrDefaultAsync();

                // Verificar se o usuário corrente pode criar um novo usuário
                object perfilNaoAutorizado = VerificarPermissaoPerfil(perfilCorrente, novoUsuario);

                if (!(perfilNaoAutorizado is null))
                    return StatusCode(StatusCodes.Status500InternalServerError, perfilNaoAutorizado);

                rolesQuery = rolesQuery
                    .Where(p => p.TipoAcesso == novoUsuario.TipoAcesso && p.NivelAcesso == novoUsuario.NivelAcesso)
                    .Select(p => new Perfil
                    {
                        Id = p.Id,
                        Name = p.Name
                    })
                    .AsQueryable();
                // Consulta o perfil que se deseja atribuir ao novo usuário
                var perfil = await rolesQuery.FirstOrDefaultAsync();

                if (perfil is null)
                    return StatusCode(StatusCodes.Status400BadRequest,
                                           Respostas.Atencao("Seu usuário não está autorizado a criar um usuário com esse tipo e nivel de acesso!"));

                // Validar se já existe algum usuário com o mesmo nome de usuário ou email
                object usuarioJaExiste = await ValidarNomeEmailUnicos(novoUsuario, usuarioCorrente.Id);

                if (!(usuarioJaExiste is null))
                    return StatusCode(StatusCodes.Status500InternalServerError, usuarioJaExiste);

                // Mapeando para a classe do usuário
                Usuario usuario = _mapper.Map<Usuario>(novoUsuario);

                // Faz a criação do usuario
                usuario.DataDaCriacao = DateTime.Now;
                usuario.Ativo = novoUsuario.Ativo;
                usuario.PhoneNumber = novoUsuario.Telefone;

                var usuarioCriado = await _userManager.CreateAsync(usuario, usuario.PasswordHash);

                if (!usuarioCriado.Succeeded)
                    return StatusCode(
                        StatusCodes.Status500InternalServerError,
                        Respostas.Erro("Não foi possível criar o usuário!"));

                // Atribui estabelecimento ao usuário se houver
                if (novoUsuario.EstabelecimentosIds != null)
                {
                    List<Estabelecimento> estabelecimento = new List<Estabelecimento>();

                    for (var i = 0; i < novoUsuario.EstabelecimentosIds.Count(); i++)
                    {
                        estabelecimento.Add(await _webApiContext.Estabelecimentos.FindAsync(novoUsuario.EstabelecimentosIds[i]));
                    }

                    var usuarioEstabelecimentoLista = new List<UsuarioEstabelecimento>();

                    for (var i = 0; i < estabelecimento.Count(); i++)
                    {
                        var usuarioEstabelecimento = new UsuarioEstabelecimento();

                        usuarioEstabelecimento.Estabelecimento = estabelecimento[i];
                        usuarioEstabelecimento.Usuario = usuario;

                        usuarioEstabelecimentoLista.Add(usuarioEstabelecimento);
                    }

                    await _webApiContext.UsuariosEstabelecimentos.AddRangeAsync(usuarioEstabelecimentoLista);
                }
                if (await _webApiContext.SaveChangesAsync() == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, Respostas.Erro("Erro interno ao salvar registro"));
                }
                // Atribui ao novo usuário o perfil desejado
                string nomeDoPerfil = $"{novoUsuario.TipoAcesso.ToString()} {novoUsuario.NivelAcesso.ToString()}";

                var perfilVinculado = await _userManager.AddToRoleAsync(usuario, nomeDoPerfil);

                if (!perfilVinculado.Succeeded)
                    return StatusCode(
                        StatusCodes.Status500InternalServerError,
                        Respostas.Erro("Não foi possível atribuir o perfil ao usuário!"));

                return Ok(Respostas.Sucesso("Usuário criado com sucesso!"));

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Respostas.Erro(ex.Message));
            }
        }

        //  GET: /usuario/consultar/{id}
        [HttpGet("consultar/{id}")]
        public async Task<ActionResult> ConsultarUsuario([FromRoute] int id)
        {
            try
            {
                if (!(id > 0))
                    return StatusCode(StatusCodes.Status400BadRequest,
                                           Respostas.Erro("Identificador do usuário inválido ou inexistente!"));

                Usuario usuarioCorrente = await _userManager.GetUserAsync(HttpContext.User);

                perfilCorrenteQuery = perfilCorrenteQuery
                   .Where(up => up.UserId == usuarioCorrente.Id)
                   .Select(up => new UsuarioPerfil
                   {
                       UserId = up.UserId,
                       Perfil = up.Perfil
                   })
                   .AsQueryable();
                UsuarioPerfil perfilCorrente = await perfilCorrenteQuery.FirstOrDefaultAsync();

                if (perfilCorrente.Perfil.TipoAcesso.ToString() == "Empresa")
                {
                    if (usuarioCorrente.Id != id)
                    {
                        return Ok(null);
                    }
                }

                var usuario = await _userManager.Users
                    .Where(u => u.Id == id)
                    .Select(u => new
                    {
                        id = u.Id,
                        nome = u.UserName,
                        email = u.Email,
                        telefone = u.PhoneNumber,
                        celular = u.Celular,
                        imagemUsuario = u.Imagem,
                        nivelAcesso = u.UsuariosPerfis.FirstOrDefault().Perfil.NivelAcesso,
                        tipoAcesso = u.UsuariosPerfis.FirstOrDefault().Perfil.TipoAcesso,
                        tipoAcessoNome = u.UsuariosPerfis.FirstOrDefault().Perfil.TipoAcesso.ToString() == "Escritorio" ? "Escritório" : "Cliente",
                        nivelAcessoNome = u.UsuariosPerfis.FirstOrDefault().Perfil.NivelAcesso.ToString() == "Gerente" ? "Supervisor" : "Operador",
                        estabelecimentosIds = u.UsuariosEstabelecimentos.Select(x => x.EstabelecimentoId),
                        estabelecimentosIdsNomes = u.UsuariosEstabelecimentos.Select(x => x.Estabelecimento).Select(y => y.RazaoSocial),
                        ativo = u.Ativo
                    }).FirstOrDefaultAsync();

                if (usuario.imagemUsuario != null)
                {
                    usuario.imagemUsuario.Uri = "";
                }

                //usuário não encontrado
                if (usuario is null)
                {
                    return NotFound(Respostas.Atencao($"Não foi encontrado nenhum usuário com a id = {id} no banco de dados!"));
                }

                // Retorno com sucesso
                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Respostas.Erro(ex.Message));
            }
        }

        private object VerificarPermissaoPerfil(UsuarioPerfil perfilCorrente, NovoUsuarioDto novoUsuario, string acao = "criar")
        {
            bool permitido = true;

            if (TipoAcesso.SulProg == novoUsuario.TipoAcesso && NivelAcesso.Manutencao == novoUsuario.NivelAcesso)
            {
                permitido = false;
            }
            else if (perfilCorrente.Perfil.NivelAcesso == NivelAcesso.Gerente)
            {
                if (perfilCorrente.Perfil.TipoAcesso > novoUsuario.TipoAcesso)
                {
                    permitido = false;
                }
            }
            else if (perfilCorrente.Perfil.NivelAcesso == NivelAcesso.Operador)
            {
                if (acao == "atualizar")
                {
                    if (perfilCorrente.UserId != novoUsuario.Id)
                    {
                        permitido = false;
                    }
                }
                else if (perfilCorrente.Perfil.TipoAcesso >= novoUsuario.TipoAcesso)

                {
                    permitido = false;
                }
            }

            if (!permitido)
            {
                return Respostas.Atencao($"Seu usuário não está autorizado a {acao} um usuário, por conta do seu tipo e/ ou nivel de acesso!");
            }
            else
            {
                return null;
            }
        }

        private async Task<object> ValidarNomeEmailUnicos(NovoUsuarioDto novoUsuario, int idUsuarioCorrente)
        {
            Usuario usuario = await _userManager.FindByNameAsync(novoUsuario.Nome);

            if (!(usuario is null) && idUsuarioCorrente != usuario.Id && novoUsuario.Id != usuario.Id)
                return Respostas.Atencao("Já existe um usuário com o Nome informado!");

            usuario = await _userManager.FindByEmailAsync(novoUsuario.Email);

            if (!(usuario is null) && idUsuarioCorrente != usuario.Id && novoUsuario.Id != usuario.Id)
            {
                return Respostas.Atencao("Já existe um usuário com o Email informado!");
            }
            else if (!(usuario is null) && novoUsuario.Id != usuario.Id)
            {
                return Respostas.Atencao("Já existe um usuário com o Email informado!");
            }
            return null;
        }        
    }
}
