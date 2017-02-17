namespace ProcessesTheater.Core
{
    using System;
    using System.Threading;

    /// <summary>
    /// Abstract wrapper behavior.
    /// </summary>
    public abstract class AbstractWrapperBehavior : IBehavior
    {
        /// <summary>
        /// Inner behavior.
        /// </summary>
        protected readonly IBehavior InnerBehavior;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractWrapperBehavior" /> class.
        /// </summary>
        /// <param name="behavior"> Inner behavior. </param>
        protected AbstractWrapperBehavior(IBehavior behavior)
        {
            if (behavior == null)
            {
                throw new ArgumentNullException("behavior");
            }

            this.InnerBehavior = behavior;
        }

        /// <summary>
        /// Act method.
        /// </summary>
        /// <param name="value"> Value to execute behavior on. </param>
        /// <param name="cancellationToken"> Cancellation token. </param>
        public virtual void Act(IEffect value, CancellationToken cancellationToken)
        {
            this.InnerBehavior.Act(value, cancellationToken);
        }

        /// <summary>
        /// To string.
        /// </summary>
        /// <returns> String value. </returns>
        public override string ToString()
        {
            return string.Format("{0}>{1}", this.GetType().Name, this.InnerBehavior);
        }
    }
}
