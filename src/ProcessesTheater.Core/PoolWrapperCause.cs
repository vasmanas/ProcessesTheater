namespace ProcessesTheater.Core
{
    using System.Threading;

    /// <summary>
    /// Pool wrapper cause.
    /// </summary>
    public class PoolWrapperCause : PauseWrapperCause
    {
        /// <summary>
        /// Previous data found.
        /// </summary>
        private bool previousDataFound = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="PoolWrapperCause" /> class.
        /// </summary>
        /// <param name="cause"> Inner cause. </param>
        /// <param name="poolPeriodSec"> Period to wait if effect is not returned. In seconds. </param>
        /// <param name="firstTimeExecute"> First time execute don't wait. </param>
        public PoolWrapperCause(ICause cause, double poolPeriodSec, bool firstTimeExecute = true)
            : base(cause, poolPeriodSec, firstTimeExecute)
        {
            // if first time to execute then set as previous data found.
            this.previousDataFound = firstTimeExecute;
        }
        
        /// <summary>
        /// Wait for cause to happen.
        /// </summary>
        /// <param name="cancellationToken"> Cancellation token. </param>
        /// <returns> The <see cref="IEffect"/>. </returns>
        public override IEffect Check(CancellationToken cancellationToken)
        {
            this.Pause(!this.previousDataFound, cancellationToken);

            var effect = this.InnerCause.Check(cancellationToken);

            this.previousDataFound = effect != null;

            return effect;
        }
    }
}
