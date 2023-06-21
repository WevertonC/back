namespace Persistencia.Models.SulProg
{
    public class ClienteSulProg
    {
        public int Id { get; set; }

        public string RazaoSocial { get; set; }

        public string NomeFantasia { get; set; }

        public string Cep { get; set; }

        public string Logradouro { get; set; }

        public string Bairro { get; set; }

        public string Complemento { get; set; }

        public int Numero { get; set; }

        public string Cidade { get; set; }

        public string UF { get; set; }

        public string Pais { get; set; }

        public string Cnpj { get; set; }

        public string Cpf { get; set; }

        public string InscricaoEstadual { get; set; }

        public string InscricaoMunicipal { get; set; }

        public string DDDTelefone { get; set; }

        public string Telefone { get; set; }

        public string DDDCelular { get; set; }

        public string Celular { get; set; }

        public string DDDFax { get; set; }

        public string Fax { get; set; }

        public string Email { get; set; }

        public bool Ativo { get; set; }

        public virtual LicencaSoftware LicencaSoftware { get; set; }
    }
}
