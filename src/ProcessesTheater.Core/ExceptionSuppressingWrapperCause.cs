namespace ProcessesTheater.Core
{
    using System;
    using System.Threading;
    using Logging;

    /// <summary>
    /// Exception suppressing wrapper cause.
    /// </summary>
    public class ExceptionSuppressingWrapperCause : AbstractWrapperCause
    {
        /// <summary> Logging elemet. </summary>
        private readonly ILog log;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionSuppressingWrapperCause" /> class.
        /// </summary>
        /// <param name="cause"> Inner cause. </param>
        public ExceptionSuppressingWrapperCause(ICause cause)
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
            try
            {
                return this.InnerCause.Check(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (ThreadAbortException)
            {
                throw;
            }
            catch (Exception ex)
            {
                this.log.ErrorFormat(
                    "Error occurred during execution of '{0}' cause. Exception details: {1}",
                    this.ToString(),
                    ex);
            }

            return null;
        }
    }
}
