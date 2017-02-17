namespace ProcessesTheater.Core
{
    using System;
    using System.Threading;

    /// <summary>
    /// Now cause.
    /// </summary>
    public class NowCause : ICause
    {
        /// <summary>
        /// Check causes effect.
        /// </summary>
        /// <param name="cancellationToken"> Cancellation token. </param>
        /// <returns> The <see cref="IEffect"/>. </returns>
        public IEffect Check(CancellationToken cancellationToken)
        {
            return new SingleValueEffect<DateTime>(DateTime.UtcNow);
        }
    }
}
