﻿using DepartamentosAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Xml;

namespace DepartamentosAPI.Repositories
{
    public class ActividadRepository : Repository<Actividades>
    {
        private readonly ItesrcneActividadesContext _context;
        public ActividadRepository( ItesrcneActividadesContext context): base (context) 
        {
            _context = context;
        }
        // 0 borrador, 1 publicado, 2 eliminado
        public IEnumerable<Actividades>? GetActividades()
        {
            return _context.Actividades.Include(x=>x.IdDepartamentoNavigation).Where(x => x.Estado == 1 );
        }
        public IEnumerable<Actividades>? GetActividadesEliminadas()
        {
            return _context.Actividades.Include(x => x.IdDepartamentoNavigation).Where(x => x.Estado == 2);
        }
        public IEnumerable<Actividades>? GetBorradores()
        {
            return _context.Actividades.Include(x => x.IdDepartamentoNavigation).Where(x => x.Estado == 0);
        }
        public IEnumerable<Actividades> ? GetActividadesByDepartamento(int id)
        {
            return _context.Actividades.Include(x=>x.IdDepartamentoNavigation).Where(x=>x.IdDepartamento == id);
        }
        public override Actividades? Get(object id)
        {
            if (id == null || !int.TryParse(id.ToString(), out int actividadId))
            {
                return null;
            }
            return _context.Actividades.Include(x=>x.IdDepartamentoNavigation).FirstOrDefault(x=>x.Id == actividadId) ;
        }
        public IEnumerable<Actividades>? GetActividadesByDepartamentoAndSubdepartamentos(int departamentoId, int estado)
        {
            var departamentosIds = _context.Departamentos
                .Where(d => d.Id == departamentoId || d.IdSuperior == departamentoId)
                .Select(d => d.Id)
                .ToList();

            return _context.Actividades
                .Include(x => x.IdDepartamentoNavigation)
                .Where(x => departamentosIds.Contains(x.IdDepartamento) && x.Estado == estado)
                .ToList();
        }
    }
}
