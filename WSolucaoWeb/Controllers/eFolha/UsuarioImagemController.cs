using System;
using System.Threading.Tasks;
using System.IO;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Authorization;
using WSolucaoWeb.Dtos.eFolha;
using Persistencia.DataBases;
using Persistencia.Utils;
using Persistencia.Models.EFolha;

namespace WSolucaoWeb.Controllers.eFolha
{
    [Authorize]
    [Route("imagem")]
    [ApiController]
    public class UsuarioImagemController : ControllerBase
    {
        private readonly WebApiContext _context;
        private readonly IConfiguration _configuration;

        public UsuarioImagemController(WebApiContext context,
                                 IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
    }
}
