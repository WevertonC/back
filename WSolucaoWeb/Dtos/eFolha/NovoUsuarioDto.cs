using System.ComponentModel.DataAnnotations;
using SendGrid.Helpers.Mail;
using Persistencia.Models.Enums;
using Persistencia.Models.EFolha;

namespace WSolucaoWeb.Dtos.eFolha
{
    public class NovoUsuarioDto
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public string Celular { get; set; }

        public string Email { get; set; }

        public TipoAcesso TipoAcesso { get; set; }

        public NivelAcesso NivelAcesso { get; set; }

        public virtual UsuarioImagem ImagemUsuario { get; set; }

        public bool Ativo { get; set; }

        public int[] EstabelecimentosIds { get; set; }

        public string Telefone { get; set; }

        public string Senha { get; set; }

        public string ConfirmacaoSenha { get; set; }
    }
}
