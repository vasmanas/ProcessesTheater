namespace ProcessesTheater.Core
{
    using System.Threading;

    /// <summary>
    /// Cause interface.
    /// </summary>
    public interface ICause
    {
        /// <summary>
        /// Check cause for effect.
        /// </summary>
        /// <param name="cancellationToken"> Cancellation token. </param>
        /// <returns> The <see cref="IEffect"/>. </returns>
        IEffect Check(CancellationToken cancellationToken);
    }
}