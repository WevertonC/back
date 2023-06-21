using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Persistencia.Migrations
{
    public partial class v_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "empresas",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigoempresa = table.Column<int>(type: "integer", nullable: false),
                    razaosocial = table.Column<string>(type: "text", nullable: true),
                    nomefantasia = table.Column<string>(type: "text", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_empresas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "perfis",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tipoacesso = table.Column<int>(type: "integer", nullable: false),
                    nivelacesso = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalizedname = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    concurrencystamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_perfis", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    celular = table.Column<string>(type: "text", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    datadacriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    nome = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalizedusername = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalizedemail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    emailconfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    senha = table.Column<string>(type: "text", nullable: true),
                    securitystamp = table.Column<string>(type: "text", nullable: true),
                    concurrencystamp = table.Column<string>(type: "text", nullable: true),
                    telefone = table.Column<string>(type: "text", nullable: true),
                    phonenumberconfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    twofactorenabled = table.Column<bool>(type: "boolean", nullable: false),
                    lockoutend = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    lockoutenabled = table.Column<bool>(type: "boolean", nullable: false),
                    accessfailedcount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_usuarios", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    roleid = table.Column<int>(type: "integer", nullable: false),
                    claimtype = table.Column<string>(type: "text", nullable: true),
                    claimvalue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aspnetroleclaims", x => x.id);
                    table.ForeignKey(
                        name: "fk_aspnetroleclaims_aspnetroles_roleid",
                        column: x => x.roleid,
                        principalTable: "perfis",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userid = table.Column<int>(type: "integer", nullable: false),
                    claimtype = table.Column<string>(type: "text", nullable: true),
                    claimvalue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aspnetuserclaims", x => x.id);
                    table.ForeignKey(
                        name: "fk_aspnetuserclaims_aspnetusers_userid",
                        column: x => x.userid,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    loginprovider = table.Column<string>(type: "text", nullable: false),
                    providerkey = table.Column<string>(type: "text", nullable: false),
                    providerdisplayname = table.Column<string>(type: "text", nullable: true),
                    userid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aspnetuserlogins", x => new { x.loginprovider, x.providerkey });
                    table.ForeignKey(
                        name: "fk_aspnetuserlogins_aspnetusers_userid",
                        column: x => x.userid,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    userid = table.Column<int>(type: "integer", nullable: false),
                    loginprovider = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aspnetusertokens", x => new { x.userid, x.loginprovider, x.name });
                    table.ForeignKey(
                        name: "fk_aspnetusertokens_aspnetusers_userid",
                        column: x => x.userid,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "estabelecimentos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    empresaid = table.Column<int>(type: "integer", nullable: false),
                    contadorid = table.Column<int>(type: "integer", nullable: true),
                    codigoestabelecimento = table.Column<int>(type: "integer", nullable: false),
                    razaosocial = table.Column<string>(type: "text", nullable: true),
                    razaosocialcompleta = table.Column<string>(type: "text", nullable: true),
                    nomefantasia = table.Column<string>(type: "text", nullable: true),
                    cnpjcpf = table.Column<string>(type: "text", nullable: true),
                    cei = table.Column<string>(type: "text", nullable: true),
                    caepf = table.Column<string>(type: "text", nullable: true),
                    inscricaoestadual = table.Column<string>(type: "text", nullable: true),
                    inscricaosubstitutotributario = table.Column<string>(type: "text", nullable: true),
                    inscricaomunicipal = table.Column<string>(type: "text", nullable: true),
                    inscricaosuframa = table.Column<string>(type: "text", nullable: true),
                    numeroalvara = table.Column<string>(type: "text", nullable: true),
                    inicioatividade = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    possuinire = table.Column<bool>(type: "boolean", nullable: false),
                    inscricaotse = table.Column<string>(type: "text", nullable: true),
                    inscricaobancocentral = table.Column<string>(type: "text", nullable: true),
                    inscricaosusep = table.Column<string>(type: "text", nullable: true),
                    inscricaocvm = table.Column<string>(type: "text", nullable: true),
                    inscricaoantt = table.Column<string>(type: "text", nullable: true),
                    dataalteracaosociedadesimples = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    localregistrocontrato = table.Column<string>(type: "text", nullable: true),
                    numeroregistrocontrato = table.Column<string>(type: "text", nullable: true),
                    dataregistrocontrato = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    endereco = table.Column<string>(type: "text", nullable: true),
                    numeroendereco = table.Column<string>(type: "text", nullable: true),
                    complementoendereco = table.Column<string>(type: "text", nullable: true),
                    bairro = table.Column<string>(type: "text", nullable: true),
                    cep = table.Column<string>(type: "text", nullable: true),
                    codigomunicipio = table.Column<int>(type: "integer", nullable: false),
                    nomemunicipio = table.Column<string>(type: "text", nullable: true),
                    uf = table.Column<string>(type: "text", nullable: true),
                    dddtelefone = table.Column<string>(type: "text", nullable: true),
                    telefone = table.Column<string>(type: "text", nullable: true),
                    dddcelular = table.Column<string>(type: "text", nullable: true),
                    celular = table.Column<string>(type: "text", nullable: true),
                    email = table.Column<string>(type: "text", nullable: true),
                    linkbpo = table.Column<string>(type: "text", nullable: true),
                    nickname = table.Column<string>(type: "text", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    usuarioid = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_estabelecimentos", x => x.id);
                    table.ForeignKey(
                        name: "fk_estabelecimentos_empresas_empresaid",
                        column: x => x.empresaid,
                        principalTable: "empresas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_estabelecimentos_usuario_usuarioid",
                        column: x => x.usuarioid,
                        principalTable: "usuarios",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "usuariosimagens",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usuarioimagemid = table.Column<int>(type: "integer", nullable: false),
                    nomeimagem = table.Column<string>(type: "text", nullable: true),
                    uri = table.Column<string>(type: "text", nullable: true),
                    nomearquivostorage = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_usuariosimagens", x => x.id);
                    table.ForeignKey(
                        name: "fk_usuariosimagens_usuarios_usuarioimagemid",
                        column: x => x.usuarioimagemid,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "usuarioslinkstemporarios",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usuarioid = table.Column<int>(type: "integer", nullable: true),
                    tipodelinktemporario = table.Column<int>(type: "integer", nullable: false),
                    validoate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_usuarioslinkstemporarios", x => x.id);
                    table.ForeignKey(
                        name: "fk_usuarioslinkstemporarios_usuario_usuarioid",
                        column: x => x.usuarioid,
                        principalTable: "usuarios",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "usuariosperfis",
                columns: table => new
                {
                    userid = table.Column<int>(type: "integer", nullable: false),
                    roleid = table.Column<int>(type: "integer", nullable: false),
                    id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_usuariosperfis", x => new { x.userid, x.roleid });
                    table.ForeignKey(
                        name: "fk_usuariosperfis_perfis_roleid",
                        column: x => x.roleid,
                        principalTable: "perfis",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_usuariosperfis_usuarios_userid",
                        column: x => x.userid,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "arquivos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    estabelecimentoid = table.Column<int>(type: "integer", nullable: true),
                    categoriaid = table.Column<int>(type: "integer", nullable: true),
                    ticketinteracaoid = table.Column<int>(type: "integer", nullable: true),
                    usuarioid = table.Column<int>(type: "integer", nullable: true),
                    nomearquivo = table.Column<string>(type: "text", nullable: true),
                    datavencimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    datahoraenvio = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    tamanhoembytes = table.Column<int>(type: "integer", nullable: true),
                    uri = table.Column<string>(type: "text", nullable: true),
                    nomearquivostorage = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_arquivos", x => x.id);
                    table.ForeignKey(
                        name: "fk_arquivos_aspnetusers_usuarioid",
                        column: x => x.usuarioid,
                        principalTable: "usuarios",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_arquivos_estabelecimentos_estabelecimentoid",
                        column: x => x.estabelecimentoid,
                        principalTable: "estabelecimentos",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "estabelecimentosimagens",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    imagemlogomarcaid = table.Column<int>(type: "integer", nullable: false),
                    nomeimagem = table.Column<string>(type: "text", nullable: true),
                    uri = table.Column<string>(type: "text", nullable: true),
                    nomearquivostorage = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_estabelecimentosimagens", x => x.id);
                    table.ForeignKey(
                        name: "fk_estabelecimentosimagens_estabelecimentos_imagemlogomarcaid",
                        column: x => x.imagemlogomarcaid,
                        principalTable: "estabelecimentos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "usuariosestabelecimentos",
                columns: table => new
                {
                    usuarioid = table.Column<int>(type: "integer", nullable: false),
                    estabelecimentoid = table.Column<int>(type: "integer", nullable: false),
                    id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_usuariosestabelecimentos", x => new { x.usuarioid, x.estabelecimentoid });
                    table.ForeignKey(
                        name: "fk_usuariosestabelecimentos_estabelecimentos_estabelecimentoid",
                        column: x => x.estabelecimentoid,
                        principalTable: "estabelecimentos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_usuariosestabelecimentos_usuario_usuarioid",
                        column: x => x.usuarioid,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_arquivos_estabelecimentoid",
                table: "arquivos",
                column: "estabelecimentoid");

            migrationBuilder.CreateIndex(
                name: "ix_arquivos_usuarioid",
                table: "arquivos",
                column: "usuarioid");

            migrationBuilder.CreateIndex(
                name: "ix_aspnetroleclaims_roleid",
                table: "AspNetRoleClaims",
                column: "roleid");

            migrationBuilder.CreateIndex(
                name: "ix_aspnetuserclaims_userid",
                table: "AspNetUserClaims",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "ix_aspnetuserlogins_userid",
                table: "AspNetUserLogins",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "ix_estabelecimentos_empresaid",
                table: "estabelecimentos",
                column: "empresaid");

            migrationBuilder.CreateIndex(
                name: "ix_estabelecimentos_usuarioid",
                table: "estabelecimentos",
                column: "usuarioid");

            migrationBuilder.CreateIndex(
                name: "ix_estabelecimentosimagens_imagemlogomarcaid",
                table: "estabelecimentosimagens",
                column: "imagemlogomarcaid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "perfis",
                column: "normalizedname",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "usuarios",
                column: "normalizedemail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "usuarios",
                column: "normalizedusername",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_usuariosestabelecimentos_estabelecimentoid",
                table: "usuariosestabelecimentos",
                column: "estabelecimentoid");

            migrationBuilder.CreateIndex(
                name: "ix_usuariosimagens_usuarioimagemid",
                table: "usuariosimagens",
                column: "usuarioimagemid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_usuarioslinkstemporarios_usuarioid",
                table: "usuarioslinkstemporarios",
                column: "usuarioid");

            migrationBuilder.CreateIndex(
                name: "ix_usuariosperfis_roleid",
                table: "usuariosperfis",
                column: "roleid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "arquivos");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "estabelecimentosimagens");

            migrationBuilder.DropTable(
                name: "usuariosestabelecimentos");

            migrationBuilder.DropTable(
                name: "usuariosimagens");

            migrationBuilder.DropTable(
                name: "usuarioslinkstemporarios");

            migrationBuilder.DropTable(
                name: "usuariosperfis");

            migrationBuilder.DropTable(
                name: "estabelecimentos");

            migrationBuilder.DropTable(
                name: "perfis");

            migrationBuilder.DropTable(
                name: "empresas");

            migrationBuilder.DropTable(
                name: "usuarios");
        }
    }
}
