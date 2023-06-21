using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Npgsql;
using WSolucaoWeb.Dtos.eFolha;
using Persistencia.Models.Geral;
using Persistencia.DataBases;
using Persistencia.Utils;
using Persistencia.Models.EFolha;

namespace WSolucaoWeb.Controllers.eFolha
{
    [Authorize]
    [Route("empresa")]
    [ApiController]
    public class EmpresaController : ControllerBase
    {
        private readonly WebApiContext _context;
        private readonly IConfiguration _configuration;

        public EmpresaController(WebApiContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        [HttpGet("listarcombo")]
        public async Task<ActionResult> ListarEmpresaCombo()
        {
            try
            {

                IQueryable<object> query = _context.Empresas
                    .Select(e => new
                    {
                        value = e.Id,
                        text = ("000" + e.CodigoEmpresa).Substring(("000" + e.CodigoEmpresa).Length - 4) + " - " + e.RazaoSocial
                    }).OrderBy(e => e.value);

                var empresas = await query.ToArrayAsync();

                return Ok(new { empresas });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Respostas.Erro(ex.Message));
            }
        }

        // POST: /empresa/inserir
        [HttpPost("inserir")]
        public async Task<ActionResult> CriarEmpresa(EmpresaDto empresaDto)
        {
            try
            {
                // Validando o modelo de entrada
                if (!ModelState.IsValid)
                {
                    string messages = string.Join(" ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                    return BadRequest(Respostas.MensagemDeErroDoModelo(messages));
                }

                IQueryable<Empresa> empresaExisteQuery = _context.Empresas.AsQueryable();
                IQueryable<Empresa> empresaExisteWQuery = _context.Empresas.AsQueryable();

                empresaExisteQuery = empresaExisteQuery.Where(e => e.Id == empresaDto.Id).AsQueryable();
                empresaExisteWQuery = empresaExisteWQuery.Where(e => e.CodigoEmpresa == empresaDto.CodigoEmpresa).AsQueryable();

                Empresa empresaExiste = await empresaExisteQuery.FirstOrDefaultAsync();
                Empresa empresaWExiste = await empresaExisteWQuery.FirstOrDefaultAsync();

                if (!(empresaExiste is null))
                {
                    return BadRequest(Respostas.Atencao($"Já existe uma empresa cadastrada com o id = {empresaDto.Id}!"));
                }

                if (!(empresaWExiste is null))
                {
                    return BadRequest(Respostas.Erro($"Já existe uma empresa cadastrada com o codigoEmpresa = {empresaDto.CodigoEmpresa}!"));
                }   

                Empresa empresa = new Empresa
                {
                    Id = empresaDto.Id,
                    CodigoEmpresa = empresaDto.CodigoEmpresa,
                    NomeFantasia = empresaDto.NomeFantasia,
                    RazaoSocial = empresaDto.RazaoSocial,
                    Ativo = empresaDto.Ativo
                };

                _context.Empresas.Add(empresa);

                if (await _context.SaveChangesAsync() == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, Respostas.Erro("Erro interno ao salvar registro de empresa de codigoEmpresa " + empresa.CodigoEmpresa));
                }

                if (empresaDto.SincWeb)
                {
                    var id_portal = empresa.Id;
                    return Ok(new { mensagem = "Empresa de codigoEmpresa " + empresa.CodigoEmpresa + " criada com sucesso", id_portal });
                }
                else
                {
                    return Ok(Respostas.Custom("Empresa criada com sucesso.", "success", empresa));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Respostas.Erro(ex.Message));
            }
        }
    }
}
