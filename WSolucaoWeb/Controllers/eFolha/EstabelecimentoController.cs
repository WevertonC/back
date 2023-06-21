using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Authorization;
using Persistencia.Utils;
using WSolucaoWeb.Dtos.eFolha;
using Persistencia.DataBases;
using Persistencia.Models.EFolha;
using Persistencia.Models.Geral;

namespace WSolucaoWeb.Controllers.eFolha
{
    [Authorize]
    [Route("estabelecimento")]
    [ApiController]
    public class EstabelecimentosController : ControllerBase
    {
        private readonly WebApiContext _context;
        private readonly IConfiguration _configuration;
        private readonly WebApiContext _webApiContext;
        private IQueryable<Estabelecimento> estabelecimentoQuery;

        public EstabelecimentosController(WebApiContext context,
            IConfiguration configuration,
            WebApiContext webApiContext)
        {
            _context = context;
            _configuration = configuration;
            _webApiContext = webApiContext;
            estabelecimentoQuery = context.Estabelecimentos.AsQueryable();
        }

        // GET: api/Estabelecimentos
        [HttpGet]
        public async Task<ActionResult<List<Estabelecimento>>> GetEstabelecimentos()
        {
            return await estabelecimentoQuery.ToListAsync();
        }

        // GET: api/Estabelecimentos/5
        [HttpGet("consultar/{id}")]
        public async Task<ActionResult<Estabelecimento>> GetEstabelecimento(int id)
        {
            estabelecimentoQuery = estabelecimentoQuery
                                        .Where(x => x.Id == id)
                                        .Include(x => x.ImagemLogoMarca)
                                        .AsQueryable();

            var estabelecimento = await estabelecimentoQuery
                                        .FirstOrDefaultAsync();

            if (estabelecimento.ImagemLogoMarca != null)
            {
                estabelecimento.ImagemLogoMarca.Uri = "";
            }

            if (estabelecimento == null)
            {
                return NotFound();
            }

            return estabelecimento;
        }

        [HttpGet("maiorcodigo/{empresaId}")]
        public async Task<int> MaiorCodigo(int empresaId)
        {
            estabelecimentoQuery = estabelecimentoQuery
                .Where(x => x.EmpresaId == empresaId)
                .AsQueryable();
            int maior = 0;
            int x = await estabelecimentoQuery.CountAsync();
            if (x == 0)
            {
                return maior;
            }
            maior = await estabelecimentoQuery.MaxAsync(x => x.CodigoEstabelecimento);
            return maior;
        }

        // POST: api/Estabelecimentos
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost("inserir")]
        public async Task<ActionResult> PostEstabelecimento(EstabelecimentoDto estabelecimentoDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var nImagens = estabelecimentoDto.ImagemLogoMarca;

                estabelecimentoDto.ImagemLogoMarca = null;

                Estabelecimento estabelecimento = new Estabelecimento
                {
                    Id = estabelecimentoDto.Id,
                    CodigoEstabelecimento = estabelecimentoDto.CodigoEstabelecimento,
                    NomeFantasia = estabelecimentoDto.NomeFantasia,
                    RazaoSocial = estabelecimentoDto.RazaoSocial,
                    Nickname = estabelecimentoDto.Nickname,
                    CnpjCpf = estabelecimentoDto.CnpjCpf,
                    InicioAtividade = estabelecimentoDto.InicioAtividade,
                    Cep = estabelecimentoDto.Cep,
                    Endereco = estabelecimentoDto.Endereco,
                    NumeroEndereco = estabelecimentoDto.NumeroEndereco,
                    ComplementoEndereco = estabelecimentoDto.ComplementoEndereco,
                    Bairro = estabelecimentoDto.Bairro,
                    NomeMunicipio = estabelecimentoDto.NomeMunicipio,
                    Uf = estabelecimentoDto.Uf,
                    DddTelefone = estabelecimentoDto.DddTelefone,
                    Telefone = estabelecimentoDto.Telefone,
                    DddCelular = estabelecimentoDto.DddCelular,
                    Celular = estabelecimentoDto.Celular,
                    Email = estabelecimentoDto.Email,
                    LinkBpo = estabelecimentoDto.LinkBpo,
                    Ativo = estabelecimentoDto.Ativo,
                    ImagemLogoMarca = estabelecimentoDto.ImagemLogoMarca,
                    EmpresaId = estabelecimentoDto.EmpresaId,
                    CodigoMunicipio = estabelecimentoDto.CodigoMunicipio,
                };

                if (estabelecimentoDto.SincWeb)
                {
                    var empresa = await _context.Empresas.Where(e => e.CodigoEmpresa == estabelecimentoDto.EmpresaId).FirstOrDefaultAsync();

                    estabelecimentoDto.EmpresaId = empresa.Id;
                    estabelecimento.EmpresaId = empresa.Id;

                    estabelecimentoQuery = estabelecimentoQuery
                        .Where(e => e.EmpresaId == estabelecimentoDto.EmpresaId && e.CodigoEstabelecimento == estabelecimentoDto.CodigoEstabelecimento)
                        .AsQueryable();

                    Estabelecimento estabelecimentoWExiste = await estabelecimentoQuery.FirstOrDefaultAsync();

                    if (!(estabelecimentoWExiste is null))
                    {
                        return BadRequest(Respostas.Erro($"Já existe um estabelecimento cadastrado com o CodigoEstabelecimento = {estabelecimentoDto.CodigoEstabelecimento} na empresa {estabelecimentoDto.EmpresaId}!"));
                    }
                }

                estabelecimento.PossuiNire = true;
                if (!estabelecimento.PossuiNire && estabelecimento.PossuiNire)
                {
                    estabelecimento.PossuiNire = true;
                }

                _context.Estabelecimentos.Add(estabelecimento);

                if (await _context.SaveChangesAsync() == null)
                {
                    if (estabelecimentoDto.SincWeb)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, Respostas.Erro("Erro interno ao salvar registro de Estabelecimento de codigoEstabelecimento " + estabelecimentoDto.CodigoEstabelecimento + "na empresa " + estabelecimentoDto.EmpresaId));
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, Respostas.Erro("Erro interno ao salvar registro"));
                    }
                }

                if (estabelecimentoDto.SincWeb)
                {
                    var id_portal = estabelecimento.Id;

                    return Ok(new { mensagem = "Estabelecimento de codigoEstabelecimento " + estabelecimentoDto.CodigoEstabelecimento + " e empresaId " + estabelecimentoDto.EmpresaId + " criado com sucesso", id_portal });
                }
                else
                {
                    return CreatedAtAction("GetEstabelecimento", new { id = estabelecimento.Id }, estabelecimento);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Respostas.Erro(ex.Message));
            }
        }

        // PUT: api/Estabelecimentos/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("atualizar")]
        public async Task<IActionResult> PutEstabelecimento(EstabelecimentoDto estabelecimentoDto)
        {
            if (!(estabelecimentoDto.Id > 0))
            {
                return StatusCode(StatusCodes.Status400BadRequest,
                       Respostas.Erro("Identificador do estabelecimento inválido ou inexistente!"));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (estabelecimentoDto.SincWeb)
            {
                var empresa = await _context.Empresas.Where(e => e.CodigoEmpresa == estabelecimentoDto.EmpresaId).FirstOrDefaultAsync();

                estabelecimentoDto.EmpresaId = empresa.Id;

                estabelecimentoQuery = estabelecimentoQuery
                .Where(e => e.EmpresaId == estabelecimentoDto.EmpresaId && e.CodigoEstabelecimento == estabelecimentoDto.CodigoEstabelecimento)
                .AsNoTracking()
                .AsQueryable();

                Estabelecimento vEstabelecimento = await estabelecimentoQuery.FirstOrDefaultAsync();

                if (vEstabelecimento is null)
                {
                    return BadRequest(Respostas.Erro($"Estabelecimento com codigoEstabelecimento = {estabelecimentoDto.CodigoEstabelecimento} não encontrado para empresa {estabelecimentoDto.EmpresaId}."));
                }
            }
            else
            {
                estabelecimentoQuery = estabelecimentoQuery
                .Where(x => x.Id == estabelecimentoDto.Id)
                .AsNoTracking()
                .AsQueryable();

                Estabelecimento est = await estabelecimentoQuery.FirstOrDefaultAsync();

                if (estabelecimentoDto.EmpresaId == 0)
                {
                    estabelecimentoDto.EmpresaId = est.EmpresaId;
                }
            }

            estabelecimentoDto.PossuiNire = true;
            if (!estabelecimentoDto.PossuiNire && estabelecimentoDto.PossuiNire)
            {
                estabelecimentoDto.PossuiNire = true;
            }

            estabelecimentoQuery = estabelecimentoQuery
                .Where(x => x.Id == estabelecimentoDto.Id)
                .AsNoTracking()
                .AsQueryable();

            Estabelecimento estabelecimento = await estabelecimentoQuery.FirstOrDefaultAsync();

            estabelecimento.Id = estabelecimentoDto.Id;
            estabelecimento.CodigoEstabelecimento = estabelecimentoDto.CodigoEstabelecimento;
            estabelecimento.NomeFantasia = estabelecimentoDto.NomeFantasia;
            estabelecimento.RazaoSocial = estabelecimentoDto.RazaoSocial;
            estabelecimento.Nickname = estabelecimentoDto.Nickname;
            estabelecimento.CnpjCpf = estabelecimentoDto.CnpjCpf;
            estabelecimento.InicioAtividade = estabelecimentoDto.InicioAtividade;
            estabelecimento.Cep = estabelecimentoDto.Cep;
            estabelecimento.Endereco = estabelecimentoDto.Endereco;
            estabelecimento.NumeroEndereco = estabelecimentoDto.NumeroEndereco;
            estabelecimento.ComplementoEndereco = estabelecimentoDto.ComplementoEndereco;
            estabelecimento.Bairro = estabelecimentoDto.Bairro;
            estabelecimento.NomeMunicipio = estabelecimentoDto.NomeMunicipio;
            estabelecimento.Uf = estabelecimentoDto.Uf;
            estabelecimento.DddTelefone = estabelecimentoDto.DddTelefone;
            estabelecimento.Telefone = estabelecimentoDto.Telefone;
            estabelecimento.DddCelular = estabelecimentoDto.DddCelular;
            estabelecimento.Celular = estabelecimentoDto.Celular;
            estabelecimento.Email = estabelecimentoDto.Email;
            estabelecimento.LinkBpo = estabelecimentoDto.LinkBpo;
            estabelecimento.Ativo = estabelecimentoDto.Ativo;
            estabelecimento.ImagemLogoMarca = estabelecimentoDto.ImagemLogoMarca;
            estabelecimento.EmpresaId = estabelecimentoDto.EmpresaId;
            estabelecimento.CodigoMunicipio = estabelecimentoDto.CodigoMunicipio;
            estabelecimento.PossuiNire = estabelecimentoDto.PossuiNire;

            _context.Entry(estabelecimento).State = EntityState.Modified;

            try
            {
                if (await _context.SaveChangesAsync() == null)
                {
                    if (estabelecimentoDto.SincWeb)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, Respostas.Erro("Erro interno ao salvar registro de estabelecimento com CodigoEstabelecimento " + estabelecimentoDto.CodigoEstabelecimento + " e empresaId " + estabelecimentoDto.EmpresaId));
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, Respostas.Erro("Erro interno ao salvar registro"));
                    }

                }

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await EstabelecimentoExists(estabelecimentoDto.Id))
                {
                    if (estabelecimentoDto.SincWeb)
                    {
                        return BadRequest(Respostas.Erro($"Erro! Estabelecimento com id = {estabelecimentoDto.Id} não Cadastrada."));
                    }
                    else
                    {
                        return NotFound();
                    }
                        
                }
                else
                {
                    throw;
                }
            }

            if (estabelecimentoDto.SincWeb)
            {
                var id_portal = estabelecimentoDto.Id;

                return Ok(new { mensagem = "Estabelecimento com CodigoEstabelecimento " + estabelecimentoDto.CodigoEstabelecimento + " e empresaId " + estabelecimentoDto.EmpresaId + " atualizada com sucesso.", id_portal });
            }
            else
            {
                return NoContent();
            }                
        }

        private async Task<bool> EstabelecimentoExists(int id)
        {
            return await estabelecimentoQuery.AnyAsync(e => e.Id == id);
        }

        [HttpPost("listar")]
        public async Task<ActionResult> ListarEstabelecimentos(PropriedadesTabelaDto prop)
        {
            try
            {
                if (prop.FiltroArray[0] != "")
                    prop.FiltroArray[0] = prop.FiltroArray[0].ToString().ToLower();
                if (prop.FiltroArray[1] != "")
                    prop.FiltroArray[1] = prop.FiltroArray[1].ToString().Replace(".", "")
                        .Replace("/", "").Replace("-", "").ToLower();

                if (prop.FiltroArray[2] != "")
                    prop.FiltroArray[2] = prop.FiltroArray[2].ToLower();
                if (prop.FiltroArray[3] != "")
                    prop.FiltroArray[3] = prop.FiltroArray[3].ToString().ToLower();
                if (prop.FiltroArray[4] != "")
                    prop.FiltroArray[4] = prop.FiltroArray[4].ToLower();

                var auxText = prop.FiltroArray[5].ToLower();
                if (auxText != "")
                {
                    if (auxText.Length == 3 && auxText == "sim")
                    {
                        auxText = "true";
                    }
                    else if (auxText.Length == 3 && auxText == "não")
                    {
                        auxText = "false";
                    }
                    else
                    {
                        auxText = "0";
                    }
                }

                IQueryable<object> query = _context.Estabelecimentos
                    .Where(e => (prop.FiltroArray[0] == "" || prop.FiltroArray[0] == null) &&
                                 (prop.FiltroArray[1] == "" || prop.FiltroArray[1] == null) &&
                                 (prop.FiltroArray[2] == "" || prop.FiltroArray[2] == null) &&
                                 (prop.FiltroArray[3] == "" || prop.FiltroArray[3] == null) &&
                                 (prop.FiltroArray[4] == "" || prop.FiltroArray[4] == null) &&
                                 (prop.FiltroArray[5] == "" || prop.FiltroArray[5] == null) ? 1 == 1 :

                                    (("000" + e.Empresa.CodigoEmpresa.ToString()).Substring(("000" + e.Empresa.CodigoEmpresa.ToString()).Length - 4) +
                                    " - " + e.Empresa.RazaoSocial).ToLower()
                                    .Contains(prop.FiltroArray[0]) &&
                                    (("000" + e.CodigoEstabelecimento.ToString()).Substring(("000" + e.CodigoEstabelecimento.ToString()).Length - 4) +
                                    " - " + e.RazaoSocial + " - " + e.CnpjCpf).ToLower()
                                    .Contains(prop.FiltroArray[1]) &&
                                    e.Nickname.ToLower().Contains(prop.FiltroArray[2]) &&
                                    e.Telefone.ToLower().Contains(prop.FiltroArray[3]) &&
                                    e.Email.ToLower().Contains(prop.FiltroArray[4]) &&
                                    e.Ativo.ToString().ToLower().Contains(auxText))
                    .Select(e => new
                    {
                        e.Id,
                        Empresa = ("000" + e.Empresa.CodigoEmpresa).Substring(("000" + e.Empresa.CodigoEmpresa).Length - 4) + " - " + e.Empresa.RazaoSocial,
                        Estabelecimento = ("000" + e.CodigoEstabelecimento).Substring(("000" + e.CodigoEstabelecimento).Length - 4) + " - " + e.RazaoSocial + " - " + e.CnpjCpf,
                        e.Nickname,
                        e.NomeFantasia,
                        e.ImagemLogoMarca,
                        e.Cep,
                        e.Endereco,
                        e.NumeroEndereco,
                        e.ComplementoEndereco,
                        e.Bairro,
                        e.CodigoMunicipio,
                        e.NomeMunicipio,
                        e.Uf,
                        e.DddTelefone,
                        e.Telefone,
                        e.Celular,
                        e.Email,
                        e.Ativo
                    })
                    .AsQueryable();

                var count = await query.CountAsync();

                prop.NomeColunaOrdem = char.ToUpper(prop.NomeColunaOrdem[0]) + prop.NomeColunaOrdem.Substring(1);

                if (prop.Ordem == "false")
                    query = query.OrderBy(p => EF.Property<object>(p, prop.NomeColunaOrdem));
                else if (prop.Ordem == "true")
                    query = query.OrderByDescending(p => EF.Property<object>(p, prop.NomeColunaOrdem));

                query = query
                    .Skip((prop.Page - 1) * prop.Size)
                    .Take(prop.Size);

                var estabelecimentos = await query.ToArrayAsync();

                return Ok(new { totalDeRegistros = count, estabelecimentos });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Respostas.Erro(ex.Message));
            }
        }

        [HttpGet("listarcombo")]
        public async Task<ActionResult> ListarEstabelecimentosCombo()
        {
            try
            {

                IQueryable<object> query = _context.Estabelecimentos
                    .Select(e => new
                    {
                        value = e.Id,
                        text = ("000" + e.Id).Substring(("000" + e.Id).Length - 4)
                        + " - " +
                        e.RazaoSocial
                        + " - " +
                        (e.CnpjCpf.Length == 11 ? Convert.ToUInt64(e.CnpjCpf).ToString(@"000\.000\.000\-00") : Convert.ToUInt64(e.CnpjCpf.Length).ToString(@"00\.000\.000\/0000\-00"))
                    })
                    .OrderBy(x => x.value)
                    .AsQueryable();



                var estabelecimentos = await query.ToArrayAsync();

                return Ok(new { estabelecimentos });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Respostas.Erro(ex.Message));
            }
        }

        [HttpGet("listarcomboselecaoestabelecimento/{id}")]
        public async Task<ActionResult> ListarSelecaoEstabelecimentoCombo([FromRoute] int id)
        {
            try
            {
                IQueryable<object> estabelecimentosIds = _context.UsuariosEstabelecimentos
                    .Include(f => f.Estabelecimento)
                    .Where(x => x.UsuarioId == id)
                    .Select(e => new
                    {
                        value = e.EstabelecimentoId,
                        text = e.Estabelecimento.RazaoSocial
                    })
                    .OrderBy(e => e.value)
                    .AsQueryable();

                var estabelecimentos = await estabelecimentosIds.ToArrayAsync();
                return Ok(new { estabelecimentos });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Respostas.Erro(ex.Message));
            }
        }

        [HttpGet("validacodigoestabelecimento/{empresaId}/{codigoEstabelecimento}")]
        public async Task<bool> ValidaCodigo(int empresaId, int codigoEstabelecimento)
        {
            estabelecimentoQuery = estabelecimentoQuery
                .Where(x => x.EmpresaId == empresaId && x.CodigoEstabelecimento == codigoEstabelecimento)
                .AsQueryable();
            Estabelecimento x = await estabelecimentoQuery.FirstOrDefaultAsync();
            if (x != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpPut("atualizarativogrid/{id}")]
        public async Task<ActionResult> AtualizarAtivoGrid(Estabelecimento estabelecimento)
        {
            try
            {
                ModelState.Remove("Uf");
                ModelState.Remove("RazaoSocial");
                ModelState.Remove("NomeMunicipio");
                ModelState.Remove("CodigoEstabelecimento");
                ModelState.Remove("CodigoMunicipio");

                if (!ModelState.IsValid)
                {
                    string messages = string.Join(" ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                    return BadRequest(Respostas.MensagemDeErroDoModelo(messages));
                }

                if (!(estabelecimento.Id > 0))
                {
                    return StatusCode(StatusCodes.Status400BadRequest,
                                           Respostas.Erro("Identificador do estabelecimento inválido ou inexistente!"));
                }

                Estabelecimento estabelecimentox = await _webApiContext.Estabelecimentos.Where(x => x.Id == estabelecimento.Id).FirstOrDefaultAsync();

                estabelecimentox.Ativo = estabelecimento.Ativo;

                _webApiContext.Entry(estabelecimentox).State = EntityState.Modified;

                await _webApiContext.SaveChangesAsync();

                return Ok(Respostas.Sucesso("Operação concluída com sucesso!"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Respostas.Erro(ex.Message));
            }
        }
    }
}
