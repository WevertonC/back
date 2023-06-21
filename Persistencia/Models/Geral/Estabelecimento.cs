using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Persistencia.Utils;
using Persistencia.Models.EFolha;

namespace Persistencia.Models.Geral
{
    public class Estabelecimento
    {
        public int Id { get; set; }

        public int EmpresaId { get; set; }

        [JsonIgnore]
        public virtual Empresa Empresa { get; set; }

        public int? ContadorId { get; set; }

        public int CodigoEstabelecimento { get; set; }

        public string RazaoSocial { get; set; }

        public string RazaoSocialCompleta { get; set; }

        public string NomeFantasia { get; set; }

        public string CnpjCpf { get; set; }

        public string Cei { get; set; }

        public string Caepf { get; set; }

        public string InscricaoEstadual { get; set; }

        public string InscricaoSubstitutoTributario { get; set; }

        public string InscricaoMunicipal { get; set; }

        public string InscricaoSuframa { get; set; }

        public string NumeroAlvara { get; set; }

        public DateTime? InicioAtividade { get; set; }

        public bool PossuiNire { get; set; }

        public string InscricaoTse { get; set; }

        public string InscricaoBancoCentral { get; set; }

        public string InscricaoSusep { get; set; }

        public string InscricaoCvm { get; set; }

        public string InscricaoAntt { get; set; }

        public DateTime DataAlteracaoSociedadeSimples { get; set; }

        public string LocalRegistroContrato { get; set; }

        public string NumeroRegistroContrato { get; set; }

        public DateTime DataRegistroContrato { get; set; }

        public string Endereco { get; set; }

        public string NumeroEndereco { get; set; }

        public string ComplementoEndereco { get; set; }

        public string Bairro { get; set; }

        public string Cep { get; set; }

        public int CodigoMunicipio { get; set; }

        public string NomeMunicipio { get; set; }

        [ValidarEstado(ErrorMessage = "Campo UF Incorreto!")]
        public string Uf { get; set; }

        public string DddTelefone { get; set; }

        public string Telefone { get; set; }

        public string DddCelular { get; set; }

        public string Celular { get; set; }

        public string Email { get; set; }

        public virtual EstabelecimentoImagem ImagemLogoMarca { get; set; }

        public string LinkBpo { get; set; }

        public string Nickname { get; set; }

        public bool Ativo { get; set; }

        [JsonIgnore]
        public virtual IList<Arquivo> Arquivos { get; set; }

        [JsonIgnore]
        public List<UsuarioEstabelecimento> UsuariosEstabelecimentos { get; set; }

        [JsonIgnore]
        [NotMapped]
        public virtual IList<Usuario> Usuarios { get; set; }
    }
}
