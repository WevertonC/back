using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Persistencia.Models.Enums;

namespace Persistencia.Models.EFolha
{
    public class Perfil : IdentityRole<int>
    {
        public Perfil(TipoAcesso tipoAcesso, NivelAcesso nivelAcesso)
        {
            TipoAcesso = tipoAcesso;
            NivelAcesso = nivelAcesso;
            Name = $"{tipoAcesso.ToString()} {nivelAcesso.ToString()}";
        }

        public Perfil() { }

        public TipoAcesso TipoAcesso { get; set; }

        public NivelAcesso NivelAcesso { get; set; }

        public List<UsuarioPerfil> UsuariosPerfis { get; set; }
    }
}
