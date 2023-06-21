using Persistencia.Models.EFolha;
using Persistencia.Models.Enums;
using System;
using System.Collections.Generic;

namespace WSolucaoWeb.Dtos.eFolha
{
    public class UsuarioDto
    {

        public int Id { get; set; }

        public virtual string Email { get; set; }

        public virtual string UserName { get; set; }

        public string Celular { get; set; }

        public DateTime DataDaCriacao { get; set; }

        public ImagemDto Imagem { get; set; }

        public List<UsuarioPerfilDto> UsuariosPerfis { get; set; }

        public List<UsuarioEstabelecimento> UsuariosEstabelecimentos { get; set; }
    }

    public class UsuarioPerfilDto
    {
        public PerfilDto Perfil { get; set; }
    }

    public class PerfilDto
    {
        public string Name { get; set; }

        public TipoAcesso TipoAcesso { get; set; }

        public NivelAcesso NivelAcesso { get; set; }
    }

    public class ImagemDto
    {
        public string Nome { get; set; }
        public string Uri { get; set; }
    }
}
