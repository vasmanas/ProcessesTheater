namespace ProcessesTheater.Core
{
    using System.Threading;

    /// <summary>
    /// Behavior interface.
    /// </summary>
    public interface IBehavior
    {
        /// <summary>
        /// Act method.
        /// </summary>
        /// <param name="value"> Value to execute behavior on. </param>
        /// <param name="cancellationToken"> Cancellation token. </param>
        void Act(IEffect value, CancellationToken cancellationToken);
    }
}