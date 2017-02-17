namespace ProcessesTheater.Core
{
    using System;
    using System.Threading;
    
    using Logging;

    /// <summary>
    /// Retry wrapper cause.
    /// </summary>
    public class RetryWrapperCause : AbstractWrapperCause
    {
        /// <summary> Logging elemet. </summary>
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
        /// Initializes a new instance of the <see cref="RetryWrapperCause" /> class.
        /// </summary>
        /// <param name="cause"> Cause to wrap. </param>
        /// <param name="maxRetryAttempts"> Max retry attempts. </param>
        /// <param name="retryPeriod"> Retry period. </param>
        public RetryWrapperCause(ICause cause, int maxRetryAttempts, TimeSpan retryPeriod, ILog log = null)
            : this(cause, maxRetryAttempts, i => retryPeriod)
        {
            this.log = log ?? new EmptyLog();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RetryWrapperCause" /> class.
        /// </summary>
        /// <param name="cause"> Cause to wrap. </param>
        /// <param name="maxRetryAttempts"> Max retry attempts. </param>
        /// <param name="retryPeriodProvider"> Retry period provider. </param>
        public RetryWrapperCause(
            ICause cause,
            int maxRetryAttempts,
            Func<int, TimeSpan> retryPeriodProvider = null)
            : base(cause)
        {
            if (maxRetryAttempts < 0)
            {
                throw new ArgumentOutOfRangeException(
                    "maxRetryAttempts",
                    "MaxRetryAttempts property value must be greater or equal to 0.");
            }

            this.maxRetryAttempts = maxRetryAttempts;
            this.retryPeriodProvider = retryPeriodProvider;
        }

        /// <summary>
        /// Check causes effect.
        /// </summary>
        /// <param name="cancellationToken"> Cancellation token. </param>
        /// <returns> The <see cref="IEffect"/>. </returns>
        public override IEffect Check(CancellationToken cancellationToken)
        {
            for (var i = 0; i <= this.maxRetryAttempts; i++)
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
                    if (i == this.maxRetryAttempts)
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
                            "Error occurred during execution of '{0}' cause. Execution will be retried (attempt {1} of {2}) in {3} seconds. Exception details: {4}",
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
                            "Error occurred during execution of '{0}' cause. Execution will be retried (attempt {1} of {2}) imediatly. Exception details: {4}",
                            this.ToString(),
                            i + 1,
                            this.maxRetryAttempts,
                            ex);
                    }

                    cancellationToken.ThrowIfCancellationRequested();
                }
            }

            throw new IndexOutOfRangeException("Maximum retry count eccede.");
        }
    }
}
