using Persistencia.Models.Enums;
using System;

namespace Persistencia.Models.EFolha
{
    public class UsuarioLinkTemporario
    {
        public int Id { get; set; }

        public int? UsuarioId { get; set; }

        public TipoDeLinkTemporario TipoDeLinkTemporario { get; set; }

        public DateTime ValidoAte { get; set; }

        public Usuario Usuario { get; set; }
    }
}
