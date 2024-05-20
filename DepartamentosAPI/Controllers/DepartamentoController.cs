using AutoMapper;
using DepartamentosAPI.Helpers;
using DepartamentosAPI.Models.DTOS;
using DepartamentosAPI.Models.Entities;
using DepartamentosAPI.Models.Validators;
using DepartamentosAPI.Repositories;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;


namespace DepartamentosAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    [ApiController]
    public class DepartamentoController : ControllerBase
    {
        private readonly DepartamentoRepository _repository;
        private readonly ActividadRepository _actividadRepository;
        
        private readonly IMapper _mapper;
        public DepartamentoController(DepartamentoRepository repoDepartamento, IMapper mapper, ActividadRepository actividadRepository )
        {
                _repository = repoDepartamento;
                _actividadRepository = actividadRepository;
                _mapper = mapper;   
        }
        [HttpGet]
        public IActionResult GetAll()
        {
           
            var departamentos = _repository.GetAll().Select(x=> new DepartamentoDTO
            {
                Id = x.Id,
                Nombre = x.Nombre,
                DepartamentoSuperior = x.IdSuperiorNavigation != null ? x.IdSuperiorNavigation.Nombre : null,
            });
            return Ok(departamentos);
        }
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            
            var departamento = _repository.Get(id);
            if (departamento != null)
            {
                var departamentoDTO = _mapper.Map<DepartamentoDTO>(departamento);
                return Ok(departamentoDTO);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPost]
        public IActionResult Agregar(DepartamentoCreateDTO dto)
        {
            if (dto != null)
            {
                ValidationResult validate = DepartamentoValidator.Validate(dto,_repository.Context);
                if (validate.IsValid)
                {
                    var departamento = new Departamentos
                    {
                        Nombre = dto.Nombre,
                        Password = Encryption.StringToSHA512(dto.Contraseña),
                        Username = dto.Usuario,
                        IdSuperior = dto.IdSuperior??null
                    };
                    _repository.Insert(departamento);
                    return Ok("departamento agregado");
                }
                else
                {
                    return BadRequest(validate.Errors.Select(x=>x.ErrorMessage));
                }
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpPut("{id}")]
        public IActionResult Editar (DepartamentoCreateDTO dto)
        {
            ValidationResult validate = DepartamentoValidator.Validate(dto, _repository.Context);
            if (validate.IsValid)
            {
                var departamento = _repository.Get(dto.Id??0);
                if (departamento != null)
                {
                    departamento.Nombre = dto.Nombre;
                    departamento.Username = dto.Usuario;
                    departamento.Password = Encryption.StringToSHA512(dto.Contraseña);
                    departamento.IdSuperior = dto.IdSuperior;
                    _repository.Update(departamento);
                    return Ok("departamento actualizado");
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return BadRequest(validate.Errors.Select(x => x.ErrorMessage));
            }


        }
        [HttpDelete("{id}")]
        public IActionResult Eliminar(int id)
        {
            var departamento = _repository.Get(id);      
            if (departamento != null)
            {
                var actividadesDepartamento = _actividadRepository.GetActividadesByDepartamento(id)?.ToList();
                if (actividadesDepartamento.Count != 0)
                {
                    foreach (var actividad in actividadesDepartamento)
                    {

                        _actividadRepository.Delete(actividad);
                    }
                }
                var departamentosSub = _repository.GetAll().Where(x=>x.IdSuperior == departamento.Id);
                if (departamentosSub != null)
                {
                    foreach (var item in departamentosSub)
                    {
                        var actividadesDepartamentosub   = _actividadRepository.GetAll().Where(x=>x.IdDepartamento == item.Id);
                        if(actividadesDepartamento != null)
                        {
                            foreach (var actividad in actividadesDepartamentosub)
                            {
                                _actividadRepository.Delete(actividad);
                            }
                        }
                        _repository.Delete(item);
                    }
                }
                departamento.IdSuperior = null;
                _repository.Update(departamento);
                _repository.Delete(departamento);
                return Ok("Departamento eliminado");
            }
            else
            {
                return NotFound();
            }
        }

    }
}
