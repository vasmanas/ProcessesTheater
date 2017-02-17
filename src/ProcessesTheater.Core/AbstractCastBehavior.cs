namespace ProcessesTheater.Core
{
    using System;
    using System.Threading;

    /// <summary>
    /// Abstract cast behavior.
    /// </summary>
    /// <typeparam name="TEffect"> Effect type. </typeparam>
    public abstract class AbstractCastBehavior<TEffect> : IBehavior where TEffect : class, IEffect
    {
        /// <summary>
        /// Tells what to do if value is null.
        /// </summary>
        private readonly ExecutionDecision valueNullDecidion;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractCastBehavior{TEffect}" /> class.
        /// </summary>
        /// <param name="valueNullDecidion"> Tells what to do if value is null. </param>
        protected AbstractCastBehavior(ExecutionDecision valueNullDecidion = ExecutionDecision.ThrowException)
        {
            this.valueNullDecidion = valueNullDecidion;
        }

        /// <summary>
        /// Act method.
        /// </summary>
        /// <param name="value"> Value to execute behavior on. </param>
        /// <param name="cancellationToken"> Cancellation token. </param>
        public void Act(IEffect value, CancellationToken cancellationToken)
        {
            if (value == null)
            {
                switch (this.valueNullDecidion)
                {
                    case ExecutionDecision.ThrowException:
                        throw new ArgumentNullException("value");

                    case ExecutionDecision.QuitExecution:
                        return;
                }
            }

            var spec = value as TEffect;

            if (spec == null)
            {
                switch (this.valueNullDecidion)
                {
                    case ExecutionDecision.ThrowException:
                        throw new InvalidCastException(string.Format("Can't cast effect to type {0}", typeof(TEffect)));

                    case ExecutionDecision.QuitExecution:
                        return;
                }
            }

            this.Act(spec, cancellationToken);
        }

        /// <summary>
        /// Act method.
        /// </summary>
        /// <param name="value"> Value to execute behavior on. </param>
        /// <param name="cancellationToken"> Cancellation token. </param>
        public abstract void Act(TEffect value, CancellationToken cancellationToken);
    }
}
