namespace ProcessesTheater.Core
{
    using System;
    using System.Threading;
    
    using Logging;

    /// <summary>
    /// Retry wrapper behavior.
    /// </summary>
    public class RetryWrapperBehavior : AbstractWrapperBehavior
    {
        /// <summary> Logging elememt. </summary>
        private readonly ILog log;

        /// <summary>
        /// Max retry attempts.
        /// </summary>
        private readonly int maxRetryAttempts;

        /// <summary>
        /// Retry period provider.
        /// </summary>
        private readonly Func<int, TimeSpan> retryPeriodProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="RetryWrapperBehavior" /> class.
        /// </summary>
        /// <param name="behavior"> Behavior to wrap. </param>
        /// <param name="maxRetryAttempts"> Max retry attempts. </param>
        /// <param name="retryPeriodProvider"> Retry period provider. </param>
        public RetryWrapperBehavior(
            IBehavior behavior,
            int maxRetryAttempts = 1,
            Func<int, TimeSpan> retryPeriodProvider = null,
            ILog log = null)
            : base(behavior)
        {
            if (maxRetryAttempts < 1)
            {
                throw new ArgumentOutOfRangeException(
                    "maxRetryAttempts",
                    "MaxRetryAttempts property value must be greater or equal to 1.");
            }

            this.log = log ?? new EmptyLog();

            this.maxRetryAttempts = maxRetryAttempts;
            this.retryPeriodProvider = retryPeriodProvider;
        }

        /// <summary>
        /// Act method.
        /// </summary>
        /// <param name="value"> Value to execute behavior on. </param>
        /// <param name="cancellationToken"> Cancellation token. </param>
        public override void Act(IEffect value, CancellationToken cancellationToken)
        {
            for (var i = 0; i < this.maxRetryAttempts; i++)
            {
                try
                {
                    this.InnerBehavior.Act(value, cancellationToken);

                    return;
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
                    if ((i + 1) >= this.maxRetryAttempts)
                    {
                        throw;
                    }

                    TimeSpan? nextTry = null;

                    if (this.retryPeriodProvider != null)
                    {
                        nextTry = this.retryPeriodProvider(i);

                        if (nextTry.Value.Ticks == 0)
                        {
                            nextTry = null;
                        }
                    }

                    if (nextTry.HasValue)
                    {
                        this.log.ErrorFormat(
                            "Error occurred during execution of '{0}' behavior. Execution will be retried (attempt {1} of {2}) in {3} seconds. Exception details: {4}",
                            this.ToString(),
                            i + 1,
                            this.maxRetryAttempts,
                            nextTry.Value,
                            ex);

                        // Break the loop when the wait handle was signaled.
                        cancellationToken.WaitHandle.WaitOne(nextTry.Value);
                    }
                    else
                    {
                        this.log.ErrorFormat(
                            "Error occurred during execution of '{0}' behavior. Execution will be retried (attempt {1} of {2}) imediatly. Exception details: {3}",
                            this.ToString(),
                            i + 1,
                            this.maxRetryAttempts,
                            ex);
                    }
                }

                cancellationToken.ThrowIfCancellationRequested();
            }
        }
    }
}
