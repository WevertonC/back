using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using AutoMapper;
using System.Text.Json.Serialization;
using Persistencia.DataBases;
using Persistencia.Models.EFolha;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
var url = $"http://0.0.0.0:{port}";

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddDbContext<WebApiContext>((serviceProvider, options) =>
{
    var baseDeDados = serviceProvider.GetService<IHttpContextAccessor>().HttpContext?.Request?.Headers["numeroDeSerie"].ToString();

    string stringDeConexao;

    if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
    {
        stringDeConexao = Environment.GetEnvironmentVariable("ConnectionDesenvolvimento");
    }
    else
    {
        stringDeConexao = Environment.GetEnvironmentVariable("ConnectionProducao");
    }

    if (!(baseDeDados is null))
        stringDeConexao = string.Format(stringDeConexao, $"escritorio{baseDeDados}");
    else
        stringDeConexao = string.Format(stringDeConexao, "escritorio000000");

    options.UseNpgsql(stringDeConexao).UseLowerCaseNamingConvention();

    options.UseNpgsql(stringDeConexao, x => x.MigrationsHistoryTable("__EFMigrationsHistory", "public")).ReplaceService<IHistoryRepository, ConfigurarMigrationHistoryRepository>();
});

IdentityBuilder bdr = builder.Services.AddIdentityCore<Usuario>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 4;
    options.User.AllowedUserNameCharacters = null;
    options.User.RequireUniqueEmail = true;
});

bdr = new IdentityBuilder(bdr.UserType, typeof(Perfil), bdr.Services);
bdr.AddEntityFrameworkStores<WebApiContext>();
bdr.AddRoleValidator<RoleValidator<Perfil>>();
bdr.AddRoleManager<RoleManager<Perfil>>();
bdr.AddSignInManager<SignInManager<Usuario>>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKeyResolver = (token, secutiryToken, kid, validationParameters) =>
        {
            SecurityKey issuerSigningKey = IssuerSigningKeyResolverCustom(builder.Services);
            return new List<SecurityKey>() { issuerSigningKey };
        }
    };
});

SecurityKey IssuerSigningKeyResolverCustom(IServiceCollection svc)
{
    var baseDeDados = svc.BuildServiceProvider().GetService<IHttpContextAccessor>().HttpContext?.Request?.Headers["numeroDeSerie"].ToString();
    SecurityKey issuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes($"{baseDeDados}{Environment.GetEnvironmentVariable("ChaveDoToken")}"));
    return issuerSigningKey;
}

builder.Services.AddControllersWithViews(options => { options.SuppressAsyncSuffixInActionNames = false; });

builder.Services.AddMvc()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddControllers();
builder.Services.AddAutoMapper();
builder.Services.AddCors();
builder.Services.AddHttpClient();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    c.OperationFilter<AddRequiredHeaderParameter>();

    c.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    {
        Description = "`Informar Apenas o Token!!!` - N�o precisa do `Bearer_` prefixo",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Scheme = "bearer"
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseAuthentication();
app.UseAuthorization();
app.UseDefaultFiles();
// Ativando middlewares para uso do Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = string.Empty;
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

if (app.Environment.IsDevelopment())
{
    app.Run();
}
else
{
    app.Run(url);
}
