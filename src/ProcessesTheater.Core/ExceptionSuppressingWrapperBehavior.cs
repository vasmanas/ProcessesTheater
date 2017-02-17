namespace ProcessesTheater.Core
{
    using System;
    using System.Threading;
    using Logging;

    /// <summary>
    /// Exception suppressing wrapper behavior.
    /// </summary>
    public class ExceptionSuppressingWrapperBehavior : AbstractWrapperBehavior
    {
        /// <summary> Logging element. </summary>
        private readonly ILog log;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionSuppressingWrapperBehavior" /> class.
        /// </summary>
        /// <param name="behavior"> Behavior to wrap. </param>
        public ExceptionSuppressingWrapperBehavior(IBehavior behavior, ILog log = null)
            : base(behavior)
        {
            this.log = log ?? new EmptyLog();
        }

        /// <summary>
        /// Act method.
        /// </summary>
        /// <param name="value"> Value to execute behavior on. </param>
        /// <param name="cancellationToken"> Cancellation token. </param>
        public override void Act(IEffect value, CancellationToken cancellationToken)
        {
            try
            {
                this.InnerBehavior.Act(value, cancellationToken);
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
                    "Error occurred during execution of '{0}' behavior. Exception details: {1}",
                    this.ToString(),
                    ex);
            }
        }
    }
}
