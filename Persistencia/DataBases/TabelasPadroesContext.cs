using Microsoft.EntityFrameworkCore;
using Persistencia.Models.TabelasPadroes;

namespace Persistencia.DataBases
{
    public class TabelasPadroesContext : DbContext
    {
        public TabelasPadroesContext(DbContextOptions<TabelasPadroesContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mapeamento tabelas
            modelBuilder.Entity<MunicipiosIbge>().ToTable("municipiosibge");
            modelBuilder.Entity<Pais>().ToTable("paises");
            // Mapeamento campos
            modelBuilder.Entity<MunicipiosIbge>().Property(l => l.Id).HasColumnName("id");
            modelBuilder.Entity<MunicipiosIbge>().Property(l => l.CodigoUf).HasColumnName("codigouf");
            modelBuilder.Entity<MunicipiosIbge>().Property(l => l.Uf).HasColumnName("uf");
            modelBuilder.Entity<MunicipiosIbge>().Property(l => l.CodigoMunicipio).HasColumnName("codigomunicipio");
            modelBuilder.Entity<MunicipiosIbge>().Property(l => l.NomeMunicipio).HasColumnName("nomemunicipio");

            modelBuilder.Entity<Pais>().Property(l => l.Id).HasColumnName("id");
            modelBuilder.Entity<Pais>().Property(l => l.Cod_pais).HasColumnName("cod_pais");
            modelBuilder.Entity<Pais>().Property(l => l.Nom_pais).HasColumnName("nom_pais");
            modelBuilder.Entity<Pais>().Property(l => l.Dt_ini).HasColumnName("dt_ini");
            modelBuilder.Entity<Pais>().Property(l => l.Dt_fim).HasColumnName("dt_fim");
        }

        public DbSet<MunicipiosIbge> MunicipiosIbge { get; set; }
        public DbSet<Pais> Paises { get; set; }

    }
}
