namespace ProcessesTheater.Core
{
    using System;
    using System.Threading;

    /// <summary>
    /// Abstract wrapper cause.
    /// </summary>
    public abstract class AbstractWrapperCause : ICause
    {
        /// <summary>
        /// Inner cause.
        /// </summary>
        protected readonly ICause InnerCause;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractWrapperCause" /> class.
        /// </summary>
        /// <param name="cause"> Inner cause. </param>
        protected AbstractWrapperCause(ICause cause)
        {
            if (cause == null)
            {
                throw new ArgumentNullException("cause");
            }

            this.InnerCause = cause;
        }

        /// <summary>
        /// Check causes effect.
        /// </summary>
        /// <param name="cancellationToken"> Cancellation token. </param>
        /// <returns> The <see cref="IEffect"/>. </returns>
        public virtual IEffect Check(CancellationToken cancellationToken)
        {
            return this.InnerCause.Check(cancellationToken);
        }

        /// <summary>
        /// To string.
        /// </summary>
        /// <returns> String value. </returns>
        public override string ToString()
        {
            return string.Format("{0}>{1}", this.GetType().Name, this.InnerCause);
        }
    }
}
