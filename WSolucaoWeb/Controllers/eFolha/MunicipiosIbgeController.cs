using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using WSolucaoWeb.Dtos.eFolha;
using Persistencia.Models.TabelasPadroes;
using Persistencia.Utils;
using Persistencia.DataBases;

namespace WSolucaoWeb.Controllers.eFolha
{
    [Authorize]
    [Route("municipiosibge")]
    [ApiController]

    public class MunicipiosIbgeController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public MunicipiosIbgeController(IConfiguration config)
        {
            _configuration = config;
        }


        [HttpGet("consultar/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult> Consultar([FromRoute] int id)
        {
            try
            {

                var optionsBuilderSulProg = new DbContextOptionsBuilder<TabelasPadroesContext>();
                optionsBuilderSulProg.UseNpgsql(Environment.GetEnvironmentVariable("KingHost"));
                using var context = new TabelasPadroesContext(optionsBuilderSulProg.Options);

                MunicipiosIbge municipiosIbge = await context.MunicipiosIbge.Where(e => e.Id == id).FirstOrDefaultAsync();

                //empresa não encontrado
                if (municipiosIbge is null)
                {
                    return NotFound(Respostas.Atencao($"Não foi encontrado nenhum registro com a id = {id}!"));
                }

                // Retorno com sucesso
                return Ok(municipiosIbge);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Respostas.Erro(ex.Message));
            }
        }

        [HttpGet("consultarporcodigomunicipio/{codigoMunicipio}")]
        [AllowAnonymous]
        public async Task<ActionResult> ConsultarPorCodigoMunicipio([FromRoute] string codigoMunicipio)
        {
            try
            {

                var optionsBuilderSulProg = new DbContextOptionsBuilder<TabelasPadroesContext>();
                optionsBuilderSulProg.UseNpgsql(Environment.GetEnvironmentVariable("KingHost"));
                using var context = new TabelasPadroesContext(optionsBuilderSulProg.Options);

                MunicipiosIbge municipiosIbge = await context.MunicipiosIbge.Where(e => e.CodigoMunicipio == codigoMunicipio).FirstOrDefaultAsync();

                if (municipiosIbge is null)
                {
                    return NotFound(Respostas.Atencao("Código Município inválido."));
                }

                return Ok(municipiosIbge);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Respostas.Erro(ex.Message));
            }
        }

        [HttpGet("listarcombo")]
        [AllowAnonymous]
        public async Task<ActionResult<List<object>>> ListarCombo()
        {
            try
            {
                var optionsBuilderSulProg = new DbContextOptionsBuilder<TabelasPadroesContext>();
                optionsBuilderSulProg.UseNpgsql(Environment.GetEnvironmentVariable("KingHost"));
                using var context = new TabelasPadroesContext(optionsBuilderSulProg.Options);

                IQueryable<object> query = context.MunicipiosIbge
                    .Select(ls => new
                    {
                        ls.Id,
                        ls.CodigoUf,
                        ls.Uf,
                        ls.CodigoMunicipio,
                        ls.NomeMunicipio
                    }).OrderBy(ls => ls.Id);

                var municipiosIbge = await query.ToArrayAsync();

                return Ok(new { municipiosIbge });
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

        [HttpPost("listar")]
        [AllowAnonymous]
        public async Task<ActionResult> Listar([FromBody] PropriedadesTabelaDto prop)
        {
            try
            {
                if (prop.FiltroArray[0] != "")
                    prop.FiltroArray[0] = prop.FiltroArray[0].ToLower();
                if (prop.FiltroArray[1] != "")
                    prop.FiltroArray[1] = prop.FiltroArray[1].ToLower();
                if (prop.FiltroArray[2] != "")
                    prop.FiltroArray[2] = prop.FiltroArray[2].ToLower();
                if (prop.FiltroArray[3] != "")
                    prop.FiltroArray[3] = prop.FiltroArray[3].ToLower();
                if (prop.FiltroArray[4] != "")
                    prop.FiltroArray[4] = prop.FiltroArray[4].ToLower();

                var optionsBuilderSulProg = new DbContextOptionsBuilder<TabelasPadroesContext>();
                optionsBuilderSulProg.UseNpgsql(Environment.GetEnvironmentVariable("KingHost"));
                using var context = new TabelasPadroesContext(optionsBuilderSulProg.Options);

                IQueryable<object> query = context.MunicipiosIbge
                    .Where(e => (prop.FiltroArray[0] == "" || prop.FiltroArray[0] == null) &&
                                 (prop.FiltroArray[1] == "" || prop.FiltroArray[1] == null) &&
                                 (prop.FiltroArray[2] == "" || prop.FiltroArray[2] == null) &&
                                 (prop.FiltroArray[3] == "" || prop.FiltroArray[3] == null) &&
                                 (prop.FiltroArray[4] == "" || prop.FiltroArray[4] == null) ? 1 == 1 : 

                                    e.Id.ToString().ToLower().Contains(prop.FiltroArray[0]) &&
                                    e.CodigoUf.ToLower().Contains(prop.FiltroArray[1]) &&
                                    e.Uf.ToLower().Contains(prop.FiltroArray[2]) &&
                                    e.CodigoMunicipio.ToLower().Contains(prop.FiltroArray[3]) &&
                                    e.NomeMunicipio.ToLower().Contains(prop.FiltroArray[4]))
                    .Select(e => new
                    {
                        e.Id,
                        e.CodigoUf,
                        e.Uf,
                        e.CodigoMunicipio,
                        e.NomeMunicipio
                    });

                var count = await query.CountAsync();

                prop.NomeColunaOrdem = char.ToUpper(prop.NomeColunaOrdem[0]) + prop.NomeColunaOrdem.Substring(1);

                if (prop.Ordem == "false")
                    query = query.OrderBy(p => EF.Property<object>(p, prop.NomeColunaOrdem));
                else if (prop.Ordem == "true")
                    query = query.OrderByDescending(p => EF.Property<object>(p, prop.NomeColunaOrdem));

                query = query
                    .Skip((prop.Page - 1) * prop.Size)
                    .Take(prop.Size);

                var municipiosIbge = await query.ToArrayAsync();

                return Ok(new { totalDeRegistros = count, municipiosIbge });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Respostas.Erro(ex.Message));
            }
        }
    }
}
