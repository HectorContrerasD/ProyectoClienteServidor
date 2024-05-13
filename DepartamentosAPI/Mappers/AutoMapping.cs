using DepartamentosAPI.Models.Entities;
using AutoMapper;
using DepartamentosAPI.Models.DTOS;

namespace DepartamentosAPI.Mappers
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<Actividades, ActividadDTO>();
            CreateMap<ActividadDTO, Actividades>();
            CreateMap<Departamentos, DepartamentoDTO>();
            CreateMap<DepartamentoDTO, Departamentos>();
        }
    }
}
