// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IReadOnlyRepository.cs" company="contentedcoder.com">
//   contentedcoder.com
// </copyright>
// <summary>
//   The ReadOnlyRepository interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Dupper.Firebird
{
    /// <summary>
    /// The ReadOnlyRepository interface.
    /// </summary>
    /// <typeparam name="T">
    /// The aggregate root entity.
    /// </typeparam>
    public interface IReadOnlyRepository<T> where T : EntityBase
    {
        /// <summary>
        /// The find by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        T FindById(int id);

        /// <summary>
        /// The get all.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable"/> of T.
        /// </returns>
        IEnumerable<T> GetAll();
    }
}