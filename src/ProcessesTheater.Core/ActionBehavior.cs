namespace ProcessesTheater.Core
{
    using System;
    using System.Threading;

    /// <summary>
    /// Action behavior.
    /// </summary>
    /// <typeparam name="TEffect"> Effect type. </typeparam>
    public class ActionBehavior<TEffect> : AbstractCastBehavior<TEffect> where TEffect : class, IEffect
    {
        /// <summary>
        /// Action.
        /// </summary>
        private readonly Action<TEffect> action;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionBehavior{TEffect}" /> class.
        /// </summary>
        /// <param name="action"> Action. </param>
        public ActionBehavior(Action<TEffect> action)
        {
            this.action = action;
        }

        /// <summary>
        /// Act method.
        /// </summary>
        /// <param name="value"> Value to execute behavior on. </param>
        /// <param name="cancellationToken"> Cancellation token. </param>
        public override void Act(TEffect value, CancellationToken cancellationToken)
        {
            this.action(value);
        }
    }
}
