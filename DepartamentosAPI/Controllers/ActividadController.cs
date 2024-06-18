﻿using AutoMapper;
using DepartamentosAPI.Models.DTOS;
using DepartamentosAPI.Models.Entities;
using DepartamentosAPI.Models.Validators;
using DepartamentosAPI.Repositories;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System.Security.Claims;

namespace DepartamentosAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles ="Admin,User")]
    [ApiController]
    public class ActividadController : ControllerBase
    {
        
        private readonly ActividadRepository repoActividad;
        private readonly IMapper mapper;
        public ActividadController(ActividadRepository actividadRepository, IMapper mapper)
        {
            repoActividad = actividadRepository;
            this.mapper = mapper;
        }


        [HttpGet("Publicadas")]
        public IActionResult GetActividadesPublicadas(int departamentoId)
        {
            var actividades = repoActividad.GetActividadesByDepartamentoAndSubdepartamentos(departamentoId, 1)?
                .OrderBy(x => x.Titulo)
                .Select(x => new ActividadDTO
                {
                    Id = x.Id,
                    Titulo = x.Titulo,
                    Departamento = x.IdDepartamentoNavigation.Nombre,
                    Descripcion = x.Descripcion,
                    FechaActualizacion = x.FechaActualizacion,
                    FechaCreacion = x.FechaCreacion,
                    FechaRealizacion = x.FechaRealizacion
                });
            return Ok(actividades);
        }

        [HttpGet("Borradores")]
        public IActionResult GetBorradores(int departamentoId)
        {
            var actividades = repoActividad.GetActividadesByDepartamentoAndSubdepartamentos(departamentoId, 0)?
                .OrderBy(x => x.Titulo)
                .Select(x => new ActividadDTO
                {
                    Id = x.Id,
                    Titulo = x.Titulo,
                    Departamento = x.IdDepartamentoNavigation.Nombre,
                    Descripcion = x.Descripcion,
                    FechaActualizacion = x.FechaActualizacion,
                    FechaCreacion = x.FechaCreacion,
                    FechaRealizacion = x.FechaRealizacion
                });

            return Ok(actividades);
        }

        [HttpGet("Eliminadas")]
        public IActionResult GetActividadesEliminadas(int departamentoId)
        {
            var actividades = repoActividad.GetActividadesByDepartamentoAndSubdepartamentos(departamentoId, 2)?
                .Select(x => new ActividadDTO
                {
                    Id = x.Id,
                    Titulo = x.Titulo,
                    Departamento = x.IdDepartamentoNavigation.Nombre,
                    Descripcion = x.Descripcion,
                    FechaActualizacion = x.FechaActualizacion,
                    FechaCreacion = x.FechaCreacion,
                    FechaRealizacion = x.FechaRealizacion
                });
            return Ok(actividades);
        }
        [HttpGet("{id}")]
        public IActionResult GetActividad(int id)
        {
            var actividad = repoActividad.Get(id);
            if (actividad != null)
            {
                var actividadDTO = mapper.Map<ActividadDTO>(actividad);
                return Ok(actividadDTO);
            }
            else
            {
                return NotFound();
            }

        }
        [HttpPost]
        public IActionResult Agregar(ActividadCreateDTO actividad)
        {
            if (actividad != null)
            {
                ValidationResult validate = ActividadValidator.Validate(actividad);
                if (validate.IsValid)
                {
                    var actividadAdd = new Actividades
                    {
                        Descripcion = actividad.Descripcion,
                        Titulo = actividad.Titulo,
                        IdDepartamento = int.Parse( User.FindFirstValue("Id")),
                        FechaCreacion = DateTime.Now,
                        FechaActualizacion = DateTime.Now,
                        FechaRealizacion = actividad.FechaRealizacion,
                        Estado = 0
                    };
                    repoActividad.Insert(actividadAdd);

                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", $"{actividad.Id}.png");
                    var bytes = Convert.FromBase64String(actividad.Imagen);
                    System.IO.File.WriteAllBytes(path, bytes);
                    return Ok(actividadAdd);
                }
                else
                {
                    return BadRequest(validate.Errors.Select(x => x.ErrorMessage));
                }
            }
            else
            {
                return BadRequest();
            }

        }
        [HttpPut("{id}")]
        public IActionResult Editar(ActividadCreateDTO act)
        {
            ValidationResult validate = ActividadValidator.Validate(act);
            if (validate.IsValid)
            {
                var actividadEditar = repoActividad.Get(act.Id ?? 0);
                if (actividadEditar != null)
                {
                    actividadEditar.Titulo = act.Titulo;
                    actividadEditar.Estado = act.Estado;
                    actividadEditar.Descripcion = act.Descripcion;
                    actividadEditar.IdDepartamento = act.Departamento;
                    actividadEditar.FechaActualizacion = DateTime.Now;
                    actividadEditar.FechaRealizacion = act.FechaRealizacion;
                    repoActividad.Update(actividadEditar);
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", $"{act.Id}.png");
                    var bytes = Convert.FromBase64String(act.Imagen);
                    System.IO.File.WriteAllBytes(path, bytes);
                    return Ok(actividadEditar);
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
        [HttpPut("Publicar")]
        public IActionResult Publicar(ActividadCreateDTO actividad)
        {
            ValidationResult validate = ActividadValidator.Validate(actividad);
            if (validate.IsValid)
            {
                var actividadAPublicar = repoActividad.Get(actividad.Id);
                if (actividadAPublicar != null)
                {
                    actividadAPublicar.Estado = 1;
                    repoActividad.Update(actividadAPublicar);
                    return Ok();
                }
                else
                {
                    return NotFound();
                }
            }
            else { return BadRequest(validate.Errors.Select(x => x.ErrorMessage)); }
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
             var actividadDel =repoActividad.Get(id);
            if (actividadDel != null)
            {
                actividadDel.Estado = 2;
                repoActividad.Update(actividadDel);
                return Ok(actividadDel);
            }
            else
            {
                return NotFound();
            }
        }
    }
}

