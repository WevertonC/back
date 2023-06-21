using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Persistencia.Validations.CustomValidations;
using Persistencia.Models.Enums;
using WebApi.Models.Enums;

namespace WSolucaoWeb.Dtos.eFolha
{
    public class ArquivoDto
    {
        public int Id { get; set; }

        public string NomeArquivo { get; set; }

        public DateTime? DataVencimento { get; set; }

        public decimal? Valor { get; set; }

        public int? CategoriaId { get; set; }

        public int? EstabelecimentoId { get; set; }

        public EOrigemArquivo Origem { get; set; }

        public DateTime DataEnvio { get; private set; }

        [NotMapped]
        [DataType(DataType.Upload)]
        [MaxFileSize(5 * 1024 * 1024)]
        public virtual IFormFile Mime { get; set; }

        public int? TamanhoEmBytes { get; set; }

        public int? UsuarioId { get; set; }

        public bool Ativo { get; private set; }

        public DateTime? DataInatividade { get; private set; }

        public string TabelaReferencia { get; set; }

        public int? TicketInteracaoId { get; set; }
    }
}
