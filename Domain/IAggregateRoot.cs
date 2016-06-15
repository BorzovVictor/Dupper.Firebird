// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAggregateRoot.cs" company="contentedcoder.com">
//   contentedcoder.com
// </copyright>
// <summary>
//   The aggregate root for use in the repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Dupper.Firebird
{
    /// <summary>
    /// The aggregate root for use in the repository.
    /// </summary>
    /// <remarks>
    /// This indicates what objects can be directly loaded from the repository.
    /// </remarks>
    public interface IAggregateRoot
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        //int Id { get; set; }
    }
}