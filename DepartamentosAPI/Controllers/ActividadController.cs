﻿using AutoMapper;
using DepartamentosAPI.Hubs;
using DepartamentosAPI.Models.DTOS;
using DepartamentosAPI.Models.Entities;
using DepartamentosAPI.Models.Validators;
using DepartamentosAPI.Repositories;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.FileProviders;
using System.Security.Claims;

namespace DepartamentosAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,User")]
    [ApiController]
    public class ActividadController : ControllerBase
    {
        
        private readonly ActividadRepository repoActividad;
        private readonly IMapper mapper;
        private readonly IHubContext<NotificacionesHub> hubContext;
        public ActividadController(ActividadRepository actividadRepository, IMapper mapper, IHubContext<NotificacionesHub> hub)
        {
            repoActividad = actividadRepository;
            this.mapper = mapper;
            this.hubContext = hub;
        }

        public async Task Notificar(string msg)
        {
            await hubContext.Clients.All.SendAsync("RecibirMensaje", msg);
        }
        [HttpGet("Publicadas/{departamentoId}")]
        public  IActionResult GetActividadesPublicadas(int departamentoId)
        {
            
            var actividadessub = repoActividad.GetActividadesByDepartamentoAndSubdepartamentos(departamentoId, 1)?
                .Select(x => new ActividadDTO
                {
                    Id = x.Id,
                    Titulo = x.Titulo,
                    Departamento = x.IdDepartamentoNavigation.Nombre,
                    Descripcion = x.Descripcion,
                    FechaActualizacion = x.FechaActualizacion,
                    FechaCreacion = x.FechaCreacion,
                    FechaRealizacion = x.FechaRealizacion,
                   
                }).OrderBy(x=>x.Titulo);
          

            return Ok(actividadessub);
        }

        [HttpGet("Borradores/{departamentoId}")]
        public IActionResult GetBorradores(int departamentoId)
        {
            var actividades = repoActividad.GetActividadesByDepartamento(departamentoId)?.Where(x=>x.Estado ==0)
                .OrderBy(x => x.Titulo)
                .Select(x => new ActividadDTO
                {
                    Id = x.Id,
                    Titulo = x.Titulo,
                    Departamento = x.IdDepartamentoNavigation.Nombre,
                    Descripcion = x.Descripcion,
                    FechaActualizacion = x.FechaActualizacion,
                    FechaCreacion = x.FechaCreacion,
                    FechaRealizacion = x.FechaRealizacion,
                    Imagen = ConvertBase64($"wwwroot/images/{x.Id}.jpg")
                });

            return Ok(actividades);
        }

        [HttpGet("Eliminadas/{departamentoId}")]
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
                    FechaRealizacion = x.FechaRealizacion,
                    Imagen = ConvertBase64($"wwwroot/images/{x.Id}.jpg")
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
        public async Task< IActionResult> Agregar(ActividadCreateDTO actividad)
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
                    if (string.IsNullOrEmpty(actividad.Imagen))
                    {
                         System.IO.File.Copy("wwwroot/images/Default.jpg", $"wwwroot/images/{actividadAdd.Id}.jpg");
                    }
                    else
                    {

                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", $"{actividadAdd.Id}.jpg");
                        var bytes = Convert.FromBase64String(actividad.Imagen);
                        System.IO.File.WriteAllBytes(path, bytes);
                    }
                    await Notificar("Actividad agregada");
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
        [HttpPut]
        public async Task< IActionResult> Editar(ActividadCreateDTO act)
        {
            ValidationResult validate = ActividadValidator.Validate(act);
            if (validate.IsValid)
            {
                var actividadEditar = repoActividad.Get(act.Id ?? 0);
                if (actividadEditar != null)
                {
                    if (actividadEditar.IdDepartamento == int.Parse(User.FindFirstValue("id")??"0"))
                    {

                        actividadEditar.Titulo = act.Titulo;
                    
                        actividadEditar.Descripcion = act.Descripcion;
                   
                        actividadEditar.FechaActualizacion = DateTime.Now;
                        actividadEditar.FechaRealizacion = act.FechaRealizacion;
                        repoActividad.Update(actividadEditar);
                        if (string.IsNullOrEmpty(act.Imagen))
                        {

                            System.IO.File.Copy("wwwroot/images/0.png", $"wwwroot/images/{act.Id}.jpg");
                        }
                        else
                        {

                            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", $"{act.Id}.jpg");
                            var bytes = Convert.FromBase64String(act.Imagen);
                            System.IO.File.WriteAllBytes(path, bytes);
                        }
                        await Notificar("Actividad Editada");
                        return Ok(act);
                    }
                    else
                    {
                        return BadRequest("Solo puedes editar tus propias actividades");
                    }
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
        [HttpPut("Publicar/{id}")]
        public async Task< IActionResult> Publicar(int id)
        {
     
            var actividadAPublicar = repoActividad.Get(id);
            if (actividadAPublicar != null)
            {
                if (actividadAPublicar.IdDepartamento == int.Parse(User.FindFirstValue("id")??"0"))
                {
                    actividadAPublicar.Estado = 1;
                    repoActividad.Update(actividadAPublicar);
                    await Notificar("Actividad Publicada");
                    return Ok("Publicada");

                }
                else
                {
                    return BadRequest("Solo pueden publicarse las actividades de tu propio departamento");
                }
            }
            else
            {
                    return NotFound();
            }
      
        }
        [HttpDelete("{id}")]
        public async Task< IActionResult> Delete(int id)
        {
             var actividadDel =repoActividad.Get(id);
            if (actividadDel != null)
            {
                if (actividadDel.IdDepartamento  == int.Parse(User.FindFirstValue("id")??"0"))
                {

                    actividadDel.Estado = 2;
                    repoActividad.Update(actividadDel);
                    await Notificar("Publicacion eliminada");
                    return Ok(actividadDel);
                }
                else
                {
                   return BadRequest("Esta actividad no le pertenece a este departamento");
                }
            }
            else
            {
                return NotFound();
            }
        }
        public string ConvertBase64(string imagePath)
        {
            if (System.IO.File.Exists(imagePath))
            {
                byte[] imageArray = System.IO.File.ReadAllBytes(imagePath);
                string base64ImageRepresentation = Convert.ToBase64String(imageArray);
                return base64ImageRepresentation;
            }
            return "";
        }
    }
}

