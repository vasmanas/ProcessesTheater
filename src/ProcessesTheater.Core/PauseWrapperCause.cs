namespace ProcessesTheater.Core
{
    using System;
    using System.Threading;

    /// <summary>
    /// Pause wrapped cause.
    /// </summary>
    public class PauseWrapperCause : AbstractWrapperCause
    {
        /// <summary>
        /// Pause lock.
        /// </summary>
        protected readonly ManualResetEventSlim PauseLock = new ManualResetEventSlim(false);

        /// <summary>
        /// Period in milliseconds.
        /// </summary>
        private readonly int periodMs;

        /// <summary>
        /// Last execution.
        /// </summary>
        private DateTime lastExecution;

        /// <summary>
        /// Initializes a new instance of the <see cref="PauseWrapperCause" /> class.
        /// </summary>
        /// <param name="cause"> Inner cause. </param>
        /// <param name="periodSec"> Period to wait in seconds. </param>
        /// <param name="firstTimeExecute"> First time execute don't wait. </param>
        public PauseWrapperCause(ICause cause, double periodSec, bool firstTimeExecute = true)
            : base(cause)
        {
            this.periodMs = Convert.ToInt32(periodSec * 1000);

            if (this.periodMs <= 0)
            {
                throw new ArgumentOutOfRangeException("periodSec", "Value must be grater than zero");
            }

            this.lastExecution = DateTime.UtcNow;

            if (firstTimeExecute)
            {
                this.lastExecution = this.lastExecution.Subtract(TimeSpan.FromMilliseconds(this.periodMs));
            }
        }

        /// <summary>
        /// Notify of premature execution.
        /// </summary>
        public void Notify()
        {
            this.PauseLock.Set();
        }

        /// <summary>
        /// Wait for cause to happen.
        /// </summary>
        /// <param name="cancellationToken"> Cancellation token. </param>
        /// <returns> The <see cref="IEffect"/>. </returns>
        public override IEffect Check(CancellationToken cancellationToken)
        {
            this.Pause(true, cancellationToken);

            return this.InnerCause.Check(cancellationToken);
        }

        /// <summary>
        /// Pause execution.
        /// </summary>
        /// <param name="wait"> If true wait, else just set last execution date. </param>
        /// <param name="cancellationToken"> Cancellation token. </param>
        protected void Pause(bool wait, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            if (wait)
            {
                var period = this.periodMs - (int)(now - this.lastExecution).TotalMilliseconds;

                if (period > 0)
                {
                    now = now.AddMilliseconds(period);

                    this.PauseLock.Wait(period, cancellationToken);
                }
            }

            this.PauseLock.Reset();

            this.lastExecution = now;
        }
    }
}
