using DepartamentosAPI.Models.Entities;
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
        private List<int> GetAllSubdepartamentos(int departamentoId)
        {
            List<int> subdepartamentosIds = new List<int>();
            var subdepartamentos = _context.Departamentos
                                           .Where(d => d.IdSuperior == departamentoId)
                                           .Select(d => d.Id)
                                           .ToList();

            foreach (var subId in subdepartamentos)
            {
                subdepartamentosIds.Add(subId);
                subdepartamentosIds.AddRange(GetAllSubdepartamentos(subId));
            }

            return subdepartamentosIds;
        }

        public IEnumerable<Actividades>? GetActividadesByDepartamentoAndSubdepartamentos(int departamentoId, int estado)
        {
            var departamentosIds = new List<int> { departamentoId };
            departamentosIds.AddRange(GetAllSubdepartamentos(departamentoId));

            return _context.Actividades
                           .Include(x => x.IdDepartamentoNavigation)
                           .Where(x => departamentosIds.Contains(x.IdDepartamento) && x.Estado == estado)
                           .ToList();
        }
    }
}
