using Microsoft.EntityFrameworkCore;
using Persistencia.Models.SulProg;

namespace Persistencia.DataBases
{
    public class SulProgContext : DbContext
    {
        public SulProgContext(DbContextOptions<SulProgContext> options) : base(options) { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Altera o nome da tabela de Users Entity pata Usuarios
            modelBuilder.Entity<LicencaSoftware>().ToTable("licencasoftware");
            modelBuilder.Entity<ClienteSulProg>().ToTable("cliente");

            // Altera o nome das colunas da tabela usuario
            modelBuilder.Entity<LicencaSoftware>().Property(l => l.Id).HasColumnName("id");
            modelBuilder.Entity<LicencaSoftware>().Property(l => l.NumeroSerie).HasColumnName("numeroserie");
            modelBuilder.Entity<LicencaSoftware>().Property(l => l.ClienteId).HasColumnName("clienteid");
            modelBuilder.Entity<LicencaSoftware>().Property(l => l.NumeroBases).HasColumnName("numerobases");
            modelBuilder.Entity<LicencaSoftware>().Property(l => l.NumeroUsuariosSimultaneos).HasColumnName("numerousuariossimultaneos");
            modelBuilder.Entity<LicencaSoftware>().Property(l => l.StatusLicenca).HasColumnName("statuslicenca");
            modelBuilder.Entity<LicencaSoftware>().Property(l => l.DataContrato).HasColumnName("datacontrato");
            modelBuilder.Entity<LicencaSoftware>().Property(l => l.DataCancelamento).HasColumnName("datacancelamento");
            modelBuilder.Entity<LicencaSoftware>().Property(l => l.MotivoCancelamento).HasColumnName("motivocancelamento");
            modelBuilder.Entity<LicencaSoftware>().Property(l => l.DataRegistro).HasColumnName("dataregistro");
            modelBuilder.Entity<LicencaSoftware>().Property(l => l.ImplantadaNoCliente).HasColumnName("implantadanocliente");

            modelBuilder.Entity<ClienteSulProg>().Property(c => c.Id).HasColumnName("id");
            modelBuilder.Entity<ClienteSulProg>().Property(c => c.RazaoSocial).HasColumnName("razaosocial");
            modelBuilder.Entity<ClienteSulProg>().Property(c => c.NomeFantasia).HasColumnName("nomefantasia");
            modelBuilder.Entity<ClienteSulProg>().Property(c => c.Cep).HasColumnName("cep");
            modelBuilder.Entity<ClienteSulProg>().Property(c => c.Logradouro).HasColumnName("logradouro");
            modelBuilder.Entity<ClienteSulProg>().Property(c => c.Bairro).HasColumnName("bairro");
            modelBuilder.Entity<ClienteSulProg>().Property(c => c.Complemento).HasColumnName("complemento");
            modelBuilder.Entity<ClienteSulProg>().Property(c => c.Numero).HasColumnName("numero");
            modelBuilder.Entity<ClienteSulProg>().Property(c => c.Cidade).HasColumnName("cidade");
            modelBuilder.Entity<ClienteSulProg>().Property(c => c.UF).HasColumnName("uf");
            modelBuilder.Entity<ClienteSulProg>().Property(c => c.Pais).HasColumnName("pais");
            modelBuilder.Entity<ClienteSulProg>().Property(c => c.Cnpj).HasColumnName("cnpj");
            modelBuilder.Entity<ClienteSulProg>().Property(c => c.Cpf).HasColumnName("cpf");
            modelBuilder.Entity<ClienteSulProg>().Property(c => c.InscricaoEstadual).HasColumnName("inscricaoestadual");
            modelBuilder.Entity<ClienteSulProg>().Property(c => c.InscricaoMunicipal).HasColumnName("inscricaomunicipal");
            modelBuilder.Entity<ClienteSulProg>().Property(c => c.DDDTelefone).HasColumnName("dddtelefone");
            modelBuilder.Entity<ClienteSulProg>().Property(c => c.Telefone).HasColumnName("telefone");
            modelBuilder.Entity<ClienteSulProg>().Property(c => c.DDDCelular).HasColumnName("dddcelular");
            modelBuilder.Entity<ClienteSulProg>().Property(c => c.Celular).HasColumnName("celular");
            modelBuilder.Entity<ClienteSulProg>().Property(c => c.DDDFax).HasColumnName("dddfax");
            modelBuilder.Entity<ClienteSulProg>().Property(c => c.Fax).HasColumnName("fax");
            modelBuilder.Entity<ClienteSulProg>().Property(c => c.Email).HasColumnName("email");
            modelBuilder.Entity<ClienteSulProg>().Property(c => c.Ativo).HasColumnName("ativo");


            modelBuilder.Entity<ClienteSulProg>().HasOne(c => c.LicencaSoftware).WithOne(l => l.Cliente).HasForeignKey<LicencaSoftware>(l => l.ClienteId);
        }

        public DbSet<LicencaSoftware> LicencaSoftwares { get; set; }
        public DbSet<ClienteSulProg> ClientesSulProg { get; set; }
    }
}
