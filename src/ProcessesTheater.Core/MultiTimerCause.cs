namespace ProcessesTheater.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    
    using Logging;

    /// <summary>
    /// Multi timer cause.
    /// </summary>
    public class MultiTimerCause : ICause
    {
        /// <summary> Logging element. </summary>
        private readonly ILog log;

        /// <summary>
        /// Pause lock.
        /// </summary>
        private readonly ManualResetEventSlim pauseLock = new ManualResetEventSlim(false);

        /// <summary>
        /// Intervals.
        /// </summary>
        private readonly List<Interval> intervals = new List<Interval>();

        /// <summary>
        /// Block on wait.
        /// </summary>
        private readonly bool blockOnWait;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiTimerCause" /> class.
        /// </summary>
        /// <param name="blockOnWait">The Block on wait flag.</param>
        public MultiTimerCause(bool blockOnWait, ILog log = null)
        {
            this.log = log ?? new EmptyLog();
            this.blockOnWait = blockOnWait;
        }

        /// <summary>
        /// Check causes effect.
        /// </summary>
        /// <param name="cancellationToken"> Cancellation token. </param>
        /// <returns> The <see cref="IEffect"/>. </returns>
        public IEffect Check(CancellationToken cancellationToken)
        {
            if (!this.intervals.Any())
            {
                throw new InvalidOperationException(
                    string.Format("No intervals found, add intervals to '{0}' cause", this.GetType().Name));
            }

            Interval interval = null;
            var now = new DateTime();

            if (this.blockOnWait)
            {
                do
                {
                    if (interval != null)
                    {
                        var waitPeriod = interval.Occured - now;

                        this.pauseLock.Wait(waitPeriod, cancellationToken);
                    }

                    if (interval == null || this.pauseLock.IsSet)
                    {
                        this.pauseLock.Reset();

                        interval = this.intervals.OrderBy(e => e.Occured).First();
                    }

                    now = DateTime.UtcNow;
                }
                while (interval.Occured > now);
            }
            else
            {
                interval = this.intervals.OrderBy(e => e.Occured).First();
                now = DateTime.UtcNow;

                if (interval.Occured <= now)
                {
                    return null;
                }
            }

            this.log.DebugFormat("Timer '{0}' elapsed, previous occured: {1}", interval.Duration, interval.Occured);

            var elapsed = interval.Occured;
            interval.Occured = elapsed.Add(interval.Duration);

            if (interval.Occured < now)
            {
                interval.Occured = now;
            }

            return new TimerElapsedEffect(elapsed, interval.Duration);
        }

        /// <summary>
        /// Add interval.
        /// </summary>
        /// <param name="interval"> Supplied interval. </param>
        public void AddInterval(TimeSpan interval)
        {
            if (interval.TotalMilliseconds <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    string.Format("Interval must be at least one millisecond, actual value {0}", interval));
            }

            if (this.intervals.Any(e => e.Duration == interval))
            {
                throw new ArgumentException("An element with the same key already exists in the Intervals collection");
            }

            this.intervals.Add(new Interval(interval));
            this.pauseLock.Set();
        }

        /// <summary>
        /// Interval class.
        /// </summary>
        private class Interval
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Interval" /> class.
            /// </summary>
            /// <param name="duration"> Interval duration. </param>
            public Interval(TimeSpan duration)
            {
                this.Duration = duration;
                this.Occured = DateTime.UtcNow;
            }

            /// <summary>
            /// Gets interval duration.
            /// </summary>
            public TimeSpan Duration { get; private set; }

            /// <summary>
            /// Gets or sets last occured interval.
            /// </summary>
            public DateTime Occured { get; set; }
        }
    }
}
