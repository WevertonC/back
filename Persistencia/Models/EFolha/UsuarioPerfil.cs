using Microsoft.AspNetCore.Identity;

namespace Persistencia.Models.EFolha
{
    public class UsuarioPerfil : IdentityUserRole<int>
    {
        public int Id { get; set; }

        public Usuario Usuario { get; set; }

        public Perfil Perfil { get; set; }
    }
}
