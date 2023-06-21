using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Persistencia.Models.Geral;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistencia.Models.EFolha;
using Persistencia.Utils;
using Swashbuckle.Swagger;

namespace Persistencia.DataBases
{
    public class WebApiContext : IdentityDbContext<Usuario, Perfil, int, IdentityUserClaim<int>, UsuarioPerfil, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public WebApiContext(DbContextOptions<WebApiContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Empresa>().ToTable("empresas");
            modelBuilder.Entity<Estabelecimento>().ToTable("estabelecimentos");

            modelBuilder.Entity<Arquivo>().ToTable("arquivos");
            modelBuilder.Entity<Perfil>().ToTable("perfis");
            modelBuilder.Entity<Usuario>().ToTable("usuarios");
            modelBuilder.Entity<UsuarioEstabelecimento>().ToTable("usuariosestabelecimentos");
            modelBuilder.Entity<UsuarioImagem>().ToTable("usuariosimagens");
            modelBuilder.Entity<UsuarioLinkTemporario>().ToTable("usuarioslinkstemporarios");
            modelBuilder.Entity<UsuarioPerfil>().ToTable("usuariosperfis");
            modelBuilder.Entity<EstabelecimentoImagem>().ToTable("estabelecimentosimagens");

            modelBuilder.Entity<Usuario>().Property(p => p.UserName).HasColumnName("nome");
            modelBuilder.Entity<Usuario>().Property(p => p.PhoneNumber).HasColumnName("telefone");
            modelBuilder.Entity<Usuario>().Property(p => p.PasswordHash).HasColumnName("senha");

            modelBuilder.Entity<UsuarioPerfil>(usuarioPerfil =>
            {
                usuarioPerfil.HasKey(up => new { up.UserId, up.RoleId });
                usuarioPerfil.HasOne(up => up.Perfil).WithMany(p => p.UsuariosPerfis).HasForeignKey(up => up.RoleId).IsRequired();
                usuarioPerfil.HasOne(up => up.Usuario).WithMany(u => u.UsuariosPerfis).HasForeignKey(up => up.UserId).IsRequired();
            });

            modelBuilder.Entity<UsuarioEstabelecimento>(usuarioCliente =>
            {
                usuarioCliente.HasKey(up => new { up.UsuarioId, up.EstabelecimentoId });
                usuarioCliente.HasOne(up => up.Estabelecimento).WithMany(p => p.UsuariosEstabelecimentos).HasForeignKey(up => up.EstabelecimentoId).IsRequired();
                usuarioCliente.HasOne(up => up.Usuario).WithMany(u => u.UsuariosEstabelecimentos).HasForeignKey(up => up.UsuarioId).IsRequired();
            });

            modelBuilder.Entity<Usuario>().HasOne(u => u.Imagem).WithOne().HasForeignKey<UsuarioImagem>(i => i.UsuarioImagemId);
            modelBuilder.Entity<Estabelecimento>().HasOne(u => u.ImagemLogoMarca).WithOne().HasForeignKey<EstabelecimentoImagem>(i => i.ImagemLogomarcaId);

            modelBuilder.Entity<UsuarioLinkTemporario>().HasOne(lt => lt.Usuario).WithMany(u => u.LinkTemporarios).HasForeignKey(lt => lt.UsuarioId);
            modelBuilder.Entity<Estabelecimento>().HasOne(e => e.Empresa).WithMany(cc => cc.Estabelecimentos).OnDelete(DeleteBehavior.Restrict).HasForeignKey(cc => cc.EmpresaId);
        }   

        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Estabelecimento> Estabelecimentos { get; set; }

        // Entidades eFolha
        public DbSet<Arquivo> Arquivo { get; set; }
        public DbSet<Perfil> Perfis { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<UsuarioEstabelecimento> UsuariosEstabelecimentos { get; set; }
        public DbSet<UsuarioImagem> UsuariosImagens { get; set; }
        public DbSet<UsuarioLinkTemporario> UsuariosLinksTemporarios { get; set; }
        public DbSet<UsuarioPerfil> UsuariosPerfis { get; set; }
        public DbSet<EstabelecimentoImagem> EstabelecimentosImagens { get; set; }
    }
}
