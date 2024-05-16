using AutoMapper;
using DepartamentosAPI.Helpers;
using DepartamentosAPI.Models.DTOS;
using DepartamentosAPI.Models.Entities;
using DepartamentosAPI.Models.Validators;
using DepartamentosAPI.Repositories;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Xml;

namespace DepartamentosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartamentoController : ControllerBase
    {
        private readonly DepartamentoRepository _repository;
        private readonly IMapper _mapper;
        public DepartamentoController(DepartamentoRepository repoDepartamento, IMapper mapper )
        {
                _repository = repoDepartamento;
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
                        IdSuperior = dto.IdSuperior
                    };
                    _repository.Insert(departamento);
                    return Ok(departamento);
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
                    return Ok(departamento);
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
