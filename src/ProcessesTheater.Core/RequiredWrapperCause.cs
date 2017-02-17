namespace ProcessesTheater.Core
{
    using System.Threading;

    /// <summary>
    /// Required wrapper cause.
    /// </summary>
    public class RequiredWrapperCause : AbstractWrapperCause
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredWrapperCause" /> class.
        /// </summary>
        /// <param name="cause"> Inner cause. </param>
        public RequiredWrapperCause(ICause cause)
            : base(cause)
        {
        }

        /// <summary>
        /// Wait for cause to happen.
        /// </summary>
        /// <param name="cancellationToken"> Cancellation token. </param>
        /// <returns> The <see cref="IEffect"/>. </returns>
        public override IEffect Check(CancellationToken cancellationToken)
        {
            IEffect effect;

            while ((effect = this.InnerCause.Check(cancellationToken)) == null)
            {
            }

            return effect;
        }
    }
}
