using System.Text.Json.Serialization;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Persistencia.Models.Geral;
using Persistencia.Validations.CustomValidations;

namespace Persistencia.Models.EFolha
{
    public class Arquivo
    {
        public int Id { get; set; }

        public int? EstabelecimentoId { get; set; }

        public virtual Estabelecimento Estabelecimento { get; set; }

        public int? CategoriaId { get; set; }

        public int? TicketInteracaoId { get; set; }

        public int? UsuarioId { get; set; }

        public Usuario Usuario { get; set; }

        public string NomeArquivo { get; set; }

        public DateTime? DataVencimento { get; set; }

        public DateTime? DataHoraEnvio { get; private set; }

        public int? TamanhoEmBytes { get; set; }

        public string Uri { get; set; }

        public string NomeArquivoStorage { get; set; }

        [NotMapped]
        [DataType(DataType.Upload)]
        [MaxFileSize(5 * 1024 * 1024)]
        public virtual IFormFile Mime { get; set; }

        public Arquivo()
        {
            DataHoraEnvio = DateTime.Now;
        }
    }
}
