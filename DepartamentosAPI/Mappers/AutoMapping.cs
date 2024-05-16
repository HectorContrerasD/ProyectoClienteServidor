using DepartamentosAPI.Models.Entities;
using AutoMapper;
using DepartamentosAPI.Models.DTOS;

namespace DepartamentosAPI.Mappers
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<Actividades, ActividadDTO>()
            .ForMember(dest => dest.Titulo, opt => opt.MapFrom(src => src.Titulo))
            .ForMember(dest => dest.Departamento, opt => opt.MapFrom(src => src.IdDepartamentoNavigation.Nombre))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.Descripcion))
            .ForMember(dest => dest.FechaActualizacion, opt => opt.MapFrom(src => src.FechaActualizacion))
            .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => src.FechaCreacion))
            .ForMember(dest => dest.FechaRealizacion, opt => opt.MapFrom(src => src.FechaRealizacion));


            ;


            CreateMap<ActividadDTO, Actividades>();
            CreateMap<Departamentos, DepartamentoDTO>();
            CreateMap<DepartamentoDTO, Departamentos>();
        }
    }
}
