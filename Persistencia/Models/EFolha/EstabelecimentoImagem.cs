using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Persistencia.Validations.CustomValidations;

namespace Persistencia.Models.EFolha
{
    public class EstabelecimentoImagem
    {
        [Key]
        public int Id { get; set; }

        public int ImagemLogomarcaId { get; set; }

        public string NomeImagem { get; set; }

        public string Uri { get; set; }

        public string NomeArquivoStorage { get; set; }

        [NotMapped]
        [DataType(DataType.Upload)]
        [MaxFileSize(5 * 1024 * 1024)]
        public virtual IFormFile Mime { get; set; }
    }
}
