namespace ProcessesTheater.Core
{
    using System.Threading;

    /// <summary>
    /// Basic non asynchronous character.
    /// </summary>
    public class BasicCharacter : ICharacter
    {
        /// <summary>
        /// Cancelation token.
        /// </summary>
        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        /// <summary>
        /// Character cause.
        /// </summary>
        private readonly ICause cause;

        /// <summary>
        /// Character behavior.
        /// </summary>
        private readonly IBehavior behavior;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicCharacter"/> class.
        /// </summary>
        /// <param name="cause"> Cause to check. </param>
        /// <param name="behavior"> Behavior to execute. </param>
        public BasicCharacter(ICause cause, IBehavior behavior)
        {
            this.cause = cause;
            this.behavior = behavior;
        }

        /// <summary>
        /// Start working.
        /// </summary>
        public void Start()
        {
            var token = this.cts.Token;

            while (!token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();

                var effect = this.cause.Check(token);

                token.ThrowIfCancellationRequested();

                this.behavior.Act(effect, token);
            }

            token.ThrowIfCancellationRequested();
        }

        /// <summary>
        /// Stop working.
        /// </summary>
        public void Stop()
        {
            this.cts.Cancel();
        }
    }
}
