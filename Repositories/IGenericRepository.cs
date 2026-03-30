namespace MedSync_API.Repositories
{
    /// <summary>
    /// Interfaz genérica de repositorio con operaciones CRUD estándar.
    /// Cada entidad del dominio tiene su propio repositorio que implementa esta interfaz,
    /// siendo la única capa que accede directamente al ApplicationDbContext.
    /// </summary>
    /// <typeparam name="T">Tipo de entidad del dominio.</typeparam>
    public interface IGenericRepository<T> where T : class
    {
        /// <summary>Obtiene una entidad por su identificador único.</summary>
        Task<T?> ObtenerPorIdAsync(int id);

        /// <summary>Obtiene todas las entidades del tipo especificado.</summary>
        Task<IEnumerable<T>> ObtenerTodosAsync();

        /// <summary>Agrega una nueva entidad a la base de datos.</summary>
        Task AgregarAsync(T entidad);

        /// <summary>Marca una entidad existente como modificada para persistir los cambios.</summary>
        Task ActualizarAsync(T entidad);

        /// <summary>Elimina una entidad de la base de datos.</summary>
        Task EliminarAsync(T entidad);
    }
}
