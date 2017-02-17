namespace ProcessesTheater.Core
{
    using System;

    /// <summary>
    /// Timer elapsed effect.
    /// </summary>
    public class TimerElapsedEffect : IEffect
    {
        /// <summary>
        /// When elapsed.
        /// </summary>
        private readonly DateTime elapsed;

        /// <summary>
        /// Interval length.
        /// </summary>
        private readonly TimeSpan interval;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimerElapsedEffect" /> class.
        /// </summary>
        /// <param name="elapsed"> When elapsed. </param>
        /// <param name="interval"> Elapsed interval length. </param>
        public TimerElapsedEffect(DateTime elapsed, TimeSpan interval)
        {
            this.elapsed = elapsed;
            this.interval = interval;
        }

        /// <summary>
        /// Gets elapsed.
        /// </summary>
        public DateTime Elapsed
        {
            get
            {
                return this.elapsed;
            }
        }

        /// <summary>
        /// Gets interval.
        /// </summary>
        public TimeSpan Interval
        {
            get
            {
                return this.interval;
            }
        }

        /// <summary>
        /// To string.
        /// </summary>
        /// <returns> String value. </returns>
        public override string ToString()
        {
            return string.Format("{0}:{1},{2}", this.GetType().Name, this.elapsed, this.interval);
        }
    }
}
