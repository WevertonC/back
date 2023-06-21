using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using AutoMapper;
using WSolucaoWeb.Dtos.eFolha;
using Persistencia.Models.SulProg;
using Persistencia.DataBases;
using Persistencia.Models.Enums;
using Persistencia.Models.EFolha;

namespace WSolucaoWeb.Controllers.eFolha
{
    [Route("criacao")]
    [ApiController]
    public class CriacaoController : ControllerBase
    {

        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<Perfil> _roleManager;
        private readonly WebApiContext _webApiContext;

        private string NumeroDeSerieEscritorio { get; set; }
        private string Escritorio { get; set; }

        public CriacaoController(IMapper mapper,
                                 UserManager<Usuario> userManager,
                                 RoleManager<Perfil> roleManager,
                                 IConfiguration config,
                                 WebApiContext webApiContext)
        {
            _mapper = mapper;
            _configuration = config;
            _userManager = userManager;
            _roleManager = roleManager;
            _webApiContext = webApiContext;
        }

        [HttpPost("cadastrointerno")]
        [AllowAnonymous]
        public async Task<ActionResult> CreatePrimeiroUsuarioUsuario([FromBody] PrimeiroUsuarioDto usuarioDto, [FromHeader] string numeroDeSerie)
        {
            try
            {
                if (usuarioDto.Senha != usuarioDto.ConfirmacaoSenha)
                    return BadRequest(new
                    {
                        code = "Atenção!",
                        message = "As Senhas informadas são diferentes!",
                        detailedMessage = "Verifique e tente novamente."
                    });

                // Carrega o número de série do escritório
                NumeroDeSerieEscritorio = numeroDeSerie;

                object baseDeDadosExistente = await VerificarSeBaseDeDadosJaExiste();

                if (!(baseDeDadosExistente is null))
                    return StatusCode(StatusCodes.Status400BadRequest, baseDeDadosExistente);

                string email = "passarIf";
                object clienteNaoEncontrado = await VerificarSeEhClienteAtivoSulProg(email);

                if (!(clienteNaoEncontrado is null))
                    return StatusCode(StatusCodes.Status400BadRequest, clienteNaoEncontrado);

                // Mapeando para a classe do usuário
                Usuario usuario = _mapper.Map<Usuario>(usuarioDto);
                usuario.Ativo = true;

                object usuarioCriado = await CriarBaseDeDadosComPrimeiroUsuario(usuario);

                if (!(usuarioCriado is null))
                    return StatusCode(StatusCodes.Status500InternalServerError, usuarioCriado);

                object problemasNaCricao = await CriarPerfils();

                if (!(problemasNaCricao is null))
                    return StatusCode(StatusCodes.Status500InternalServerError, problemasNaCricao);

                return StatusCode(StatusCodes.Status201Created, new
                {
                    code = "Sucesso!",
                    message = "O cadastro foi realizado com sucesso!",
                    detailedMessage = "Você já pode acessar o sistema."
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


        [HttpPost("verificarcadastro")]
        [AllowAnonymous]
        public async Task<ActionResult> VerificarCadastro([FromBody] PrimeiroUsuarioDto usuarioDto, [FromHeader] string numeroDeSerie)
        {
            try
            {
                // Carrega o número de série do escritório
                NumeroDeSerieEscritorio = numeroDeSerie;

                object baseDeDadosExistente = await VerificarSeBaseDeDadosJaExiste();

                if (!(baseDeDadosExistente is null))
                    return StatusCode(StatusCodes.Status400BadRequest, baseDeDadosExistente);

                object clienteNaoEncontrado = await VerificarSeEhClienteAtivoSulProg(usuarioDto.Email);

                if (!(clienteNaoEncontrado is null))
                    return StatusCode(StatusCodes.Status400BadRequest, clienteNaoEncontrado);

                // Mapeando para a classe do usuário
                Usuario usuario = _mapper.Map<Usuario>(usuarioDto);
                usuario.Ativo = true;

                string y = "";

                Random rnd = new Random();

                for (var i = 0; i < 6; i++)
                {
                    y += rnd.Next(0, 9);
                }

                var x = (int.Parse(y) * 324).ToString();

                var z = System.Text.Encoding.UTF8.GetBytes(x);
                var w = Convert.ToBase64String(z);

                return Ok(w);
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

        [HttpPost("criar")]
        [AllowAnonymous]
        public async Task<ActionResult> Criar([FromBody] PrimeiroUsuarioDto usuarioDto, [FromHeader] string numeroDeSerie)
        {
            try
            {
                NumeroDeSerieEscritorio = numeroDeSerie;

                object baseDeDadosExistente = await VerificarSeBaseDeDadosJaExiste();

                if (!(baseDeDadosExistente is null))
                    return StatusCode(StatusCodes.Status400BadRequest, baseDeDadosExistente);

                object clienteNaoEncontrado = await VerificarSeEhClienteAtivoSulProg(usuarioDto.Email);

                if (!(clienteNaoEncontrado is null))
                    return StatusCode(StatusCodes.Status400BadRequest, clienteNaoEncontrado);

                // Mapeando para a classe do usuário
                Usuario usuario = _mapper.Map<Usuario>(usuarioDto);
                usuario.Ativo = true;

                object usuarioCriado = await CriarBaseDeDadosComPrimeiroUsuario(usuario);

                if (!(usuarioCriado is null))
                    return StatusCode(StatusCodes.Status500InternalServerError, usuarioCriado);

                object problemasNaCricao = await CriarPerfils();

                if (!(problemasNaCricao is null))
                    return StatusCode(StatusCodes.Status500InternalServerError, problemasNaCricao);

                return StatusCode(StatusCodes.Status201Created, new
                {
                    code = "Sucesso!",
                    message = "O cadastro foi realizado com sucesso!",
                    detailedMessage = "Você já pode acessar o sistema."
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

        private async Task<object> VerificarSeBaseDeDadosJaExiste()
        {
            // Faz conexão com o banco de dados postgres

            string stringDeConexao;
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                stringDeConexao = Environment.GetEnvironmentVariable("ConnectionDesenvolvimento");
            }
            else
            {
                stringDeConexao = Environment.GetEnvironmentVariable("ConnectionProducao");
            }

            await using var conn = new NpgsqlConnection(string.Format(stringDeConexao, "postgres"));
            await conn.OpenAsync();

            // Faz uma consulta para verificar se a base de dados ja esta criada
            string scriptSql = $"select datname::varchar from pg_database where datistemplate = false and datname::varchar = \'escritorio{NumeroDeSerieEscritorio}\'";
            await using var cmd = new NpgsqlCommand(scriptSql, conn);
            await using var reader = await cmd.ExecuteReaderAsync();
            if (reader.HasRows)
            {
                await reader.ReadAsync();
                string baseDeDados = reader.GetString(0);
                return new
                {
                    code = "Atenção!",
                    message = "O escritório informado já adquiriu o produto.",
                    detailedMessage = "Favor entrar em contato com o nosso suporte (42) 3224-3417 / suporte@sulprog.com.br - Cliente: " + baseDeDados
                };
            }
            return null;
        }


        private async Task<object> VerificarSeEhClienteAtivoSulProg(string email)
        {

            // Consulta se o cliente consta na base de dados da Sulprog
            var optionsBuilderSulProg = new DbContextOptionsBuilder<SulProgContext>();
            optionsBuilderSulProg.UseNpgsql(Environment.GetEnvironmentVariable("SulProgProducao"));
            using var context = new SulProgContext(optionsBuilderSulProg.Options);

            LicencaSoftware licencaSoftware = await context.LicencaSoftwares.Where(l => l.NumeroSerie == NumeroDeSerieEscritorio)
                                                                            .Include(x => x.Cliente)
                                                                            .FirstOrDefaultAsync();

            if (licencaSoftware is null)
            {
                return new
                {
                    code = "Atenção!",
                    message = "O escritório informado não foi localizado.",
                    detailedMessage = "Favor entrar em contato com o nosso suporte (42) 3224-3417 / suporte@sulprog.com.br"
                };
            }

            if (licencaSoftware.StatusLicenca == StatusLicenca.Cancelada)
            {
                return new
                {
                    code = "Atenção!",
                    message = "Sua licença de software não está ativa no momento.",
                    detailedMessage = "Favor entrar em contato com o nosso suporte (42) 3224-3417 / suporte@sulprog.com.br"
                };
            }

            if (email != "passarIf")
            {
                if (licencaSoftware.Cliente.Email != email)
                {
                    return new
                    {
                        code = "Atenção!",
                        message = "Este e-mail não confere com o cadastrado para este número de série.",
                        detailedMessage = "Favor entrar em contato com o nosso suporte (42) 3224-3417 / suporte@sulprog.com.br"
                    };
                }
            }

            ClienteSulProg clienteSulProg = await context.ClientesSulProg.Where(csp => csp.Id == licencaSoftware.ClienteId).FirstOrDefaultAsync();

            Escritorio = clienteSulProg.RazaoSocial;

            return null;
        }

        private async Task<object> CriarBaseDeDadosComPrimeiroUsuario(Usuario usuario)
        {
            string stringDeConexao;
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                stringDeConexao = Environment.GetEnvironmentVariable("ConnectionDesenvolvimento");
            }
            else
            {
                stringDeConexao = Environment.GetEnvironmentVariable("ConnectionProducao");
            }

            var optionsBuilderWebApi = new DbContextOptionsBuilder<WebApiContext>();
            optionsBuilderWebApi.UseNpgsql(string.Format(stringDeConexao, $"escritorio{NumeroDeSerieEscritorio}"));
            using var context = new WebApiContext(optionsBuilderWebApi.Options);

            // Realisa a criação da base de dados
            await context.Database.MigrateAsync();

            return await CriarUsuario(usuario, TipoAcesso.Escritorio, NivelAcesso.Gerente);
        }

        private async Task<object> CriarUsuario(Usuario usuario, TipoAcesso tipoAcesso, NivelAcesso nivelAcesso)
        {
            // Faz a criação do usuario
            usuario.DataDaCriacao = DateTime.Now;
            usuario.Ativo = true;

            var usuarioCriado = await _userManager.CreateAsync(usuario, usuario.PasswordHash);

            if (!usuarioCriado.Succeeded)
            {
                return new
                {
                    code = "Erro!",
                    message = "Não foi possível criar o usuário!",
                    detailedMessage = "Favor entrar em contato com o nosso suporte (42) 3224-3417 / suporte@sulprog.com.br"
                };
            }

            Perfil perfil = new Perfil(tipoAcesso, nivelAcesso);

            var perfilCriado = await _roleManager.CreateAsync(perfil);

            if (!perfilCriado.Succeeded)
            {
                return new
                {
                    code = "Erro!",
                    message = "Não foi possível criar o perfil do usuário!",
                    detailedMessage = "Favor entrar em contato com o nosso suporte (42) 3224-3417 / suporte@sulprog.com.br"
                };
            }

            string nomeDoPerfil = $"{tipoAcesso.ToString()} {nivelAcesso.ToString()}";

            var perfilVinculado = await _userManager.AddToRoleAsync(usuario, nomeDoPerfil);

            if (!perfilVinculado.Succeeded)
            {
                return new
                {
                    code = "Erro!",
                    message = "Não foi possível atribuir o perfil ao usuário!",
                    detailedMessage = "Favor entrar em contato com o nosso suporte (42) 3224-3417 / suporte@sulprog.com.br"
                };
            }

            return null;
        }
        private async Task<object> CriarPerfils()
        {
            object problemaNaCriacao = await CriarPerfil(TipoAcesso.Escritorio, NivelAcesso.Operador);

            if (!(problemaNaCriacao is null))
                return problemaNaCriacao;

            problemaNaCriacao = await CriarPerfil(TipoAcesso.Empresa, NivelAcesso.Gerente);

            if (!(problemaNaCriacao is null))
                return problemaNaCriacao;

            problemaNaCriacao = await CriarPerfil(TipoAcesso.Empresa, NivelAcesso.Operador);

            if (!(problemaNaCriacao is null))
                return problemaNaCriacao;

            return null;
        }

        private async Task<object> CriarPerfil(TipoAcesso tipoAcesso, NivelAcesso nivelAcesso)
        {
            Perfil perfil = new Perfil(tipoAcesso, nivelAcesso);

            var perfilCriado = await _roleManager.CreateAsync(perfil);

            if (!perfilCriado.Succeeded)
            {
                return new
                {
                    code = "Erro!",
                    message = "Não foi possível criar o perfil do usuário!",
                    detailedMessage = "Favor entrar em contato com o nosso suporte (42) 3224-3417 / suporte@sulprog.com.br"
                };
            }

            return null;
        }

    }
}
