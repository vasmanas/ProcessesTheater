namespace ProcessesTheater.Core
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using Logging;

    /// <summary>
    /// Debug behavior.
    /// </summary>
    public class DebugBehavior : AbstractWrapperBehavior
    {
        /// <summary> Logger. </summary>
        private readonly ILog log;

        /// <summary>
        /// Name for debuging.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugBehavior" /> class.
        /// </summary>
        /// <param name="behavior"> Inner behavior. </param>
        /// <param name="name"> Name to print. </param>
        public DebugBehavior(IBehavior behavior, string name = null, ILog log = null)
            : base(behavior)
        {
            this.log = log ?? new EmptyLog();
            this.name = string.IsNullOrWhiteSpace(name) ? Guid.NewGuid().ToString() : name;
        }

        /// <summary>
        /// Act method.
        /// </summary>
        /// <param name="value"> Value to execute behavior on. </param>
        /// <param name="cancellationToken"> Cancellation token. </param>
        public override void Act(IEffect value, CancellationToken cancellationToken)
        {
            this.log.DebugFormat("Started executing \"{0}\"", this.name);

            var timer = new Stopwatch();
            timer.Start();

            try
            {
                this.InnerBehavior.Act(value, cancellationToken);
            }
            catch
            {
                timer.Stop();

                this.log.DebugFormat("Finished executing \"{0}\" with exception. Duration: {1}", this.name, timer.Elapsed);

                throw;
            }

            timer.Stop();

            this.log.DebugFormat("Finished executing \"{0}\". Duration: {1}", this.name, timer.Elapsed);
        }

        /// <summary>
        /// To string.
        /// </summary>
        /// <returns> String value. </returns>
        public override string ToString()
        {
            return this.InnerBehavior.ToString();
        }
    }
}
