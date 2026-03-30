using MedSync_API.Data;
using Microsoft.EntityFrameworkCore;

namespace MedSync_API.Repositories
{
    /// <summary>
    /// Implementación genérica de repositorio que encapsula el acceso a ApplicationDbContext.
    /// Todos los repositorios concretos heredan de esta clase para reutilizar las operaciones CRUD.
    /// </summary>
    /// <typeparam name="T">Tipo de entidad del dominio mapeada por EF Core.</typeparam>
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        /// <summary>Contexto de base de datos compartido inyectado vía constructor.</summary>
        protected readonly ApplicationDbContext _context;

        /// <summary>DbSet tipado para la entidad T, usado en las operaciones CRUD.</summary>
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        /// <inheritdoc/>
        public async Task<T?> ObtenerPorIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<T>> ObtenerTodosAsync()
        {
            return await _dbSet.ToListAsync();
        }

        /// <inheritdoc/>
        public async Task AgregarAsync(T entidad)
        {
            await _dbSet.AddAsync(entidad);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task ActualizarAsync(T entidad)
        {
            _dbSet.Update(entidad);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task EliminarAsync(T entidad)
        {
            _dbSet.Remove(entidad);
            await _context.SaveChangesAsync();
        }
    }
}
