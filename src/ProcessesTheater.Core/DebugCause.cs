namespace ProcessesTheater.Core
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    
    using Logging;

    /// <summary>
    /// Debug cause.
    /// </summary>
    public class DebugCause : AbstractWrapperCause
    {
        /// <summary> Logging elemet. </summary>
        private readonly ILog log;

        /// <summary>
        /// Name for debuging.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugCause" /> class.
        /// </summary>
        /// <param name="cause"> Inner cause. </param>
        /// <param name="name"> Name to print. </param>
        public DebugCause(ICause cause, string name = null, ILog log = null)
            : base(cause)
        {
            this.log = log ?? new EmptyLog();
            this.name = string.IsNullOrWhiteSpace(name) ? Guid.NewGuid().ToString() : name;
        }

        /// <summary>
        /// Wait for cause to happen.
        /// </summary>
        /// <param name="cancellationToken"> Cancellation token. </param>
        /// <returns> The <see cref="IEffect"/>. </returns>
        public override IEffect Check(CancellationToken cancellationToken)
        {
            this.log.DebugFormat("Started checking \"{0}\"", this.name);

            var timer = new Stopwatch();
            timer.Start();

            IEffect effect;

            try
            {
                effect = this.InnerCause.Check(cancellationToken);
            }
            catch
            {
                timer.Stop();

                this.log.DebugFormat("Finished checking \"{0}\" with exception. Duration: {1}", this.name, timer.Elapsed);

                throw;
            }

            timer.Stop();

            this.log.DebugFormat(
                "Finished checking \"{0}\" and effect {1}. Duration: {2}",
                this.name,
                effect == null ? "(null)" : effect.ToString(),
                timer.Elapsed);

            return effect;
        }

        /// <summary>
        /// To string.
        /// </summary>
        /// <returns> String value. </returns>
        public override string ToString()
        {
            return this.InnerCause.ToString();
        }
    }
}
