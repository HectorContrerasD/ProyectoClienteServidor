using DepartamentosAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DepartamentosAPI.Repositories
{
    public class DepartamentoRepository : Repository<Departamentos>
    {
        private readonly ItesrcneActividadesContext _context;
        public DepartamentoRepository(ItesrcneActividadesContext context) : base(context)
        {
            _context = context;
        }
        public override  IEnumerable<Departamentos> GetAll()
        {
            return _context.Departamentos.Include(x=>x.InverseIdSuperiorNavigation).OrderBy(x=>x.Nombre);
        }
        public override Departamentos? Get(object id)
        {
            if (id == null || !int.TryParse(id.ToString(), out int departamentoId))
            {
                return null;
            }
            return _context.Departamentos.Include(x => x.IdSuperiorNavigation).FirstOrDefault(x => x.Id == departamentoId);
        }
    }
}
