// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRepository.cs" company="contentedcoder.com">
//   contentedcoder.com
// </copyright>
// <summary>
//   The repository interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Dupper.Firebird
{
    /// <summary>
    /// The repository interface.
    /// </summary>
    /// <typeparam name="T">
    /// The domain entity
    /// </typeparam>
    public interface IRepository<T> : IReadOnlyRepository<T> where T : EntityBase
    {
        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        T Add(T item);

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        void Remove(T item);

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        object Update(T item);

        /// <summary>
        /// Deletes the record by the parameters
        /// </summary>
        /// <param name="param"></param>
        void Delete(object param);
    }
}