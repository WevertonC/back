using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Persistencia.Models.Geral;

namespace Persistencia.Models.EFolha
{
    public class Usuario : IdentityUser<int>
    {
        public string Celular { get; set; }

        public bool Ativo { get; set; }

        public DateTime DataDaCriacao { get; set; }

        [JsonIgnore]
        public virtual UsuarioImagem Imagem { get; set; }

        [JsonIgnore]
        public List<UsuarioPerfil> UsuariosPerfis { get; set; }

        [JsonIgnore]
        public ICollection<UsuarioLinkTemporario> LinkTemporarios { get; set; }

        [JsonIgnore]
        public List<UsuarioEstabelecimento> UsuariosEstabelecimentos { get; set; }

        [JsonIgnore]
        public virtual List<Estabelecimento> Estabelecimentos { get; set; }

    }
}
