using AutoMapper;
using DepartamentosAPI.Helpers;
using DepartamentosAPI.Hubs;
using DepartamentosAPI.Models.DTOS;
using DepartamentosAPI.Models.Entities;
using DepartamentosAPI.Models.Validators;
using DepartamentosAPI.Repositories;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Metadata;


namespace DepartamentosAPI.Controllers
{
    [Route("api/[controller]")]
    //[Authorize(Roles = "Admin")]
    [ApiController]
    public class DepartamentoController : ControllerBase
    {
        private readonly DepartamentoRepository _repository;
        private readonly ActividadRepository _actividadRepository;
        private readonly IHubContext<NotificacionesHub> hubContext;
        private readonly IMapper _mapper;
        public DepartamentoController(DepartamentoRepository repoDepartamento, IMapper mapper, ActividadRepository actividadRepository, IHubContext<NotificacionesHub> hub )
        {
                _repository = repoDepartamento;
                _actividadRepository = actividadRepository;
                this.hubContext = hub;
                _mapper = mapper;   
        }
        public async Task Notificar(string msg)
        {
            await hubContext.Clients.All.SendAsync("RecibirMensaje", msg);
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
        public async Task< IActionResult> Agregar(DepartamentoCreateDTO dto)
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
                    await Notificar("Departamento Agregado");
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
        public async Task< IActionResult> Editar (DepartamentoCreateDTO dto)
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
                    await Notificar("Departamento Actualizado");
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
        public async Task< IActionResult> Eliminar(int id)
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
                var departamentosSub = _repository.GetAll().Where(x=>x.IdSuperior == departamento.Id).ToList();
                if (departamentosSub != null)
                {
                    foreach (var a in departamentosSub)
                    {
                        a.IdSuperior = departamento.IdSuperior;
                        _repository.Update(a);

                    }
                }
                departamento.IdSuperior = null;
                _repository.Update(departamento);
                _repository.Delete(departamento);
                await Notificar($"Departamento eliminado: {departamento.Nombre}");
                await hubContext.Clients.User(departamento.Nombre).SendAsync("Logout");
                return Ok("Departamento eliminado");
            }
            else
            {
                return NotFound();
            }
        }

    }
}
