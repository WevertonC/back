using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace Persistencia.Models.Geral
{
    public class Empresa
    {
        public int Id { get; set; }

        public int CodigoEmpresa { get; set; }

        public string RazaoSocial { get; set; }

        public string NomeFantasia { get; set; }

        [JsonIgnore]
        public virtual List<Estabelecimento> Estabelecimentos { get; set; }

        public bool Ativo { get; set; }
    }
}
