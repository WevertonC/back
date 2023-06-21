using AutoMapper;
using Persistencia.Models.EFolha;

namespace WSolucaoWeb.Dtos.eFolha
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Usuario, PrimeiroUsuarioDto>()
                .ForMember(dto => dto.Nome, obj => obj.MapFrom(src => src.UserName))
                .ForMember(dto => dto.Senha, obj => obj.MapFrom(src => src.PasswordHash))
                .ReverseMap();

            CreateMap<Usuario, NovoUsuarioDto>()
                .ForMember(dto => dto.Nome, obj => obj.MapFrom(src => src.UserName))
                .ForMember(dto => dto.Senha, obj => obj.MapFrom(src => src.PasswordHash))
                .ReverseMap();
        }
    }
}
