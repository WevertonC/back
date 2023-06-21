using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using AutoMapper;
using SendGrid.Helpers.Mail;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using WSolucaoWeb.Dtos.eFolha;
using Persistencia.Models.SulProg;
using Persistencia.Utils;
using Persistencia.Models.Enums;
using Persistencia.DataBases;
using WebApi.Models.Enums;

namespace WSolucaoWeb.Controllers.eFolha
{
    [Authorize]
    [Route("sulprog")]
    [ApiController]
    public class SulProgController : ControllerBase
    {

        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public SulProgController(IMapper mapper,
                                 IConfiguration config)
        {
            _mapper = mapper;
            _configuration = config;
        }

        // GET: /sulprog/clientes
        [HttpGet("clientes")]
        [AllowAnonymous]
        public async Task<ActionResult<List<object>>> ConsultarClientesSulProg([FromQuery] FiltroEscritorio filtro, string id)
        {
            try
            {
                // Consulta se o cliente consta na base de dados da Sulprog
                var optionsBuilderSulProg = new DbContextOptionsBuilder<SulProgContext>();
                optionsBuilderSulProg.UseNpgsql(Environment.GetEnvironmentVariable("SulProgProducao"));
                using var context = new SulProgContext(optionsBuilderSulProg.Options);

                IQueryable<LicencaSoftware> query = context.LicencaSoftwares
                    .Select(ls => new LicencaSoftware
                    {
                        NumeroSerie = ls.NumeroSerie,
                        Cliente = new ClienteSulProg
                        {
                            Cpf = ls.Cliente.Cpf,
                            Cnpj = ls.Cliente.Cnpj,
                            Ativo = ls.Cliente.Ativo
                        }
                    }).Where(ls => ls.Cliente.Ativo == true);

                List<LicencaSoftware> licencas = new List<LicencaSoftware>();

                List<string> bases = await ConsultarBasesDeDadosJaExistentes();

                Regex nSerieRgx = new Regex(@"^[0-9]{6}$");
                Regex cpfRgx = new Regex(@"^[0-9]{11}$");
                Regex cnpjRgx = new Regex(@"^[0-9]{14}$");

                if (nSerieRgx.IsMatch(id))
                    query = query.Where(ls => ls.NumeroSerie == id);
                else if (cpfRgx.IsMatch(id))
                    query = query.Where(ls => ls.Cliente.Cpf == id);
                else if (cnpjRgx.IsMatch(id))
                    query = query.Where(ls => ls.Cliente.Cnpj == id);
                else
                    return BadRequest(Respostas.Erro("Não foi possível localizar o seu escritório!"));

                if (filtro == FiltroEscritorio.PossuiWeb)
                    query = query
                        .Where(ls => bases.Contains(ls.NumeroSerie));
                else if (filtro == FiltroEscritorio.NaoPossuiWeb)
                    query = query
                        .Where(ls => !bases.Contains(ls.NumeroSerie));

                query = query
                    .AsNoTracking()
                    .OrderBy(ls => ls.NumeroSerie);

                var resultado = await query.FirstOrDefaultAsync();

                if (resultado is null)
                    return BadRequest(Respostas.Erro("Não foi possível localizar o seu escritório!"));

                var clientes = _mapper.Map<LicencaSoftwareDto>(resultado);

                return Ok(clientes);
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


        private async Task<List<string>> ConsultarBasesDeDadosJaExistentes()
        {
            List<string> bases = new List<string>();

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
            string scriptSql = $"select substr(datname::varchar,11,6) from pg_database where datistemplate = false and datname::varchar like \'escritorio%\'";
            await using var cmd = new NpgsqlCommand(scriptSql, conn);
            await using var reader = await cmd.ExecuteReaderAsync();

            if (reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    string baseDeDados = reader.GetString(0);
                    bases.Add(baseDeDados);
                }
            }

            return bases;
        }

        // Rotina para verificar se a api está no ar ou não
        [HttpGet("consultarapi")]
        [AllowAnonymous]
        public async Task<string> ConsultarApi()
        {

            return "Ok";
        }

        [HttpGet("verificarmoduloswsolucao")]
        [AllowAnonymous]
        public async Task<bool> VerificarModulosWSolucao([FromHeader] string numeroDeSerie)
        {
            var optionsBuilderSulProg = new DbContextOptionsBuilder<SulProgContext>();
            optionsBuilderSulProg.UseNpgsql(Environment.GetEnvironmentVariable("SulProgProducao"));
            using var context = new SulProgContext(optionsBuilderSulProg.Options);

            var nsObj = await context.LicencaSoftwares.Where(x => x.NumeroSerie == numeroDeSerie).FirstOrDefaultAsync();

            string stringDeConexao = Environment.GetEnvironmentVariable("SulProgProducao");
            await using var conn = new NpgsqlConnection(stringDeConexao);
            await conn.OpenAsync();

            string scriptSql = "select * from modulodalicenca " +
                               "where licencasoftwareid = " + nsObj.Id + " " +
                               "and modulo <> 13" +
                               "limit 1";

            await using var cmd = new NpgsqlCommand(scriptSql, conn);
            await using var reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                return true;
            }

            return false;
        }
    }
}
