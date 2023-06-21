using System;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Persistencia.Utils;
using Persistencia.Models.EFolha;
using Persistencia.DataBases;

namespace WSolucaoWeb.Controllers.eFolha
{
    [Authorize]
    [Route("arquivo")]
    [ApiController]
    public class ArquivoController : ControllerBase
    {
        private readonly WebApiContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<Usuario> _userManager;

        private string NumeroDeSerieEscritorio { get; set; }
        private string Escritorio { get; set; }

        private MemoryStream _file;

        public ArquivoController(WebApiContext context,
                                 IConfiguration configuration,
                                 UserManager<Usuario> userManager)
        {
            _context = context;
            _configuration = configuration;
            _userManager = userManager;

            _file = new MemoryStream();
        }

        [HttpGet("listarcalendario/{id}")]
        public async Task<ActionResult> ListarArquivosCalendario(int id)
        {
            try
            {
                IQueryable<object> query = _context.Arquivo
                    .Where(x => x.EstabelecimentoId == id)
                    .Select(e => new
                    {
                        id = e.Id,
                        title = e.NomeArquivo,
                        start = e.DataVencimento,
                        url = "",
                        allDay = true
                    })
                    .AsQueryable();

                var events = await query.ToArrayAsync();

                return Ok(new { events });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Respostas.Erro(ex.Message));
            }
        }
    }
}
