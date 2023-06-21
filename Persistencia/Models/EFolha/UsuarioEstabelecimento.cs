using Persistencia.Models.Geral;

namespace Persistencia.Models.EFolha
{
    public class UsuarioEstabelecimento
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }

        public int EstabelecimentoId { get; set; }

        public Usuario Usuario { get; set; }

        public Estabelecimento Estabelecimento { get; set; }
    }
}
