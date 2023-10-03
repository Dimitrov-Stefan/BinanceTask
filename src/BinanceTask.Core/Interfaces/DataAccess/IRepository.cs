using BinanceTask.Core.Entities.Abstract;
using System.Linq.Expressions;

namespace BinanceTask.Core.Interfaces.DataAccess;

/// <summary>
/// Represents a generic repository interface for performing CRUD (Create, Read, Update, Delete) operations on entities of type T.
/// </summary>
/// <typeparam name="T">The type of entity that the repository operates on. It should inherit from <see cref="EntityBase"/>.</typeparam>
public interface IRepository<T> where T : EntityBase
{
    /// <summary>
    /// Asynchronously retrieves an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to retrieve.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the retrieved entity,
    /// or <c>null</c> if no entity with the specified identifier is found.
    /// </returns>
    Task<T?> GetByIdAsync(int id);

    /// <summary>
    /// Asynchronously retrieves all entities of type T.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains an enumerable collection of entities.
    /// </returns>
    Task<IEnumerable<T>> ListAsync();

    /// <summary>
    /// Asynchronously retrieves a filtered list of entities based on a specified predicate.
    /// </summary>
    /// <param name="predicate">A predicate expression to filter the entities.</param>
    /// <param name="take">The maximum number of entities to retrieve (optional).</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains an enumerable collection of filtered entities.
    /// </returns>
    Task<IEnumerable<T>> ListAsync(Expression<Func<T, bool>> predicate, int take = int.MaxValue);

    /// <summary>
    /// Asynchronously adds an entity to the repository.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddAsync(T entity);

    /// <summary>
    /// Asynchronously adds a collection of entities to the repository.
    /// </summary>
    /// <param name="entities">The collection of entities to add.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddRangeAsync(IEnumerable<T> entities);

    /// <summary>
    /// Asynchronously updates an entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateAsync(T entity);

    /// <summary>
    /// Asynchronously deletes an entity from the repository.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteAsync(T entity);
}
